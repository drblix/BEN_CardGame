using System.Collections;
using UnityEngine;

public class Card : MonoBehaviour
{
    // Declarations
    public CardSlot filledSpot;
    private AudioSource source;

    private const float FLIPPED_VALUE = 270f;
    private const float UNFLIPPED_VALUE = 90f;

    private const float MOVE_SPEED = 24f;
    private const float FLIP_SPEED = 3f;
    
    private DrawingDeck.CardNumber number = DrawingDeck.CardNumber.One;
    private DrawingDeck.CardColor color = DrawingDeck.CardColor.Red;
    
    public DrawingDeck.CardNumber Number { get { return number; } }
    public DrawingDeck.CardColor Color { get { return color; } }

    // False means logo is showing, true means number and color is showing
    private bool flipped = false;
    public bool Flipped { get { return flipped; } }
    private bool isFlipping = false;
    private bool isMoving = false;
    public bool IsMoving { get { return isMoving; } }

    private bool ownedByPlayer = false;
    public bool OwnedByPlayer { get { return ownedByPlayer; } }

    private void Awake() 
    {
        source = GetComponent<AudioSource>();
    }

    public void SetAttributes(DrawingDeck.CardColor c, DrawingDeck.CardNumber num, Texture texture, bool owned)
    {
        color = c;
        number = num;
        ownedByPlayer = owned;
        GetComponent<MeshRenderer>().material.SetTexture("_MainTex", texture);
    }

    /// <summary>
    /// Flips the cards over: specifically, so the non-logo side is displayed
    /// </summary>
    public IEnumerator FlipCard() 
    {
        // Returns early if already flipping
        if (isFlipping) { yield break; }
        
        isFlipping = true;
        // declarations
        float timer = 0f;
        float rotAmnt = transform.eulerAngles.y;

        source.Play();
        
        // Loop that continues until 1 second has passed
        while (timer < 1f) {
            // Linearally interpolates the rotational value and applies it as a quaternion rotation to the card object
            if (!flipped) {
                rotAmnt = Mathf.Lerp(FLIPPED_VALUE, UNFLIPPED_VALUE, timer * FLIP_SPEED);
            }
            else {
                rotAmnt = Mathf.Lerp(UNFLIPPED_VALUE, FLIPPED_VALUE, timer * FLIP_SPEED);
            }

            transform.rotation = Quaternion.Euler(0f, rotAmnt, 0f);
            
            timer += Time.deltaTime;
            // Waits for frame to pass; stops animation from being instant
            yield return new WaitForEndOfFrame();
        }

        flipped = !flipped;
        // Sets rotation to goal value
        transform.rotation = flipped ? Quaternion.Euler(0f, UNFLIPPED_VALUE, 0f) : Quaternion.Euler(0f, FLIPPED_VALUE, 0f);
        isFlipping = false;
    }

    // Moves the card to a position overtime
    public IEnumerator MoveCard(Vector3 to, bool leaveSpot) 
    {
        // Returns early if already moving
        if (isMoving) { yield break; }
        isMoving = true;
        
        // Clears filledSpot variable if 'leaveSpot' is true
        if (leaveSpot && filledSpot) { 
            filledSpot.Filled = false;
            filledSpot = null;
        }

        // Moves card towards vector2 position
        while (transform.position != to) {
            transform.position = Vector3.MoveTowards(transform.position, to, Time.deltaTime * MOVE_SPEED);
            yield return new WaitForEndOfFrame();
        }

        // Snaps card to position and sets flag to false
        transform.position = to;
        isMoving = false;
    }
    
    // Debugging
    public override string ToString()
    {
        return "Color: " + color + " || Number: " + number;
    }
    
    // Used for wild card instances
    public void SetColor(DrawingDeck.CardColor c) => color = c;
    
    // Overriden equals function to simplify comparisons
    public bool Equals (Card card) 
    {
        if (!card || this.Number == DrawingDeck.CardNumber.Wild)
            return true;

        return this.Number == card.Number || this.Color == card.Color;
    }
}
