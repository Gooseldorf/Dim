using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float damage = 10;
    [SerializeField] private float fireRate = 1;
    [SerializeField] private float range = 30;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private AudioClip fire;
    [SerializeField] private AudioClip reload;
    [SerializeField] private AudioClip noAmmo;
    private AudioSource _weaponAudioSource;
    private Camera _camera;
    public bool CanShoot { get; set;}
    private EnemyBehavior _currentEnemy;
    private Light flash;
    private bool _shootCooling;

    private void Awake()
    {
        _camera = Camera.main;
        flash = GetComponentInChildren<Light>();
        _weaponAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0) && !_shootCooling)
        {
            Shoot();
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private void Shoot()
    {
        _shootCooling = true;
        _weaponAudioSource.PlayOneShot(fire,3);
        muzzleFlash.Play();
        StartCoroutine(GunShotFlash());
        
        if (Physics.Raycast(_camera.transform.position, _camera.transform.forward, out var hit, range))
        {
            if (hit.collider.gameObject.layer == 8)
            {
                hit.collider.TryGetComponent(out _currentEnemy);
                _currentEnemy.TakeDamage(damage);
            }
        }
    }

    private IEnumerator GunShotFlash()
    {
        flash.enabled = true;
        yield return new WaitForSeconds(fireRate);
        flash.enabled = false;
        //yield return new WaitForSeconds(fireRate);
        _shootCooling = false;
    }
}
