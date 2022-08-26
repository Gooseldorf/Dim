using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private Image progressBar;
    private float _target;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        loadingCanvas.SetActive(false);
    }

    private void Update()
    {
        progressBar.fillAmount = Mathf.MoveTowards(progressBar.fillAmount, _target, Time.deltaTime * 2);
    }

    public async void LoadScene(string sceneName)
    {
        progressBar.fillAmount = 0;
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;
        loadingCanvas.SetActive(true);
        do
        {
            _target = scene.progress;
      
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true;
        loadingCanvas.SetActive(false);
        _target = 0;
    }
}
