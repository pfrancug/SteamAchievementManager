namespace SAM.Picker
{
    partial class GamePicker
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                _ListDownloader?.Dispose();
                _LogoDownloader?.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GamePicker));
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator1;
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator2;
            System.Windows.Forms.ToolStripSeparator _ToolStripSeparator3;
            _ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            _ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            _ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this._AddGameTextBox = new System.Windows.Forms.ToolStripTextBox();
            this._CallbackTimer = new System.Windows.Forms.Timer(this.components);
            this._DownloadStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._FilterDemosButton = new System.Windows.Forms.ToolStripButton();
            this._FilterGamesButton = new System.Windows.Forms.ToolStripButton();
            this._FilterGamesLabel = new System.Windows.Forms.ToolStripLabel();
            this._FilterJunkButton = new System.Windows.Forms.ToolStripButton();
            this._FilterModsButton = new System.Windows.Forms.ToolStripButton();
            this._FilterShowLabel = new System.Windows.Forms.ToolStripLabel();
            this._FindGamesLabel = new System.Windows.Forms.ToolStripLabel();
            this._GameListView = new SAM.Picker.MyListView();
            this._ListWorker = new System.ComponentModel.BackgroundWorker();
            this._LogoImageList = new System.Windows.Forms.ImageList(this.components);
            this._LogoWorker = new System.ComponentModel.BackgroundWorker();
            this._PickerStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this._PickerStatusStrip = new System.Windows.Forms.StatusStrip();
            this._PickerToolStrip = new System.Windows.Forms.ToolStrip();
            this._RefreshGamesButton = new System.Windows.Forms.ToolStripButton();
            this._SearchGameTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.unlockAllGames = new System.Windows.Forms.ToolStripButton();
            this.unlockAllProgress = new System.Windows.Forms.ToolStripProgressBar();
            this._PickerToolStrip.SuspendLayout();
            this._PickerStatusStrip.SuspendLayout();
            this.SuspendLayout();
            //
            // _ToolStripSeparator1
            //
            _ToolStripSeparator1.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _ToolStripSeparator1.Name = "_ToolStripSeparator1";
            _ToolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            //
            // _ToolStripSeparator2
            //
            _ToolStripSeparator2.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _ToolStripSeparator2.Name = "_ToolStripSeparator2";
            _ToolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            //
            // _ToolStripSeparator3
            //
            _ToolStripSeparator3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            _ToolStripSeparator3.Name = "_ToolStripSeparator3";
            _ToolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            //
            // _LogoImageList
            //
            this._LogoImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
            this._LogoImageList.ImageSize = new System.Drawing.Size(184, 69);
            this._LogoImageList.TransparentColor = System.Drawing.Color.Transparent;
            //
            // _CallbackTimer
            //
            this._CallbackTimer.Enabled = true;
            this._CallbackTimer.Tick += new System.EventHandler(this.OnTimer);
            //
            // _PickerToolStrip
            //
            this._PickerToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._PickerToolStrip.Location = new System.Drawing.Point(0, 0);
            this._PickerToolStrip.Name = "_PickerToolStrip";
            this._PickerToolStrip.TabIndex = 1;
            this._PickerToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[]{
            this._RefreshGamesButton,
            this.unlockAllGames,
            _ToolStripSeparator1,
            this._FilterGamesLabel,
            this._SearchGameTextBox,
            _ToolStripSeparator2,
            this._FindGamesLabel,
            this._AddGameTextBox,
            this.toolStripButton1,
            _ToolStripSeparator3,
            this._FilterShowLabel,
            this._FilterGamesButton,
            this._FilterDemosButton,
            this._FilterModsButton,
            this._FilterJunkButton
            });
            //
            // _RefreshGamesButton
            //
            this._RefreshGamesButton.Click += new System.EventHandler(this.OnRefresh);
            this._RefreshGamesButton.Image = global::SAM.Picker.Properties.Resources.Refresh;
            this._RefreshGamesButton.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this._RefreshGamesButton.Name = "_RefreshGamesButton";
            this._RefreshGamesButton.Text = "Refresh Games";
            // 
            // unlockAllGames
            // 
            this.unlockAllGames.Click += new System.EventHandler(this.unlockAllGames_Click);
            this.unlockAllGames.Enabled = false;
            this.unlockAllGames.Image = global::SAM.Picker.Properties.Resources.Unlock;
            this.unlockAllGames.Margin = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.unlockAllGames.Name = "unlockAllGames";
            this.unlockAllGames.Text = "Unlock All";
            //
            // _FilterGamesLabel
            //
            this._FilterGamesLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._FilterGamesLabel.Name = "_FilterGamesLabel";
            this._FilterGamesLabel.Text = "Filter:";
            //
            // _SearchGameTextBox
            //
            this._SearchGameTextBox.AutoSize = false;
            this._SearchGameTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._SearchGameTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OnFilterUpdate);
            this._SearchGameTextBox.Name = "_SearchGameTextBox";
            this._SearchGameTextBox.Size = new System.Drawing.Size(100, 25);
            //
            // _FindGamesLabel
            //
            this._FindGamesLabel.Name = "_FindGamesLabel";
            this._FindGamesLabel.Text = "ID:";
            //
            // _AddGameTextBox
            //
            this._AddGameTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._AddGameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.OnAddGameTextBoxKeyDown);
            this._AddGameTextBox.Name = "_AddGameTextBox";
            this._AddGameTextBox.Size = new System.Drawing.Size(100, 25);
            //
            // toolStripButton1
            //
            this.toolStripButton1.AutoSize = false;
            this.toolStripButton1.Click += new System.EventHandler(this.OnAddGame);
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(30, 25);
            this.toolStripButton1.Text = "Go";
            //
            // _FilterShowLabel
            //
            this._FilterShowLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this._FilterShowLabel.Name = "_FilterShowLabel";
            this._FilterShowLabel.Text = "Show:";
            //
            // _FilterGamesButton
            //
            this._FilterGamesButton.Checked = true;
            this._FilterGamesButton.CheckedChanged += new System.EventHandler(this.OnFilterUpdate);
            this._FilterGamesButton.CheckOnClick = true;
            this._FilterGamesButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this._FilterGamesButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this._FilterGamesButton.Name = "_FilterGamesButton";
            this._FilterGamesButton.Text = "Games";
            //
            // _FilterDemosButton
            //
            this._FilterDemosButton.CheckedChanged += new System.EventHandler(this.OnFilterUpdate);
            this._FilterDemosButton.CheckOnClick = true;
            this._FilterDemosButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this._FilterDemosButton.Name = "_FilterDemosButton";
            this._FilterDemosButton.Text = "Demos";
            //
            // _FilterModsButton
            //
            this._FilterModsButton.CheckedChanged += new System.EventHandler(this.OnFilterUpdate);
            this._FilterModsButton.CheckOnClick = true;
            this._FilterModsButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this._FilterModsButton.Name = "_FilterModsButton";
            this._FilterModsButton.Text = "Mods";
            //
            // _FilterJunkButton
            //
            this._FilterJunkButton.CheckedChanged += new System.EventHandler(this.OnFilterUpdate);
            this._FilterJunkButton.CheckOnClick = true;
            this._FilterJunkButton.Margin = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this._FilterJunkButton.Name = "_FilterJunkButton";
            this._FilterJunkButton.Text = "Junk";
            //
            // _PickerStatusStrip
            //
            this._PickerStatusStrip.Location = new System.Drawing.Point(0, 270);
            this._PickerStatusStrip.Name = "_PickerStatusStrip";
            this._PickerStatusStrip.Size = new System.Drawing.Size(742, 25);
            this._PickerStatusStrip.TabIndex = 2;
            this._PickerStatusStrip.Text = "statusStrip";
            this._PickerStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._PickerStatusLabel,
            this._DownloadStatusLabel,
            this.unlockAllProgress
            });
            //
            // _PickerStatusLabel
            //
            this._PickerStatusLabel.Name = "_PickerStatusLabel";
            this._PickerStatusLabel.Size = new System.Drawing.Size(727, 17);
            this._PickerStatusLabel.Spring = true;
            this._PickerStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // _DownloadStatusLabel
            // 
            this._DownloadStatusLabel.Image = global::SAM.Picker.Properties.Resources.Download;
            this._DownloadStatusLabel.Name = "_DownloadStatusLabel";
            this._DownloadStatusLabel.Size = new System.Drawing.Size(111, 17);
            this._DownloadStatusLabel.Text = "Download status";
            this._DownloadStatusLabel.Visible = false;
            //
            // unlockAllProgress
            // 
            this.unlockAllProgress.Name = "unlockAllProgress";
            this.unlockAllProgress.Size = new System.Drawing.Size(100, 16);
            this.unlockAllProgress.Visible = false;
            // 
            // _LogoWorker
            //
            this._LogoWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoDownloadLogo);
            this._LogoWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OnDownloadLogo);
            this._LogoWorker.WorkerSupportsCancellation = true;
            //
            // _ListWorker
            //
            this._ListWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoDownloadList);
            this._ListWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.OnDownloadList);
            this._ListWorker.WorkerSupportsCancellation = true;
            //
            // _GameListView
            // 
            this._GameListView.BackColor = System.Drawing.Color.Black;
            this._GameListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this._GameListView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.OnGameListViewDrawItem);
            this._GameListView.ForeColor = System.Drawing.Color.White;
            this._GameListView.ItemActivate += new System.EventHandler(this.OnActivateGame);
            this._GameListView.LargeImageList = this._LogoImageList;
            this._GameListView.Location = new System.Drawing.Point(0, 25);
            this._GameListView.MultiSelect = false;
            this._GameListView.Name = "_GameListView";
            this._GameListView.OwnerDraw = true;
            this._GameListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.OnGameListViewRetrieveVirtualItem);
            this._GameListView.Size = new System.Drawing.Size(742, 245);
            this._GameListView.SmallImageList = this._LogoImageList;
            this._GameListView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this._GameListView.TabIndex = 0;
            this._GameListView.TileSize = new System.Drawing.Size(184, 69);
            this._GameListView.UseCompatibleStateImageBehavior = false;
            this._GameListView.VirtualMode = true;
            // 
            // GamePicker
            //
            this.Name = "GamePicker";
            this.Text = "SAM-HC 8.0";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.MinimumSize = new System.Drawing.Size(1366, 768);
            this.ClientSize = new System.Drawing.Size(1350, 729);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.ForeColor = System.Drawing.Color.White;
            this.Controls.Add(this._GameListView);
            this.Controls.Add(this._PickerToolStrip);
            this.Controls.Add(this._PickerStatusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this._PickerToolStrip.ResumeLayout(false);
            this._PickerToolStrip.PerformLayout();
            this._PickerStatusStrip.ResumeLayout(false);
            this._PickerStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private MyListView _GameListView;
        private System.ComponentModel.BackgroundWorker _ListWorker;
        private System.ComponentModel.BackgroundWorker _LogoWorker;
        private System.Windows.Forms.ImageList _LogoImageList;
        private System.Windows.Forms.StatusStrip _PickerStatusStrip;
        private System.Windows.Forms.Timer _CallbackTimer;
        private System.Windows.Forms.ToolStrip _PickerToolStrip;
        private System.Windows.Forms.ToolStripButton _FilterDemosButton;
        private System.Windows.Forms.ToolStripButton _FilterGamesButton;
        private System.Windows.Forms.ToolStripButton _FilterJunkButton;
        private System.Windows.Forms.ToolStripButton _FilterModsButton;
        private System.Windows.Forms.ToolStripButton _RefreshGamesButton;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripButton unlockAllGames;
        private System.Windows.Forms.ToolStripLabel _FilterGamesLabel;
        private System.Windows.Forms.ToolStripLabel _FilterShowLabel;
        private System.Windows.Forms.ToolStripLabel _FindGamesLabel;
        private System.Windows.Forms.ToolStripProgressBar unlockAllProgress;
        private System.Windows.Forms.ToolStripStatusLabel _DownloadStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel _PickerStatusLabel;
        private System.Windows.Forms.ToolStripTextBox _AddGameTextBox;
        private System.Windows.Forms.ToolStripTextBox _SearchGameTextBox;
    }
}
