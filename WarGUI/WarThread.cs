using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace WarGUI
{
    class WarThread
    {
        private long n;
        private StatsInfo stat;
        private List<Object> Args;

        public long Games { get { return n; } }
        public StatsInfo Stats { get { return stat; } }

        public WarThread(long Games, DateTime Dtime, List<Object> Arguments)
        {
            n = Games;
            stat = new StatsInfo(Dtime);
            Args = Arguments;
        }

        public void StartSim()
        {
            // Create decks for each player, plus a deck to draw cards from
            List<Deck> CardDeck = new List<Deck>();
            Queue<Deck> PlayerDeck = new Queue<Deck>();
            Queue<Deck> ComputerDeck = new Queue<Deck>();

            // Pick who to deal cards to first
            int dealFirst = (int)Args[1];
            bool deal = Convert.ToBoolean(dealFirst);

            // Add cards to the main deck
            PopulateDeck(CardDeck, (bool)Args[3]);

            long i = 0;
            while (i < n) // TODO: Add cancel function
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
                i++;
            }
        }

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
    }
}
