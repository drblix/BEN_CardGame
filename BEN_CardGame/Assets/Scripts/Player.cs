using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    private DrawingDeck drawingDeck;
    private PlacingDeck placingDeck;

    public List<Card> playerDeck = new List<Card>();

    // Setting to true will have the player start first
    public bool isTurn = true;

    private void Awake() {
        drawingDeck = FindObjectOfType<DrawingDeck>();
        placingDeck = FindObjectOfType<PlacingDeck>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) && !drawingDeck.gameOver) {
            MouseClicked();
        }
        else if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(0);
        }
    }

    private void MouseClicked() {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Physics.Raycast(mousePoint, Vector3.forward, out RaycastHit hitInfo, 50f)) {
            if (hitInfo.collider) {
                if (hitInfo.collider.CompareTag("PlayingCard")) {
                    Card card = hitInfo.collider.GetComponent<Card>();
                    if (card.IsMoving || drawingDeck.CardsMoving(playerDeck) || !card.OwnedByPlayer) { return; }

                    if (!card.Flipped) {
                        StartCoroutine(card.FlipCard());
                    }
                    else if (placingDeck.CanPlaceCard(card) && isTurn) {
                        card.GetComponent<BoxCollider>().enabled = false;
                        card.filledSpot.SetFilled(false);
                        card.filledSpot = null;
                        playerDeck.Remove(card);
                        StartCoroutine(drawingDeck.ShiftCards(0));
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
}
