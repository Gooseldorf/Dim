using System.Collections;
using TMPro;
using UnityEngine;

public class HintsScript : MonoBehaviour
{
    private PauseMenu _pauseMenu;
    private TextMeshProUGUI _hints;
    private string _hint1 = "F - toggle flashlight";
    private string _hint2 = "Light and movement speed affect your visibility. The stealth indicator is located in the lower right corner of the screen";
    private string _hint3 = "To interact with object just focus on it by pressing RMB";
    private string _hint4 = "Places where you can hide are marked with a green light.";

    private void OnEnable()
    {
        Level2Handler.Level2started += ChangeHintsForLevel2;
        Trigger.MonsterTrigger += ShowHidingHints;
    }

    private void OnDisable()
    {
        Level2Handler.Level2started -= ChangeHintsForLevel2;
        Trigger.MonsterTrigger -= ShowHidingHints;

    }

    private void Awake()
    {
        _pauseMenu = FindObjectOfType<PauseMenu>();
        _hints = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void ShowHints()
    {
        _pauseMenu.CanPause = true;
        StartCoroutine(ShowHideHints());
    }

    public void ShowHidingHints()
    {
        StartCoroutine(ShowHideHidingHints());
    }

    private void ChangeHintsForLevel2()
    {
        _hint1 = "LMB - fire. R - reload.";
        _hint2 = "Destroy all ghouls on the level! The enemy counter is in the upper right corner of the screen.";
        _hint3 = "Get back on the streets. The exit is where you came from";
        _hint4 = "Turn off the flashlight! Places where you can hide are marked with a green light. ";
    }

    private IEnumerator ShowHideHints()
    {
        _hints.text = _hint1;
        yield return new WaitForSeconds(8);
        _hints.text = _hint2;
        yield return new WaitForSeconds(8);
        _hints.text = _hint3;
        yield return new WaitForSeconds(8);
        _hints.text = "";
    }

    private IEnumerator ShowHideHidingHints()
    {
        _hints.text = _hint4;
        yield return new WaitForSeconds(4);
        _hints.text = "";

    }
}
