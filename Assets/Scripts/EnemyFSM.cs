using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(WeaponController))]
[RequireComponent(typeof(Health))]
public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { Idle, Chase, Attack, Flee }

    [Header("Current State")]
    public EnemyState currentState = EnemyState.Idle;

    [Header("Detection & Combat Parameters")]
    [SerializeField] private Transform playerTarget;
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float attackRadius = 8f; // Radio donde se detiene a disparar
    [SerializeField] private float safeDistance = 20f;
    [SerializeField] private float fleeRunDistance = 5f;
    [SerializeField, Range(0f, 1f)] private float fleeHealthPercentage = 0.3f; // 30% de vida

    [Header("Patrol Parameters")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waitTimeAtWaypoint = 2f;

    private int currentWaypointIndex = 0;
    private float waitTimer = 0f;
    private bool isWaiting = false;

    // Referencias a los módulos
    private NavMeshAgent agent;
    private WeaponController weaponController;
    private Health healthComponent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        weaponController = GetComponent<WeaponController>();
        healthComponent = GetComponent<Health>();
    }

    private void Update()
    {
        if (playerTarget == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);

        DetermineState(distanceToPlayer);
        ExecuteState();
    }

    private void DetermineState(float distanceToPlayer)
    {
        // Leemos el porcentaje de vida directamente del módulo Health
        bool isLowHealth = healthComponent.GetHealthPercentage() <= fleeHealthPercentage;
        EnemyState previousState = currentState;

        if (isLowHealth && distanceToPlayer <= detectionRadius)
        {
            currentState = EnemyState.Flee;
        }
        else if (currentState == EnemyState.Flee && distanceToPlayer > safeDistance)
        {
            currentState = EnemyState.Idle;
        }
        else if (!isLowHealth)
        {
            if (distanceToPlayer <= attackRadius)
            {
                currentState = EnemyState.Attack;
            }
            else if (distanceToPlayer <= detectionRadius)
            {
                currentState = EnemyState.Chase;
            }
            else
            {
                currentState = EnemyState.Idle;
            }
        }

        if (previousState != EnemyState.Idle && currentState == EnemyState.Idle)
        {
            isWaiting = false;
        }
    }

    private void ExecuteState()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                Patrol();
                break;

            case EnemyState.Chase:
                agent.isStopped = false;
                agent.SetDestination(playerTarget.position);
                break;

            case EnemyState.Attack:
                // Detener el movimiento en seco
                if (!agent.isStopped)
                {
                    agent.ResetPath();
                    agent.isStopped = true;
                }

                // Rotar suavemente hacia el jugador (ignorando eje Y para no inclinarse)
                Vector3 direction = (playerTarget.position - transform.position).normalized;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction), Time.deltaTime * 5f);
                }

                // Intentar disparar (el WeaponController maneja la cadencia y recarga)
                weaponController.TryShoot();
                break;

            case EnemyState.Flee:
                agent.isStopped = false;
                Vector3 fleeDirection = (transform.position - playerTarget.position).normalized;
                Vector3 fleeTargetPosition = transform.position + (fleeDirection * fleeRunDistance);

                NavMeshHit hit;
                if (NavMesh.SamplePosition(fleeTargetPosition, out hit, fleeRunDistance, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
                else
                {
                    agent.SetDestination(fleeTargetPosition);
                }
                break;
        }
    }

    private void Patrol()
    {
        agent.isStopped = false;

        if (waypoints == null || waypoints.Length == 0) return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTimeAtWaypoint)
            {
                isWaiting = false;
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                agent.SetDestination(waypoints[currentWaypointIndex].position);
            }
        }
        else
        {
            agent.SetDestination(waypoints[currentWaypointIndex].position);
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.1f)
            {
                isWaiting = true;
                waitTimer = 0f;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red; // Color rojo para el rango de ataque
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, safeDistance);
    }
}