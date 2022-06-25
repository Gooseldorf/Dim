using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : Interactable
{
    public override void OnInteract()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        switch (gameObject.name)
        {
            case "KeyFromQuestDoor1":
                player.GetComponent<Inventory>().hasKeyFromQuestDoor1 = true;
                gameObject.SetActive(false);
                break;
            case "KeyFromQuestDoor2":
                player.GetComponent<Inventory>().hasKeyFromQuestDoor2 = true;
                gameObject.SetActive(false);
                break;
            case "KeyFromQuestDoor3":
                player.GetComponent<Inventory>().hasKeyFromQuestDoor3 = true;
                gameObject.SetActive(false);
                break;
        }
    }

    public override void OnFocus()
    {
    }

    public override void OnLoseFocus()
    {
    }
}
