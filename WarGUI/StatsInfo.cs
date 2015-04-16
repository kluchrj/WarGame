using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarGUI
{
    /// <summary>
    /// stores info about a set of games including
    ///     Player / Computer wins and Draws
    ///     Predictions based on card weight
    ///     Start time of the operation
    /// </summary>
    class StatsInfo
    {
        private int playerwins;
        private int computerwins;
        private int draws;
        private int correctpred;
        private int incorrectpred;

        private int weightedtoplayer;
        private int weightedtocomputer;

        private double turns;

        DateTime Start;

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
        public int IncorrectPred {
            get { return incorrectpred; }
            set { incorrectpred = value; }
        }
        public int WeightedToPlayer {
            get { return weightedtoplayer; }
            set { weightedtoplayer = value; }
        }
        public int WeightedToComputer {
            get { return weightedtocomputer; }
            set { weightedtocomputer = value; }
        }

        public DateTime Time {
            get { return Start; }
        }

        public double Turns {
            get { return turns; }
            set { turns = value; }
        }

        public StatsInfo(DateTime time)
        {
            Start = time;

            playerwins = 0;
            computerwins = 0;
            draws = 0;
            correctpred = 0;
            incorrectpred = 0;

            weightedtoplayer = 0;
            weightedtocomputer = 0;

            turns = 0;
        }
    }
}
