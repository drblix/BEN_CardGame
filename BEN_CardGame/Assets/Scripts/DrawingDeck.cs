using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingDeck : MonoBehaviour
{
    [SerializeField]
    private GameObject playingCard;

    // 0 == Player; 1 == AI
    public void DrawCard(int drawer) {
        if (drawer == 0) {
            Card card = Instantiate(playingCard, transform.position, Quaternion.identity).GetComponent<Card>();

            card.MoveCard(new Vector2(0f, 0f));
        }
    }
}
