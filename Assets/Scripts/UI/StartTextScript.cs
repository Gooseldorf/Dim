using System.Collections;
using TMPro;
using UnityEngine;

public class StartTextScript : MonoBehaviour
{
    [SerializeField] private GameObject continueButton;
    
    private void Start()
    {
        StartCoroutine(ShowStartText());
    }

    IEnumerator ShowStartText()
    {
        gameObject.GetComponent<TextMeshProUGUI>().alpha = 0;
        yield return new WaitForSeconds(3);
        gameObject.GetComponent<TextMeshProUGUI>().alpha = 200;
        yield return new WaitForSeconds(2);
        continueButton.SetActive(true);
    }


}
