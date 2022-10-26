using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayCards : MonoBehaviour {
	// init deck and card
	public Card Card;
	public Deck Deck;
	public int HandValue = 0;
	private int PlayerChips = 250;
	public GameObject[] Hand;
	public int CardFlipIndex = 0;
	List<Card> Aces = new List<Card>();
	
    // start the play hand by drawing two cards, both player and dealer
	public void StartPlayHand() {
        // draw two cards for player/dealer
		GetCard();
		GetCard();
	}

    // gets card  and checks its value
	public int GetCard() {
        // Get a card, use deal card to assign sprite and value to card on table
        int CardValue = Deck.DealSingleCard(Hand[CardFlipIndex].GetComponent<Card>());
        // Show card on game screen
        Hand[CardFlipIndex].GetComponent<Renderer>().enabled = true;
        // Add card value to running total of the hand
        HandValue += CardValue;
        // If value is 1, it is an ace
        if (CardValue == 1) {
            Aces.Add(Hand[CardFlipIndex].GetComponent<Card>());
        }
        // Cehck if we should use an 11 instead of a 1
        AcesMath();
        CardFlipIndex++;
        return HandValue;
	}

	// AceCheck
    // determines if ace will be 1 or 11
	public void AcesMath() {
        // for each ace card in the list of Aces
        foreach (Card ace in Aces) {
			// if the player busts, set ace to 11
            if (HandValue+10<22 && ace.GetCardValue() == 1) {
                // set card value to the hand
                ace.SetCardValue(11);
                HandValue += 10;
            } else if (HandValue>21 && ace.GetCardValue() == 11) {
                // if converting, adjust gameobject value and hand value
                ace.SetCardValue(1);
                HandValue -= 10;
            }
        }
	}

	// bet minimum amount of chips
	// adjust money
	public void MoveChips(int ChipNum) {
        if (PlayerChips!=0) {
            PlayerChips += ChipNum;
        }
	}

    // gets the chips as long as player chips are above 0
    public int GetChips() {
        //return PlayerChips;
        // will not keep drawing from player chips once at zero
        if (PlayerChips > 0) {
            return PlayerChips;
        }
        else {
            return 0;
        }
	}

    // checking if player chips are above zero before sending
    public int CheckChips() {
        if (PlayerChips > 0)
        {
            return GetChips();
        }
        else {
            return 0;
        }
    }

    // public function to check how many chips the player has (private)
    // if this is false, chips will not draw from wallet
    public bool CheckWallet() {
        if (PlayerChips > 0) {
            return true;
        }
        else {
            return false;
        }
    }

	// hides cards
	public void TableReset() {
        // while hand length is greater than i, increment
        for (int i=0; i<Hand.Length; i++) {
            // reset hand position and hide cards again
            Hand[i].GetComponent<Card>().CardReset();
            Hand[i].GetComponent<Renderer>().enabled = false;
        }

		// reset Card and Hand vals, aces list
        CardFlipIndex = 0;
        HandValue = 0;
        Aces = new List<Card>();
	}
}
