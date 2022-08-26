using UnityEngine;

public class TriggerLvl2 : MonoBehaviour
{
    private Collider _player;
    [SerializeField] private GameObject[] activeEnemies;

    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == _player)
        {
            foreach (var enemy in activeEnemies)
            {
                if(enemy != null)
                    enemy.GetComponent<EnemyBehavior>().ActivateEnemy();
                else
                    gameObject.SetActive(false);

            }
            gameObject.SetActive(false);
        }
    }
}
