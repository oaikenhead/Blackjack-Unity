using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// deck class, this shuffles the cards and gets card values from the deck
public class Deck : MonoBehaviour {

	public Sprite[] Cards;
    int[] CardWeight = new int[53];
	int CardIndex = 0;

	// start by getting values from cards in deck
	void Start () {
		GetCardVals();
	}
	
    // get values from the deck in unity
	void GetCardVals() {
        int num = 0;
        // Loop to assign values to the cards
        for (int i=0; i<Cards.Length; i++) {
            num = i;
            
            // count up to the amout of cards, 52
            num %= 13;
            
            // if there is a remainder after x/13, use it
            // unless over 10, then use 10
            if(num > 10 || num == 0) {
                num = 10;
            }
            
            // card weight is assigned the remainder
            CardWeight[i] = num++;
        }
	}

    // shuffles the deck
    public void ShuffleDeck() {
        // swapping array positions as we are taking one off the length and decrementing as we move through the deck
        for (int i=Cards.Length-1; i>0; --i) {
            int j = Mathf.FloorToInt(Random.Range(0.0f, 1.0f) * Cards.Length-1) + 1;
            Sprite face = Cards[i];
            Cards[i] = Cards[j];
            Cards[j] = face;
            int NewCardValue = CardWeight[i];
            CardWeight[i] = CardWeight[j];
            CardWeight[j] = NewCardValue;
        }
        // set the card index back to 1
        CardIndex = 1;
    }

    // deals a single card when called, used by player and dealer
    public int DealSingleCard(Card Card) {
        // set dealt card with sprite in index
        Card.SetCardSprite(Cards[CardIndex]);
        // set the value
        Card.SetCardValue(CardWeight[CardIndex++]);
        // return the card drawn
        return Card.GetCardValue();
    }

    public Sprite GetBack() {
        // returns first postion of array, card back sprite
        return Cards[0];
    }
}
