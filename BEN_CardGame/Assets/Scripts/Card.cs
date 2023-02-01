using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    private const float FLIPPED_VALUE = 270f;
    private const float UNFLIPPED_VALUE = 90f;

    private AudioSource source;

    private int number = 1;
    private string color = "Red";
    
    public int Number { get { return number; } }
    public string Color { get { return color; } }

    // false means logo is showing, true means number and color is showing
    private bool flipped = false;
    public bool Flipped { get { return flipped; } }
    private bool isFlipping = false;
    private bool isMoving = false;

    private void Awake() 
    {
        source = GetComponent<AudioSource>();
    }

    public IEnumerator FlipCard() 
    {
        if (isFlipping) { yield break; }

        isFlipping = true;
        const float SPEED = 2f;
        float timer = 0f;
        float rotAmnt = transform.eulerAngles.y;

        source.Play();

        while (timer < 1f) {
            if (!flipped) {
                rotAmnt = Mathf.Lerp(FLIPPED_VALUE, UNFLIPPED_VALUE, timer * SPEED);
            }
            else {
                rotAmnt = Mathf.Lerp(UNFLIPPED_VALUE, FLIPPED_VALUE, timer * SPEED);
            }
            
            transform.rotation = Quaternion.Euler(0f, rotAmnt, 0f);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        flipped = !flipped;
        transform.rotation = flipped ? Quaternion.Euler(0f, UNFLIPPED_VALUE, 0f) : Quaternion.Euler(0f, FLIPPED_VALUE, 0f);
        isFlipping = false;
    }

    public IEnumerator MoveCard(Vector2 to) 
    {
        if (isMoving) { yield break; }

        const float SPEED = 2f;
        float timer = 0f;

        while (true) {
            transform.position = Vector2.MoveTowards(transform.position, to, timer * SPEED);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

    }
}
