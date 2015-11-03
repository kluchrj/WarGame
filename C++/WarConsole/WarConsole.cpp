#include <algorithm>
#include <deque>
#include <random>
#include <vector>

#include "conio.h"

#include "Card.h"

using namespace std;

static void PopulateDeck(vector<Card*>& Deck, const bool Joker = false);
static void DealCards(deque<Card*>& PlayerDeck, deque<Card*>& ComDeck, const vector<Card*>& CardDeck);
static void PrintDecks(const deque<Card*>& AQueue, const deque<Card*>& BQueue);
static void CombineDecks(deque<Card*>& Deck, vector<Card*>& ToAdd);
static void TieBreaker(deque<Card*>& PlayerDeck, deque<Card*>& ComDeck, vector<Card*>& TempDeck);
static void PrintList(const vector<Card*>& AList);

int main()
{
	cout << "Use jokers in the deck? (y/n) ";

	string input;
	cin >> input;

	bool jokers = false;
	if (tolower(input[0]) == 'y')
		jokers = true;

	// Set up the Deck
	vector<Card*> MasterDeck;

	PopulateDeck(MasterDeck, jokers);

	auto engine = default_random_engine{};
	shuffle(MasterDeck.begin(), MasterDeck.end(), engine);

	// Set up the Player and Computer's decks
	deque<Card*> PlayerDeck;
	deque<Card*> ComputerDeck;
	vector<Card*> TempDeck;

	DealCards(PlayerDeck, ComputerDeck, MasterDeck);
	cout << "\nPrint decks? (y/n) ";
	cin >> input;


	if (tolower(input[0]) == 'y')
	{
		cout << "\n\tPlayer:\t\t\t\tComputer:" << endl
			<< "\t---------------------------------------------------" << endl;

		PrintDecks(PlayerDeck, ComputerDeck);
	}

	// Main game loop
	while (PlayerDeck.size() > 0 && ComputerDeck.size() > 0)
	{
		cout << endl << "--------------------------" << endl
			<< "Player: " << PlayerDeck.size() << " cards" << endl
			<< "Computer: " << ComputerDeck.size() << " cards" << endl
			<< "--------------------------" << endl;

		Card * PlayerCard = PlayerDeck[0];
		PlayerDeck.pop_front();

		Card * ComputerCard = ComputerDeck[0];
		ComputerDeck.pop_front();

		cout << "Player's Card: " << *PlayerCard << endl
			<< "Computer's Card: " << *ComputerCard << endl;

		TempDeck.push_back(PlayerCard);
		TempDeck.push_back(ComputerCard);

		if (PlayerCard->Value() > ComputerCard->Value())
		{
			cout << "\nThe Player takes the hand" << endl;
			CombineDecks(PlayerDeck, TempDeck);
		}
		else if (ComputerCard->Value() > PlayerCard->Value())
		{
			cout << "\nThe Computer takes the hand" << endl;
			CombineDecks(ComputerDeck, TempDeck);
		}
		else
		{
			cout << "\nTie" << endl;
			_getch(); // pause

			TieBreaker(PlayerDeck, ComputerDeck, TempDeck);
		}

		_getch();
	}

	if (PlayerDeck.size() == 0 && ComputerDeck.size() == 0) // There was a tie for every card!
		cout << "\nIt's a draw!" << endl;
	else if (ComputerDeck.size() == 0)
		cout << "\nThe Player wins!" << endl;
	else
		cout << "\nThe Computer wins!" << endl;

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

		cout << "Each player puts down two cards\n" << endl;
	}
	else if (PlayerDeck.size() > 1 && ComDeck.size() > 1)
	{
		// if a player only has 2 cards, put down 1 and reveal the other
		TempDeck.push_back(PlayerDeck[0]);
		TempDeck.push_back(ComDeck[0]);
		PlayerDeck.pop_front();
		ComDeck.pop_front();
		
		cout << "Each player puts down one card\n" << endl;
	}
	
	Card * PlayerCard = PlayerDeck[0];
	Card * ComputerCard = ComDeck[0];
	PlayerDeck.pop_front();
	ComDeck.pop_front();

	cout << "Player's Card: " << *PlayerCard << endl;
	cout << "Computer's Card: " << *ComputerCard << endl;

	TempDeck.push_back(PlayerCard);
	TempDeck.push_back(ComputerCard);

	if (PlayerCard->Value() > ComputerCard->Value())
	{
		cout << "\nThe Player wins the tie and gets:" << endl;
		PrintList(TempDeck);

		CombineDecks(PlayerDeck, TempDeck);
		return;
	}
	else if (ComputerCard->Value() > PlayerCard->Value())
	{
		cout << "\nThe Computer wins the tie and gets:" << endl;
		PrintList(TempDeck);

		CombineDecks(ComDeck, TempDeck);
		return;
	}
	else
	{
		cout << "\nTie (again)\n" << endl;
		_getch();

		// Recurse until there is a looser
		TieBreaker(PlayerDeck, ComDeck, TempDeck);
	}
}

static void PopulateDeck(vector<Card*>& Deck, const bool Joker)
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
	// We shuffle the pool of cards to prevent an instance
	// where a game can loop forever with no victor

	// Randomness isn't really important here, just speed
	// TODO: do this with shuffle, random_shuffle is deprecated in C++14
	random_shuffle(ToAdd.begin(), ToAdd.end());

	for (unsigned int i = 0; i < ToAdd.size(); i++)
	{
		Deck.push_back(ToAdd[i]);
	}

	ToAdd.clear();
}

static void PrintList(const vector<Card*>& AList)
{
	for (unsigned int i = 0; i < AList.size(); i++)
		cout << '\t' << *AList[i] << endl;
}

static void PrintDecks(const deque<Card*>& AQueue, const deque<Card*>& BQueue)
{
	if (AQueue.size() != BQueue.size())
		return;

	for (unsigned int i = 0; i < AQueue.size(); i++)
	{
		cout << '\t';

		if (AQueue[i]->Print().size() < 6)
			cout << *AQueue[i] << "\t\t";
		else if (AQueue[i]->Print().size() < 16)
			cout << *AQueue[i] << "\t";
		else
			cout << *AQueue[i];

		cout << "\t\t" << *BQueue[i] << endl;
	}
}
