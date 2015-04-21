﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WarGUI
{
    public partial class War : Form
    {
        private int PlayerWins;
        private int ComputerWins;
        private int Draws;
        private int CorrectPred;
        private int IncorrectPred;

        private double PlayerAvg;
        private double ComputerAvg;
        private double DrawAvg;
        private double PredictAvg;

        private int WeightedToPlayer;
        private int WeightedToComputer;

        private double Turns;

        public War()
        {
            InitializeComponent();

            num_iterations.Maximum = long.MaxValue;
        }

        private void War_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Stop the worker thread when the form is closing
            if (WarWorker.IsBusy)
                WarWorker.CancelAsync();
        }

        // UI
        //----------------------------------------------------------------------------------------------------------

        private void btn_start_Click(object sender, EventArgs e)
        {
            if (!WarWorker.IsBusy)
            {
                btn_start.Text = "&Stop";

                num_iterations.Enabled = false;
                chk_jokers.Enabled = false;
                chk_fastshuffle.Enabled = false;

                btn_start.Focus();

                lbl_status.Text = String.Format("Simulating (0/{0})", num_iterations.Value);
                LogMessage(String.Format("Simulating {0} games...", num_iterations.Value));

                long i = (long)num_iterations.Value;

                WarWorker.RunWorkerAsync(i);
            }
            else
            {
                btn_start.Text = "&Start";
                WarWorker.CancelAsync();
            }
        }

        private void btn_clear_Click(object sender, EventArgs e)
        {
            ComputerAvg = ComputerWins = Draws = 0;
            PlayerAvg = PlayerWins = 0;
            CorrectPred = IncorrectPred = 0;
            WeightedToComputer = WeightedToPlayer = 0;

            lbl_cwin_val.Text = "0 (0%)";
            lbl_pwins_val.Text = "0 (0%)";
            lbl_draws_val.Text = "0 (0%)";

            lbl_compweight_val.Text = "0 (0%)";
            lbl_playerweight_val.Text = "0 (0%)";
            lbl_winnerweight_val.Text = "0 (0%)";

            lbl_sims_val.Text = "0";

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
            {
                btn_start_Click(sender, e);
            }
        }

        private void chk_jokers_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                btn_start_Click(sender, e);
            }
        }

        private void chk_fastshuffle_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return)
            {
                btn_start_Click(sender, e);
            }
        }

        void LogMessage(String Msg)
        {
            DateTime now = DateTime.Now;
            lbox_log.Items.Add(String.Format("[{0}] {1}", now, Msg));
            lbox_log.TopIndex = lbox_log.Items.Count - 1; // Scroll to bottom
        }

        // Worker functions
        //----------------------------------------------------------------------------------------------------------

        private void WarWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Record when the operation started
            DateTime Start = DateTime.Now;

            // Record When we last updated the UI
            DateTime LastUpdated = DateTime.Now;

            // Create decks for each player, plus a deck to draw cards from
            List<Deck> CardDeck = new List<Deck>();
            Queue<Deck> PlayerDeck = new Queue<Deck>();
            Queue<Deck> ComputerDeck = new Queue<Deck>();

            // Get the number of iterations from the argument
            long j = (long)e.Argument;
            long i = 0;

            bool fastShuffle = chk_fastshuffle.Checked;
            
            // Add cards to the main deck
            PopulateDeck(CardDeck, chk_jokers.Checked);

            // Create our stats class to store information about this iteration of games
            StatsInfo stat = new StatsInfo(Start);
            
            while (i < j && !WarWorker.CancellationPending)
            {
                GameInfo result = RunGame(CardDeck, PlayerDeck, ComputerDeck, fastShuffle);
                
                if (result.GetWiner == Winner.Player)
                {
                    stat.PlayerWins++;
                    if (result.PlayerWeight > result.ComputerWeight)
                        stat.CorrectPred++;
                    else
                        stat.IncorrectPred++;
                }
                else if (result.GetWiner == Winner.Computer)
                {
                    stat.ComputerWins++;
                    if (result.ComputerWeight > result.PlayerWeight)
                        stat.CorrectPred++;
                    else
                        stat.IncorrectPred++;
                }
                else
                    stat.Draws++;

                if (result.PlayerWeight > result.ComputerWeight)
                    stat.WeightedToPlayer++;
                else if (result.ComputerWeight > result.PlayerWeight)
                    stat.WeightedToComputer++;

                stat.Turns += (double)result.Turns;

                PlayerDeck.Clear();
                ComputerDeck.Clear();
                i++;

                // Only update the UI every 15ms, or it can lock up
                TimeSpan diff = DateTime.Now - LastUpdated;
                if (diff.TotalMilliseconds > 15)
                {
                    double precentage = ((double)i / (double)j) * 100.0;
                    WarWorker.ReportProgress((int)precentage, i);
                    LastUpdated = DateTime.Now;
                }
            }

            if (WarWorker.CancellationPending)
                e.Cancel = true;

            e.Result = stat;
        }

        private void WarWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {   
            lbl_status.Text = string.Format("Simulating {0}% ({1}/{2})", e.ProgressPercentage, e.UserState, num_iterations.Value);

            // Don't update the progress bar if we've closed the application
            if (!pbar_progress.IsDisposed)
                pbar_progress.Value = (int)e.ProgressPercentage;
        }

        private void WarWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled && e.Error == null)
            {
                StatsInfo result = (StatsInfo)e.Result;

                TimeSpan timeDiff = DateTime.Now - result.Time;

                double time = timeDiff.TotalSeconds;

                if (time > 1.0)
                    LogMessage(String.Format("Completed in {0:0.#} seconds", time));
                else
                    LogMessage(String.Format("Completed in {0:0} ms", timeDiff.TotalMilliseconds));

                ComputeAverages(result);
            }
            else if (e.Cancelled)
                LogMessage("Cancelled");
            else
                LogMessage("Error");

            lbl_status.Text = "Ready";
            btn_start.Text = "&Start";

            num_iterations.Enabled = true;
            chk_jokers.Enabled = true;
            chk_fastshuffle.Enabled = true;

            if (!pbar_progress.IsDisposed)
                pbar_progress.Value = 0;
        }

        // Game functions
        //----------------------------------------------------------------------------------------------------------

        private GameInfo RunGame(List<Deck> CardDeck, Queue<Deck> PlayerDeck, Queue<Deck> ComputerDeck, bool FastShuffle)
        {
            if (FastShuffle)
                CardDeck.FastShuffle();
            else
                CardDeck.Shuffle();

            DealCards(PlayerDeck, ComputerDeck, CardDeck);

            // Calculate hand weight for each player
            int PlayerWeight = 0, ComputerWeight = 0;

            foreach (Deck c in PlayerDeck)
                PlayerWeight += c.Value;

            foreach (Deck c in ComputerDeck)
                ComputerWeight += c.Value;

            // Main game loop
            int turns = 0;

            while (PlayerDeck.Count > 0 && ComputerDeck.Count > 0)
            {
                turns++;

                // Grab a card from each player
                Deck PlayerCard = PlayerDeck.Dequeue();
                Deck ComputerCard = ComputerDeck.Dequeue();

                // Check to see who's card has a higher value
                if (PlayerCard.Value > ComputerCard.Value)
                {
                    PlayerDeck.Enqueue(PlayerCard);
                    PlayerDeck.Enqueue(ComputerCard);
                }
                else if (ComputerCard.Value > PlayerCard.Value)
                {
                    ComputerDeck.Enqueue(ComputerCard);
                    ComputerDeck.Enqueue(PlayerCard);
                }
                else // tie
                {
                    List<Deck> Temp = new List<Deck>();
                    Temp.Add(ComputerCard);
                    Temp.Add(PlayerCard);

                    TieBreaker(PlayerDeck, ComputerDeck, Temp);
                }
            }

            if (PlayerDeck.Count == 0 && ComputerDeck.Count == 0) // There was a tie for every card!
                return new GameInfo(Winner.Draw, ComputerWeight, PlayerWeight, turns);
            else if (PlayerDeck.Count == 0)
                return new GameInfo(Winner.Computer, ComputerWeight, PlayerWeight, turns);
            else
                return new GameInfo(Winner.Player, ComputerWeight, PlayerWeight, turns);
        }

        void TieBreaker(Queue<Deck> PlayerDeck, Queue<Deck> ComDeck, List<Deck> TempDeck)
        {
            // If a player runs out of cards they loose the tie, and the game
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
                // if a player has less than 2 cards, don't put down 3
                TempDeck.Add(PlayerDeck.Dequeue());
                TempDeck.Add(ComDeck.Dequeue());
            }

            Deck PlayerCard = PlayerDeck.Dequeue();
            Deck ComputerCard = ComDeck.Dequeue();

            if (PlayerCard.Value > ComputerCard.Value)
            {
                PlayerDeck.Enqueue(PlayerCard);
                PlayerDeck.Enqueue(ComputerCard);
                CombineDecks(PlayerDeck, TempDeck);
            }
            else if (ComputerCard.Value > PlayerCard.Value)
            {
                ComDeck.Enqueue(ComputerCard);
                ComDeck.Enqueue(PlayerCard);
                CombineDecks(ComDeck, TempDeck);
            }
            else // tie (again)
            {
                TempDeck.Add(ComputerCard);
                TempDeck.Add(PlayerCard);

                // Recurse until there is a looser
                TieBreaker(PlayerDeck, ComDeck, TempDeck);
            }
        }

        void PopulateDeck(List<Deck> Cards, bool Joker)
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

        void DealCards(Queue<Deck> PlayerDeck, Queue<Deck> ComDeck, List<Deck> CardDeck, bool DealPlayerFirst = true)
        {
            for (int i = 0; i < CardDeck.Count; i++)
                if (i % 2 == 0)
                    if (DealPlayerFirst)
                        PlayerDeck.Enqueue(CardDeck[i]);
                    else
                        ComDeck.Enqueue(CardDeck[i]);
                else
                    if (DealPlayerFirst)
                        ComDeck.Enqueue(CardDeck[i]);
                    else
                        PlayerDeck.Enqueue(CardDeck[i]);
        }

        void CombineDecks(Queue<Deck> Deck, List<Deck> ToAdd)
        {
            foreach (Deck c in ToAdd)
                Deck.Enqueue(c);
        }

        // Stats
        //----------------------------------------------------------------------------------------------------------

        void ComputeAverages(StatsInfo stats)
        {
            PlayerWins += stats.PlayerWins;
            ComputerWins += stats.ComputerWins;
            Draws += stats.Draws;

            WeightedToComputer += stats.WeightedToComputer;
            WeightedToPlayer += stats.WeightedToPlayer;

            CorrectPred += stats.CorrectPred;
            IncorrectPred += stats.IncorrectPred;

            double Total = PlayerWins + ComputerWins + Draws;

            if (Total < 1)
                return;
            
            PlayerAvg = PlayerWins / Total * 100.0;
            ComputerAvg = ComputerWins / Total * 100.0;
            DrawAvg = Draws / Total * 100.0;

            Total = CorrectPred + IncorrectPred;
            PredictAvg = CorrectPred / Total * 100.0;

            // Update labels
            lbl_cwin_val.Text = String.Format("{0} ({1:0.##}%)", ComputerWins, ComputerAvg);
            lbl_pwins_val.Text = String.Format("{0} ({1:0.##}%)", PlayerWins, PlayerAvg);
            lbl_draws_val.Text = String.Format("{0} ({1:0.##}%)", Draws, DrawAvg);
            
            if (WeightedToComputer > 0 && WeightedToPlayer > 0)
            {
                lbl_compweight_val.Text = String.Format("{0} ({1:0.#}%)", WeightedToComputer, WeightedToComputer / Total * 100.0);
                lbl_playerweight_val.Text = String.Format("{0} ({1:0.#}%)", WeightedToPlayer, WeightedToPlayer / Total * 100);
                lbl_winnerweight_val.Text = String.Format("{0} ({1:0.#}%)", CorrectPred, PredictAvg);
            }

            lbl_sims_val.Text = String.Format("{0}", Total);

            Turns += stats.Turns;
            double avgturns = Turns / Total;

            lbl_turns_val.Text = String.Format("{0:0}", avgturns);
        }
    }
}
