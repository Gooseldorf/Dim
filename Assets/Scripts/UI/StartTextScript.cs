using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartTextScript : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;
    IEnumerator Start()
    {
        gameObject.GetComponent<TextMeshProUGUI>().alpha = 0;
        yield return new WaitForSeconds(3);
        gameObject.GetComponent<TextMeshProUGUI>().alpha = 200;
        yield return new WaitForSeconds(2);
        continueButton.SetActive(true);
    }


}
