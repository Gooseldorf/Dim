using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    public void Awake()
    {
        gameObject.layer = 11;
    }

    public abstract void OnInteract();
    public abstract void OnFocus();
    public abstract void OnLoseFocus();
}
