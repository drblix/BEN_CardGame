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
        while (!PlaceCard()) {
            drawingDeck.DrawCard(1);
            yield return new WaitUntil(() => !drawingDeck.CardsMoving(computerDeck));
            yield return new WaitForSeconds(Random.Range(.4f, .9f));
        }

        player.isTurn = true;
        Debug.Log("PLAYER'S TURN");
    }

    /// <summary>
    /// AI algorithm to find and place a card
    /// </summary>
    /// <returns>Returns true if placed a card</returns>
    private bool PlaceCard()
    {
        foreach (Card card in computerDeck)
        {
            if (card.Number == "Wild") {
                card.filledSpot.SetFilled(false);
                card.filledSpot = null;
                string chosenColor = DetermineWildColor();
                card.SetColor(chosenColor);
                StartCoroutine(NotifyWildCard(chosenColor));
                StartCoroutine(card.FlipCard());
                placingDeck.PlaceCard(card, 1);
                drawingDeck.ShiftCards(1);
                computerDeck.Remove(card);
                return true;
            }

            if (placingDeck.TopCard.Color == card.Color || placingDeck.TopCard.Number == card.Number) {
                card.filledSpot.SetFilled(false);
                card.filledSpot = null;
                StartCoroutine(card.FlipCard());
                placingDeck.PlaceCard(card, 1);
                drawingDeck.ShiftCards(1);
                computerDeck.Remove(card);
                return true;
            }
        }

        return false;
    }

    private string DetermineWildColor()
    {
        string[] colors = {"Red", "Blue", "Green", "Yellow"};
        int[] clrAmnts = {0, 0, 0, 0};

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

        if (largestIndex != -1) {
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
        else {
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

        for (int i = 0; i < arr.Length; i++) {
            int curNum = arr[i];
            if (!maxValue.HasValue || curNum > maxValue.Value) {
                maxValue = curNum;
                index = i;
            }
        }

        // Checks if there are multiple "largest numbers"
        for (int i = 0; i < arr.Length; i++) {
            if (arr[i] == arr[index] && i != index) {
                return -1;
            }
        }

        return index;
    }

    private IEnumerator NotifyWildCard(string color)
    {
        wildNotification.SetText("COMPUTER CHOSE " + color.ToUpper());

        switch (color)
        {
            case "Red":
                wildNotification.color = new Color(255,35,35);
                break;
            case "Blue":
                wildNotification.color = new Color(35, 35, 255);
                break;
            case "Green":
                wildNotification.color = new Color(35, 255, 35);
                break;
            case "Yellow":
                wildNotification.color = new Color(255, 208, 35);
                break;
        }

        wildNotification.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        wildNotification.gameObject.SetActive(false);
    }
}
