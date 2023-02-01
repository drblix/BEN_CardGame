using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private DrawingDeck drawingDeck;

    private void Awake() {
        drawingDeck = FindObjectOfType<DrawingDeck>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            MouseClicked();
        }
    }

    private void MouseClicked() {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Physics.Raycast(mousePoint, Vector3.forward, out RaycastHit hitInfo, 50f)) {
            if (hitInfo.collider) {
                if (hitInfo.collider.CompareTag("PlayingCard")) {
                    Card card = hitInfo.collider.GetComponent<Card>();

                    if (!card.Flipped) {
                        StartCoroutine(card.FlipCard());
                    }
                }
                else if (hitInfo.collider.name.Equals("DrawingDeck")) {
                    drawingDeck.DrawCard(0);
                }
            }
            Debug.Log(hitInfo.collider.name);
        }
    }
}
