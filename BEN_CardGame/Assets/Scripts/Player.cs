using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private DrawingDeck drawingDeck;
    private PlacingDeck placingDeck;

    public List<Card> playerDeck = new List<Card>();

    public bool isTurn = true;

    private void Awake() {
        drawingDeck = FindObjectOfType<DrawingDeck>();
        placingDeck = FindObjectOfType<PlacingDeck>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && isTurn) {
            MouseClicked();
        }
    }

    private void MouseClicked() {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Physics.Raycast(mousePoint, Vector3.forward, out RaycastHit hitInfo, 50f)) {
            if (hitInfo.collider) {
                if (hitInfo.collider.CompareTag("PlayingCard")) {
                    Card card = hitInfo.collider.GetComponent<Card>();
                    if (card.IsMoving || CardsMoving() || !card.OwnedByPlayer) { return; }

                    if (!card.Flipped) {
                        StartCoroutine(card.FlipCard());
                    }
                    else if (placingDeck.CanPlaceCard(card)) {
                        card.GetComponent<BoxCollider>().enabled = false;
                        card.filledSpot.SetFilled(false);
                        card.filledSpot = null;
                        playerDeck.Remove(card);
                        StartCoroutine(ShiftCards());
                        placingDeck.PlaceCard(card, 0);

                        // AI's turn
                    }
                }
                else if (hitInfo.collider.name.Equals("DrawingDeck")) {
                    drawingDeck.DrawCard(0);
                }
            }
        }
    }

    public IEnumerator ShiftCards()
    {
        foreach (Card card in playerDeck)
        {
            card.filledSpot.SetFilled(false);
            card.filledSpot = null;
            Vector2 to = drawingDeck.FindNextOpenSlot(card, 0);
            StartCoroutine(card.MoveCard(to, false));
            yield return new WaitForEndOfFrame();
        }
    }

    private bool CardsMoving()
    {
        foreach (Card card in playerDeck)
        {
            if (card.IsMoving) {
                return true;
            }
        }

        return false;
    }
}
