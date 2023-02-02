using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Computer : MonoBehaviour
{
    private Player player;
    private PlacingDeck placingDeck;
    private DrawingDeck drawingDeck;

    public List<Card> computerDeck = new List<Card>();

    [SerializeField]
    private TextMeshProUGUI wildNotification;

    private void Awake()
    {
        player = FindObjectOfType<Player>();
        placingDeck = FindObjectOfType<PlacingDeck>();
        drawingDeck = FindObjectOfType<DrawingDeck>();
    }

    public IEnumerator StartTurn()
    {
        Debug.Log(placingDeck.TopCard.ToString());
        yield return new WaitForSeconds(Random.Range(1.5f, 3.25f));
        while (!PlaceCard())
        {
            drawingDeck.DrawCard(1);
            yield return new WaitUntil(() => !drawingDeck.CardsMoving(computerDeck));
            yield return new WaitForSeconds(Random.Range(.4f, .9f));
        }

        if (!drawingDeck.gameOver)
        {
            player.isTurn = true;
            Debug.Log("PLAYER'S TURN");
        }
    }

    /// <summary>
    /// AI algorithm to find and place a card
    /// </summary>
    /// <returns>Returns true if placed a card</returns>
    private bool PlaceCard()
    {
        List<Card> wildCards = new List<Card>();

        foreach (Card card in computerDeck)
        {
            // Treats wildcards as a last resort for the AI
            if (card.Number == "Wild") {
                wildCards.Add(card);
                continue;
            }

            if (placingDeck.TopCard.Color == card.Color || placingDeck.TopCard.Number == card.Number)
            {
                card.filledSpot.SetFilled(false);
                card.filledSpot = null;

                StartCoroutine(card.FlipCard());
                computerDeck.Remove(card);
                placingDeck.PlaceCard(card, 1);
                StartCoroutine(drawingDeck.ShiftCards(1));

                return true;
            }
        }

        if (wildCards.Count > 0)
        {
            wildCards[0].filledSpot.SetFilled(false);
            wildCards[0].filledSpot = null;

            string chosenColor = DetermineWildColor();
            wildCards[0].SetColor(chosenColor);

            StartCoroutine(NotifyWildCard(chosenColor));

            StartCoroutine(wildCards[0].FlipCard());
            placingDeck.PlaceCard(wildCards[0], 1);
            computerDeck.Remove(wildCards[0]);
            StartCoroutine(drawingDeck.ShiftCards(1));

            return true;
        }

        return false;
    }

    private string DetermineWildColor()
    {
        string[] colors = { "Red", "Blue", "Green", "Yellow" };
        int[] clrAmnts = { 0, 0, 0, 0 };

        foreach (Card card in computerDeck)
        {
            if (card.Number == "Wild") { continue; }

            switch (card.Color)
            {
                case "Red":
                    clrAmnts[0] += 1;
                    break;
                case "Blue":
                    clrAmnts[1] += 1;
                    break;
                case "Yellow":
                    clrAmnts[2] += 1;
                    break;
                case "Green":
                    clrAmnts[3] += 1;
                    break;
            }

        }

        int largestIndex = LargestElement(clrAmnts);

        if (largestIndex != -1)
        {
            switch (largestIndex)
            {
                case 0:
                    return "Red";
                case 1:
                    return "Blue";
                case 2:
                    return "Yellow";
                case 3:
                    return "Green";
                default:
                    return colors[Random.Range(0, colors.Length)];
            }
        }
        else
        {
            return colors[Random.Range(0, colors.Length)];
        }
    }

    /// <summary>
    /// Returns the index of the largest element in the array
    /// </summary>
    /// <param name="arr">Array to be itereated</param>
    /// <returns>Index of largest element; -1 if there are multiple largest elements</returns>
    private int LargestElement(int[] arr)
    {
        int? maxValue = null;
        int index = -1;

        for (int i = 0; i < arr.Length; i++)
        {
            int curNum = arr[i];
            if (!maxValue.HasValue || curNum > maxValue.Value)
            {
                maxValue = curNum;
                index = i;
            }
        }

        // Checks if there are multiple "largest numbers"
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] == arr[index] && i != index)
            {
                return -1;
            }
        }

        return index;
    }

    private IEnumerator NotifyWildCard(string color)
    {
        wildNotification.SetText("COMPUTER CHOSE " + color.ToUpper());

        // A color value that is used multiple times below
        const float X = 35f / 255f;
        switch (color)
        {
            case "Red":
                wildNotification.color = new Color(1, X, X);
                break;
            case "Blue":
                wildNotification.color = new Color(X, X, 1);
                break;
            case "Green":
                wildNotification.color = new Color(X, 1, X);
                break;
            case "Yellow":
                wildNotification.color = new Color(1, 208 / 255f, X);
                break;
        }

        wildNotification.transform.parent.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        wildNotification.transform.parent.gameObject.SetActive(false);
    }
}
