using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsGround, whatIsPlayer;
    private NavMeshAgent agent;

    private Vector3 lastPosition;
    private Vector3 walkPoint;
    private bool walkPointSet = false;
    private bool isRaycastChasing = false;

    private const float MOVEMENT_THRESHOLD = 0.01f;
    private const float DISTANCE_TO_WALK_POINT_THRESHOLD = 4f;
    private const float RAYCAST_GROUND_LENGTH = 5f;
    private const float PATROL_RESET_TIME = 1f;

    public float walkPointRange;
    public float sightRange, sightRangeRaycast, attackRange;
    private float stationaryTimer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        CheckPlayerProximity();
        ResetIfStuck();
    }

    private void CheckPlayerProximity()
    {
        if (IsPlayerInAttackRange())
        {
            AttackPlayer();
        }
        else if (IsPlayerInCloseRange())
        {
            StartChasePlayer();
        }
        else if (IsPlayerInSightRaycastRange() || isRaycastChasing)
        {
            PerformRaycastChase();
        }
        else
        {
            Patrol();
        }
    }

    private bool IsPlayerInAttackRange()
    {
        return Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
    }

    private bool IsPlayerInCloseRange()
    {
        return Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
    }

    private bool IsPlayerInSightRaycastRange()
    {
        return Physics.CheckSphere(transform.position, sightRangeRaycast, whatIsPlayer);
    }

    private void StartChasePlayer()
    {
        isRaycastChasing = false;
        ChasePlayer();
    }

    private void PerformRaycastChase()
    {
        if (RaycastToPlayer())
        {
            isRaycastChasing = true;
        }
        else if (isRaycastChasing)
        {
            CheckLastPositionSeen();
        }
    }

    private bool RaycastToPlayer()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (player.position - transform.position).normalized, out hit, sightRangeRaycast))
        {
            if (hit.transform == player)
            {
                ChasePlayer();
                return true;
            }
        }
        return false;
    }

    private void CheckLastPositionSeen()
    {
        if (Vector3.Distance(transform.position, agent.destination) < DISTANCE_TO_WALK_POINT_THRESHOLD)
        {
            Patrol();
        }
    }

    private void ChasePlayer()
    {
        walkPointSet = false;
        if (Physics.Raycast(player.position, -transform.up, RAYCAST_GROUND_LENGTH, whatIsGround))
        {
            agent.SetDestination(player.position);
        }
    }

    private void AttackPlayer()
    {
        GameplayManager.Instance.EndGame("You have been eaten by cthulululu.");
    }

    private void Patrol()
    {
        isRaycastChasing = false;
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }
        if (walkPointSet)
        {
            agent.SetDestination(walkPoint);
            CheckWalkPointReached();
        }

        Debug.DrawLine(transform.position, walkPoint, Color.green);
    }

    private void SearchWalkPoint()
    {
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);
        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, RAYCAST_GROUND_LENGTH, whatIsGround))
            walkPointSet = true;
        
        Debug.DrawRay(walkPoint, -transform.up * RAYCAST_GROUND_LENGTH, Color.red, 1f);
    }

    private void CheckWalkPointReached()
    {
        if (Vector3.Distance(transform.position, walkPoint) < DISTANCE_TO_WALK_POINT_THRESHOLD)
            walkPointSet = false;
    }

    private void ResetIfStuck()
    {
        if (HasMoved())
        {
            ResetStationaryTimer();
        }
        else
        {
            UpdateStationaryTimer();
            if (ShouldResetPatrol())
            {
                ResetPatrol();
            }
        }
        lastPosition = transform.position;
    }

    private bool HasMoved()
    {
        return Vector3.Distance(transform.position, lastPosition) >= MOVEMENT_THRESHOLD;
    }

    private void ResetStationaryTimer()
    {
        stationaryTimer = 0;
    }

    private void UpdateStationaryTimer()
    {
        stationaryTimer += Time.deltaTime;
    }

    private bool ShouldResetPatrol()
    {
        return stationaryTimer >= PATROL_RESET_TIME;
    }

    private void ResetPatrol()
    {
        stationaryTimer = 0;
        walkPointSet = false;
        Patrol();
    }
}
