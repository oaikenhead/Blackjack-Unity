using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// this class is for all cards on the table
// cards are assigned a value according to face value
public class Card : MonoBehaviour {
	// setting card value to 0 for init
	public int value = 0;

	// getting value for card
	public int GetCardValue() { return value; }

	// setting card to new value
	public void SetCardValue(int NewCardValue) { value = NewCardValue; }

    // setting the new card sprite on the game scene
    public void SetCardSprite(Sprite newCardSprite) {
        gameObject.GetComponent<SpriteRenderer>().sprite = newCardSprite;
    }

	// Reset cards for next hand by bringing cardback to front
	public void CardReset() {
        // create a sprite called cardback that finds the card back in the deck
        Sprite CardBack = GameObject.Find("Deck").GetComponent<Deck>().GetBack();
        // set game object calling card reset to the card back
        gameObject.GetComponent<SpriteRenderer>().sprite = CardBack;
        // value set to 0 so card back can be found again
        value = 0;
	}
}
