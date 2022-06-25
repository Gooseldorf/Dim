using System;
using System.Collections;
using PlayerMovement;
using UnityEngine;

public class SaveSpace : MonoBehaviour
{
    private Collider _player;
    private bool _canTrigger;
    private ParticleSystem _saveSpaceParticles;
    private Light _saveSpaceLight;
    [SerializeField] private float maxLightIntensity;
    [SerializeField] private int maxParticles;
    private ParticleSystem.MainModule _maxParticleFading; 
    private bool _isFading;
    private bool _wasTriggered;

    private void OnEnable()
    {
        EventManager.HideAllSafeSpaces += SetInactive;
        EventManager.OnMonsterTrigger += SetActive;
    }

    private void OnDisable()
    {
        EventManager.HideAllSafeSpaces -= SetInactive;
        EventManager.OnMonsterTrigger -= SetActive;
    }

    private void Awake()
    {
        _saveSpaceLight = GetComponentInChildren<Light>();
        _saveSpaceParticles = GetComponentInChildren<ParticleSystem>();
        _saveSpaceLight.intensity = 0;
        _maxParticleFading = _saveSpaceParticles.main;
        _maxParticleFading.maxParticles = 0;
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider>();
    }

    private void Update()
    {
        if (_isFading)
        {
            _saveSpaceLight.intensity -= Time.deltaTime; 
            _maxParticleFading.maxParticles -= 1;
            if (_saveSpaceLight.intensity <= 0 && _maxParticleFading.maxParticles <= 0)
                _isFading = false;
        }
    }

    private void SetActive()
    {
        _canTrigger = true;
        _wasTriggered = false;
        ShowSafeSpace();
    }

    private void SetInactive()
    {
        _canTrigger = false;
        _saveSpaceLight.intensity = 0;
        _maxParticleFading.maxParticles = 0;
    }
    
    private void OnTriggerStay(Collider other)
    {
        if (other == _player && !_player.GetComponent<FirstPersonController>().flashLightOn && !_wasTriggered)
        {
            Invoke(nameof(PlayerTurnedLightsOff),0.5f);
            _wasTriggered = true;
        }
    }
    private void PlayerTurnedLightsOff()
    {
        _player.GetComponent<FirstPersonController>().canUseFlashlight = false;
        _canTrigger = false;
        EventManager.SafeSpaceTrigger();
        StartCoroutine(HideSafeSpace());
    }

    private IEnumerator HideSafeSpace()
    {
        yield return new WaitForSeconds(6f);
        _isFading = true;
        yield return new WaitForSeconds(1f);
        _player.GetComponent<FirstPersonController>().canUseFlashlight = true;
        EventManager.HideSafeSpaces();
    }
    private void ShowSafeSpace()
    {
        _saveSpaceLight.intensity = maxLightIntensity;
        _maxParticleFading.maxParticles = maxParticles;
    }
}
