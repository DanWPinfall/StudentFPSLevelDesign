using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum State { Roam, Investigate, Engage }
    public State state;

    [Header("Perception")]
    public float viewDistance = 25f;
    public float viewAngle = 70f;
    public LayerMask visionMask;
    bool hasSeenPlayer;

    [Header("Combat")]
    public float attackRange = 10f;
    public float fireRate = 0.6f;
    public Transform shootPoint;
    public GameObject enemyBulletPrefab;
    public LayerMask shootBlockMask;

    [Header("Movement")]
    public float roamRadius = 15f;
    public float investigateSpeed = 5f;

    NavMeshAgent agent;
    public Transform player;
    Vector3 lastKnownPlayerPos;
    float nextFireTime;

    bool playerVisible;

    Animator animator;


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }


    void Start()
    {
        GameManager.Instance.RegisterEnemy();

        PickRoamPoint();
        state = State.Roam;
    }

    void Update()
    {
        DetectPlayer();

        switch (state)
        {
            case State.Roam:
                UpdateRoam();
                break;
            case State.Investigate:
                UpdateInvestigate();
                break;
            case State.Engage:
                UpdateEngage();
                break;
        }

        UpdateAnimator();
    }


    void UpdateRoam()
    {
        if (!agent.hasPath || agent.remainingDistance < 1f)
            PickRoamPoint();
    }

    void UpdateInvestigate()
    {
        if (hasSeenPlayer)
        {
            state = State.Engage;
            return;
        }

        agent.speed = investigateSpeed;
        agent.SetDestination(lastKnownPlayerPos);
    }

    void UpdateEngage()
    {
        float dist = Vector3.Distance(transform.position, player.position);

        lastKnownPlayerPos = player.position;

        if (dist <= attackRange)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            FacePlayer();
            TryShoot();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
    }

    void DetectPlayer()
    {
        playerVisible = false;

        Vector3 dir = (player.position - transform.position).normalized;
        float dist = Vector3.Distance(transform.position, player.position);

        if (dist > viewDistance) return;

        float angle = Vector3.Angle(transform.forward, dir);
        if (angle > viewAngle * 0.5f) return;

        if (Physics.Raycast(
            transform.position + Vector3.up,
            dir,
            dist,
            visionMask))
            return;

        playerVisible = true;
        hasSeenPlayer = true;
        lastKnownPlayerPos = player.position;
        state = State.Engage;
    }

    public void OnHearNoise(Vector3 pos)
    {
        if (hasSeenPlayer) return;

        lastKnownPlayerPos = pos;
        state = State.Investigate;
    }

    void TryShoot()
    {
        if (Time.time < nextFireTime)
            return;

        Vector3 origin = shootPoint.position;
        Vector3 target = player.position + Vector3.up * 0.5f;
        Vector3 dir = target - origin;
        float dist = dir.magnitude;

        if (Physics.Raycast(origin, dir.normalized, out RaycastHit hit, dist, shootBlockMask))
        {
            if (!hit.collider.CompareTag("Player"))
                return;
        }

        nextFireTime = Time.time + fireRate;

        animator.SetTrigger("Shoot");

        Instantiate(
            enemyBulletPrefab,
            shootPoint.position,
            Quaternion.LookRotation(dir)
        );
    }


    void FacePlayer()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0f;
        transform.rotation = Quaternion.LookRotation(lookDir);
    }


    void PickRoamPoint()
    {
        Vector3 random = Random.insideUnitSphere * roamRadius;
        random += transform.position;

        if (NavMesh.SamplePosition(random, out NavMeshHit hit, roamRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }


    public void Die(Vector3 force)
    {
        GameManager.Instance.EnemyKilled();

        GetComponent<NavMeshAgent>().enabled = false;
        GetComponent<Collider>().enabled = false;
        animator.enabled = false;
        enabled = false;

        EnableRagdoll(force);
    }

    void EnableRagdoll(Vector3 force)
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = false;
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    void UpdateAnimator()
    {
        float speed = agent.velocity.magnitude;

        animator.SetFloat("Speed", speed);
        animator.SetBool("IsMoving", speed > 0.1f);
        animator.SetBool("HasTarget", state == State.Engage);
        animator.SetBool("IsRunning", state == State.Engage);
    }

}
