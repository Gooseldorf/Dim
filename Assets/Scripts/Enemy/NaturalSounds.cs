using UnityEngine;
using Random = UnityEngine.Random;

public class NaturalSounds : MonoBehaviour
{
    [SerializeField] private AudioClip[] ghoulNaturalSounds;
    [SerializeField] private float naturalSoundsOffset = 0.5f;

    private AudioSource _ghoulNaturalSounds;
    private float _naturalSoundsTimer = 0;
    

    private void Awake()
    {
        _ghoulNaturalSounds = GetComponent<AudioSource>();
    }

    private void Update()
    {
        PlayNaturalSounds();
    }

    private void PlayNaturalSounds()
    {
        _naturalSoundsTimer -= Time.deltaTime;
        if (_naturalSoundsTimer <= 0)
        {
            _ghoulNaturalSounds.PlayOneShot(ghoulNaturalSounds[Random.Range(0, ghoulNaturalSounds.Length - 1)]);
            _naturalSoundsTimer = naturalSoundsOffset;
        }
    }
}
