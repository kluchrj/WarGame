﻿using System;

namespace WarGUI
{
    /// <summary>
    /// Stores the following info about a set of games:
    ///     Player / Computer wins and Draws
    ///     Overall card weight of both players
    ///     Start time of the operation
    ///     Total number of turns taken
    /// </summary>
    class GameStats
    {
        // Variables
        private int playerwins;
        private int computerwins;
        private int draws;
        private int correctpred;
        private int playerweight;
        private int computerweight;

        private DateTime Start;
        private double turns;

        // Properties
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
        public int Total
        {
            get { return playerwins + computerwins + draws; }
        }

        public DateTime Time {
            get { return Start; }
        }
        public double Turns {
            get { return turns; }
            set { turns = value; }
        }

        public GameStats() { }

        public GameStats(DateTime time)
        {
            Start = time;
        }

        public void AddToStats(GameStats s)
        {
            Start = s.Time;
            this.ComputerWeight += s.ComputerWeight;
            this.ComputerWins += s.ComputerWins;
            this.CorrectPred += s.CorrectPred;
            this.Draws += s.Draws;
            this.PlayerWeight += s.PlayerWeight;
            this.PlayerWins += s.PlayerWins;
            this.Turns += s.Turns;
        }

        public void ClearStats()
        {
            playerwins = 0;
            computerwins = 0;
            draws = 0;
            correctpred = 0;
            playerweight = 0;
            computerweight = 0;
            
            turns = 0;
        }
    }
}
