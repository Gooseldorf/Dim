using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Level2Handler : MonoBehaviour
{
    public static Action Level2started;
    [SerializeField] private AudioSource ambientAudioSource;

    private void Awake()
    {
        Level2started?.Invoke();
        ambientAudioSource.volume = 0.3f;
    }
}
