using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarGUI
{
    public enum Winner { Player, Computer, Draw }

    /// <summary>
    /// Stores the following info about a single game of war:
    ///     the winner
    ///     the card weight of the computer and player
    ///     the ammount of turns taken
    /// </summary>
    class GameInfo
    {
        private Winner Victor;
        private int CWeight;
        private int PWeight;
        private int turns;

        public Winner GetWiner
        {
            get { return Victor; }
        }

        public int ComputerWeight
        {
            get { return CWeight; }
        }

        public int PlayerWeight
        {
            get { return PWeight; }
        }

        public int Turns
        {
            get { return turns; }
        }

        public GameInfo(Winner victor, int ComputerWeight, int PlayerWeight, int NumTurns)
        {
            Victor = victor;
            CWeight = ComputerWeight;
            PWeight = PlayerWeight;
            turns = NumTurns;
        }
    }
}
