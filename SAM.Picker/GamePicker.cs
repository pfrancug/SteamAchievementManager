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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using System.Xml.XPath;
using static SAM.Picker.InvariantShorthand;
using APITypes = SAM.API.Types;

namespace SAM.Picker
{
    internal class WebClientWithTimeout : WebClient
    {
        private readonly int _Timeout;

        public WebClientWithTimeout(int timeout)
        {
            _Timeout = timeout;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);
            if (request != null)
            {
                request.Timeout = _Timeout;
            }
            return request;
        }
    }

    internal partial class GamePicker : Form
    {
        private readonly API.Client _SteamClient;
        private readonly Dictionary<uint, GameInfo> _Games;
        private readonly List<GameInfo> _FilteredGames;
        private readonly HashSet<GameInfo> _FilteredGamesSet;
        private List<GameInfo> _SortedGames;
        private readonly WebClient _ListDownloader;
        private readonly WebClient _LogoDownloader;
        private readonly object _LogoLock;
        private readonly HashSet<string> _LogosAttempting;
        private readonly HashSet<string> _LogosAttempted;
        private readonly Queue<GameInfo> _LogoQueue;
        private readonly API.Callbacks.AppDataChanged _AppDataChangedCallback;

        public GamePicker(API.Client client)
        {
            _Games = new();
            _FilteredGames = new();
            _FilteredGamesSet = new();
            _SortedGames = new();
            _ListDownloader = new WebClientWithTimeout(30000);
            _LogoDownloader = new WebClientWithTimeout(5000);
            _LogoLock = new();
            _LogosAttempting = new();
            _LogosAttempted = new();
            _LogoQueue = new();
            InitializeComponent();
            Bitmap blank = new(_LogoImageList.ImageSize.Width, _LogoImageList.ImageSize.Height);
            using (var g = Graphics.FromImage(blank))
            {
                g.Clear(Color.DimGray);
            }
            _LogoImageList.Images.Add("Blank", blank);
            _SteamClient = client;
            _AppDataChangedCallback =
                client.CreateAndRegisterCallback<API.Callbacks.AppDataChanged>();
            _AppDataChangedCallback.OnRun += OnAppDataChanged;
            DarkMode.ApplyDarkTheme(this);
            AddGames();
        }

        private void OnAppDataChanged(APITypes.AppDataChanged param)
        {
            if (param.Result == false)
            {
                return;
            }
            if (_Games.TryGetValue(param.Id, out var game) == false)
            {
                return;
            }
            game.Name = _SteamClient.SteamApps001.GetAppData(game.Id, "name");
            AddGameToLogoQueue(game);
            DownloadNextLogo();
        }

        private void DoDownloadList(object sender, DoWorkEventArgs e)
        {
            var worker = sender as BackgroundWorker;
            _PickerStatusLabel.Text = "Downloading game list...";
            byte[] bytes;
            try
            {
                bytes = _ListDownloader.DownloadData(new Uri("https://gib.me/sam/games.xml"));
            }
            catch (Exception ex)
            {
                Invoke(
                    (MethodInvoker)
                        delegate
                        {
                            MessageBox.Show(
                                this,
                                $"Failed to download game list.\n\nError: {ex.Message}\n\nPlease check your internet connection and try again.",
                                "Download Error",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error
                            );
                        }
                );
                e.Cancel = true;
                return;
            }
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            List<KeyValuePair<uint, string>> pairs = new();
            using (MemoryStream stream = new(bytes, false))
            {
                XPathDocument document = new(stream);
                var navigator = document.CreateNavigator();
                var nodes = navigator.Select("/games/game");
                while (nodes.MoveNext() == true)
                {
                    string type = nodes.Current.GetAttribute("type", "");
                    if (string.IsNullOrEmpty(type) == true)
                    {
                        type = "normal";
                    }
                    pairs.Add(new((uint)nodes.Current.ValueAsLong, type));
                }
            }
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }
            _PickerStatusLabel.Text = "Checking game ownership...";
            int count = 0;
            foreach (var kv in pairs)
            {
                if (++count % 50 == 0 && worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                AddGame(kv.Key, kv.Value);
            }
        }

        private void OnDownloadList(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                _PickerStatusLabel.Text =
                    $"Displaying {_GameListView.Items.Count} games. Total {_Games.Count} games.";
                _RefreshGamesButton.Enabled = true;
                unlockAllGames.Enabled = true;
                return;
            }
            if (e.Error != null)
            {
                AddDefaultGames();
                MessageBox.Show(
                    e.Error.ToString(),
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            _SortedGames = _Games.Values.OrderBy(gi => gi.Name).ToList();
            RefreshGames();
            _RefreshGamesButton.Enabled = true;
            unlockAllGames.Enabled = true;
            DownloadNextLogo();
        }

        private void RefreshGames()
        {
            var nameSearch = _SearchGameTextBox.Text.Length > 0 ? _SearchGameTextBox.Text : null;
            var wantNormals = _FilterGamesButton.Checked == true;
            var wantDemos = _FilterDemosButton.Checked == true;
            var wantMods = _FilterModsButton.Checked == true;
            var wantJunk = _FilterJunkButton.Checked == true;
            _FilteredGames.Clear();
            _FilteredGamesSet.Clear();
            foreach (var info in _SortedGames)
            {
                if (
                    nameSearch != null
                    && info.Name.IndexOf(nameSearch, StringComparison.OrdinalIgnoreCase) < 0
                )
                {
                    continue;
                }
                bool wanted = info.Type switch
                {
                    "normal" => wantNormals,
                    "demo" => wantDemos,
                    "mod" => wantMods,
                    "junk" => wantJunk,
                    _ => true,
                };
                if (wanted == false)
                {
                    continue;
                }
                _FilteredGames.Add(info);
                _FilteredGamesSet.Add(info);
            }
            _GameListView.VirtualListSize = _FilteredGames.Count;
            _PickerStatusLabel.Text =
                $"Displaying {_GameListView.Items.Count} games. Total {_Games.Count} games.";
            if (_GameListView.Items.Count > 0)
            {
                _GameListView.Items[0].Selected = true;
                _GameListView.Select();
            }
        }

        private void OnGameListViewRetrieveVirtualItem(
            object sender,
            RetrieveVirtualItemEventArgs e
        )
        {
            var info = _FilteredGames[e.ItemIndex];
            e.Item = info.Item = new() { Text = info.Name, ImageIndex = info.ImageIndex };
        }

        private void OnGameListViewSearchForVirtualItem(
            object sender,
            SearchForVirtualItemEventArgs e
        )
        {
            if (e.Direction != SearchDirectionHint.Down || e.IsTextSearch == false)
            {
                return;
            }
            var count = _FilteredGames.Count;
            if (count < 2)
            {
                return;
            }
            var text = e.Text;
            int startIndex = e.StartIndex;
            Predicate<GameInfo> predicate;
            {
                predicate = gi =>
                    gi.Name != null
                    && gi.Name.StartsWith(text, StringComparison.CurrentCultureIgnoreCase);
            }
            int index;
            if (e.StartIndex >= count)
            {
                index = _FilteredGames.FindIndex(0, startIndex - 1, predicate);
            }
            else if (startIndex <= 0)
            {
                index = _FilteredGames.FindIndex(0, count, predicate);
            }
            else
            {
                index = _FilteredGames.FindIndex(startIndex, count - startIndex, predicate);
                if (index < 0)
                {
                    index = _FilteredGames.FindIndex(0, startIndex - 1, predicate);
                }
            }
            e.Index = index < 0 ? -1 : index;
        }

        private void DoDownloadLogo(object sender, DoWorkEventArgs e)
        {
            var info = (GameInfo)e.Argument;
            _LogosAttempted.Add(info.ImageUrl);
            try
            {
                var data = _LogoDownloader.DownloadData(new Uri(info.ImageUrl));
                using MemoryStream stream = new(data, false);
                Bitmap bitmap = new(stream);
                e.Result = new LogoInfo(info.Id, bitmap);
            }
            catch (Exception)
            {
                e.Result = new LogoInfo(info.Id, null);
            }
        }

        private void OnDownloadLogo(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Cancelled == true)
            {
                return;
            }
            if (
                e.Result is LogoInfo logoInfo
                && logoInfo.Bitmap != null
                && _Games.TryGetValue(logoInfo.Id, out var gameInfo) == true
            )
            {
                _GameListView.BeginUpdate();
                var imageIndex = _LogoImageList.Images.Count;
                _LogoImageList.Images.Add(gameInfo.ImageUrl, logoInfo.Bitmap);
                gameInfo.ImageIndex = imageIndex;
                _GameListView.EndUpdate();
            }
            DownloadNextLogo();
        }

        private void DownloadNextLogo()
        {
            lock (_LogoLock)
            {
                if (_LogoWorker.IsBusy == true)
                {
                    return;
                }
                GameInfo info;
                while (true)
                {
                    if (_LogoQueue.Count == 0)
                    {
                        _DownloadStatusLabel.Visible = false;
                        return;
                    }
                    info = _LogoQueue.Dequeue();
                    if (info.Item == null || _FilteredGamesSet.Contains(info) == false)
                    {
                        _LogosAttempting.Remove(info.ImageUrl);
                        continue;
                    }
                    try
                    {
                        if (info.Item.Bounds.IntersectsWith(_GameListView.ClientRectangle) == false)
                        {
                            _LogosAttempting.Remove(info.ImageUrl);
                            continue;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        _LogosAttempting.Remove(info.ImageUrl);
                        continue;
                    }
                    break;
                }
                _DownloadStatusLabel.Text = $"Downloading {1 + _LogoQueue.Count} game icons...";
                _DownloadStatusLabel.Visible = true;
                if (_LogoWorker.IsBusy == false)
                {
                    _LogoWorker.RunWorkerAsync(info);
                }
            }
        }

        private string GetGameImageUrl(uint id)
        {
            string candidate;
            var currentLanguage = _SteamClient.SteamApps008.GetCurrentGameLanguage();
            candidate = _SteamClient.SteamApps001.GetAppData(
                id,
                _($"small_capsule/{currentLanguage}")
            );
            if (string.IsNullOrEmpty(candidate) == false)
            {
                return _(
                    $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{id}/{candidate}"
                );
            }
            if (currentLanguage != "english")
            {
                candidate = _SteamClient.SteamApps001.GetAppData(id, "small_capsule/english");
                if (string.IsNullOrEmpty(candidate) == false)
                {
                    return _(
                        $"https://shared.cloudflare.steamstatic.com/store_item_assets/steam/apps/{id}/{candidate}"
                    );
                }
            }
            candidate = _SteamClient.SteamApps001.GetAppData(id, "logo");
            if (string.IsNullOrEmpty(candidate) == false)
            {
                return _(
                    $"https://cdn.steamstatic.com/steamcommunity/public/images/apps/{id}/{candidate}.jpg"
                );
            }
            return null;
        }

        private void AddGameToLogoQueue(GameInfo info)
        {
            if (info.ImageIndex > 0)
            {
                return;
            }
            var imageUrl = GetGameImageUrl(info.Id);
            if (string.IsNullOrEmpty(imageUrl) == true)
            {
                return;
            }
            info.ImageUrl = imageUrl;
            int imageIndex = _LogoImageList.Images.IndexOfKey(imageUrl);
            if (imageIndex >= 0)
            {
                info.ImageIndex = imageIndex;
            }
            else if (
                _LogosAttempting.Contains(imageUrl) == false
                && _LogosAttempted.Contains(imageUrl) == false
            )
            {
                _LogosAttempting.Add(imageUrl);
                _LogoQueue.Enqueue(info);
            }
        }

        private bool OwnsGame(uint id)
        {
            return _SteamClient.SteamApps008.IsSubscribedApp(id);
        }

        private void AddGame(uint id, string type)
        {
            if (_Games.ContainsKey(id) == true)
            {
                return;
            }
            if (OwnsGame(id) == false)
            {
                return;
            }
            GameInfo info = new(id, type);
            info.Name = _SteamClient.SteamApps001.GetAppData(info.Id, "name");
            _Games.Add(id, info);
        }

        private void AddGames()
        {
            _Games.Clear();
            _RefreshGamesButton.Enabled = false;
            _ListWorker.RunWorkerAsync();
        }

        private void AddDefaultGames()
        {
            AddGame(480, "normal");
        }

        private void OnTimer(object sender, EventArgs e)
        {
            _CallbackTimer.Enabled = false;
            _SteamClient.RunCallbacks(false);
            _CallbackTimer.Enabled = true;
        }

        private void OnActivateGame(object sender, EventArgs e)
        {
            var focusedItem = (sender as MyListView)?.FocusedItem;
            var index = focusedItem != null ? focusedItem.Index : -1;
            if (index < 0 || index >= _FilteredGames.Count)
            {
                return;
            }
            var info = _FilteredGames[index];
            if (info == null)
            {
                return;
            }
            try
            {
                Process.Start("SAM.Game.exe", info.Id.ToString(CultureInfo.InvariantCulture));
            }
            catch (Win32Exception)
            {
                MessageBox.Show(
                    this,
                    "Failed to start SAM.Game.exe.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void OnRefresh(object sender, EventArgs e)
        {
            _AddGameTextBox.Text = "";
            AddGames();
        }

        private void OnAddGameTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                e.SuppressKeyPress = true;
                OnAddGame(sender, e);
            }
        }

        private void OnAddGame(object sender, EventArgs e)
        {
            var text = _AddGameTextBox.Text;
            if (uint.TryParse(text, out uint id) == false)
            {
                MessageBox.Show(
                    this,
                    "Please enter a valid game ID.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            if (OwnsGame(id) == false)
            {
                MessageBox.Show(
                    this,
                    "You don't own that game.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }
            if (_ListWorker.IsBusy)
            {
                _ListWorker.CancelAsync();
            }
            while (_LogoQueue.Count > 0)
            {
                var logo = _LogoQueue.Dequeue();
                _LogosAttempted.Remove(logo.ImageUrl);
            }
            _Games.Clear();
            AddGame(id, "normal");
            _SortedGames = _Games.Values.OrderBy(gi => gi.Name).ToList();
            _FilterGamesButton.Checked = true;
            RefreshGames();
            DownloadNextLogo();
        }

        private void OnFilterUpdate(object sender, EventArgs e)
        {
            while (_LogoQueue.Count > 0)
            {
                var dequeuedInfo = _LogoQueue.Dequeue();
                _LogosAttempting.Remove(dequeuedInfo.ImageUrl);
            }
            RefreshGames();
            DownloadNextLogo();
            if (sender == _SearchGameTextBox)
            {
                _SearchGameTextBox.Focus();
            }
        }

        private void OnGameListViewDrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            var info = _FilteredGames[e.ItemIndex];
            if (info.ImageIndex <= 0)
            {
                AddGameToLogoQueue(info);
                DownloadNextLogo();
            }
        }

        private void unlockAllGames_Click(object sender, EventArgs e)
        {
            var gamesToUnlock = _FilteredGames.Count > 0 ? _FilteredGames : _Games.Values.ToList();
            if (
                MessageBox.Show(
                    "This will open and close A LOT of windows.\n\nIn your case, it could be "
                        + gamesToUnlock.Count
                        + " windows.\n\nWhile this shouldn't cause a performance drop, it might get annoying if you're trying to do something.\n\nIs this OK?",
                    "Warning",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                ) != DialogResult.No
            )
            {
                unlockAllProgress.Visible = true;
                unlockAllProgress.Value = 0;
                unlockAllProgress.Maximum = gamesToUnlock.Count;
                foreach (var Game in gamesToUnlock)
                {
                    unlockAllProgress.Value++;
                    try
                    {
                        var process = Process.Start(
                            "SAM.Game.exe",
                            Game.Id.ToString(CultureInfo.InvariantCulture) + " auto"
                        );
                        if (process != null && process.HasExited != true)
                        {
                            process.WaitForExit();
                        }
                    }
                    catch (Win32Exception)
                    {
                        MessageBox.Show(
                            this,
                            "Failed to start SAM.Game.exe.",
                            "Error",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
                unlockAllProgress.Visible = false;
            }
        }
    }
}
