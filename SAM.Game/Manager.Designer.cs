namespace SAM.Game
{
    partial class Manager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manager));
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator2;
            _ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            _ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._AchievementCheckBoxColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._AchievementDescriptionColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._AchievementIconColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._AchievementNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._AchievementProtectedColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._AchievementUnlockTimeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this._AchievementImageList = new System.Windows.Forms.ImageList(this.components);
            this._AchievementListView = new SAM.Game.DoubleBufferedListView();
            this._AchievementsNavButton = new System.Windows.Forms.Button();
            this._AchievementsPanel = new System.Windows.Forms.Panel();
            this._AchievementsToolStrip = new System.Windows.Forms.ToolStrip();
            this._CallbackTimer = new System.Windows.Forms.Timer(this.components);
            this._ContentPanel = new System.Windows.Forms.Panel();
            this._CountryStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._DisplayLabel = new System.Windows.Forms.ToolStripLabel();
            this._DisplayLockedOnlyButton = new System.Windows.Forms.ToolStripButton();
            this._DisplayUnlockedOnlyButton = new System.Windows.Forms.ToolStripButton();
            this._DownloadStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._EnableStatsEditingCheckBox = new System.Windows.Forms.ToolStripButton();
            this._GameStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._InvertAllButton = new System.Windows.Forms.ToolStripButton();
            this._LockAllButton = new System.Windows.Forms.ToolStripButton();
            this._MainStatusStrip = new System.Windows.Forms.StatusStrip();
            this._MainToolStrip = new System.Windows.Forms.ToolStrip();
            this._MatchingStringLabel = new System.Windows.Forms.ToolStripLabel();
            this._MatchingStringTextBox = new System.Windows.Forms.ToolStripTextBox();
            this._ReloadButton = new System.Windows.Forms.ToolStripButton();
            this._ResetButton = new System.Windows.Forms.ToolStripButton();
            this._SideNavPanel = new System.Windows.Forms.Panel();
            this._StatisticsDataGridView = new System.Windows.Forms.DataGridView();
            this._StatisticsNavButton = new System.Windows.Forms.Button();
            this._StatisticsPanel = new System.Windows.Forms.Panel();
            this._StatisticsToolStrip = new System.Windows.Forms.ToolStrip();
            this._StoreButton = new System.Windows.Forms.ToolStripButton();
            this._UnlockAllButton = new System.Windows.Forms.ToolStripButton();
            this._AchievementsToolStrip.SuspendLayout();
            this._ContentPanel.SuspendLayout();
            this._MainStatusStrip.SuspendLayout();
            this._MainToolStrip.SuspendLayout();
            this._SideNavPanel.SuspendLayout();
            this._StatisticsPanel.SuspendLayout();
            this._StatisticsToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._StatisticsDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // _ToolStripSeparator1
            // 
            _ToolStripSeparator1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _ToolStripSeparator1.Name = "_ToolStripSeparator1";
            _ToolStripSeparator1.Size = new System.Drawing.Size(5, 20);
            // 
            // _ToolStripSeparator2
            // 
            _ToolStripSeparator2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _ToolStripSeparator2.Name = "_ToolStripSeparator2";
            _ToolStripSeparator2.Size = new System.Drawing.Size(5, 20);
            // 
            // _MainToolStrip
            // 
            this._MainToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._MainToolStrip.Location = new System.Drawing.Point(0, 0);
            this._MainToolStrip.Name = "_MainToolStrip";
            this._MainToolStrip.TabIndex = 1;
            this._MainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._ReloadButton,
            this._ResetButton,
            this._StoreButton,
            });
            // 
            // _ReloadButton
            // 
            this._ReloadButton.Click += new System.EventHandler(this.OnRefresh);
            this._ReloadButton.Enabled = false;
            this._ReloadButton.Image = global::SAM.Game.Resources.Refresh;
            this._ReloadButton.Name = "_ReloadButton";
            this._ReloadButton.Text = "Refresh";
            // 
            // _ResetButton
            // 
            this._ResetButton.Click += new System.EventHandler(this.OnResetAllStats);
            this._ResetButton.Image = global::SAM.Game.Resources.Reset;
            this._ResetButton.Name = "_ResetButton";
            this._ResetButton.Text = "Reset";
            // 
            // _StoreButton
            // 
            this._StoreButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this._StoreButton.Click += new System.EventHandler(this.OnStore);
            this._StoreButton.Enabled = false;
            this._StoreButton.Image = global::SAM.Game.Resources.Save;
            this._StoreButton.Name = "_StoreButton";
            this._StoreButton.Text = "Commit Changes";
            // 
            // _AchievementImageList
            // 
            this._AchievementImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this._AchievementImageList.ImageSize = new System.Drawing.Size(64, 64);
            // 
            // _MainStatusStrip
            // 
            this._MainStatusStrip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._MainStatusStrip.Location = new System.Drawing.Point(0, 370);
            this._MainStatusStrip.Name = "_MainStatusStrip";
            this._MainStatusStrip.TabIndex = 4;
            this._MainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._CountryStatusLabel,
            this._GameStatusLabel,
            this._DownloadStatusLabel
            });
            // 
            // _CountryStatusLabel
            // 
            this._CountryStatusLabel.Name = "_CountryStatusLabel";
            // 
            // _GameStatusLabel
            // 
            this._GameStatusLabel.Name = "_GameStatusLabel";
            this._GameStatusLabel.Spring = true;
            this._GameStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _DownloadStatusLabel
            // 
            this._DownloadStatusLabel.Image = global::SAM.Game.Resources.Download;
            this._DownloadStatusLabel.Name = "_DownloadStatusLabel";
            this._DownloadStatusLabel.Text = "Download status";
            this._DownloadStatusLabel.Visible = false;
            // 
            // _CallbackTimer
            // 
            this._CallbackTimer.Enabled = true;
            this._CallbackTimer.Tick += new System.EventHandler(this.OnTimer);
            // 
            // _SideNavPanel
            // 
            this._SideNavPanel.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this._SideNavPanel.Controls.Add(this._AchievementsNavButton);
            this._SideNavPanel.Controls.Add(this._StatisticsNavButton);
            this._SideNavPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._SideNavPanel.Height = 40;
            this._SideNavPanel.Name = "_SideNavPanel";
            this._SideNavPanel.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            // 
            // _AchievementsNavButton
            // 
            this._AchievementsNavButton.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._AchievementsNavButton.Click += new System.EventHandler(this.OnShowAchievements);
            this._AchievementsNavButton.FlatAppearance.BorderSize = 0;
            this._AchievementsNavButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._AchievementsNavButton.Location = new System.Drawing.Point(5, 5);
            this._AchievementsNavButton.Name = "_AchievementsNavButton";
            this._AchievementsNavButton.Size = new System.Drawing.Size(120, 35);
            this._AchievementsNavButton.Text = "Achievements";
            // 
            // _StatisticsNavButton
            // 
            this._StatisticsNavButton.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            this._StatisticsNavButton.Click += new System.EventHandler(this.OnShowStatistics);
            this._StatisticsNavButton.FlatAppearance.BorderSize = 0;
            this._StatisticsNavButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._StatisticsNavButton.Location = new System.Drawing.Point(130, 5);
            this._StatisticsNavButton.Name = "_StatisticsNavButton";
            this._StatisticsNavButton.Size = new System.Drawing.Size(120, 35);
            this._StatisticsNavButton.Text = "Statistics";
            // 
            // _ContentPanel
            // 
            this._ContentPanel.Controls.Add(this._AchievementsPanel);
            this._ContentPanel.Controls.Add(this._StatisticsPanel);
            this._ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._ContentPanel.Name = "_ContentPanel";
            // 
            // _AchievementsPanel
            // 
            this._AchievementsPanel.Controls.Add(this._AchievementListView);
            this._AchievementsPanel.Controls.Add(this._AchievementsToolStrip);
            this._AchievementsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._AchievementsPanel.Name = "_AchievementsPanel";
            this._AchievementsPanel.Padding = new System.Windows.Forms.Padding(5);
            this._AchievementsPanel.Visible = true;
            // 
            // _AchievementListView
            // 
            this._AchievementListView.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this._AchievementListView.AllowColumnReorder = false;
            this._AchievementListView.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this._AchievementListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._AchievementListView.ColumnWidthChanging += new System.Windows.Forms.ColumnWidthChangingEventHandler(this.OnAchievementListViewColumnWidthChanging);
            this._AchievementListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._AchievementListView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.OnAchievementListViewDrawColumnHeader);
            this._AchievementListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.OnAchievementListViewDrawItem);
            this._AchievementListView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.OnAchievementListViewDrawSubItem);
            this._AchievementListView.ForeColor = System.Drawing.Color.White;
            this._AchievementListView.FullRowSelect = false;
            this._AchievementListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this._AchievementListView.HideSelection = true;
            this._AchievementListView.LargeImageList = this._AchievementImageList;
            this._AchievementListView.Name = "_AchievementListView";
            this._AchievementListView.OwnerDraw = true;
            this._AchievementListView.SmallImageList = this._AchievementImageList;
            this._AchievementListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this._AchievementListView.TabIndex = 4;
            this._AchievementListView.UseCompatibleStateImageBehavior = false;
            this._AchievementListView.View = System.Windows.Forms.View.Details;
            this._AchievementListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this._AchievementCheckBoxColumnHeader,
            this._AchievementIconColumnHeader,
            this._AchievementNameColumnHeader,
            this._AchievementDescriptionColumnHeader,
            this._AchievementUnlockTimeColumnHeader,
            this._AchievementProtectedColumnHeader,
            });
            // 
            // _AchievementCheckBoxColumnHeader
            // 
            this._AchievementCheckBoxColumnHeader.Text = "";
            this._AchievementCheckBoxColumnHeader.Width = 32;
            // 
            // _AchievementIconColumnHeader
            // 
            this._AchievementIconColumnHeader.Text = "";
            this._AchievementIconColumnHeader.Width = 64;
            // 
            // _AchievementNameColumnHeader
            // 
            this._AchievementNameColumnHeader.Text = "Name";
            this._AchievementNameColumnHeader.Width = 200;
            // 
            // _AchievementDescriptionColumnHeader
            // 
            this._AchievementDescriptionColumnHeader.Text = "Description";
            this._AchievementDescriptionColumnHeader.Width = -2;
            // 
            // _AchievementUnlockTimeColumnHeader
            // 
            this._AchievementUnlockTimeColumnHeader.Text = "Unlock Time";
            this._AchievementUnlockTimeColumnHeader.Width = 125;
            // 
            // _AchievementProtectedColumnHeader
            // 
            this._AchievementProtectedColumnHeader.Text = "Protected";
            this._AchievementProtectedColumnHeader.Width = -2;
            // 
            // _AchievementsToolStrip
            // 
            this._AchievementsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._AchievementsToolStrip.Location = new System.Drawing.Point(0, 0);
            this._AchievementsToolStrip.Name = "_AchievementsToolStrip";
            this._AchievementsToolStrip.TabIndex = 1;
            this._AchievementsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._LockAllButton,
            this._UnlockAllButton,
            this._InvertAllButton,
            _ToolStripSeparator1,
            this._DisplayLabel,
            this._DisplayLockedOnlyButton,
            this._DisplayUnlockedOnlyButton,
            _ToolStripSeparator2,
            this._MatchingStringLabel,
            this._MatchingStringTextBox
            });
            // 
            // _LockAllButton
            // 
            this._LockAllButton.Click += new System.EventHandler(this.OnLockAll);
            this._LockAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._LockAllButton.Image = global::SAM.Game.Resources.Lock;
            this._LockAllButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this._LockAllButton.Name = "_LockAllButton";
            this._LockAllButton.ToolTipText = "Lock all achievements.";
            // 
            // _InvertAllButton
            // 
            this._InvertAllButton.Click += new System.EventHandler(this.OnInvertAll);
            this._InvertAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._InvertAllButton.Image = global::SAM.Game.Resources.Invert;
            this._InvertAllButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this._InvertAllButton.Name = "_InvertAllButton";
            this._InvertAllButton.ToolTipText = "Invert all achievements.";
            // 
            // _UnlockAllButton
            // 
            this._UnlockAllButton.Click += new System.EventHandler(this.OnUnlockAll);
            this._UnlockAllButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._UnlockAllButton.Image = global::SAM.Game.Resources.Unlock;
            this._UnlockAllButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this._UnlockAllButton.Name = "_UnlockAllButton";
            this._UnlockAllButton.ToolTipText = "Unlock all achievements.";
            // 
            // _DisplayLabel
            // 
            this._DisplayLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._DisplayLabel.Name = "_DisplayLabel";
            this._DisplayLabel.Text = "Show only:";
            // 
            // _DisplayLockedOnlyButton
            // 
            this._DisplayLockedOnlyButton.CheckOnClick = true;
            this._DisplayLockedOnlyButton.Click += new System.EventHandler(this.OnDisplayCheckedOnly);
            this._DisplayLockedOnlyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._DisplayLockedOnlyButton.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this._DisplayLockedOnlyButton.Name = "_DisplayLockedOnlyButton";
            this._DisplayLockedOnlyButton.Text = "Locked";
            // 
            // _DisplayUnlockedOnlyButton
            // 
            this._DisplayUnlockedOnlyButton.CheckOnClick = true;
            this._DisplayUnlockedOnlyButton.Click += new System.EventHandler(this.OnDisplayUncheckedOnly);
            this._DisplayUnlockedOnlyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this._DisplayUnlockedOnlyButton.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this._DisplayUnlockedOnlyButton.Name = "_DisplayUnlockedOnlyButton";
            this._DisplayUnlockedOnlyButton.Text = "Unlocked";
            // 
            // _MatchingStringLabel
            // 
            this._MatchingStringLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._MatchingStringLabel.Name = "_MatchingStringLabel";
            this._MatchingStringLabel.Text = "Filter:";
            // 
            // _MatchingStringTextBox
            // 
            this._MatchingStringTextBox.AutoSize = false;
            this._MatchingStringTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._MatchingStringTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnFilterUpdate);
            this._MatchingStringTextBox.Name = "_MatchingStringTextBox";
            this._MatchingStringTextBox.Size = new System.Drawing.Size(100, 20);
            this._MatchingStringTextBox.ToolTipText = "Type at least 3 characters that must appear in the name or description";
            // 
            // _StatisticsPanel
            // 
            this._StatisticsPanel.Controls.Add(this._StatisticsDataGridView);
            this._StatisticsPanel.Controls.Add(this._StatisticsToolStrip);
            this._StatisticsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._StatisticsPanel.Name = "_StatisticsPanel";
            this._StatisticsPanel.Padding = new System.Windows.Forms.Padding(5);
            this._StatisticsPanel.Visible = false;
            // 
            // _StatisticsToolStrip
            // 
            this._StatisticsToolStrip.BackColor = System.Drawing.Color.FromArgb(45, 45, 48);
            this._StatisticsToolStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this._StatisticsToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._StatisticsToolStrip.Location = new System.Drawing.Point(0, 0);
            this._StatisticsToolStrip.Name = "_StatisticsToolStrip";
            this._StatisticsToolStrip.TabIndex = 1;
            this._StatisticsToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._EnableStatsEditingCheckBox
            });
            // 
            // _EnableStatsEditingCheckBox
            // 
            this._EnableStatsEditingCheckBox.CheckOnClick = true;
            this._EnableStatsEditingCheckBox.Click += new System.EventHandler(this.OnStatAgreementChecked);
            this._EnableStatsEditingCheckBox.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.ImageAndText;
            this._EnableStatsEditingCheckBox.ForeColor = System.Drawing.Color.White;
            this._EnableStatsEditingCheckBox.Image = global::SAM.Game.Resources.CheckBoxBlank;
            this._EnableStatsEditingCheckBox.Name = "_EnableStatsEditingCheckBox";
            this._EnableStatsEditingCheckBox.Text = "I understand that by modifying the values of stats, I may screw things up and can\'t blame anyone but myself.";
            // 
            // _StatisticsDataGridView
            // 
            this._StatisticsDataGridView.AllowUserToAddRows = false;
            this._StatisticsDataGridView.AllowUserToDeleteRows = false;
            this._StatisticsDataGridView.AllowUserToResizeRows = false;
            this._StatisticsDataGridView.BackgroundColor = System.Drawing.Color.Black;
            this._StatisticsDataGridView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._StatisticsDataGridView.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.Single;
            this._StatisticsDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.OnStatCellEndEdit);
            this._StatisticsDataGridView.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this._StatisticsDataGridView.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(40, 40, 40);
            this._StatisticsDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this._StatisticsDataGridView.ColumnHeadersHeight = 25;
            this._StatisticsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this._StatisticsDataGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.OnStatDataError);
            this._StatisticsDataGridView.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this._StatisticsDataGridView.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this._StatisticsDataGridView.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._StatisticsDataGridView.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this._StatisticsDataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._StatisticsDataGridView.EnableHeadersVisualStyles = false;
            this._StatisticsDataGridView.GridColor = System.Drawing.Color.FromArgb(60, 60, 60);
            this._StatisticsDataGridView.Name = "_StatisticsDataGridView";
            this._StatisticsDataGridView.RowHeadersVisible = false;
            this._StatisticsDataGridView.TabIndex = 0;
            // 
            // Manager
            // 
            this.Name = "Manager";
            this.Text = "SAM-HC 8.0";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(1350, 729);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.ForeColor = System.Drawing.Color.White;
            this.Controls.Add(this._ContentPanel);
            this.Controls.Add(this._SideNavPanel);
            this.Controls.Add(this._MainToolStrip);
            this.Controls.Add(this._MainStatusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1366, 768);
            this._ContentPanel.ResumeLayout(false);
            this._SideNavPanel.ResumeLayout(false);
            this._MainToolStrip.ResumeLayout(false);
            this._MainToolStrip.PerformLayout();
            this._MainStatusStrip.ResumeLayout(false);
            this._MainStatusStrip.PerformLayout();
            this._AchievementsPanel.ResumeLayout(false);
            this._AchievementsPanel.PerformLayout();
            this._AchievementsToolStrip.ResumeLayout(false);
            this._AchievementsToolStrip.PerformLayout();
            this._StatisticsPanel.ResumeLayout(false);
            this._StatisticsPanel.PerformLayout();
            this._StatisticsToolStrip.ResumeLayout(false);
            this._StatisticsToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._StatisticsDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private DoubleBufferedListView _AchievementListView;
        private System.Windows.Forms.Button _AchievementsNavButton;
        private System.Windows.Forms.Button _StatisticsNavButton;
        private System.Windows.Forms.ColumnHeader _AchievementCheckBoxColumnHeader;
        private System.Windows.Forms.ColumnHeader _AchievementDescriptionColumnHeader;
        private System.Windows.Forms.ColumnHeader _AchievementIconColumnHeader;
        private System.Windows.Forms.ColumnHeader _AchievementNameColumnHeader;
        private System.Windows.Forms.ColumnHeader _AchievementProtectedColumnHeader;
        private System.Windows.Forms.ColumnHeader _AchievementUnlockTimeColumnHeader;
        private System.Windows.Forms.DataGridView _StatisticsDataGridView;
        private System.Windows.Forms.ImageList _AchievementImageList;
        private System.Windows.Forms.Panel _AchievementsPanel;
        private System.Windows.Forms.Panel _ContentPanel;
        private System.Windows.Forms.Panel _SideNavPanel;
        private System.Windows.Forms.Panel _StatisticsPanel;
        private System.Windows.Forms.StatusStrip _MainStatusStrip;
        private System.Windows.Forms.Timer _CallbackTimer;
        private System.Windows.Forms.ToolStrip _AchievementsToolStrip;
        private System.Windows.Forms.ToolStrip _MainToolStrip;
        private System.Windows.Forms.ToolStrip _StatisticsToolStrip;
        private System.Windows.Forms.ToolStripButton _DisplayLockedOnlyButton;
        private System.Windows.Forms.ToolStripButton _DisplayUnlockedOnlyButton;
        private System.Windows.Forms.ToolStripButton _EnableStatsEditingCheckBox;
        private System.Windows.Forms.ToolStripButton _InvertAllButton;
        private System.Windows.Forms.ToolStripButton _LockAllButton;
        private System.Windows.Forms.ToolStripButton _ReloadButton;
        private System.Windows.Forms.ToolStripButton _ResetButton;
        private System.Windows.Forms.ToolStripButton _StoreButton;
        private System.Windows.Forms.ToolStripButton _UnlockAllButton;
        private System.Windows.Forms.ToolStripLabel _DisplayLabel;
        private System.Windows.Forms.ToolStripLabel _MatchingStringLabel;
        private System.Windows.Forms.ToolStripStatusLabel _CountryStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel _DownloadStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel _GameStatusLabel;
        private System.Windows.Forms.ToolStripTextBox _MatchingStringTextBox;
    }
}
