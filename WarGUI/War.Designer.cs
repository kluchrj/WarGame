namespace WarGUI
{
    partial class War
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
            this.lbl_iterations = new System.Windows.Forms.Label();
            this.btn_start = new System.Windows.Forms.Button();
            this.num_iterations = new System.Windows.Forms.NumericUpDown();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.pbar_progress = new System.Windows.Forms.ToolStripProgressBar();
            this.lbl_status = new System.Windows.Forms.ToolStripStatusLabel();
            this.lbox_log = new System.Windows.Forms.ListBox();
            this.gbox_log = new System.Windows.Forms.GroupBox();
            this.WarWorker = new System.ComponentModel.BackgroundWorker();
            this.lbl_cwins = new System.Windows.Forms.Label();
            this.lbl_cwin_val = new System.Windows.Forms.Label();
            this.lbl_pwins = new System.Windows.Forms.Label();
            this.lbl_pwins_val = new System.Windows.Forms.Label();
            this.lbl_draws = new System.Windows.Forms.Label();
            this.lbl_draws_val = new System.Windows.Forms.Label();
            this.gbox_stats = new System.Windows.Forms.GroupBox();
            this.lbl_turns_val = new System.Windows.Forms.Label();
            this.lbl_turns = new System.Windows.Forms.Label();
            this.lbl_sims_val = new System.Windows.Forms.Label();
            this.lbl_sims = new System.Windows.Forms.Label();
            this.btn_clearlog = new System.Windows.Forms.Button();
            this.btn_clear = new System.Windows.Forms.Button();
            this.lbl_winnerweight_val = new System.Windows.Forms.Label();
            this.lbl_winnerweight = new System.Windows.Forms.Label();
            this.lbl_playerweight_val = new System.Windows.Forms.Label();
            this.lbl_playerweight = new System.Windows.Forms.Label();
            this.lbl_compweight_val = new System.Windows.Forms.Label();
            this.lbl_compweight = new System.Windows.Forms.Label();
            this.chk_jokers = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.num_iterations)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.gbox_log.SuspendLayout();
            this.gbox_stats.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_iterations
            // 
            this.lbl_iterations.AutoSize = true;
            this.lbl_iterations.Location = new System.Drawing.Point(93, 17);
            this.lbl_iterations.Name = "lbl_iterations";
            this.lbl_iterations.Size = new System.Drawing.Size(50, 13);
            this.lbl_iterations.TabIndex = 1;
            this.lbl_iterations.Text = "Iterations";
            // 
            // btn_start
            // 
            this.btn_start.Location = new System.Drawing.Point(12, 12);
            this.btn_start.Name = "btn_start";
            this.btn_start.Size = new System.Drawing.Size(75, 23);
            this.btn_start.TabIndex = 2;
            this.btn_start.Text = "&Start";
            this.btn_start.UseVisualStyleBackColor = true;
            this.btn_start.Click += new System.EventHandler(this.btn_start_Click);
            // 
            // num_iterations
            // 
            this.num_iterations.Increment = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.num_iterations.Location = new System.Drawing.Point(149, 15);
            this.num_iterations.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_iterations.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_iterations.Name = "num_iterations";
            this.num_iterations.Size = new System.Drawing.Size(118, 20);
            this.num_iterations.TabIndex = 3;
            this.num_iterations.ThousandsSeparator = true;
            this.num_iterations.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pbar_progress,
            this.lbl_status});
            this.statusStrip1.Location = new System.Drawing.Point(0, 350);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(684, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 8;
            this.statusStrip1.Text = "statusStrip";
            // 
            // pbar_progress
            // 
            this.pbar_progress.Name = "pbar_progress";
            this.pbar_progress.Size = new System.Drawing.Size(100, 16);
            // 
            // lbl_status
            // 
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(39, 17);
            this.lbl_status.Text = "Ready";
            // 
            // lbox_log
            // 
            this.lbox_log.FormattingEnabled = true;
            this.lbox_log.Location = new System.Drawing.Point(6, 19);
            this.lbox_log.Name = "lbox_log";
            this.lbox_log.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbox_log.Size = new System.Drawing.Size(429, 277);
            this.lbox_log.TabIndex = 0;
            // 
            // gbox_log
            // 
            this.gbox_log.Controls.Add(this.lbox_log);
            this.gbox_log.Location = new System.Drawing.Point(12, 41);
            this.gbox_log.Name = "gbox_log";
            this.gbox_log.Size = new System.Drawing.Size(441, 306);
            this.gbox_log.TabIndex = 7;
            this.gbox_log.TabStop = false;
            this.gbox_log.Text = "Log";
            // 
            // WarWorker
            // 
            this.WarWorker.WorkerReportsProgress = true;
            this.WarWorker.WorkerSupportsCancellation = true;
            this.WarWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WarWorker_DoWork);
            this.WarWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.WarWorker_ProgressChanged);
            this.WarWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.WarWorker_RunWorkerCompleted);
            // 
            // lbl_cwins
            // 
            this.lbl_cwins.AutoSize = true;
            this.lbl_cwins.Location = new System.Drawing.Point(6, 19);
            this.lbl_cwins.Name = "lbl_cwins";
            this.lbl_cwins.Size = new System.Drawing.Size(82, 13);
            this.lbl_cwins.TabIndex = 0;
            this.lbl_cwins.Text = "Computer Wins:";
            // 
            // lbl_cwin_val
            // 
            this.lbl_cwin_val.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_cwin_val.Location = new System.Drawing.Point(94, 19);
            this.lbl_cwin_val.Name = "lbl_cwin_val";
            this.lbl_cwin_val.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl_cwin_val.Size = new System.Drawing.Size(115, 13);
            this.lbl_cwin_val.TabIndex = 1;
            this.lbl_cwin_val.Text = "0 (0%)";
            this.lbl_cwin_val.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_pwins
            // 
            this.lbl_pwins.AutoSize = true;
            this.lbl_pwins.Location = new System.Drawing.Point(6, 32);
            this.lbl_pwins.Name = "lbl_pwins";
            this.lbl_pwins.Size = new System.Drawing.Size(66, 13);
            this.lbl_pwins.TabIndex = 2;
            this.lbl_pwins.Text = "Player Wins:";
            // 
            // lbl_pwins_val
            // 
            this.lbl_pwins_val.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_pwins_val.Location = new System.Drawing.Point(94, 32);
            this.lbl_pwins_val.Name = "lbl_pwins_val";
            this.lbl_pwins_val.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl_pwins_val.Size = new System.Drawing.Size(115, 13);
            this.lbl_pwins_val.TabIndex = 3;
            this.lbl_pwins_val.Text = "0 (0%)";
            this.lbl_pwins_val.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_draws
            // 
            this.lbl_draws.AutoSize = true;
            this.lbl_draws.Location = new System.Drawing.Point(6, 45);
            this.lbl_draws.Name = "lbl_draws";
            this.lbl_draws.Size = new System.Drawing.Size(40, 13);
            this.lbl_draws.TabIndex = 4;
            this.lbl_draws.Text = "Draws:";
            // 
            // lbl_draws_val
            // 
            this.lbl_draws_val.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_draws_val.Location = new System.Drawing.Point(94, 45);
            this.lbl_draws_val.Name = "lbl_draws_val";
            this.lbl_draws_val.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl_draws_val.Size = new System.Drawing.Size(115, 13);
            this.lbl_draws_val.TabIndex = 5;
            this.lbl_draws_val.Text = "0 (0%)";
            this.lbl_draws_val.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // gbox_stats
            // 
            this.gbox_stats.Controls.Add(this.lbl_turns_val);
            this.gbox_stats.Controls.Add(this.lbl_turns);
            this.gbox_stats.Controls.Add(this.lbl_sims_val);
            this.gbox_stats.Controls.Add(this.lbl_sims);
            this.gbox_stats.Controls.Add(this.btn_clearlog);
            this.gbox_stats.Controls.Add(this.btn_clear);
            this.gbox_stats.Controls.Add(this.lbl_winnerweight_val);
            this.gbox_stats.Controls.Add(this.lbl_winnerweight);
            this.gbox_stats.Controls.Add(this.lbl_playerweight_val);
            this.gbox_stats.Controls.Add(this.lbl_playerweight);
            this.gbox_stats.Controls.Add(this.lbl_compweight_val);
            this.gbox_stats.Controls.Add(this.lbl_compweight);
            this.gbox_stats.Controls.Add(this.lbl_draws_val);
            this.gbox_stats.Controls.Add(this.lbl_cwins);
            this.gbox_stats.Controls.Add(this.lbl_draws);
            this.gbox_stats.Controls.Add(this.lbl_cwin_val);
            this.gbox_stats.Controls.Add(this.lbl_pwins);
            this.gbox_stats.Controls.Add(this.lbl_pwins_val);
            this.gbox_stats.Location = new System.Drawing.Point(459, 41);
            this.gbox_stats.Name = "gbox_stats";
            this.gbox_stats.Size = new System.Drawing.Size(215, 306);
            this.gbox_stats.TabIndex = 10;
            this.gbox_stats.TabStop = false;
            this.gbox_stats.Text = "Stats";
            // 
            // lbl_turns_val
            // 
            this.lbl_turns_val.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_turns_val.Location = new System.Drawing.Point(125, 130);
            this.lbl_turns_val.Name = "lbl_turns_val";
            this.lbl_turns_val.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl_turns_val.Size = new System.Drawing.Size(84, 13);
            this.lbl_turns_val.TabIndex = 16;
            this.lbl_turns_val.Text = "0";
            this.lbl_turns_val.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_turns
            // 
            this.lbl_turns.AutoSize = true;
            this.lbl_turns.Location = new System.Drawing.Point(6, 130);
            this.lbl_turns.Name = "lbl_turns";
            this.lbl_turns.Size = new System.Drawing.Size(98, 13);
            this.lbl_turns.TabIndex = 15;
            this.lbl_turns.Text = "Avg Turns / Game:";
            // 
            // lbl_sims_val
            // 
            this.lbl_sims_val.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_sims_val.Location = new System.Drawing.Point(125, 117);
            this.lbl_sims_val.Name = "lbl_sims_val";
            this.lbl_sims_val.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl_sims_val.Size = new System.Drawing.Size(84, 13);
            this.lbl_sims_val.TabIndex = 14;
            this.lbl_sims_val.Text = "0";
            this.lbl_sims_val.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_sims
            // 
            this.lbl_sims.AutoSize = true;
            this.lbl_sims.Location = new System.Drawing.Point(6, 117);
            this.lbl_sims.Name = "lbl_sims";
            this.lbl_sims.Size = new System.Drawing.Size(63, 13);
            this.lbl_sims.TabIndex = 13;
            this.lbl_sims.Text = "Simulations:";
            // 
            // btn_clearlog
            // 
            this.btn_clearlog.Location = new System.Drawing.Point(108, 273);
            this.btn_clearlog.Name = "btn_clearlog";
            this.btn_clearlog.Size = new System.Drawing.Size(101, 23);
            this.btn_clearlog.TabIndex = 12;
            this.btn_clearlog.Text = "Clear &Log";
            this.btn_clearlog.UseVisualStyleBackColor = true;
            this.btn_clearlog.Click += new System.EventHandler(this.btn_clearlog_Click);
            // 
            // btn_clear
            // 
            this.btn_clear.Location = new System.Drawing.Point(6, 273);
            this.btn_clear.Name = "btn_clear";
            this.btn_clear.Size = new System.Drawing.Size(101, 23);
            this.btn_clear.TabIndex = 11;
            this.btn_clear.Text = "&Clear Stats";
            this.btn_clear.UseVisualStyleBackColor = true;
            this.btn_clear.Click += new System.EventHandler(this.btn_clear_Click);
            // 
            // lbl_winnerweight_val
            // 
            this.lbl_winnerweight_val.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_winnerweight_val.Location = new System.Drawing.Point(125, 95);
            this.lbl_winnerweight_val.Name = "lbl_winnerweight_val";
            this.lbl_winnerweight_val.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl_winnerweight_val.Size = new System.Drawing.Size(84, 13);
            this.lbl_winnerweight_val.TabIndex = 11;
            this.lbl_winnerweight_val.Text = "0 (0%)";
            this.lbl_winnerweight_val.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_winnerweight
            // 
            this.lbl_winnerweight.AutoSize = true;
            this.lbl_winnerweight.Location = new System.Drawing.Point(6, 95);
            this.lbl_winnerweight.Name = "lbl_winnerweight";
            this.lbl_winnerweight.Size = new System.Drawing.Size(99, 13);
            this.lbl_winnerweight.TabIndex = 10;
            this.lbl_winnerweight.Text = "Correct Predictions:";
            // 
            // lbl_playerweight_val
            // 
            this.lbl_playerweight_val.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_playerweight_val.Location = new System.Drawing.Point(125, 82);
            this.lbl_playerweight_val.Name = "lbl_playerweight_val";
            this.lbl_playerweight_val.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl_playerweight_val.Size = new System.Drawing.Size(84, 13);
            this.lbl_playerweight_val.TabIndex = 9;
            this.lbl_playerweight_val.Text = "0 (0%)";
            this.lbl_playerweight_val.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_playerweight
            // 
            this.lbl_playerweight.AutoSize = true;
            this.lbl_playerweight.Location = new System.Drawing.Point(6, 82);
            this.lbl_playerweight.Name = "lbl_playerweight";
            this.lbl_playerweight.Size = new System.Drawing.Size(100, 13);
            this.lbl_playerweight.TabIndex = 8;
            this.lbl_playerweight.Text = "Weighted to Player:";
            // 
            // lbl_compweight_val
            // 
            this.lbl_compweight_val.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_compweight_val.Location = new System.Drawing.Point(125, 69);
            this.lbl_compweight_val.Name = "lbl_compweight_val";
            this.lbl_compweight_val.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lbl_compweight_val.Size = new System.Drawing.Size(84, 13);
            this.lbl_compweight_val.TabIndex = 7;
            this.lbl_compweight_val.Text = "0 (0%)";
            this.lbl_compweight_val.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbl_compweight
            // 
            this.lbl_compweight.AutoSize = true;
            this.lbl_compweight.Location = new System.Drawing.Point(6, 69);
            this.lbl_compweight.Name = "lbl_compweight";
            this.lbl_compweight.Size = new System.Drawing.Size(116, 13);
            this.lbl_compweight.TabIndex = 6;
            this.lbl_compweight.Text = "Weighted to Computer:";
            // 
            // chk_jokers
            // 
            this.chk_jokers.AutoSize = true;
            this.chk_jokers.Location = new System.Drawing.Point(282, 18);
            this.chk_jokers.Name = "chk_jokers";
            this.chk_jokers.Size = new System.Drawing.Size(57, 17);
            this.chk_jokers.TabIndex = 11;
            this.chk_jokers.Text = "&Jokers";
            this.chk_jokers.UseVisualStyleBackColor = true;
            // 
            // War
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 372);
            this.Controls.Add(this.chk_jokers);
            this.Controls.Add(this.gbox_log);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.num_iterations);
            this.Controls.Add(this.btn_start);
            this.Controls.Add(this.lbl_iterations);
            this.Controls.Add(this.gbox_stats);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "War";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "War Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.num_iterations)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.gbox_log.ResumeLayout(false);
            this.gbox_stats.ResumeLayout(false);
            this.gbox_stats.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_iterations;
        private System.Windows.Forms.Button btn_start;
        private System.Windows.Forms.NumericUpDown num_iterations;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar pbar_progress;
        private System.Windows.Forms.ToolStripStatusLabel lbl_status;
        private System.Windows.Forms.ListBox lbox_log;
        private System.Windows.Forms.GroupBox gbox_log;
        private System.ComponentModel.BackgroundWorker WarWorker;
        private System.Windows.Forms.Label lbl_cwins;
        private System.Windows.Forms.Label lbl_cwin_val;
        private System.Windows.Forms.Label lbl_pwins;
        private System.Windows.Forms.Label lbl_pwins_val;
        private System.Windows.Forms.Label lbl_draws;
        private System.Windows.Forms.Label lbl_draws_val;
        private System.Windows.Forms.GroupBox gbox_stats;
        private System.Windows.Forms.Label lbl_winnerweight_val;
        private System.Windows.Forms.Label lbl_winnerweight;
        private System.Windows.Forms.Label lbl_playerweight_val;
        private System.Windows.Forms.Label lbl_playerweight;
        private System.Windows.Forms.Label lbl_compweight_val;
        private System.Windows.Forms.Label lbl_compweight;
        private System.Windows.Forms.Button btn_clear;
        private System.Windows.Forms.Button btn_clearlog;
        private System.Windows.Forms.Label lbl_sims_val;
        private System.Windows.Forms.Label lbl_sims;
        private System.Windows.Forms.Label lbl_turns_val;
        private System.Windows.Forms.Label lbl_turns;
        private System.Windows.Forms.CheckBox chk_jokers;
    }
}

