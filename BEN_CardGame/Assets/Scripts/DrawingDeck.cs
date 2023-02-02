using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingDeck : MonoBehaviour
{
    private const int DECK_SIZE = 22;
    private const int STARTING_DECK = 7;

    private Player player;
    private Computer computer;

    [SerializeField]
    private GameObject playingCard;

    [SerializeField]
    private Transform playerDeck;

    [SerializeField]
    private Transform computerDeck;

    [SerializeField]
    private Texture[] cardTextures;

    [SerializeField]
    private CardSlot[] playerCardSlots;

    [SerializeField]
    private CardSlot[] computerCardSlots;

    private readonly string[] cardColors = { "Red", "Blue", "Green", "Yellow" };

    private readonly string[] cardNumbers = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Wild", "DrawTwo" };

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        computer = FindObjectOfType<Computer>();
        GetComponent<BoxCollider>().enabled = false;
        player.playerDeck.Capacity = DECK_SIZE;
        StartCoroutine(StartingCards());
    }

    private IEnumerator StartingCards()
    {
        for (int i = 0; i < STARTING_DECK; i++)
        {
            yield return new WaitForSeconds(.35f);
            DrawCard(0);
            yield return new WaitForSeconds(.35f);
            DrawCard(1);
        }

        GetComponent<BoxCollider>().enabled = true;
    }

    public IEnumerator DealCards(int amount, int toDeck) {
        for (int i = 0; i < amount; i++) {
            yield return new WaitForSeconds(.35f);
            DrawCard(toDeck);
        }
    }

    // 0 == Player; 1 == Computer
    public void DrawCard(int drawer)
    {
        if (drawer == 0 && player.playerDeck.Count < DECK_SIZE)
        {
            Card card = Instantiate(playingCard, transform.position, Quaternion.Euler(0f, 270f, 0f)).GetComponent<Card>();
            card.GetComponent<AudioSource>().Play();

            RandomizeAttributes(card, true);

            player.playerDeck.Add(card);
            Vector2 slot = FindNextOpenSlot(card, drawer);
            StartCoroutine(card.MoveCard(slot, false));
        }
        else if (drawer == 1)
        {
            Card card = Instantiate(playingCard, transform.position, Quaternion.Euler(0f, 270f, 0f)).GetComponent<Card>();
            card.GetComponent<AudioSource>().Play();

            RandomizeAttributes(card, false);
            
            computer.computerDeck.Add(card);
            Vector2 slot = FindNextOpenSlot(card, drawer);
            StartCoroutine(card.MoveCard(slot, false));
        }
    }

    // Randomizes the color and number of a card
    // 'card' == the card to be randomized
    private void RandomizeAttributes(Card card, bool ownedByPlayer)
    {
        string chosenColor = cardColors[Random.Range(0, cardColors.Length)];
        string chosenNumber = cardNumbers[Random.Range(0, cardNumbers.Length)];
        string combinedStr = chosenColor + chosenNumber;
        Texture chosenText = null;

        if (chosenNumber != "Wild")
        {
            foreach (Texture texture in cardTextures)
            {
                if (combinedStr.Equals(texture.name))
                {
                    chosenText = texture;
                    break;
                }
            }
        }
        else
        {
            chosenText = cardTextures[40];
        }

        card.name = combinedStr;
        card.SetAttributes(chosenColor, chosenNumber, chosenText, ownedByPlayer);
    }

    /// <summary>
    /// Finds the next open slot in a deck for a card to move to
    /// </summary>
    /// <param name="card">The card to check for</param>
    /// <param name="deck">Which deck to search (0 = player; 1 = computer)</param>
    /// <returns>A Vector2 position to move to</returns>
    public Vector2 FindNextOpenSlot(Card card, int deck)
    {
        CardSlot[] selectedSlots = deck == 0 ? playerCardSlots : computerCardSlots;

        foreach (CardSlot slot in selectedSlots)
        {
            if (!slot.Filled)
            {
                slot.SetFilled(true);
                card.filledSpot = slot;
                card.transform.SetParent(slot.transform);
                return slot.transform.position;
            }
        }

        return Vector2.zero;
    }

    
    public bool CardsMoving(List<Card> deck)
    {
        foreach (Card card in deck)
        {
            if (card.IsMoving) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Shifts all cards in the deck to the next closest open slot
    /// </summary>
    /// <param name="deck">The deck to shift</param>
    public IEnumerator ShiftCards(int deck)
    {
        List<Card> chosenDeck = deck == 0 ? player.playerDeck : computer.computerDeck;

        foreach (Card card in chosenDeck)
        {
            card.filledSpot.SetFilled(false);
            card.filledSpot = null;
            Vector2 to = FindNextOpenSlot(card, deck);
            StartCoroutine(card.MoveCard(to, false));
            yield return new WaitForEndOfFrame();
        }
    }
}
