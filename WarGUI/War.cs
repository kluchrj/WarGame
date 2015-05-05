using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WarGUI
{
    public partial class War : Form
    {
        private TimeSpan gameTime;
        private StatsInfo OverallStats;

        public War()
        {
            InitializeComponent();

            OverallStats = new StatsInfo();

            num_iterations.Maximum = long.MaxValue;
            num_threads.Value = num_threads.Maximum = Environment.ProcessorCount;

            cb_dealfirst.Items.Add("Player");
            cb_dealfirst.Items.Add("Computer");
            cb_dealfirst.Items.Add("Every Other");
            cb_dealfirst.Items.Add("Random");

            // Select every other by default
            cb_dealfirst.SelectedIndex = 2;
        }

        private void War_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the worker thread when the form is closing
            if (WarWorker.IsBusy)
                WarWorker.CancelAsync();
        }

        # region UI
        // UI
        //----------------------------------------------------------------------------------------------------------

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (!WarWorker.IsBusy)
            {
                btn_start.Text = "&Stop";
                btn_start.Focus();

                num_iterations.Enabled = false;
                num_threads.Enabled = false;
                cb_dealfirst.Enabled = false;
                chk_jokers.Enabled = false;
                chk_fastshuffle.Enabled = false;

                lbl_status.Text = String.Format("Simulating (0/{0})", num_iterations.Value);
                LogMessage(String.Format("Simulating {0} games...", num_iterations.Value));

                // Create array of arugments to pass
                List<object> Arguments = new List<object>();

                Arguments.Add((long)num_iterations.Value);                           // Iterations
                Arguments.Add(cb_dealfirst.SelectedIndex);  // Deal first
                Arguments.Add(chk_fastshuffle.Checked);     // Fast shuffle
                Arguments.Add(chk_jokers.Checked);          // Jokers
                Arguments.Add((int)num_threads.Value);      // Number of threads

                WarWorker.RunWorkerAsync(Arguments);
            }
            else
            {
                btn_start.Text = "&Start";
                WarWorker.CancelAsync();
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            OverallStats = new StatsInfo();
            gameTime = TimeSpan.Zero;

            lbl_cwin_val.Text = "0 (0%)";
            lbl_pwins_val.Text = "0 (0%)";
            lbl_draws_val.Text = "0 (0%)";

            lbl_compweight_val.Text = "0";
            lbl_playerweight_val.Text = "0";
            lbl_winnerweight_val.Text = "0%";

            lbl_sims_val.Text = "0";
            lbl_turns_val.Text = "0";

            lbl_gametime_val.Text = "0 μs";

            LogMessage("Stats cleared");
        }

        private void btn_clearlog_Click(object sender, EventArgs e)
        {
            lbox_log.Items.Clear();
        }

        // Start simulating when we press enter
        private void num_iterations_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                btn_start_Click(sender, e);
        }

        private void chk_jokers_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                btn_start_Click(sender, e);
        }

        private void chk_fastshuffle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                btn_start_Click(sender, e);
        }

        private void cb_dealfirst_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
                btn_start_Click(sender, e);
        }

        void LogMessage(String Msg)
        {
            lbox_log.Items.Add(String.Format("[{0}] {1}", DateTime.Now, Msg));
            lbox_log.TopIndex = lbox_log.Items.Count - 1; // Scroll to bottom
        }
        # endregion

        // Worker functions
        //----------------------------------------------------------------------------------------------------------

        private void WarWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Record when the operation started
            DateTime Start = DateTime.Now;

            // Fetch the arguments array
            List<object> Args = (List<object>)e.Argument;

            // Get the number of logical processors
            int WarThreads = (int)Args[4];

            // Calculate the workload for each thread
            long[] workload = new long[WarThreads];

            long total = (long)Args[0];
            long extra = total % WarThreads;
            long perIteration = (total - extra) / WarThreads;

            for(int i = 0; i < WarThreads; i++)
            {
                if (extra > 0)
                {
                    workload[i] = perIteration + 1;
                    extra--;
                }
                else
                    workload[i] = perIteration;
            }

            Parallel.For(0, WarThreads, (i) =>
            {
                WarThread t = new WarThread(workload[i], Start, Args);
                t.StartSim();
                StatsInfo s = t.Stats;

                // return the result of the game
                Update(s);
                return;
            });


            StatsInfo result = new StatsInfo(Start);

            if (WarWorker.CancellationPending)
                e.Cancel = true;

            e.Result = result;
        }

        void Update(StatsInfo s)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<StatsInfo>(Update), new object[] { s });
                return;
            }

            OverallStats.AddToStats(s);
            LogMessage(s.Total.ToString());

            UpdateStats(OverallStats);
        }

        private void WarWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {   
            lbl_status.Text = string.Format("Simulating {0}% ({1}/{2})", e.ProgressPercentage, e.UserState, num_iterations.Value);

            // Don't update if the application closed
            if (!pbar_progress.IsDisposed)
                pbar_progress.Value = (int)e.ProgressPercentage;
        }

        private void WarWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                // Grab results
                StatsInfo result = (StatsInfo)e.Result;

                // Get the elapsed time
                TimeSpan timeDiff = DateTime.Now - result.Time;

                double time = timeDiff.TotalSeconds;

                gameTime += timeDiff;

                if (time > 1.0)
                    LogMessage(String.Format("Completed in {0:0.#} seconds", time));
                else
                    LogMessage(String.Format("Completed in {0:0} ms", timeDiff.TotalMilliseconds));

                UpdateStats(result);
            }
            else if (e.Cancelled)
                LogMessage("Cancelled");
            else
                LogMessage("Error");

            lbl_status.Text = "Ready";
            btn_start.Text = "&Start";

            num_iterations.Enabled = true;
            num_threads.Enabled = true;
            cb_dealfirst.Enabled = true;
            chk_jokers.Enabled = true;
            chk_fastshuffle.Enabled = true;

            if (!pbar_progress.IsDisposed)
                pbar_progress.Value = 0;
        }

        // Stats
        //----------------------------------------------------------------------------------------------------------

        void UpdateStats(StatsInfo stats)
        {
            if (stats.Total < 1)
                return;
            
            double PlayerAvg = (double)stats.PlayerWins / stats.Total * 100.0;
            double ComputerAvg = (double)stats.ComputerWins / stats.Total * 100.0;
            double DrawAvg = (double)stats.Draws / stats.Total * 100.0;

            double PredictAvg = (double)stats.CorrectPred / stats.Total * 100.0;
            double avgturns = stats.Turns / stats.Total;

            // Update labels
            lbl_cwin_val.Text = String.Format("{0} ({1:0.###}%)", stats.ComputerWins, ComputerAvg);
            lbl_pwins_val.Text = String.Format("{0} ({1:0.###}%)", stats.PlayerWins, PlayerAvg);
            lbl_draws_val.Text = String.Format("{0} ({1:0.##}%)", stats.Draws, DrawAvg);

            lbl_compweight_val.Text = String.Format("{0:0.##}", (double)stats.ComputerWeight / stats.Total);
            lbl_playerweight_val.Text = String.Format("{0:0.##}", (double)stats.PlayerWeight / stats.Total);
            lbl_winnerweight_val.Text = String.Format("{0:0.#}%", PredictAvg);
            
            lbl_sims_val.Text = String.Format("{0}", stats.Total);

            lbl_turns_val.Text = String.Format("{0:0}", avgturns);
            lbl_gametime_val.Text = String.Format("{0:0} μs", gameTime.TotalMilliseconds * 1000 / stats.Total);
        }
    }
}
