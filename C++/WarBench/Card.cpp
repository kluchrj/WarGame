#include "Card.h"

FaceCard::FaceCard(CardSuit s, CardName n)
{
	suit = s;
	name = n;
}

std::string FaceCard::Print() const
{
	std::string strname;
	std::string strsuit;

	switch (suit)
	{
	case CardSuit::Clubs:
		strsuit = "Clubs";
		break;
	case CardSuit::Diamonds:
		strsuit = "Diamonds";
		break;
	case::CardSuit::Hearts:
		strsuit = "Hearts";
		break;
	case::CardSuit::Spades:
		strsuit = "Spades";
	}

	switch (name)
	{
	case CardName::Two :
		strname = "Two";
		break;
	case CardName::Three:
		strname = "Three";
		break;
	case CardName::Four:
		strname = "Four";
		break;
	case CardName::Five:
		strname = "Five";
		break;
	case CardName::Six:
		strname = "Six";
		break;
	case CardName::Seven:
		strname = "Seven";
		break;
	case CardName::Eight:
		strname = "Eight";
		break;
	case CardName::Nine:
		strname = "Nine";
		break;
	case CardName::Ten:
		strname = "Ten";
		break;
	case CardName::Jack:
		strname = "Jack";
		break;
	case CardName::Queen:
		strname = "Queen";
		break;
	case CardName::King:
		strname = "King";
		break;
	case CardName::Ace:
		strname = "Ace";
	}

	return strname + " of " + strsuit;
}