namespace WarGUI
{
    public enum CardSuits { Spades, Diamonds, Clubs, Hearts }
    public enum CardNames { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace }

    public abstract class Deck
    {
        public abstract int Value { get; }
    }

    public class Card : Deck
    {
        private CardSuits suit;
        private CardNames name;

        public override int Value
        {
            get { return (int)name; }
        }

        public Card(CardSuits suit, CardNames name) 
        {
            this.suit = suit;
            this.name = name;
        }

        public override string ToString()
        {
            return name + " of " + suit;
        }
    }

    public class Joker : Deck
    {
        public Joker() { }

        public override int Value
        {
            get { return 15; }
        }

        public override string ToString()
        {
            return "Joker";
        }
    }
}
