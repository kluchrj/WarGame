#include "conio.h"
#include <deque>
#include <vector>
#include <algorithm>
#include <Windows.h>

#include "Card.h"

using namespace std;

static void PopulateDeck(vector<Card*>& Deck, bool Joker = false);
static void DealCards(deque<Card*>& PlayerDeck, deque<Card*>& ComDeck, const vector<Card*>& CardDeck);
static void CombineDecks(deque<Card*>& Deck, vector<Card*>& ToAdd);
static void TieBreaker(deque<Card*>& PlayerDeck, deque<Card*>& ComDeck, vector<Card*>& TempDeck);

int main()
{
	cout << "Enter the number of games to simulate: ";
	unsigned int iterations;
	cin >> iterations;

	// Performace tracking
	LARGE_INTEGER StartingTime, EndingTime, ElapsedMicroseconds;
	LARGE_INTEGER Frequency;
	
	QueryPerformanceFrequency(&Frequency);
	QueryPerformanceCounter(&StartingTime);

	// Set up the Deck
	vector<Card*> MasterDeck;
	PopulateDeck(MasterDeck);

	// Stats
	unsigned int PlayerWins = 0;
	unsigned int ComputerWins = 0;
	unsigned int Draws = 0;

	for (unsigned int i = 0; i < iterations; i++)
	{
		random_shuffle(MasterDeck.begin(), MasterDeck.end()); // TODO: Is this random enough?

		// Set up the Player and Computer's decks
		deque<Card*> PlayerDeck;
		deque<Card*> ComputerDeck;
		vector<Card*> TempDeck;

		DealCards(PlayerDeck, ComputerDeck, MasterDeck);
	
		// Main game loop
		while (PlayerDeck.size() > 0 && ComputerDeck.size() > 0)
		{
			Card * PlayerCard = PlayerDeck[0];
			PlayerDeck.pop_front();

			Card * ComputerCard = ComputerDeck[0];
			ComputerDeck.pop_front();

			TempDeck.push_back(PlayerCard);
			TempDeck.push_back(ComputerCard);

			if (PlayerCard->Value() > ComputerCard->Value())
				CombineDecks(PlayerDeck, TempDeck);
			else if (ComputerCard->Value() > PlayerCard->Value())
				CombineDecks(ComputerDeck, TempDeck);
			else
				TieBreaker(PlayerDeck, ComputerDeck, TempDeck);
		}

		if (PlayerDeck.size() == 0 && ComputerDeck.size() == 0) // There was a tie for every card!
			Draws++;
		else if (ComputerDeck.size() == 0)
			PlayerWins++;
		else
			ComputerWins++;
	}

	// Compute performance results
	QueryPerformanceCounter(&EndingTime);
	ElapsedMicroseconds.QuadPart = EndingTime.QuadPart - StartingTime.QuadPart;

	ElapsedMicroseconds.QuadPart *= 1000000;
	ElapsedMicroseconds.QuadPart /= Frequency.QuadPart;
	
	cout << "Player wins: " << PlayerWins << endl
		<< "Computer wins: " << ComputerWins << endl
		<< "Draws: " << Draws << endl;

	cout << endl << "Completed in: ";

	if (ElapsedMicroseconds.QuadPart < 1000)
	{
		cout << ElapsedMicroseconds.QuadPart << " microseconds" << endl;
	}
	else if (ElapsedMicroseconds.QuadPart < 1e+6)
	{
		double ms = (double)ElapsedMicroseconds.QuadPart / 1000.0;
		cout << ms << " miliseconds" << endl;
	}
	else
	{
		double seconds = (double)ElapsedMicroseconds.QuadPart / (double)1e+6;
		cout << seconds << " seconds" << endl;
	}

	cout << "Avg microseconds / game: "
		<< (double)ElapsedMicroseconds.QuadPart / (double)iterations << endl;

	_getch();

	// Clean up the heap
	for (unsigned int i = 0; i < MasterDeck.size(); i++)
		delete MasterDeck[i];

    return 0;
}

static void TieBreaker(deque<Card*>& PlayerDeck, deque<Card*>& ComDeck, vector<Card*>& TempDeck)
{
	// If a player runs out of cards they loose
	if (PlayerDeck.size() == 0 || ComDeck.size() == 0)
		return;

	// Each player puts down 2 cards, and reveals a third
	if (PlayerDeck.size() > 2 && ComDeck.size() > 2)
	{
		TempDeck.push_back(PlayerDeck[0]);
		PlayerDeck.pop_front();
		TempDeck.push_back(PlayerDeck[0]);
		PlayerDeck.pop_front();
		TempDeck.push_back(ComDeck[0]);
		ComDeck.pop_front();
		TempDeck.push_back(ComDeck[0]);
		ComDeck.pop_front();
	}
	else if (PlayerDeck.size() > 1 && ComDeck.size() > 1)
	{
		// if a player only has 2 cards, put down 1 and reveal the other
		TempDeck.push_back(PlayerDeck[0]);
		TempDeck.push_back(ComDeck[0]);
		PlayerDeck.pop_front();
		ComDeck.pop_front();
	}
	
	Card * PlayerCard = PlayerDeck[0];
	Card * ComputerCard = ComDeck[0];
	PlayerDeck.pop_front();
	ComDeck.pop_front();

	TempDeck.push_back(PlayerCard);
	TempDeck.push_back(ComputerCard);

	if (PlayerCard->Value() > ComputerCard->Value())
	{
		CombineDecks(PlayerDeck, TempDeck);
		return;
	}
	else if (ComputerCard->Value() > PlayerCard->Value())
	{
		CombineDecks(ComDeck, TempDeck);
		return;
	}
	else
	{
		// Recurse until there is a looser
		TieBreaker(PlayerDeck, ComDeck, TempDeck);
	}
}

static void PopulateDeck(vector<Card*>& Deck, bool Joker)
{
	for (int i = CardSuit::Clubs; i <= CardSuit::Spades; i++)
		for (int j = CardName::Two; j <= CardName::Ace; j++)
			Deck.push_back(new FaceCard(static_cast<CardSuit>(i), static_cast<CardName>(j)));

	if (Joker)
	{
		Deck.push_back(new JokerCard());
		Deck.push_back(new JokerCard());
	}
}

static void DealCards(deque<Card*> &PlayerDeck, deque<Card*>& ComDeck, const vector<Card*>& CardDeck)
{
	for (unsigned int i = 0; i < CardDeck.size(); i++)
	{
		if (i % 2 == 0)
			ComDeck.push_back(CardDeck[i]);
		else
			PlayerDeck.push_back(CardDeck[i]);
	}
}

static void CombineDecks(deque<Card*>& Deck, vector<Card*>& ToAdd)
{
	// Shuffle the pool of cards to prevent ENDLESS WAR
	random_shuffle(ToAdd.begin(), ToAdd.end());

	for (unsigned int i = 0; i < ToAdd.size(); i++)
		Deck.push_back(ToAdd[i]);

	ToAdd.clear();
}