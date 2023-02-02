using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlacingDeck : MonoBehaviour
{
    private const int MAX_CARDS = 6;

    private Player player;
    private DrawingDeck drawingDeck;

    [SerializeField]
    private GameObject wildcardMenu;

    private List<Card> placedCards = new List<Card>();

    private Card topCard;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        drawingDeck = FindObjectOfType<DrawingDeck>();
    }

    public void PlaceCard(Card card, int fromDeck)
    {
        card.transform.parent = transform;
        StartCoroutine(card.MoveCard(new Vector3(0f, 0f, -.1f * placedCards.Count), false));
        StartCoroutine(RotateXTo(card, Random.Range(-45f, 45f)));
        card.GetComponent<AudioSource>().Play();
        placedCards.Add(card);
        topCard = card;

        if (placedCards.Count > 6) {
            Card bottomCard = placedCards[0];
            placedCards.RemoveAt(0);
            Destroy(bottomCard.gameObject);

            for (int i = 0; i < placedCards.Count - 1; i++) {
                placedCards[i].transform.position = new Vector3(0f, 0f, -.1f * i);
            }
        }

        if (card.Number == "DrawTwo") {
            if (fromDeck == 0) {
                StartCoroutine(drawingDeck.DealCards(2, 1));
            }
            else {
                StartCoroutine(drawingDeck.DealCards(2, 0));
            }
        }

        if (card.Number == "Wild" && fromDeck == 0) {
            // Show UI and wait for response
            wildcardMenu.SetActive(true);
            player.isTurn = false;
        }
        else if (fromDeck == 0) {
            player.isTurn = false;
            // START AI
        }
    }

    private IEnumerator RotateXTo(Card card, float x)
    {
        float val = 0f;
        float timer = 0f;
        while (timer < 1f)
        {
            val = Mathf.Lerp(0f, x, timer * 2.5f);
            card.transform.rotation = Quaternion.Euler(val, card.transform.eulerAngles.y, card.transform.eulerAngles.z);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Snaps card to rotation
        card.transform.rotation = Quaternion.Euler(x, card.transform.eulerAngles.y, card.transform.eulerAngles.z);
    }

    public bool CanPlaceCard(Card card)
    {
        if (!topCard) { return true; }

        string topCardColor = topCard.Color;
        string topCardNumber = topCard.Number;

        if (card.Number == "Wild") {
            return true;
        }

        return card.Color == topCardColor || card.Number == topCardNumber;
    }

    public void WildCardColor(string color)
    {
        if (topCard.Number == "Wild") {
            topCard.SetColor(color);
        }

        wildcardMenu.SetActive(false);
        player.isTurn = false;

        // START AI
    }
}