/*
 * Copyright (c) 2025 Piotr Francug - HotCode
 * Copyright (c) 2024 Rick (rick 'at' gibbed 'dot' us)
 *
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 *
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using static SAM.Game.InvariantShorthand;
using APITypes = SAM.API.Types;

namespace SAM.Game
{
    internal partial class Manager : Form
    {
        private readonly long _GameId;
        private readonly API.Client _SteamClient;
        private readonly WebClient[] _IconDownloaders;
        private const int MaxParallelDownloads = 4;
        private bool _DownloadingIcons = false;
        private int _TotalIconsToDownload = 0;
        private readonly List<Stats.AchievementInfo> _IconQueue = new();
        private readonly List<Stats.StatDefinition> _StatDefinitions = new();
        private readonly List<Stats.AchievementDefinition> _AchievementDefinitions = new();
        private readonly List<Stats.AchievementInfo> _VirtualAchievementItems = new();
        private readonly BindingList<Stats.StatInfo> _Statistics = new();
        private readonly API.Callbacks.UserStatsReceived _UserStatsReceivedCallback;

        private static Bitmap LoadCheckBoxImage(bool isChecked)
        {
            return isChecked ? Resources.CheckBoxFilled : Resources.CheckBoxBlank;
        }

        public Manager(long gameId, API.Client client, bool isAuto = false)
        {
            _GameId = gameId;
            _SteamClient = client;
            InitializeComponent();
            DarkMode.ApplyDarkTheme(this);
            _AchievementImageList.Images.Add("Blank", new Bitmap(64, 64));
            ImageList checkboxImages = new() { ImageSize = new Size(16, 16) };
            checkboxImages.Images.Add(LoadCheckBoxImage(false));
            checkboxImages.Images.Add(LoadCheckBoxImage(true));
            _AchievementListView.VirtualMode = true;
            _AchievementListView.OwnerDraw = true;
            _AchievementListView.StateImageList = checkboxImages;
            _AchievementListView.RetrieveVirtualItem += OnRetrieveVirtualItem;
            _AchievementListView.DrawColumnHeader += OnAchievementListViewDrawColumnHeader;
            _AchievementListView.DrawItem += OnAchievementListViewDrawItem;
            _AchievementListView.DrawSubItem += OnAchievementListViewDrawSubItem;
            _AchievementListView.MouseDown += OnAchievementListViewMouseDown;
            _AchievementListView.MouseMove += OnAchievementListViewMouseMove;
            _AchievementListView.Resize += OnAchievementListViewResize;
            OnAchievementListViewResize(this, EventArgs.Empty);
            _StatisticsDataGridView.AutoGenerateColumns = false;
            _StatisticsDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            _StatisticsDataGridView.DataSource = new BindingSource() { DataSource = _Statistics };
            _StatisticsDataGridView.Columns.Add("name", "Name");
            _StatisticsDataGridView.Columns[0].ReadOnly = true;
            _StatisticsDataGridView.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            _StatisticsDataGridView.Columns[0].DataPropertyName = "DisplayName";
            _StatisticsDataGridView.Columns[0].DefaultCellStyle.WrapMode =
                DataGridViewTriState.False;
            _StatisticsDataGridView.Columns.Add("value", "Value");
            _StatisticsDataGridView.Columns[1].ReadOnly =
                _EnableStatsEditingCheckBox.Checked == false;
            _StatisticsDataGridView.Columns[1].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _StatisticsDataGridView.Columns[1].DataPropertyName = "Value";
            _StatisticsDataGridView.Columns.Add("extra", "Extra");
            _StatisticsDataGridView.Columns[2].ReadOnly = true;
            _StatisticsDataGridView.Columns[2].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _StatisticsDataGridView.Columns[2].DataPropertyName = "Extra";
            _StatisticsDataGridView.Columns.Add("protected", "Protected");
            _StatisticsDataGridView.Columns[3].ReadOnly = true;
            _StatisticsDataGridView.Columns[3].AutoSizeMode =
                DataGridViewAutoSizeColumnMode.AllCells;
            _StatisticsDataGridView.CellBeginEdit += OnStatisticsCellBeginEdit;
            _StatisticsDataGridView.CellFormatting += OnStatisticsCellFormatting;
            _StatisticsDataGridView.CellPainting += OnStatisticsCellPainting;
            _IconDownloaders = new WebClient[MaxParallelDownloads];
            for (int i = 0; i < MaxParallelDownloads; i++)
            {
                _IconDownloaders[i] = new WebClient();
                _IconDownloaders[i].DownloadDataCompleted += OnIconDownload;
            }
            string name = _SteamClient.SteamApps001.GetAppData((uint)_GameId, "name");
            if (name != null)
            {
                base.Text += " | " + name;
            }
            else
            {
                base.Text += " | " + _GameId.ToString(CultureInfo.InvariantCulture);
            }
            _UserStatsReceivedCallback =
                client.CreateAndRegisterCallback<API.Callbacks.UserStatsReceived>();
            _UserStatsReceivedCallback.OnRun += OnUserStatsReceived;
            OnShowAchievements(this, EventArgs.Empty);
            RefreshStats();
            if (isAuto)
            {
                base.Text += " | Automatic Unlock";
                isAutomatic = isAuto;
                _ContentPanel.Enabled = false;
                _InvertAllButton.Enabled = false;
                _LockAllButton.Enabled = false;
                _ReloadButton.Enabled = false;
                _ResetButton.Enabled = false;
                _StoreButton.Enabled = false;
                _UnlockAllButton.Enabled = false;
            }
        }

        public bool isAutomatic { get; private set; }

        private void AddAchievementIcon(Stats.AchievementInfo info, Image icon)
        {
            if (icon == null)
            {
                info.ImageIndex = 0;
            }
            else
            {
                int index = _AchievementImageList.Images.Count;
                _AchievementImageList.Images.Add(
                    info.IsAchieved == true ? info.IconNormal : info.IconLocked,
                    icon
                );
                info.ImageIndex = index;
            }
        }

        private void OnIconDownload(object sender, DownloadDataCompletedEventArgs e)
        {
            if (e.Error == null && e.Cancelled == false)
            {
                var info = (Stats.AchievementInfo)e.UserState;
                Bitmap bitmap;
                try
                {
                    using MemoryStream stream = new();
                    stream.Write(e.Result, 0, e.Result.Length);
                    bitmap = new(stream);
                }
                catch (Exception)
                {
                    bitmap = null;
                }
                AddAchievementIcon(info, bitmap);
            }
            DownloadNextIcon();
        }

        private void DownloadNextIcon()
        {
            lock (_IconQueue)
            {
                if (_IconQueue.Count == 0)
                {
                    if (_DownloadingIcons && _IconDownloaders.All(d => !d.IsBusy))
                    {
                        _DownloadingIcons = false;
                        _AchievementListView.Invalidate();
                        _DownloadStatusLabel.Visible = false;
                    }
                    return;
                }
                if (!_DownloadingIcons)
                {
                    _DownloadingIcons = true;
                    _TotalIconsToDownload = _IconQueue.Count;
                }
                var availableDownloader = _IconDownloaders.FirstOrDefault(d => !d.IsBusy);
                if (availableDownloader == null)
                {
                    return;
                }
                int remaining = _IconQueue.Count;
                _DownloadStatusLabel.Text = $"Downloading {remaining} achievement icons...";
                _DownloadStatusLabel.Visible = true;
                var info = _IconQueue[0];
                _IconQueue.RemoveAt(0);
                availableDownloader.DownloadDataAsync(
                    new Uri(
                        _(
                            $"https://cdn.steamstatic.com/steamcommunity/public/images/apps/{_GameId}/{(info.IsAchieved == true ? info.IconNormal : info.IconLocked)}"
                        )
                    ),
                    info
                );
                if (_IconQueue.Count > 0)
                {
                    DownloadNextIcon();
                }
            }
        }

        private static string TranslateError(int id) =>
            id switch
            {
                2 => "generic error -- this usually means you don't own the game",
                _ => _($"{id}"),
            };

        private static string GetLocalizedString(KeyValue kv, string language, string defaultValue)
        {
            var name = kv[language].AsString("");
            if (string.IsNullOrEmpty(name) == false)
            {
                return name;
            }
            if (language != "english")
            {
                name = kv["english"].AsString("");
                if (string.IsNullOrEmpty(name) == false)
                {
                    return name;
                }
            }
            name = kv.AsString("");
            if (string.IsNullOrEmpty(name) == false)
            {
                return name;
            }
            return defaultValue;
        }

        private bool LoadUserGameStatsSchema()
        {
            string path;
            try
            {
                string fileName = _($"UserGameStatsSchema_{_GameId}.bin");
                path = API.Steam.GetInstallPath();
                path = Path.Combine(path, "appcache", "stats", fileName);
                if (File.Exists(path) == false)
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
            var kv = KeyValue.LoadAsBinary(path);
            if (kv == null)
            {
                return false;
            }
            var currentLanguage = _SteamClient.SteamApps008.GetCurrentGameLanguage();
            _AchievementDefinitions.Clear();
            _StatDefinitions.Clear();
            var stats = kv[_GameId.ToString(CultureInfo.InvariantCulture)]["stats"];
            if (stats.Valid == false || stats.Children == null)
            {
                return false;
            }
            foreach (var stat in stats.Children)
            {
                if (stat.Valid == false)
                {
                    continue;
                }
                var rawType = stat["type_int"].Valid
                    ? stat["type_int"].AsInteger(0)
                    : stat["type"].AsInteger(0);
                var type = (APITypes.UserStatType)rawType;
                switch (type)
                {
                    case APITypes.UserStatType.Invalid:
                    {
                        break;
                    }
                    case APITypes.UserStatType.Integer:
                    {
                        var id = stat["name"].AsString("");
                        string name = GetLocalizedString(
                            stat["display"]["name"],
                            currentLanguage,
                            id
                        );
                        _StatDefinitions.Add(
                            new Stats.IntegerStatDefinition()
                            {
                                Id = stat["name"].AsString(""),
                                DisplayName = name,
                                MinValue = stat["min"].AsInteger(int.MinValue),
                                MaxValue = stat["max"].AsInteger(int.MaxValue),
                                MaxChange = stat["maxchange"].AsInteger(0),
                                IncrementOnly = stat["incrementonly"].AsBoolean(false),
                                SetByTrustedGameServer = stat["bSetByTrustedGS"].AsBoolean(false),
                                DefaultValue = stat["default"].AsInteger(0),
                                Permission = stat["permission"].AsInteger(0),
                            }
                        );
                        break;
                    }
                    case APITypes.UserStatType.Float:
                    case APITypes.UserStatType.AverageRate:
                    {
                        var id = stat["name"].AsString("");
                        string name = GetLocalizedString(
                            stat["display"]["name"],
                            currentLanguage,
                            id
                        );
                        _StatDefinitions.Add(
                            new Stats.FloatStatDefinition()
                            {
                                Id = stat["name"].AsString(""),
                                DisplayName = name,
                                MinValue = stat["min"].AsFloat(float.MinValue),
                                MaxValue = stat["max"].AsFloat(float.MaxValue),
                                MaxChange = stat["maxchange"].AsFloat(0.0f),
                                IncrementOnly = stat["incrementonly"].AsBoolean(false),
                                DefaultValue = stat["default"].AsFloat(0.0f),
                                Permission = stat["permission"].AsInteger(0),
                            }
                        );
                        break;
                    }
                    case APITypes.UserStatType.Achievements:
                    case APITypes.UserStatType.GroupAchievements:
                    {
                        if (stat.Children != null)
                        {
                            foreach (
                                var bits in stat.Children.Where(b =>
                                    string.Compare(
                                        b.Name,
                                        "bits",
                                        StringComparison.InvariantCultureIgnoreCase
                                    ) == 0
                                )
                            )
                            {
                                if (bits.Valid == false || bits.Children == null)
                                {
                                    continue;
                                }
                                foreach (var bit in bits.Children)
                                {
                                    string id = bit["name"].AsString("");
                                    string name = GetLocalizedString(
                                        bit["display"]["name"],
                                        currentLanguage,
                                        id
                                    );
                                    string desc = GetLocalizedString(
                                        bit["display"]["desc"],
                                        currentLanguage,
                                        ""
                                    );
                                    _AchievementDefinitions.Add(
                                        new()
                                        {
                                            Id = id,
                                            Name = name,
                                            Description = desc,
                                            IconNormal = bit["display"]["icon"].AsString(""),
                                            IconLocked = bit["display"]["icon_gray"].AsString(""),
                                            IsHidden = bit["display"]["hidden"].AsBoolean(false),
                                            Permission = bit["permission"].AsInteger(0),
                                        }
                                    );
                                }
                            }
                        }
                        break;
                    }
                    default:
                    {
                        throw new InvalidOperationException("invalid stat type");
                    }
                }
            }
            return true;
        }

        private void OnUserStatsReceived(APITypes.UserStatsReceived param)
        {
            if (param.Result != 1)
            {
                _GameStatusLabel.Text =
                    $"Error while retrieving stats: {TranslateError(param.Result)}";
                EnableInput();
                return;
            }
            if (LoadUserGameStatsSchema() == false)
            {
                _GameStatusLabel.Text = "Failed to load schema.";
                EnableInput();
                return;
            }
            try
            {
                GetAchievements();
            }
            catch (Exception e)
            {
                _GameStatusLabel.Text = "Error when handling achievements retrieval.";
                EnableInput();
                MessageBox.Show(
                    "Error when handling achievements retrieval:\n" + e,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            try
            {
                GetStatistics();
            }
            catch (Exception e)
            {
                _GameStatusLabel.Text = "Error when handling stats retrieval.";
                EnableInput();
                MessageBox.Show(
                    "Error when handling stats retrieval:\n" + e,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            _GameStatusLabel.Text =
                $"Retrieved {_VirtualAchievementItems.Count} achievements and {_StatisticsDataGridView.Rows.Count} statistics.";
            EnableInput();
            if (isAutomatic)
            {
                foreach (var info in _VirtualAchievementItems)
                {
                    info.IsAchieved = true;
                }
                if (_VirtualAchievementItems.Count > 0)
                {
                    _AchievementListView.RedrawItems(0, _VirtualAchievementItems.Count - 1, false);
                }
                if (_VirtualAchievementItems.Count == 0)
                {
                    Application.Exit();
                }
                var achievements = new List<Stats.AchievementInfo>();
                foreach (var info in _VirtualAchievementItems)
                {
                    if (info != null)
                    {
                        achievements.Add(info);
                    }
                }
                if (achievements.Count == 0)
                {
                    Application.Exit();
                }
                foreach (Stats.AchievementInfo info in achievements)
                {
                    if (
                        _SteamClient.SteamUserStats.SetAchievement(info.Id, info.IsAchieved)
                        == false
                    )
                    {
                        Application.Exit();
                    }
                }
                Application.Exit();
            }
        }

        private void RefreshStats()
        {
            _VirtualAchievementItems.Clear();
            _AchievementListView.VirtualListSize = 0;
            _StatisticsDataGridView.Rows.Clear();
            var steamId = _SteamClient.SteamUser.GetSteamId();
            var callHandle = _SteamClient.SteamUserStats.RequestUserStats(steamId);
            if (callHandle == API.CallHandle.Invalid)
            {
                MessageBox.Show(
                    this,
                    "Failed.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            _GameStatusLabel.Text = "Retrieving stat information...";
            DisableInput();
        }

        private bool _IsUpdatingAchievementList;

        private void GetAchievements()
        {
            var textSearch =
                _MatchingStringTextBox.Text.Length > 0 ? _MatchingStringTextBox.Text : null;
            _IsUpdatingAchievementList = true;
            _VirtualAchievementItems.Clear();
            _AchievementListView.BeginUpdate();
            _AchievementListView.VirtualListSize = 0;
            bool wantLocked = _DisplayLockedOnlyButton.Checked == true;
            bool wantUnlocked = _DisplayUnlockedOnlyButton.Checked == true;
            foreach (var def in _AchievementDefinitions)
            {
                if (string.IsNullOrEmpty(def.Id) == true)
                {
                    continue;
                }
                if (
                    _SteamClient.SteamUserStats.GetAchievementAndUnlockTime(
                        def.Id,
                        out bool isAchieved,
                        out var unlockTime
                    ) == false
                )
                {
                    continue;
                }
                bool wanted =
                    (wantLocked == false && wantUnlocked == false)
                    || isAchieved switch
                    {
                        true => wantUnlocked,
                        false => wantLocked,
                    };
                if (wanted == false)
                {
                    continue;
                }
                if (textSearch != null)
                {
                    if (
                        def.Name.IndexOf(textSearch, StringComparison.OrdinalIgnoreCase) < 0
                        && def.Description.IndexOf(textSearch, StringComparison.OrdinalIgnoreCase)
                            < 0
                    )
                    {
                        continue;
                    }
                }
                Stats.AchievementInfo info = new()
                {
                    Id = def.Id,
                    IsAchieved = isAchieved,
                    OriginalIsAchieved = isAchieved,
                    UnlockTime =
                        isAchieved == true && unlockTime > 0
                            ? DateTimeOffset.FromUnixTimeSeconds(unlockTime).LocalDateTime
                            : null,
                    IconNormal = string.IsNullOrEmpty(def.IconNormal) ? null : def.IconNormal,
                    IconLocked = string.IsNullOrEmpty(def.IconLocked)
                        ? def.IconNormal
                        : def.IconLocked,
                    Permission = def.Permission,
                    Name = def.Name,
                    Description = def.Description,
                    ImageIndex = 0,
                };
                AddAchievementToIconQueue(info, false);
                _VirtualAchievementItems.Add(info);
            }
            _IsUpdatingAchievementList = false;
            _AchievementListView.VirtualListSize = _VirtualAchievementItems.Count;
            _AchievementListView.EndUpdate();
            DownloadNextIcon();
            DownloadNextIcon();
            DownloadNextIcon();
            DownloadNextIcon();
        }

        private void OnRetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (e.ItemIndex < 0 || e.ItemIndex >= _VirtualAchievementItems.Count)
            {
                return;
            }
            var info = _VirtualAchievementItems[e.ItemIndex];
            var bgColor =
                (info.Permission & 3) == 0
                    ? _AchievementListView.BackColor
                    : Color.FromArgb(64, 0, 0);
            ListViewItem item = new()
            {
                Tag = info,
                Text = "",
                BackColor = bgColor,
                ForeColor = Color.White,
                StateImageIndex = info.IsAchieved ? 1 : 0,
            };
            item.UseItemStyleForSubItems = false;
            ListViewItem.ListViewSubItem iconSubItem = new()
            {
                Text = "",
                Tag = info.ImageIndex,
                BackColor = bgColor,
                ForeColor = Color.White,
            };
            item.SubItems.Add(iconSubItem);
            if (info.Name.StartsWith("#", StringComparison.InvariantCulture))
            {
                item.SubItems.Add(
                    new ListViewItem.ListViewSubItem(item, info.Id)
                    {
                        BackColor = bgColor,
                        ForeColor = Color.White,
                    }
                );
                item.SubItems.Add(
                    new ListViewItem.ListViewSubItem(item, "")
                    {
                        BackColor = bgColor,
                        ForeColor = Color.White,
                    }
                );
            }
            else
            {
                item.SubItems.Add(
                    new ListViewItem.ListViewSubItem(item, info.Name)
                    {
                        BackColor = bgColor,
                        ForeColor = Color.White,
                    }
                );
                item.SubItems.Add(
                    new ListViewItem.ListViewSubItem(item, info.Description)
                    {
                        BackColor = bgColor,
                        ForeColor = Color.White,
                    }
                );
            }
            item.SubItems.Add(
                new ListViewItem.ListViewSubItem(
                    item,
                    info.UnlockTime.HasValue ? info.UnlockTime.Value.ToString() : ""
                )
                {
                    BackColor = bgColor,
                    ForeColor = Color.White,
                }
            );
            item.SubItems.Add(
                new ListViewItem.ListViewSubItem(item, "")
                {
                    BackColor = bgColor,
                    ForeColor = Color.White,
                }
            );
            e.Item = item;
        }

        private void OnAchievementListViewMouseDown(object sender, MouseEventArgs e)
        {
            if (_IsUpdatingAchievementList)
            {
                return;
            }
            ListViewHitTestInfo hit = _AchievementListView.HitTest(e.Location);
            if (hit.Item != null && hit.SubItem != null)
            {
                int columnIndex = hit.Item.SubItems.IndexOf(hit.SubItem);
                if (columnIndex == 0)
                {
                    int index = hit.Item.Index;
                    if (index >= 0 && index < _VirtualAchievementItems.Count)
                    {
                        var info = _VirtualAchievementItems[index];
                        if ((info.Permission & 3) != 0)
                        {
                            return;
                        }
                        info.IsAchieved = !info.IsAchieved;
                        _AchievementListView.RedrawItems(index, index, false);
                    }
                }
            }
        }

        private void OnAchievementListViewMouseMove(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo hit = _AchievementListView.HitTest(e.Location);
            if (
                hit.Item != null
                && hit.SubItem != null
                && hit.Item.SubItems.IndexOf(hit.SubItem) == 0
            )
            {
                int index = hit.Item.Index;
                if (
                    index >= 0
                    && index < _VirtualAchievementItems.Count
                    && (_VirtualAchievementItems[index].Permission & 3) == 0
                )
                {
                    _AchievementListView.Cursor = Cursors.Hand;
                    return;
                }
            }
            _AchievementListView.Cursor = Cursors.Default;
        }

        private void OnAchievementListViewDrawColumnHeader(
            object sender,
            DrawListViewColumnHeaderEventArgs e
        )
        {
            using (var brush = new SolidBrush(Color.FromArgb(40, 40, 40)))
            {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }
            TextRenderer.DrawText(
                e.Graphics,
                e.Header.Text,
                e.Font,
                e.Bounds,
                Color.White,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter
            );
            using var borderPen = new Pen(Color.FromArgb(60, 60, 60));
            e.Graphics.DrawLine(
                borderPen,
                e.Bounds.Right - 1,
                e.Bounds.Top,
                e.Bounds.Right - 1,
                e.Bounds.Bottom - 1
            );
            e.Graphics.DrawLine(
                borderPen,
                e.Bounds.Left,
                e.Bounds.Bottom - 1,
                e.Bounds.Right - 1,
                e.Bounds.Bottom - 1
            );
        }

        private void OnAchievementListViewDrawItem(object sender, DrawListViewItemEventArgs e) { }

        private void OnAchievementListViewDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                e.DrawBackground();
                if (e.Item.StateImageIndex >= 0 && _AchievementListView.StateImageList != null)
                {
                    var stateImage = _AchievementListView.StateImageList.Images[
                        e.Item.StateImageIndex
                    ];
                    if (stateImage != null)
                    {
                        int x = e.Bounds.X + (e.Bounds.Width - stateImage.Width) / 2;
                        int y = e.Bounds.Y + (e.Bounds.Height - stateImage.Height) / 2;
                        e.Graphics.DrawImage(stateImage, x, y);
                    }
                }
                using var gridPen = new Pen(Color.FromArgb(60, 60, 60));
                e.Graphics.DrawLine(
                    gridPen,
                    e.Bounds.Right - 1,
                    e.Bounds.Top,
                    e.Bounds.Right - 1,
                    e.Bounds.Bottom
                );
                e.Graphics.DrawLine(
                    gridPen,
                    e.Bounds.Left,
                    e.Bounds.Bottom - 1,
                    e.Bounds.Right,
                    e.Bounds.Bottom - 1
                );
            }
            else if (e.ColumnIndex == 1)
            {
                e.DrawBackground();
                if (
                    e.Item.Tag is Stats.AchievementInfo info
                    && info.ImageIndex > 0
                    && info.ImageIndex < _AchievementImageList.Images.Count
                )
                {
                    Image icon = _AchievementImageList.Images[info.ImageIndex];
                    if (icon != null)
                    {
                        e.Graphics.DrawImage(icon, e.Bounds.X, e.Bounds.Y, 64, 64);
                    }
                }
                using var gridPen = new Pen(Color.FromArgb(60, 60, 60));
                e.Graphics.DrawLine(
                    gridPen,
                    e.Bounds.Right - 1,
                    e.Bounds.Top,
                    e.Bounds.Right - 1,
                    e.Bounds.Bottom
                );
                e.Graphics.DrawLine(
                    gridPen,
                    e.Bounds.Left,
                    e.Bounds.Bottom - 1,
                    e.Bounds.Right,
                    e.Bounds.Bottom - 1
                );
            }
            else if (e.ColumnIndex == 5)
            {
                e.DrawBackground();
                if (e.Item.Tag is Stats.AchievementInfo info && (info.Permission & 3) != 0)
                {
                    var icon = Resources.Protected;
                    int x = e.Bounds.X + (e.Bounds.Width - icon.Width) / 2;
                    int y = e.Bounds.Y + (e.Bounds.Height - icon.Height) / 2;
                    e.Graphics.DrawImage(icon, x, y);
                }
                using var gridPen = new Pen(Color.FromArgb(60, 60, 60));
                e.Graphics.DrawLine(
                    gridPen,
                    e.Bounds.Right - 1,
                    e.Bounds.Top,
                    e.Bounds.Right - 1,
                    e.Bounds.Bottom
                );
                e.Graphics.DrawLine(
                    gridPen,
                    e.Bounds.Left,
                    e.Bounds.Bottom - 1,
                    e.Bounds.Right,
                    e.Bounds.Bottom - 1
                );
            }
            else
            {
                e.DrawBackground();
                Rectangle textBounds = e.Bounds;
                textBounds.Inflate(-4, -2);
                string text = e.SubItem.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    TextRenderer.DrawText(
                        e.Graphics,
                        text,
                        e.Item.Font,
                        textBounds,
                        e.SubItem.ForeColor,
                        TextFormatFlags.Left
                            | TextFormatFlags.WordBreak
                            | TextFormatFlags.VerticalCenter
                    );
                }
                using var gridPen = new Pen(Color.FromArgb(60, 60, 60));
                e.Graphics.DrawLine(
                    gridPen,
                    e.Bounds.Right - 1,
                    e.Bounds.Top,
                    e.Bounds.Right - 1,
                    e.Bounds.Bottom
                );
                e.Graphics.DrawLine(
                    gridPen,
                    e.Bounds.Left,
                    e.Bounds.Bottom - 1,
                    e.Bounds.Right,
                    e.Bounds.Bottom - 1
                );
            }
        }

        private void OnAchievementListViewResize(object sender, EventArgs e)
        {
            int fixedColumnsWidth = 32 + 64 + 200 + 125 + 60;
            int scrollBarWidth = SystemInformation.VerticalScrollBarWidth;
            int availableWidth =
                _AchievementListView.ClientSize.Width - fixedColumnsWidth - scrollBarWidth - 2;
            if (availableWidth < 100)
            {
                availableWidth = 100;
            }
            _AchievementDescriptionColumnHeader.Width = availableWidth;
        }

        private void OnAchievementListViewColumnWidthChanging(
            object sender,
            ColumnWidthChangingEventArgs e
        )
        {
            e.NewWidth = _AchievementListView.Columns[e.ColumnIndex].Width;
            e.Cancel = true;
        }

        private void GetStatistics()
        {
            _Statistics.Clear();
            foreach (var stat in _StatDefinitions)
            {
                if (string.IsNullOrEmpty(stat.Id) == true)
                {
                    continue;
                }
                if (stat is Stats.IntegerStatDefinition intStat)
                {
                    if (
                        _SteamClient.SteamUserStats.GetStatValue(intStat.Id, out int value) == false
                    )
                    {
                        continue;
                    }
                    _Statistics.Add(
                        new Stats.IntStatInfo()
                        {
                            Id = intStat.Id,
                            DisplayName = intStat.DisplayName,
                            IntValue = value,
                            OriginalValue = value,
                            IsIncrementOnly = intStat.IncrementOnly,
                            Permission = intStat.Permission,
                        }
                    );
                }
                else if (stat is Stats.FloatStatDefinition floatStat)
                {
                    if (
                        _SteamClient.SteamUserStats.GetStatValue(floatStat.Id, out float value)
                        == false
                    )
                    {
                        continue;
                    }
                    _Statistics.Add(
                        new Stats.FloatStatInfo()
                        {
                            Id = floatStat.Id,
                            DisplayName = floatStat.DisplayName,
                            FloatValue = value,
                            OriginalValue = value,
                            IsIncrementOnly = floatStat.IncrementOnly,
                            Permission = floatStat.Permission,
                        }
                    );
                }
            }
        }

        private void AddAchievementToIconQueue(Stats.AchievementInfo info, bool startDownload)
        {
            int imageIndex = _AchievementImageList.Images.IndexOfKey(
                info.IsAchieved == true ? info.IconNormal : info.IconLocked
            );
            if (imageIndex >= 0)
            {
                info.ImageIndex = imageIndex;
            }
            else
            {
                lock (_IconQueue)
                {
                    _IconQueue.Add(info);
                }
                if (startDownload == true)
                {
                    DownloadNextIcon();
                }
            }
        }

        private int StoreAchievements()
        {
            if (_VirtualAchievementItems.Count == 0)
            {
                return 0;
            }
            List<Stats.AchievementInfo> achievements = new();
            foreach (var achievementInfo in _VirtualAchievementItems)
            {
                if (achievementInfo == null || !achievementInfo.IsModified)
                {
                    continue;
                }
                achievements.Add(achievementInfo);
            }
            if (achievements.Count == 0)
            {
                return 0;
            }
            foreach (var info in achievements)
            {
                if (_SteamClient.SteamUserStats.SetAchievement(info.Id, info.IsAchieved) == false)
                {
                    MessageBox.Show(
                        this,
                        $"An error occurred while setting the state for {info.Id}, aborting store.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                    return -1;
                }
            }
            return achievements.Count;
        }

        private int StoreStatistics()
        {
            if (_Statistics.Count == 0)
            {
                return 0;
            }
            var statistics = _Statistics.Where(stat => stat.IsModified == true).ToList();
            if (statistics.Count == 0)
            {
                return 0;
            }
            foreach (var stat in statistics)
            {
                if (stat is Stats.IntStatInfo intStat)
                {
                    if (
                        _SteamClient.SteamUserStats.SetStatValue(intStat.Id, intStat.IntValue)
                        == false
                    )
                    {
                        MessageBox.Show(
                            this,
                            $"An error occurred while setting the value for {stat.Id}, aborting store.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return -1;
                    }
                }
                else if (stat is Stats.FloatStatInfo floatStat)
                {
                    if (
                        _SteamClient.SteamUserStats.SetStatValue(floatStat.Id, floatStat.FloatValue)
                        == false
                    )
                    {
                        MessageBox.Show(
                            this,
                            $"An error occurred while setting the value for {stat.Id}, aborting store.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return -1;
                    }
                }
                else
                {
                    throw new InvalidOperationException("unsupported stat type");
                }
            }
            return statistics.Count;
        }

        private void DisableInput()
        {
            _ReloadButton.Enabled = false;
            _StoreButton.Enabled = false;
        }

        private void EnableInput()
        {
            _ReloadButton.Enabled = true;
            _StoreButton.Enabled = true;
        }

        private void OnTimer(object sender, EventArgs e)
        {
            _CallbackTimer.Enabled = false;
            _SteamClient.RunCallbacks(false);
            _CallbackTimer.Enabled = true;
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            RefreshStats();
        }

        private void OnLockAll(object sender, EventArgs e)
        {
            foreach (var info in _VirtualAchievementItems)
            {
                if ((info.Permission & 3) != 0)
                {
                    continue;
                }
                info.IsAchieved = false;
            }
            if (_VirtualAchievementItems.Count > 0)
            {
                _AchievementListView.RedrawItems(0, _VirtualAchievementItems.Count - 1, false);
            }
        }

        private void OnInvertAll(object sender, EventArgs e)
        {
            foreach (var info in _VirtualAchievementItems)
            {
                if ((info.Permission & 3) != 0)
                {
                    continue;
                }
                info.IsAchieved = !info.IsAchieved;
            }
            if (_VirtualAchievementItems.Count > 0)
            {
                _AchievementListView.RedrawItems(0, _VirtualAchievementItems.Count - 1, false);
            }
        }

        private void OnUnlockAll(object sender, EventArgs e)
        {
            foreach (var info in _VirtualAchievementItems)
            {
                if ((info.Permission & 3) != 0)
                {
                    continue;
                }
                info.IsAchieved = true;
            }
            if (_VirtualAchievementItems.Count > 0)
            {
                _AchievementListView.RedrawItems(0, _VirtualAchievementItems.Count - 1, false);
            }
        }

        private bool Store()
        {
            if (_SteamClient.SteamUserStats.StoreStats() == false)
            {
                MessageBox.Show(
                    this,
                    "An error occurred while storing, aborting.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
            return true;
        }

        private void OnStore(object sender, EventArgs e)
        {
            int achievements = StoreAchievements();
            if (achievements < 0)
            {
                RefreshStats();
                return;
            }
            int stats = StoreStatistics();
            if (stats < 0)
            {
                RefreshStats();
                return;
            }
            if (Store() == false)
            {
                RefreshStats();
                return;
            }
            MessageBox.Show(
                this,
                $"Stored {achievements} achievements and {stats} statistics.",
                "Information",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
            RefreshStats();
        }

        private void OnStatDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Context != DataGridViewDataErrorContexts.Commit)
            {
                return;
            }
            var view = (DataGridView)sender;
            if (e.Exception is Stats.StatIsProtectedException)
            {
                if (isAutomatic)
                {
                    Application.Exit();
                }
                else
                {
                    e.ThrowException = false;
                    e.Cancel = true;
                    view.Rows[e.RowIndex].ErrorText = "Stat is protected! -- you can't modify it";
                }
            }
            else
            {
                if (isAutomatic)
                {
                    Application.Exit();
                }
                else
                {
                    e.ThrowException = false;
                    e.Cancel = true;
                    view.Rows[e.RowIndex].ErrorText = "Invalid value";
                }
            }
        }

        private void OnStatisticsCellFormatting(
            object sender,
            DataGridViewCellFormattingEventArgs e
        )
        {
            if (e.RowIndex >= 0 && e.RowIndex < _Statistics.Count)
            {
                var stat = _Statistics[e.RowIndex];
                if (stat.IsProtected)
                {
                    e.CellStyle.BackColor = Color.FromArgb(64, 0, 0);
                    e.CellStyle.SelectionBackColor = Color.FromArgb(80, 0, 0);
                }
            }
        }

        private void OnStatisticsCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex == 3 && e.RowIndex >= 0 && e.RowIndex < _Statistics.Count)
            {
                var stat = _Statistics[e.RowIndex];
                if (stat.IsProtected)
                {
                    e.Paint(e.CellBounds, DataGridViewPaintParts.All);
                    var icon = Resources.Protected;
                    int x = e.CellBounds.X + (e.CellBounds.Width - icon.Width) / 2;
                    int y = e.CellBounds.Y + (e.CellBounds.Height - icon.Height) / 2;
                    e.Graphics.DrawImage(icon, x, y);
                    e.Handled = true;
                }
            }
        }

        private void OnStatisticsCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.ColumnIndex == 1 && e.RowIndex >= 0 && e.RowIndex < _Statistics.Count)
            {
                var stat = _Statistics[e.RowIndex];
                if (stat.IsProtected)
                {
                    e.Cancel = true;
                    MessageBox.Show(
                        this,
                        "Sorry, but this is a protected statistic and cannot be modified with SAM-HC.",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void OnStatAgreementChecked(object sender, EventArgs e)
        {
            _StatisticsDataGridView.Columns[1].ReadOnly =
                _EnableStatsEditingCheckBox.Checked == false;
            _EnableStatsEditingCheckBox.Image = _EnableStatsEditingCheckBox.Checked
                ? Resources.CheckBoxFilled
                : Resources.CheckBoxBlank;
        }

        private void OnStatCellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var view = (DataGridView)sender;
            view.Rows[e.RowIndex].ErrorText = "";
        }

        private void OnResetAllStats(object sender, EventArgs e)
        {
            if (
                MessageBox.Show(
                    "Are you absolutely sure you want to reset stats?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                ) == DialogResult.No
            )
            {
                return;
            }
            bool achievementsToo =
                DialogResult.Yes
                == MessageBox.Show(
                    "Do you want to reset achievements too?",
                    "Question",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );
            if (
                MessageBox.Show(
                    "Really really sure?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error
                ) == DialogResult.No
            )
            {
                return;
            }
            if (_SteamClient.SteamUserStats.ResetAllStats(achievementsToo) == false)
            {
                MessageBox.Show(
                    this,
                    "Failed.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            RefreshStats();
        }

        private void OnDisplayUncheckedOnly(object sender, EventArgs e)
        {
            if ((sender as ToolStripButton).Checked == true)
            {
                _DisplayLockedOnlyButton.Checked = false;
            }
            GetAchievements();
        }

        private void OnDisplayCheckedOnly(object sender, EventArgs e)
        {
            if ((sender as ToolStripButton).Checked == true)
            {
                _DisplayUnlockedOnlyButton.Checked = false;
            }
            GetAchievements();
        }

        private void OnFilterUpdate(object sender, KeyEventArgs e)
        {
            GetAchievements();
        }

        private void OnShowAchievements(object sender, EventArgs e)
        {
            _AchievementsPanel.Visible = true;
            _StatisticsPanel.Visible = false;
            _AchievementsNavButton.BackColor = Color.FromArgb(60, 60, 60);
            _StatisticsNavButton.BackColor = Color.FromArgb(50, 50, 50);
        }

        private void OnShowStatistics(object sender, EventArgs e)
        {
            _AchievementsPanel.Visible = false;
            _StatisticsPanel.Visible = true;
            _AchievementsNavButton.BackColor = Color.FromArgb(50, 50, 50);
            _StatisticsNavButton.BackColor = Color.FromArgb(60, 60, 60);
        }
    }
}
