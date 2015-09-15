using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace War
{
    public abstract class Deck
    {
        protected int value;

        public int Value
        {
            get { return value; }
        }

        public Deck() { }
    }

    public class Card : Deck
    {
        private CardSuits suit;
        private CardNames name;

        public Card(CardSuits suit, CardNames name) 
        {
            this.suit = suit;
            this.name = name;

            value = (int)name;
        }

        public override String ToString()
        {
            return name + " of " + suit;
        }
    }

    public class Joker : Deck
    {
        private string name;

        public Joker()
        {
            name = "Joker";
            value = 15;
        }

        public override String ToString()
        {
            return name;
        }
    }
}
