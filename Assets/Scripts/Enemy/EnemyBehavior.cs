using System;
using System.Collections;
using PlayerMovement;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class EnemyBehavior : MonoBehaviour
{ 
    private NavMeshAgent _agent;
    private Transform _player;
    private FirstPersonController _playerController;
    [SerializeField] private LayerMask isPlayer;
    [SerializeField] private GameObject[] placesToHide;
    
    //Attacking
    private float _timeBetweenAttacks;
    private bool _isAttacking;
    private bool _canAttack;
    [SerializeField] private float damage = 50;
    [SerializeField] private LayerMask obstacleLayerMask;
    
    //States
    [SerializeField]private float sightRange, attackRange, damageRange;
    private bool _playerInSightRange, _playerInAttackRange, _playerInDamageRange;
    [SerializeField] private float health;
    private bool _isDead;

    //Audio
    [SerializeField] private AudioClip[] ghoulActionSounds;
    [SerializeField] private AudioClip[] ghoulFootstepSounds;
    [SerializeField] private AudioClip[] ghoulDamageSounds;
    [SerializeField] private AudioClip ghoulDeath;

    private AudioSource _ghoulAudioSource;
    [SerializeField] private float footstepOffset = 0.5f;
    private float _footstepTimer = 0;
    private float _naturalSoundsTimer = 0;
    
    //Animation
    private Animation _enemyAnimation;
    
    public static Action EnemyDown;
    private bool _isLevel2;
    private bool _reactsOnLight = true;
    
    private void OnEnable()
    {
        EventManager.OnSafeSpaceTrigger += HideFromPlayer;
        Level2Handler.Level2started += () => _isLevel2 = true;
    }
    private void OnDisable()
    {
        EventManager.OnSafeSpaceTrigger -= HideFromPlayer;
        Level2Handler.Level2started -= () => _isLevel2 = true;
    }
    
    public void Awake()
    {
        _ghoulAudioSource = GetComponent<AudioSource>();
        _ghoulAudioSource.PlayOneShot(ghoulActionSounds[0]);
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _playerController = FindObjectOfType<FirstPersonController>();
        _agent = GetComponent<NavMeshAgent>();
        placesToHide = GameObject.FindGameObjectsWithTag("WalkPoint");
        _enemyAnimation = gameObject.GetComponent<Animation>();
        _enemyAnimation.clip = _enemyAnimation.GetClip("Idle");
        _enemyAnimation.Play();
    }
    
    public void Update()
    {
        _playerInSightRange = Physics.CheckSphere(transform.position, sightRange, isPlayer);
        _playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);
        _playerInDamageRange = Physics.CheckSphere(transform.position, damageRange, isPlayer);
        
        if((!_isLevel2 && !_playerController.flashLightOn) && !_playerInAttackRange && !_playerController.IsSprinting) Idle();
        if(_playerInSightRange && !_playerInAttackRange && !_isAttacking && (_isLevel2 || _playerController.flashLightOn || _playerController.IsSprinting)) Chase();
        if(_playerInSightRange && _playerInAttackRange && PlayerIsInSight()) Attack();
    }

    private void Idle()
    {
        _enemyAnimation.clip = _enemyAnimation.GetClip("Idle");
        _enemyAnimation.Play();
    }
    
    private void Chase()
    {
        _agent.SetDestination(_player.position);
        _enemyAnimation.clip = _enemyAnimation.GetClip("Run");
        _enemyAnimation.Play();
        _footstepTimer -= Time.deltaTime;
        if (_footstepTimer <= 0)
        {
            _ghoulAudioSource.PlayOneShot(ghoulFootstepSounds[Random.Range(0, ghoulFootstepSounds.Length - 1)],1f);
            _footstepTimer = footstepOffset;
        }
    }

    private bool PlayerIsInSight()
    {
        Vector3 directionToTarget = (_player.position - transform.position).normalized;
        if (!Physics.Raycast(transform.position, directionToTarget, attackRange, obstacleLayerMask))
            return true;
        else
            return false;
    }

    private void Attack()
        {
            _agent.SetDestination(transform.position);
            transform.LookAt(_player);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
            _timeBetweenAttacks = _enemyAnimation.GetClip("Attack2").length;
        
            if (!_isAttacking)
            {
                _isAttacking = true;
                _enemyAnimation.clip = _enemyAnimation.GetClip("Attack2");
                _enemyAnimation.Play();
                _ghoulAudioSource.PlayOneShot(ghoulActionSounds[1]);
            
                float timeToDeliverDamageViaAnimation = 1f;
                
                if(!_isDead) 
                    Invoke(nameof(DeliverDamageViaAnimation),timeToDeliverDamageViaAnimation);
            
                Invoke(nameof(ResetAttack),_timeBetweenAttacks);
            }
        }
    
    private void DeliverDamageViaAnimation()
        {
            if(_playerInDamageRange && !_isDead)
                FirstPersonController.OnTakeDamage(damage);
        }
    private void ResetAttack()
    {
        _isAttacking = false;
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        _ghoulAudioSource.PlayOneShot(ghoulDamageSounds[Random.Range(0, ghoulDamageSounds.Length - 1)],2);
        
        if (health < 0)
        {
            _isDead = true;
           StartCoroutine(GhoulDeath());
        }
    }

    private IEnumerator GhoulDeath()
    {
        sightRange = 0;
        gameObject.GetComponent<Collider>().enabled = false;
        _ghoulAudioSource.PlayOneShot(ghoulDeath);
        _enemyAnimation.clip = _enemyAnimation.GetClip("Death");
        _enemyAnimation.Play();
        yield return new WaitForSeconds(_enemyAnimation.clip.length);
        gameObject.GetComponentInChildren<ParticleSystem>().Play();
        gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        yield return new WaitForSeconds(1.5f);
        DestroyEnemy();
    }

    private void DestroyEnemy()
    {
        EnemyDown?.Invoke();
        Destroy(gameObject);
    }
    private void HideFromPlayer()
    {
        //_ghoulAudioSource.PlayOneShot(ghoulActionSounds[2]);
        _enemyAnimation.clip = _enemyAnimation.GetClip("Run");
        _enemyAnimation.PlayQueued("Run");
        sightRange = 0;
        Vector3 closestWaypoint = FindClosestWayPoint();
        _agent.SetDestination(closestWaypoint);
        Invoke(nameof(DestroyEnemy),9f);
    }
    Vector3 FindClosestWayPoint()
    {
        Vector3 closestWayPoint = default;
        float minDistance = Single.MaxValue;
        foreach (var waypoint in placesToHide)
        {
            if (Vector3.Distance(transform.position, waypoint.transform.position) < minDistance)
            {
                closestWayPoint = waypoint.transform.position;
                minDistance = Vector3.Distance(transform.position, waypoint.transform.position);
            }
        }
        return closestWayPoint;
    }

    public void ActivateEnemy()
    {
        _ghoulAudioSource.PlayOneShot(ghoulActionSounds[0]);
        sightRange = 20;
    }
    
    private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, sightRange);
        }
}
