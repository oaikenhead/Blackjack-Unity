using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    // buttons
	public Button DealButton;
	public Button HitButton;
	public Button StandButton;
	public Button Bet5Button;
    public Button Bet10Button;
    public Button Bet15Button;
    public Button QuitButton;

	// tabletop text
	public Text StandText;
	public Text ChipsText;
	public Text PlayerHandText;
	public Text PlayerBetText;
	public Text DealerHandText;
	public Text DisplayText;
	public Text HandsWonText;
    public Text HandsLostText;

	// giving access to player and dealer hands
	public PlayCards Player;
	public PlayCards Dealer;

	// hiding dealer 2nd card
	public GameObject HideDealerCard;

	// table pot of bets
	int TablePot = 0;

	// times player has clicked stand, twice = gameover
	private int StandChecks = 0;
	int HandsWon = 0;
    int HandsLost = 0;

	// Use this for initialization
	void Start () {
        // button click listeners
        QuitButton.onClick.AddListener(() => QuitClick());
        DealButton.onClick.AddListener(() => DealClick());
		HitButton.onClick.AddListener(() => HitClick());
		StandButton.onClick.AddListener(() => StandClick());
		Bet5Button.onClick.AddListener(() => Bet5Click());
        Bet10Button.onClick.AddListener(() => Bet10Click());
        Bet15Button.onClick.AddListener(() => Bet15Click());

        // disabling buttons at start to prevent cheating
        // player must draw cards for the game to start
        HitButton.interactable = false;
        StandButton.interactable = false;
        Bet5Button.interactable = false;
        Bet10Button.interactable = false;
        Bet15Button.interactable = false;
    }

    // function called to quit the game
    // on click listener for quitting the game
    private void QuitClick() {
        // use application lib to quit game with button
        Application.Quit();
    }

    // clicking deal starts a new game, resetting the table
    private void DealClick() {
		// reseting table
		Player.TableReset();
		Dealer.TableReset();

		// updating table text && Buttons
        // can still bet after seeing initial hand (TWIST)
		DealerHandText.gameObject.SetActive(false);
		DisplayText.gameObject.SetActive(false);
		DealButton.gameObject.SetActive(false);
        HitButton.interactable = true;
        StandButton.interactable = true;
        Bet5Button.interactable = true;
        Bet10Button.interactable = true;
        Bet15Button.interactable = true;

        // show dealer hand count
        //DealerHandText.gameObject.SetActive(false);

		// shuffle the deck and start player && dealer hands
		GameObject.Find("Deck").GetComponent<Deck>().ShuffleDeck();
		Player.StartPlayHand();
		Dealer.StartPlayHand();

		// update player hand && dealer hand count
		PlayerHandText.text = "Hand: " + Player.HandValue.ToString();
		DealerHandText.text = "Dealer: " + Dealer.HandValue.ToString();
		HideDealerCard.GetComponent<Renderer>().enabled = true;

		// button visibility during game, disabling deal
		DealButton.gameObject.SetActive(false);
        HitButton.gameObject.SetActive(true);
		StandButton.gameObject.SetActive(true);
        Bet5Button.gameObject.SetActive(true);
        Bet10Button.gameObject.SetActive(true);
        Bet15Button.gameObject.SetActive(true);

        // game pot
        TablePot = 5;
		PlayerBetText.text = "Bet: $" + TablePot.ToString();
		Player.MoveChips(-5);
        ChipsText.text = "$" + Player.GetChips().ToString();
    }
	
    // player clicks hit, wanting another card for 21
	private void HitClick() { 
        // if there is less than 10 cards on the table
		if (Player.CardFlipIndex<=10) {
            // player draw card
            Player.GetCard();
			PlayerHandText.text = "Hand: " + Player.HandValue.ToString();

            // setting bet buttons to inactive after clicking hit
            Bet5Button.gameObject.SetActive(false);
            Bet10Button.gameObject.SetActive(false);
            Bet15Button.gameObject.SetActive(false);

            // if player hand value is greater than 20, end the hand
            if (Player.HandValue>20) {
                EndHand();
            }
        }
	}

    // player clicks stand, ending their turn and letting dealer draw cards
	private void StandClick() {
        // adding to stand checks
        StandChecks++;

        // setting buttons to be hidden
		HitButton.gameObject.SetActive(false);
        Bet5Button.gameObject.SetActive(false);
        Bet10Button.gameObject.SetActive(false);
        Bet15Button.gameObject.SetActive(false);

        // dealer hits card until bust or 21 then hand over
        DealerHitCard();
        EndHand();
	}

	// dictates when the dealer hits
	private void DealerHitCard() {
        // while dealer hand value is less than 16, and there are less than 10 cards on table
        // draw card and update hidden hand text
		while (Dealer.HandValue<16 && Dealer.CardFlipIndex<10) {
            // dealer draws card
			Dealer.GetCard();
			DealerHandText.text = "Dealer: " + Dealer.HandValue.ToString();

            // if dealer hand is greater than 20, end the hand
            if (Dealer.HandValue > 20) {
                EndHand();
            }
		}
	}

	// determines winner of hand, ends the hand
	void EndHand() {
		// bool vars for player and dealer win/lose conditions
		bool WinP = Player.HandValue == 21;
		bool WinD = Dealer.HandValue == 21;
		bool BustP = Player.HandValue > 21;
		bool BustD = Dealer.HandValue > 21;
		bool End = true;
        bool AvailableChips = Player.CheckWallet();

        // GAME RULES //

        // player and dealer have not reached 21 or bust
        // stand has not been clicked twice
        // exit EndHand()
        // if stand has not been clicked AND both player and dealer have not won AND busted
        // return from EndHand() back to the game
        if (StandChecks < 1 && (!BustP && !BustD) && (!WinP && !WinD)) {
            return;
        }
        // if player loses all chips in any type of hand
        if (((AvailableChips == false) && (BustP || (!BustD && Dealer.HandValue>Player.HandValue))) || ((AvailableChips == false)&&(Dealer.HandValue == Player.HandValue))) {
            DisplayText.text = "All Chips Lost - Please Leave.";
            DealButton.interactable = false;
        }
        // dealer bust or player HandValue is > than dealer
        // no chips returned (TWIST)
        else if (BustP && BustD) {
            HandsLost++;
            HandsLostText.text = "Hands Lost: " + HandsLost.ToString();
            DisplayText.text = "Table Bust - No Chips Returned";
        }
        // player bust or deal bust and hand value is greater than player
        else if (BustP || (!BustD && (Dealer.HandValue > Player.HandValue))) {
            HandsLost++;
            HandsLostText.text = "Hands Lost: " + HandsLost.ToString();
            DisplayText.text = "You Lost!";
        }
        // player win conditions, dealer busts or player hand is greater than dealer
        else if (BustD || (Player.HandValue > Dealer.HandValue)) {
            HandsWon++;
            HandsWonText.text = "Hands Won: " + HandsWon.ToString();
            DisplayText.text = "Winner, Winner, Chicken Dinner!";
            // player is awarded double the table pot
            // the pot is only 1 players chips
            Player.MoveChips(TablePot *= 2);
        }
        // case of push, or draw, no chips returned (TWIST)
        else if (Dealer.HandValue == Player.HandValue) {
            DisplayText.text = "Push - No Chips Returned";
        }
        // the hand is not over
        else {
            End = false;
        }

		// if the hand has ended
		if (End) {
            // button visibility at end of hand
			DealButton.gameObject.SetActive(true);
			HitButton.gameObject.SetActive(false);
			StandButton.gameObject.SetActive(false);

            // dealer text && card visibility
            DisplayText.gameObject.SetActive(true);
			DealerHandText.gameObject.SetActive(true);
			HideDealerCard.GetComponent<Renderer>().enabled = false;
            
            // player chips are updated
			ChipsText.text = "$" + Player.GetChips().ToString();
			StandChecks = 0;
		}

		// END GAME RULES //

	}

    // button code for Bet $5
	void Bet5Click() {
        // using bet button to send the text from the button
		Text updateChips = Bet5Button.GetComponentInChildren(typeof(Text)) as Text;

        // removing dollar sign at beginning of bet button
		int Chips = int.Parse(updateChips.text.ToString().Remove(0, 1));

        // if player wallet is not empty, else if it is empty
        if (Player.CheckWallet() == true) {
            Player.MoveChips(-Chips);
            ChipsText.text = "$" + Player.GetChips().ToString();
            TablePot += (Chips);
            PlayerBetText.text = "Bet: $" + TablePot.ToString();
        } else if (Player.CheckWallet() == false) {
            Bet5Button.interactable = false;
            Bet10Button.interactable = false;
            Bet15Button.interactable = false;
        }
    }

    // button code for Bet $10
    void Bet10Click() {
        // using bet button to send the text from the button
        Text updateChips = Bet10Button.GetComponentInChildren(typeof(Text)) as Text;

        // removing dollar sign at beginning of bet button
        int Chips = int.Parse(updateChips.text.ToString().Remove(0, 1));
        
        // if player wallet is not empty, else if it is
        if (Player.CheckWallet() == true) {
            Player.MoveChips(-Chips);
            ChipsText.text = "$" + Player.GetChips().ToString();
            TablePot += (Chips);
            PlayerBetText.text = "Bet: $" + TablePot.ToString();
        } else if (Player.CheckWallet() == false) {
            Bet5Button.interactable = false;
            Bet10Button.interactable = false;
            Bet15Button.interactable = false;
        }
    }

    // button code for Bet $15
    void Bet15Click() {
        // using bet button to send the text from the button
        Text updateChips = Bet15Button.GetComponentInChildren(typeof(Text)) as Text;

        // removing dollar sign at beginning of bet button
        int Chips = int.Parse(updateChips.text.ToString().Remove(0, 1));

        // if player wallet is not empty, else if it is
        if (Player.CheckWallet() == true)
        {
            Player.MoveChips(-Chips);
            ChipsText.text = "$" + Player.GetChips().ToString();
            TablePot += (Chips);
            PlayerBetText.text = "Bet: $" + TablePot.ToString();
        } else if (Player.CheckWallet() == false) {
            Bet5Button.interactable = false;
            Bet10Button.interactable = false;
            Bet15Button.interactable = false;
        }
    }
}
