using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace War
{
    public enum CardSuits { Clubs, Diamonds, Hearts, Spades }
    public enum CardNames { Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King }

    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Use jokers in the deck? (y/n) ");
            string input = Console.ReadLine();

            bool joker = false;
            if (string.Compare(input, "y", true) == 0)
                joker = true;

            // Set up the Deck
            List<Deck> CardDeck = new List<Deck>();

            PopulateDeck(CardDeck, joker);
            CardDeck.Shuffle();

            // Set up the Player and Computer's decks
            Queue<Deck> PlayerDeck = new Queue<Deck>();
            Queue<Deck> ComputerDeck = new Queue<Deck>();
            List<Deck> Temp = new List<Deck>();

            DealCards(PlayerDeck, ComputerDeck, CardDeck);

            Console.Write("\nPrint decks? (y/n) ");
            input = Console.ReadLine();

            if (String.Compare(input, "y", true) == 0)
            {
                Console.WriteLine("\n\tPlayer:\t\t\t\tComputer:");
                Console.WriteLine("\t---------------------------------------------------");
                PrintDecks<Deck>(PlayerDeck, ComputerDeck);
                Console.WriteLine();
            }

            // Main game loop
            while (PlayerDeck.Count > 0 && ComputerDeck.Count > 0)
            {
                Console.WriteLine("--------------------------");
                Console.WriteLine("Player: {0} cards", PlayerDeck.Count);
                Console.WriteLine("Computer: {0} cards", ComputerDeck.Count);
                Console.WriteLine("--------------------------");
                
                Deck PlayerCard = PlayerDeck.Dequeue();
                Deck ComputerCard = ComputerDeck.Dequeue();

                Console.WriteLine("Player's Card: {0}", PlayerCard);
                Console.WriteLine("Computer's Card: {0}", ComputerCard);

                Temp.Add(PlayerCard);
                Temp.Add(ComputerCard);

                if (PlayerCard.Value > ComputerCard.Value)
                {
                    Console.WriteLine("\nThe Player takes the hand");
                    CombineDecks(PlayerDeck, Temp);
                }
                else if (ComputerCard.Value > PlayerCard.Value)
                {
                    Console.WriteLine("\nThe Computer takes the hand");
                    CombineDecks(ComputerDeck, Temp);
                }
                else
                {
                    Console.WriteLine("\nTie");
                    Console.ReadLine();

                    TieBreaker(PlayerDeck, ComputerDeck, Temp);
                }

                Temp.Clear();
                Console.ReadLine();
            }

            if (PlayerDeck.Count == 0 && ComputerDeck.Count == 0) // There was a tie for every card!
                Console.WriteLine("\nIt's a draw!");
            else if (ComputerDeck.Count == 0)
                Console.WriteLine("\nThe Player wins!");
            else
                Console.WriteLine("\nThe Computer wins!");

            Console.ReadLine();
        }

        static void TieBreaker(Queue<Deck> PlayerDeck, Queue<Deck> ComDeck, List<Deck> TempDeck)
        {
            // If a player runs out of cards they loose
            if (PlayerDeck.Count == 0 ||  ComDeck.Count == 0)
                return;

            // Each player puts down 2 cards, and reveals a third
            if (PlayerDeck.Count > 2 && ComDeck.Count > 2)
            {
                TempDeck.Add(PlayerDeck.Dequeue());
                TempDeck.Add(PlayerDeck.Dequeue());
                TempDeck.Add(ComDeck.Dequeue());
                TempDeck.Add(ComDeck.Dequeue());

                Console.WriteLine("Each player puts down two cards\n");
            }
            else if (PlayerDeck.Count > 1 && ComDeck.Count > 1)
            {
                // if a player only has 2 cards, put down 1 and reveal the other
                TempDeck.Add(PlayerDeck.Dequeue());
                TempDeck.Add(ComDeck.Dequeue());

                Console.WriteLine("Each player puts down one card\n");
            }

            Deck PlayerCard = PlayerDeck.Dequeue();
            Deck ComputerCard = ComDeck.Dequeue();

            Console.WriteLine("Player's Card: {0}", PlayerCard);
            Console.WriteLine("Computer's Card: {0}", ComputerCard);

            TempDeck.Add(PlayerCard);
            TempDeck.Add(ComputerCard);

            if (PlayerCard.Value > ComputerCard.Value)
            {
                Console.WriteLine("\nThe Player wins the tie and gets:");
                PrintList<Deck>(TempDeck);

                CombineDecks(PlayerDeck, TempDeck);
                return;
            }
            else if (ComputerCard.Value > PlayerCard.Value)
            {
                Console.WriteLine("\nThe Computer wins the tie and gets:");
                PrintList<Deck>(TempDeck);

                CombineDecks(ComDeck, TempDeck);
                return;
            }
            else
            {
                Console.WriteLine("\nTie\n");
                Console.ReadLine();

                // Recurse until there is a looser
                TieBreaker(PlayerDeck, ComDeck, TempDeck);
            }
        }

        static void PopulateDeck(List<Deck> Cards, bool Joker)
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

        static void DealCards(Queue<Deck> PlayerDeck, Queue<Deck> ComDeck, List<Deck> CardDeck)
        {
            for (int i = 0; i < CardDeck.Count; i++)
            {
                if (i % 2 == 0)
                    ComDeck.Enqueue(CardDeck[i]);
                else
                    PlayerDeck.Enqueue(CardDeck[i]);
            }
        }

        static void CombineDecks(Queue<Deck> Deck, List<Deck> ToAdd)
        {
            ToAdd.Shuffle();

            foreach (Deck c in ToAdd)
                Deck.Enqueue(c);
        }

        static void PrintList<T>(List<T> AList)
        {
            foreach (T element in AList)
                Console.WriteLine("\t{0}", element);
        }

        static void PrintDecks<T>(Queue<T> AQueue, Queue<T> BQueue)
        {
            if (AQueue.Count != BQueue.Count)
                return;

            for (int i = 0; i < AQueue.Count; i++)
            {
                if (AQueue.ElementAt(i).ToString().Length < 6)
                    Console.Write("\t{0}\t\t", AQueue.ElementAt(i));
                else if (AQueue.ElementAt(i).ToString().Length < 16)
                    Console.Write("\t{0}\t", AQueue.ElementAt(i));
                else
                    Console.Write("\t{0}", AQueue.ElementAt(i));
                Console.Write("\t\t{0}\n", BQueue.ElementAt(i));
            }
        }
    }
}
