using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitScript : MonoBehaviour
{
  private Collider _player;

  private void Awake()
  {
    _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other == _player)
      SceneManager.LoadScene("MainMenu");
  }
}
