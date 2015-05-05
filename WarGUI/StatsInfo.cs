using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarGUI
{
    /// <summary>
    /// Stores the following info about a set of games:
    ///     Player / Computer wins and Draws
    ///     Predictions based on card weight
    ///     Start time of the operation
    ///     Weights of the player and computer
    /// </summary>
    class StatsInfo
    {
        private int playerwins;
        private int computerwins;
        private int draws;
        private int correctpred;

        private int playerweight;
        private int computerweight;

        private double turns;
        private DateTime Start;

        public int PlayerWins {
            get { return playerwins; }
            set { playerwins = value; }
        }
        public int ComputerWins {
            get { return computerwins; }
            set { computerwins = value; }
        }
        public int Draws {
            get { return draws; }
            set { draws = value; }
        }
        public int CorrectPred {
            get { return correctpred; }
            set { correctpred = value; }
        }
        public int PlayerWeight {
            get { return playerweight; }
            set { playerweight = value; }
        }
        public int ComputerWeight {
            get { return computerweight; }
            set { computerweight = value; }
        }

        public DateTime Time {
            get { return Start; }
        }

        public double Turns {
            get { return turns; }
            set { turns = value; }
        }

        public int Total {
            get { return playerwins + computerwins + draws; }
        }

        public StatsInfo() { }

        public StatsInfo(DateTime time)
        {
            Start = time;
        }

        public void AddToStats(StatsInfo s)
        {
            this.ComputerWeight += s.ComputerWeight;
            this.ComputerWins += s.ComputerWins;
            this.CorrectPred += s.CorrectPred;
            this.Draws += s.Draws;
            this.PlayerWeight += s.PlayerWeight;
            this.PlayerWins += s.PlayerWins;
            this.Turns += s.Turns;
        }
    }
}
