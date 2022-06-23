using System;
using System.Collections.Generic;
using PlayerMovement;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class EnemyBehavior : MonoBehaviour
{ 
    private NavMeshAgent _agent;
    [SerializeField] Transform _player;
    [SerializeField] private LayerMask isGround, isPlayer;
    [SerializeField] private List<Transform> placesToHide;
    
    //Patrolling
    public Vector3 walkPoint;
    private bool _walkPointSet;
    public float walkPointRange;
    
    //Attacking
    private float timeBetweenAttacks;
    private bool _isAttacking;
    [SerializeField] private float rotationSpeed = 10;
    [SerializeField] private float damage = 50;
    
    //States
    [SerializeField]private float sightRange, attackRange;
    private bool playerInSightRange, playerInAttackRange;
    [SerializeField] private float health;

    //Audio
    [SerializeField] private AudioClip[] ghoulSounds;
    private AudioSource _ghoulAudioSource;
    
    //Animation
    private Animation enemyAnimation;
    
    
    private void OnEnable()
    {
        EventManager.OnSafeSpaceTrigger += HideFromPlayer;
    }
    private void OnDisable()
    {
        EventManager.OnSafeSpaceTrigger -= HideFromPlayer;
    }

    
    public void Awake()
    {
        _ghoulAudioSource = GetComponent<AudioSource>();
        _ghoulAudioSource.PlayOneShot(ghoulSounds[0]);
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _agent = GetComponent<NavMeshAgent>();
        
        enemyAnimation = gameObject.GetComponent<Animation>();
        enemyAnimation.clip = enemyAnimation.GetClip("Idle");
        enemyAnimation.Play(); 

    }
    
    public void Update()
    {
        
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, isPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, isPlayer);
        
        //if(!playerInSightRange && !playerInAttackRange) Patrolling();
        if(playerInSightRange && !playerInAttackRange && !_isAttacking) Chase();
        if(playerInSightRange && playerInAttackRange) Attack();
        

    }

    private void Patrol()
    {
        if (!_walkPointSet) 
            SearchWalkPoint();
        
        if (_walkPointSet) 
            _agent.SetDestination(walkPoint);
    
        Vector3 distanceToWalkPoint = transform.position - walkPoint;
    
        if (distanceToWalkPoint.magnitude < 1f)
            _walkPointSet = false;
    
    }
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
    
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
        if (Physics.Raycast(walkPoint, -transform.up, 2f, isGround))
            _walkPointSet = true;
    }
    
    
    private void Chase()
    {
        _agent.SetDestination(_player.position);
        enemyAnimation.clip = enemyAnimation.GetClip("Run");
        enemyAnimation.Play();
    }
    private void Attack()
    {
        _agent.SetDestination(transform.position);
        transform.LookAt(_player);
        transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
        timeBetweenAttacks = enemyAnimation.GetClip("Attack2").length;
        
        if (!_isAttacking)
        {
            _isAttacking = true;
            enemyAnimation.clip = enemyAnimation.GetClip("Attack2");
            enemyAnimation.Play();
            _ghoulAudioSource.PlayOneShot(ghoulSounds[1]);
            
            //Костыль LegacyAnimation антагониста. По хорошему - конвертировать в custom запилить ивент в анимации.
            float timeToDamageAnimation = 1f;
            Invoke(nameof(DeliverDamageByAnimation),timeToDamageAnimation);
            
            Invoke(nameof(ResetAttack),timeBetweenAttacks);
        }
    }
    private void DeliverDamageByAnimation()
    {
        FirstPersonController.OnTakeDamage(damage);
    }
    private void ResetAttack()
    {
        _isAttacking = false;
    }
    
    
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
            Invoke(nameof(DestroyEnemy),0.5f);
    }
    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }
    
    private void HideFromPlayer()
    {
        _ghoulAudioSource.PlayOneShot(ghoulSounds[2]);
        _agent.speed = 3;
        enemyAnimation.clip = enemyAnimation.GetClip("Run");
        enemyAnimation.PlayQueued("Run");
        sightRange = 0;
        Vector3 closestWaypoint = FindClosestWayPoint();
        _agent.SetDestination(closestWaypoint);
    }
    Vector3 FindClosestWayPoint()
    {
        Vector3 closestWayPoint = default;
        float minDistance = Single.MaxValue;
        foreach (var waypoint in placesToHide)
        {
            if (Vector3.Distance(transform.position, waypoint.position) < minDistance)
            {
                closestWayPoint = waypoint.position;
                minDistance = Vector3.Distance(transform.position, waypoint.position);
            }
        }
        return closestWayPoint;
    }
    
    
    
    
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
