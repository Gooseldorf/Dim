using PlayerMovement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private FirstPersonController _firstPersonController;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
        // int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // SceneManager.LoadScene(activeSceneIndex+sceneIndexOffset, LoadSceneMode.Single);
        //StartCoroutine(SceneSwitch(sceneIndexOffset));
        // SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + sceneIndexOffset);
    }

    // private IEnumerator SceneSwitch(int sceneIndexOffset)
    // {
    //     int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
    //     AsyncOperation load = SceneManager.LoadSceneAsync(activeSceneIndex + sceneIndexOffset, LoadSceneMode.Additive);
    //     yield return load;
    //     SceneManager.UnloadSceneAsync(activeSceneIndex, UnloadSceneOptions.UnloadAllEmbeddedSceneObjects);
    // }
}
