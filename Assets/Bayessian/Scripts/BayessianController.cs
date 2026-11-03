using Unity.VisualScripting;
using UnityEngine;

public class BayessianController : MonoBehaviour, IEnemy
{
    [Header("Stats")]
    public float health = 100f;
    public Transform player;

    public float visionRange = 10;
    public float priorAttackProbability = 0.5f;
    public float attackProbability;

    private float distanceToPlayer;
    private bool playerVisible;

    private MoveCtrl movement;

    void Start()
    {
        movement = GetComponent<MoveCtrl>();
    }

    // Update is called once per frame
    void Update()
    {
        ObservePlayer();

        CalculateBayessianInference();

        DecideAction();
    }

    void ObservePlayer()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        playerVisible = distanceToPlayer <= visionRange;
    }

    void CalculateBayessianInference()
    {
        // Probabilidades condicionales

        float pDistanceGivenAttack;
        float pVisibleGivenAttack;
        float pHealthGivenAttack;

        if (distanceToPlayer < 4f)
        {
            pDistanceGivenAttack = 0.8f;
        }
        else if (distanceToPlayer < 8f)
        {
            pDistanceGivenAttack = 0.5f;
        }
        else
        {
            pDistanceGivenAttack = 0.2f;
        }

        pVisibleGivenAttack = playerVisible ? 0.7f : 0.2f;

        if (health < 30f)
        {
            pHealthGivenAttack = 0.3f;
        }
        else
        {
            pHealthGivenAttack = 0.7f;
        }

        attackProbability = priorAttackProbability * pDistanceGivenAttack * pVisibleGivenAttack * pHealthGivenAttack;
        priorAttackProbability = Mathf.Clamp01(attackProbability * 3f);
    }

    void DecideAction()
    {
        if (attackProbability > 0.65f)
        {
            Debug.Log("Attacking!!");
        }
        else if (attackProbability > 0.35f)
        {
            movement.MoveToward(player.position);
        }
        else
        {
            movement.MoveAway(player.position);
        }
    }

    public void setHealth(float health)
    {
        this.health = health;
    }
}
