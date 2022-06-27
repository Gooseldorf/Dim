using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class HintsScript : MonoBehaviour
{
    private TextMeshProUGUI _hints;
    private string _hint1 = "F - включить/выключить фонарик";

    private string _hint2 =
        "Cтарайтесь не выдать себя. Bероятность обнаружения отражена в правом нижнем углу экрана";

    private string _hint3 = "Чтобы прочесть записку - наведите курсор и посмотрите внимательнее (RMB)";

    private void Awake()
    {
        _hints = gameObject.GetComponent<TextMeshProUGUI>();
    }

    public void ShowHints()
    {
        StartCoroutine(ShowHideHints());
    }

    private IEnumerator ShowHideHints()
    {
        _hints.text = _hint1;
        yield return new WaitForSeconds(5);
        _hints.text = _hint2;
        yield return new WaitForSeconds(5);
        _hints.text = _hint3;
        yield return new WaitForSeconds(5);
        gameObject.SetActive(false);
    }
}
