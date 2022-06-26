using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public void ChangeSceneTo(string sceneName)
    {
        LevelManager.Instance.LoadScene(sceneName);
    }
}
