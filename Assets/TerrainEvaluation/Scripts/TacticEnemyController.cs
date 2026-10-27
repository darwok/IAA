using UnityEngine;
using UnityEngine.AI;
using Panda;

public class TacticEnemyController : MonoBehaviour, IEnemy
{
    public Transform player;
    public float health = 100f;

    public NavMeshAgent agent;
    public TTactics tactics;

    public float lowHealthThresh = 30f;

    [Task]
    public bool isLowHealth()
    {
        return health < lowHealthThresh;
    }

    [Task]
    public void MoveToAttackPoint()
    {
        Transform point = tactics.GetBestPoint(TAState.Attack);

        if (point == null)
        {
            Task.current.Fail();
            return;
        }
        
        agent.SetDestination(point.position);
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
            Task.current.Succeed();
    }

    [Task]
    public void MoveToSafePoint()
    {
        Transform point = tactics.GetBestPoint(TAState.Flee);

        if (point == null)
        {
            Task.current.Fail();
            return;
        }

        agent.SetDestination(point.position);
        if (!agent.pathPending && agent.remainingDistance < 1.5f)
            Task.current.Succeed();
    }

    [Task]
    public void Attack()
    {
        Debug.Log("Attacking");
        Task.current.Succeed();
    }

    [Task]
    public void Idle()
    {
        Task.current.Succeed();
    }
}
