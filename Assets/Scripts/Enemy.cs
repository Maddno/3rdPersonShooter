using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
   [SerializeField] private float health = 50f;
   [SerializeField] private NavMeshAgent navMeshAgent;
   [SerializeField] private Transform player;
   [SerializeField] private LayerMask whatIsGround, whatIsPlayer;

    private GameObject[] enemys;
    [SerializeField] private int enemyCount;
    [SerializeField] private LevelManager levelManager;

    //Patroling
   [SerializeField] private Vector3 walkPoint;
   [SerializeField] private float walkPointRange;
   private bool walkPointSet;

   //Attacking
   [SerializeField] private float timeBetweenAttacks;
   private bool alreadyAttacked;
   [SerializeField] private GameObject projectile;
   [SerializeField] private Transform spawnPoint;
   [SerializeField] private float enemySpeed;
   [SerializeField] private ParticleSystem muzzleFlash;

   //States
   [SerializeField] private float sightRange;
   [SerializeField] private float attackRange;
   [SerializeField] private bool playerInSightRange;
   [SerializeField] private bool playerInAttackRange;

   private Animator enemyAnimator;

    private void Awake() 
    {
        levelManager = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
        enemyAnimator = GetComponent<Animator>();
        player = GameObject.Find("Player").transform;
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    

    private void Update() 
    {
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        enemyCount = enemys.Length;

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange)OnPatroling();
        if (playerInSightRange && !playerInAttackRange) OnChasePlayer();
        if (playerInSightRange && playerInAttackRange) OnAttackPlayer();
    }

    private void OnPatroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            navMeshAgent.SetDestination(walkPoint);
            transform.LookAt(walkPoint);

        Vector3 distanceToWalkPoin = transform.position - walkPoint;

        if (distanceToWalkPoin.magnitude < 1f)
            walkPointSet = false;

        enemyAnimator.SetBool("Walk", true);
        enemyAnimator.SetBool("Run", false);
    }
    
    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void OnChasePlayer()
    {
        navMeshAgent.SetDestination(player.position);
        enemyAnimator.SetBool("Run", true);
        enemyAnimator.SetBool("Walk", false);
    }

    private void OnAttackPlayer()
    {
        navMeshAgent.SetDestination(transform.position);

        transform.LookAt(player);

        enemyAnimator.SetBool("Run", false);
        enemyAnimator.SetBool("Walk", false);

        if (!alreadyAttacked)
        {
            muzzleFlash.Play();
            GameObject projectailObj = Instantiate(projectile, spawnPoint.transform.position, spawnPoint.transform.rotation) as GameObject;
            Rigidbody projectailRB = projectailObj.GetComponent<Rigidbody>();
            projectailRB.AddForce(projectailRB.transform.forward * enemySpeed);
            Destroy(projectailObj, 3f);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);    
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        enemyCount--;

        Destroy(gameObject);
        
        if(enemyCount <= 0)
        {
            levelManager.LoadGameWin();
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
