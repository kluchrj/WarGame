#include <iostream>
#include <string>

enum CardSuit { Clubs, Diamonds, Hearts, Spades };
enum CardName { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace };

class Card
{
	virtual std::ostream& doprint(std::ostream&) const = 0;
public:
	virtual ~Card() { }
	virtual int Value() = 0;
	virtual std::string Print() const = 0;
	friend std::ostream& operator<<(std::ostream& os, const Card& c) {
		return c.doprint(os);
	}
};

class FaceCard : public Card
{
	CardSuit suit;
	CardName name;

	std::ostream& doprint(std::ostream& os) const {
		return os << Print();
	}
public:
	FaceCard(CardSuit s, CardName n);
	~FaceCard() { }
	virtual int Value() { return name; }
	virtual std::string Print() const;
};

class JokerCard : public Card
{
	std::ostream& doprint(std::ostream& os) const {
		return os << "Joker";
	}
public:
	JokerCard() { }
	~JokerCard() { }
	virtual int Value() { return 15; }
	virtual std::string Print() const {
		return "Joker";
	}
};