using System.Collections;
using TMPro;
using UnityEngine;

public class HintsScript : MonoBehaviour
{
    private TextMeshProUGUI _hints;
    private string _hint1 = "F - toggle flashlight";
    private string _hint2 = "Light and movement speed affect your visibility. The stealth indicator is located in the lower right corner of the screen";
    private string _hint3 = "To interact with objet just focus on it by pressing RMB";

    private void OnEnable()
    {
        Level2Handler.Level2started += ChangeHintsForLevel2;
    }

    private void OnDisable()
    {
        Level2Handler.Level2started -= ChangeHintsForLevel2;
    }

    private void Awake()
    {
        _hints = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void ShowHints()
    {
        StartCoroutine(ShowHideHints());
    }

    private void ChangeHintsForLevel2()
    {
        _hint1 = "LMB - fire. R - reload.";
        _hint2 = "Destroy all ghouls on the level! The enemy counter is in the upper right corner of the screen.";
        _hint3 = "Get back on the streets. The exit is where you came from";
    }

    private IEnumerator ShowHideHints()
    {
        _hints.text = _hint1;
        yield return new WaitForSeconds(8);
        _hints.text = _hint2;
        yield return new WaitForSeconds(8);
        _hints.text = _hint3;
        yield return new WaitForSeconds(8);
        gameObject.SetActive(false);
    }
}
