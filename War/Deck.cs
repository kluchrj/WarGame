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

            switch (name)
            {
                case CardNames.Two:
                    value = 2;
                    break;
                case CardNames.Three:
                    value = 3;
                    break;
                case CardNames.Four:
                    value = 4;
                    break;
                case CardNames.Five:
                    value = 5;
                    break;
                case CardNames.Six:
                    value = 6;
                    break;
                case CardNames.Seven:
                    value = 7;
                    break;
                case CardNames.Eight:
                    value = 8;
                    break;
                case CardNames.Nine:
                    value = 9;
                    break;
                case CardNames.Ten:
                    value = 10;
                    break;
                case CardNames.Jack:
                    value = 11;
                    break;
                case CardNames.Queen:
                    value = 12;
                    break;
                case CardNames.King:
                    value = 13;
                    break;
                case CardNames.Ace:
                    value = 14;
                    break;
            }
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
