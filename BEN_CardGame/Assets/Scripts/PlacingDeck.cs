using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PlacingDeck : MonoBehaviour
{
    private const int MAX_CARDS = 6;

    private Player player;
    private Computer computer;
    private DrawingDeck drawingDeck;

    [SerializeField]
    private GameObject wildcardMenu;

    [SerializeField]
    private AudioClip[] clips;

    private List<Card> placedCards = new List<Card>();

    private Card topCard;
    public Card TopCard { get { return topCard; } }

    [SerializeField]
    private TextMeshProUGUI notification;

    [SerializeField]
    private bool cheatMode = false;

    private void Awake()
    {
        computer = FindObjectOfType<Computer>();
        player = FindObjectOfType<Player>();
        drawingDeck = FindObjectOfType<DrawingDeck>();

        if (!player.isTurn) {
            StartCoroutine(computer.StartTurn());
        }
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
            // Removes the card on the bottom of the pile
            Card bottomCard = placedCards[0];
            placedCards.RemoveAt(0);
            Destroy(bottomCard.gameObject);

            // Shifts all cards to their correct position, except the card that is currently
            // moving to the pile
            for (int i = 0; i < placedCards.Count - 1; i++) {
                placedCards[i].transform.position = new Vector3(0f, 0f, -.1f * i);
            }
        }


        if (player.playerDeck.Count == 0) {
            StartCoroutine(DisplayVictor("PLAYER"));
            return;
        }
        else if (computer.computerDeck.Count == 0) {
            StartCoroutine(DisplayVictor("COMPUTER"));
            return;
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
            // START COMPUTER TURN
            StartCoroutine(computer.StartTurn());
        }
    }

    private IEnumerator RotateXTo(Card card, float x)
    {
        const float ROTATION_SPEED = 2.5f;
        float val = 0f;
        float timer = 0f;
        while (timer < 1f)
        {
            val = Mathf.Lerp(0f, x, timer * ROTATION_SPEED);
            card.transform.rotation = Quaternion.Euler(val, card.transform.eulerAngles.y, card.transform.eulerAngles.z);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        // Snaps card to rotation
        card.transform.rotation = Quaternion.Euler(x, card.transform.eulerAngles.y, card.transform.eulerAngles.z);
    }

    public bool CanPlaceCard(Card card)
    {
        if (!topCard || cheatMode) { return true; }

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
        StartCoroutine(computer.StartTurn());
    }

    public IEnumerator DisplayVictor(string winner)
    {
        drawingDeck.gameOver = true;
        AudioSource source = GetComponent<AudioSource>();

        if (winner != "UNLUCKY...") {
            notification.SetText(winner.ToUpper() + " WINS!");
        }
        else {
            notification.SetText(winner);
        }

        notification.transform.parent.gameObject.SetActive(true);

        if (winner.Equals("PLAYER")) {
            source.clip = clips[0];
            source.Play();
        }
        else{
            source.clip = clips[1];
            source.Play();
        }

        yield return new WaitForSeconds(5f);

        notification.SetText("PLAY AGAIN?");
        notification.transform.parent.Find("YesBtn").gameObject.SetActive(true);
        notification.transform.parent.Find("NoBtn").gameObject.SetActive(true);
    }

    public void PlayAgainChoice(string yn)
    {
        if (yn.Equals("y")) {
            SceneManager.LoadScene(1);
        }
        else {
            SceneManager.LoadScene(0);
        }
    }
}
