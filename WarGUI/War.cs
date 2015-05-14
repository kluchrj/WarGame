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

        // Keeps track of how often the UI is updated
        Stopwatch LastUpdated = new Stopwatch();

        // Overall progress for the current operation
        private long Progress;

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

                Arguments.Add((long)num_iterations.Value);  // Games
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

            lbl_gametime_val.Text = "0 µs";

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

        private void num_threads_KeyPress(object sender, KeyPressEventArgs e)
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

        private void War_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the worker thread when the form is closing
            if (WarWorker.IsBusy)
                WarWorker.CancelAsync();
        }
        # endregion

        #region Worker

        // Worker functions
        //----------------------------------------------------------------------------------------------------------

        private void WarWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Reset progress
            Progress = 0;

            // Start stopwatch
            LastUpdated.Start();

            // Record when the operation started
            DateTime Start = DateTime.Now;

            // Fetch the arguments array
            List<object> Args = (List<object>)e.Argument;

            // Get the number of threads
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

            // Start a parallel loop for each thread
            Parallel.For(0, WarThreads, i =>
            {
                StatsInfo stat = new StatsInfo(Start);

                // Create decks for each player, plus a deck to draw cards from
                List<Deck> CardDeck = new List<Deck>();
                Queue<Deck> PlayerDeck = new Queue<Deck>();
                Queue<Deck> ComputerDeck = new Queue<Deck>();

                // Pick who to deal cards to first
                int dealFirst = (int)Args[1];
                bool deal = Convert.ToBoolean(dealFirst);

                // Add cards to the main deck
                PopulateDeck(CardDeck, (bool)Args[3]);

                long j = 0;
                while (j < workload[i] && !WarWorker.CancellationPending)
                {
                    // Determine who to deal if random or every other was choosen
                    if (dealFirst == 2)
                        deal = !deal;
                    else if (dealFirst == 3)
                        deal = Convert.ToBoolean(ThreadSafeRandom.ThisThreadsRandom.Next(0, 2));

                    GameInfo result = RunGame(CardDeck, PlayerDeck, ComputerDeck, (bool)Args[2], deal);

                    if (result.GetWiner == Winner.Player)
                    {
                        stat.PlayerWins++;
                        if (result.PlayerWeight > result.ComputerWeight)
                            stat.CorrectPred++;
                    }
                    else if (result.GetWiner == Winner.Computer)
                    {
                        stat.ComputerWins++;
                        if (result.ComputerWeight > result.PlayerWeight)
                            stat.CorrectPred++;
                    }
                    else
                        stat.Draws++;

                    stat.ComputerWeight += result.ComputerWeight;
                    stat.PlayerWeight += result.PlayerWeight;

                    stat.Turns += (double)result.Turns;

                    PlayerDeck.Clear();
                    ComputerDeck.Clear();
                    j++;

                    if (j % 100 == 0)
                        UpdateProgress(100);
                }

                if (!WarWorker.CancellationPending)
                    Finish(stat);

                return;
            });

            if (WarWorker.CancellationPending)
                e.Cancel = true;
        }

        void UpdateProgress(long p)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<long>(UpdateProgress), new object[] { p });
                return;
            }

            Progress += p;

            // Only update the UI every so often (15ms), or it can lock up
            if (LastUpdated.ElapsedMilliseconds < 15)
                return;

            double _Progress = (double)Progress / (double)num_iterations.Value * 100;

            pbar_progress.Value = (int)_Progress;
            lbl_status.Text = string.Format("Simulating {0}% ({1}/{2})", (int)_Progress, Progress, num_iterations.Value);

            LastUpdated.Restart();
        }

        void Finish(StatsInfo s)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<StatsInfo>(Finish), new object[] { s });
                return;
            }

            OverallStats.AddToStats(s);
        }

        private void WarWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                // Get the elapsed time
                TimeSpan timeDiff = DateTime.Now - OverallStats.Time;
                gameTime += timeDiff;

                if (timeDiff.TotalSeconds > 1.0)
                    LogMessage(String.Format("Completed in {0:0.#} seconds", timeDiff.TotalSeconds));
                else
                    LogMessage(String.Format("Completed in {0:0} ms", timeDiff.TotalMilliseconds));

                UpdateStats(OverallStats);
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

            LastUpdated.Stop();

            if (!pbar_progress.IsDisposed)
                pbar_progress.Value = 0;
        }

        #endregion

        #region WarGameFunctions

        // War Game Functions
        //----------------------------------------------------------------------------------------------------------

        private GameInfo RunGame(List<Deck> CardDeck, Queue<Deck> PlayerDeck, Queue<Deck> ComputerDeck, bool FastShuffle, bool DealFirst)
        {
            if (FastShuffle)
                CardDeck.FastShuffle();
            else
                CardDeck.Shuffle();

            DealCards(PlayerDeck, ComputerDeck, CardDeck, DealFirst);

            // Calculate hand weight for each player
            int PlayerWeight = 0, ComputerWeight = 0;

            foreach (Deck c in PlayerDeck)
                PlayerWeight += c.Value - 8;

            foreach (Deck c in ComputerDeck)
                ComputerWeight += c.Value - 8;

            // Create temporary deck to store cards in
            List<Deck> Temp = new List<Deck>();

            // Main game loop
            ulong turns = 0;

            while (PlayerDeck.Count > 0 && ComputerDeck.Count > 0)
            {
                turns++;

                // Grab a card from each player
                Deck PlayerCard = PlayerDeck.Dequeue();
                Deck ComputerCard = ComputerDeck.Dequeue();

                Temp.Add(PlayerCard);
                Temp.Add(ComputerCard);

                // Check to see who's card has a higher value
                if (PlayerCard.Value > ComputerCard.Value)
                    CombineDecks(PlayerDeck, Temp);

                else if (ComputerCard.Value > PlayerCard.Value)
                    CombineDecks(ComputerDeck, Temp);

                else // tie
                    TieBreaker(PlayerDeck, ComputerDeck, Temp);

                Temp.Clear();
            }

            if (PlayerDeck.Count == 0 && ComputerDeck.Count == 0) // There was a tie for every card!
                return new GameInfo(Winner.Draw, ComputerWeight, PlayerWeight, turns);
            else if (ComputerDeck.Count == 0)
                return new GameInfo(Winner.Player, ComputerWeight, PlayerWeight, turns);
            else
                return new GameInfo(Winner.Computer, ComputerWeight, PlayerWeight, turns);
        }

        void TieBreaker(Queue<Deck> PlayerDeck, Queue<Deck> ComDeck, List<Deck> TempDeck)
        {
            // If a player runs out of cards they loose the tie (and the game)
            if (PlayerDeck.Count == 0 || ComDeck.Count == 0)
                return;

            // In a tie, each player should put down 3 cards and reveal the last
            if (PlayerDeck.Count > 2 && ComDeck.Count > 2)
            {
                TempDeck.Add(PlayerDeck.Dequeue());
                TempDeck.Add(PlayerDeck.Dequeue());
                TempDeck.Add(ComDeck.Dequeue());
                TempDeck.Add(ComDeck.Dequeue());
            }
            else if (PlayerDeck.Count > 1 && ComDeck.Count > 1)
            {
                // if a player has less than 3 cards, but more than one, put only 2 down
                TempDeck.Add(PlayerDeck.Dequeue());
                TempDeck.Add(ComDeck.Dequeue());
            }

            Deck PlayerCard = PlayerDeck.Dequeue();
            Deck ComputerCard = ComDeck.Dequeue();

            TempDeck.Add(PlayerCard);
            TempDeck.Add(ComputerCard);

            if (PlayerCard.Value > ComputerCard.Value)
                CombineDecks(PlayerDeck, TempDeck);

            else if (ComputerCard.Value > PlayerCard.Value)
                CombineDecks(ComDeck, TempDeck);

            else // tie (again)
                TieBreaker(PlayerDeck, ComDeck, TempDeck); // Recurse until there is a looser
        }

        void PopulateDeck(List<Deck> Cards, bool Joker = false)
        {
            foreach (CardSuits suit in Enum.GetValues(typeof(CardSuits)))
                foreach (CardNames name in Enum.GetValues(typeof(CardNames)))
                    Cards.Add(new Card(suit, name));

            if (Joker)
            {
                Cards.Add(new Joker());
                Cards.Add(new Joker());
            }
        }

        void DealCards(Queue<Deck> PlayerDeck, Queue<Deck> ComDeck, List<Deck> CardDeck, bool DealComputerFirst)
        {
            for (int i = 0; i < CardDeck.Count; i++)
            {
                if (i % 2 == 0)
                {
                    if (DealComputerFirst)
                        ComDeck.Enqueue(CardDeck[i]);
                    else
                        PlayerDeck.Enqueue(CardDeck[i]);
                }
                else
                {
                    if (DealComputerFirst)
                        PlayerDeck.Enqueue(CardDeck[i]);
                    else
                        ComDeck.Enqueue(CardDeck[i]);
                }
            }
        }

        void CombineDecks(Queue<Deck> Deck, List<Deck> ToAdd)
        {
            // Shuffle the input deck to prevent ENDLESS WAR(!) or biased results
            ToAdd.FastShuffle();

            foreach (Deck c in ToAdd)
                Deck.Enqueue(c);
        }

        #endregion

        #region Stats

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
            lbl_gametime_val.Text = String.Format("{0:0} µs", gameTime.TotalMilliseconds * 1000 / stats.Total);
        }

        #endregion
    }
}
