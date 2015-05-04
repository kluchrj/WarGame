using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Diagnostics;

namespace WarGUI
{
    public partial class War : Form
    {
        private int PlayerWins;
        private int ComputerWins;
        private int Draws;
        private int CorrectPred;

        private int PlayerWeight;
        private int ComputerWeight;

        private double Turns;

        private TimeSpan gameTime;

        public War()
        {
            InitializeComponent();

            num_iterations.Maximum = long.MaxValue;
            num_threads.Maximum = Environment.ProcessorCount;
            num_threads.Value = Environment.ProcessorCount;

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
            ComputerWins = PlayerWins = Draws = 0;
            CorrectPred = ComputerWeight = PlayerWeight = 0;
            Turns = 0;
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

            // Get the arguments array
            List<object> Args = (List<object>)e.Argument;

            // Get the number of logical processors
            int WarCalculations = (int)Args[4];

            // One event is used for each WarThread object.
            ManualResetEvent[] doneEvents = new ManualResetEvent[WarCalculations];
            WarThread[] warArray = new WarThread[WarCalculations];

            // Distrubute the workload over the threads
            long[] workload = new long[WarCalculations];

            long total = (long)Args[0];
            long extra = total % WarCalculations;
            long perIteration = (total - extra) / WarCalculations;

            for(int i = 0; i < WarCalculations; i++)
            {
                if (i == 0)
                    workload[i] = perIteration + extra;
                else
                    workload[i] = perIteration;
            }

            // Configure and start threads using ThreadPool
            Console.WriteLine("launching {0} tasks...", WarCalculations);
            for (int i = 0; i < WarCalculations; i++)
            {
                doneEvents[i] = new ManualResetEvent(false);
                WarThread f = new WarThread(workload[i], Start, Args, doneEvents[i]);
                warArray[i] = f;
                ThreadPool.QueueUserWorkItem(f.ThreadPoolCallback, i);
            }

            // Wait for all threads in pool to calculate
            WaitHandle.WaitAll(doneEvents);
            Console.WriteLine("All calculations are complete.");

            StatsInfo result = new StatsInfo(Start);

            // TODO: Update progress bar

            // Combine the results
            for (int i = 0; i < WarCalculations; i++)
                result.AddToStats(warArray[i].Stats);

            if (WarWorker.CancellationPending)
                e.Cancel = true;

            e.Result = result;
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
            PlayerWins += stats.PlayerWins;
            ComputerWins += stats.ComputerWins;
            Draws += stats.Draws;

            ComputerWeight += stats.ComputerWeight;
            PlayerWeight += stats.PlayerWeight;

            CorrectPred += stats.CorrectPred;

            double Total = PlayerWins + ComputerWins + Draws;

            if (Total < 1)
                return;
            
            double PlayerAvg = PlayerWins / Total * 100.0;
            double ComputerAvg = ComputerWins / Total * 100.0;
            double DrawAvg = Draws / Total * 100.0;

            double PredictAvg = CorrectPred / Total * 100.0;

            Turns += stats.Turns;
            double avgturns = Turns / Total;

            // Update labels
            lbl_cwin_val.Text = String.Format("{0} ({1:0.###}%)", ComputerWins, ComputerAvg);
            lbl_pwins_val.Text = String.Format("{0} ({1:0.###}%)", PlayerWins, PlayerAvg);
            lbl_draws_val.Text = String.Format("{0} ({1:0.##}%)", Draws, DrawAvg);
            
            lbl_compweight_val.Text = String.Format("{0:0.##}", ComputerWeight / Total);
            lbl_playerweight_val.Text = String.Format("{0:0.##}", PlayerWeight / Total);
            lbl_winnerweight_val.Text = String.Format("{0:0.#}%", PredictAvg);
            
            lbl_sims_val.Text = String.Format("{0}", Total);

            lbl_turns_val.Text = String.Format("{0:0}", avgturns);
            lbl_gametime_val.Text = String.Format("{0:0} μs", gameTime.TotalMilliseconds * 1000 / Total);
        }
    }
}
