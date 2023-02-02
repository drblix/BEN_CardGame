using UnityEngine;

public class CardSlot : MonoBehaviour
{
    private bool filled = false;
    public bool Filled { get { return filled; } }

    public void SetFilled(bool state) => filled = state;
}
