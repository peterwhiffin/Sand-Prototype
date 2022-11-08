using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations.Rigging;

public class Enemy : MonoBehaviour, IDamageable
{
    public EnemyStateMachine StateMachine { get; private set; }
    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyPursueState PursueState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    public EnemyBlockState BlockState { get; private set; }
    public EnemyDeathState DeathState { get; private set; }
    public EnemyHitState HitState { get; private set; }

    [SerializeField] private EnemyData enemyData;
    public NavMeshAgent enemyAgent;
    public Player detectedPlayer;
    public Animator animator;
    public Transform enemySwordArmTarget;
    public Transform hitCheckPosition;
    public TwoBoneIKConstraint enemySwordArmConstraint;
    public BoxCollider blockCollider;
    public LayerMask hitMask;
    public AudioSource audioSource;
    public List<AudioClip> audioClips;
    public List<string> enemyAttackDirection;
    public List<Transform> enemyBlockingPositions;
    public List<Vector3> destinationPositions;
    public List<Transform> destinations;
    public List<GameObject> hitObjects;

    public int currentHealth;
    public int currentDestination;
    public int blockIndex;
    public int animLayer;
    public int currentIndex;

    public bool abilityDone;
    public bool endAbility;
    public bool playerInProximity;
    public bool playerDetected;
    public bool isAttacking;
    public bool shouldBattleCry;
    public bool isBlocking;   
    public bool shouldCheckHit;
    
    public bool hitByOther;
    public bool blockedByOther;

    public float attackTimer;
    public float blockTimer;




    private void Awake()
    {
        StateMachine = new EnemyStateMachine();
        IdleState = new EnemyIdleState(this, StateMachine, enemyData, "Idle");
        PatrolState = new EnemyPatrolState(this, StateMachine, enemyData, "Walk");
        PursueState = new EnemyPursueState(this, StateMachine, enemyData, "NoAnimate");
        AttackState = new EnemyAttackState(this, StateMachine, enemyData, "Run");
        BlockState = new EnemyBlockState(this, StateMachine, enemyData, "Idle");
        DeathState = new EnemyDeathState(this, StateMachine, enemyData, "NoAnimate");
        HitState = new EnemyHitState(this, StateMachine, enemyData, "NoAnimate");
    }

    private void Start()
    {
        currentHealth = enemyData.maxHealth;
        StateMachine.Initialize(IdleState);
        shouldBattleCry = true;
        abilityDone = true;
        playerInProximity = false;
        playerDetected = false;
        isAttacking = false;
        isBlocking = false;    
        endAbility = false;

        blockedByOther = false;
        hitByOther = false;

        attackTimer = 3f;
        blockTimer = 3f;

        for (int i = 0; i < destinations.Count; i++)
            destinationPositions.Add(destinations[i].position);
        
        currentDestination = Random.Range(0, destinationPositions.Count);
    }

    private void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        Debugging();
    }

    private void FixedUpdate() => StateMachine.CurrentState.PhysicsUpdate();

    public void MoveEnemy(Vector3 destination) => enemyAgent.SetDestination(destination);

    public void SightLineCheck(Vector3 playerPosition)
    {
        Vector3 directionToPlayer = playerPosition - transform.position;
        float visionAngle = Vector3.Angle(transform.forward, directionToPlayer);

        if(visionAngle < 55)
            playerDetected = true;

        if (Vector3.Distance(transform.position, playerPosition) < 5)
            playerDetected = true;
    }

    public bool ShouldBlock()
    {
        if (detectedPlayer.isAttacking && !isBlocking)
        {
            blockIndex = detectedPlayer.usingIndex;
            return true;
        }
        else
            return false;
    }

    public void Block()
    {        
        enemySwordArmConstraint.weight = Mathf.MoveTowards(enemySwordArmConstraint.weight, 1f, .1f);
        enemySwordArmTarget.SetPositionAndRotation(Vector3.Lerp(enemySwordArmTarget.position, enemyBlockingPositions[blockIndex].position, .1f), Quaternion.RotateTowards(enemySwordArmTarget.rotation, enemyBlockingPositions[blockIndex].rotation, 3f));
    }

    public bool EndBlock()
    {
        enemySwordArmConstraint.weight = Mathf.MoveTowards(enemySwordArmConstraint.weight, 0f, .1f);

        if (enemySwordArmConstraint.weight <= .01f)
            return true;
        else
            return false;
    }

    public int SwingChoice()
    {
        if (detectedPlayer.isBlocking)
        {
            if (detectedPlayer.usingIndex == 0)
                return Random.Range(1, 3);
            else if (detectedPlayer.usingIndex == 2)
                return Random.Range(0, 2);
            else
            {
                int zero = Random.Range(0, 100);
                int two = Random.Range(0, 100);

                if (zero > two)
                    return 0;
                else
                    return 2;
            }                   
        }
        else
            return Random.Range(0, 2);
    }

    public void CheckAbilityDone()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(1);

        if (stateInfo.normalizedTime >= 1)
            endAbility = true;

        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, .1f));
    }

    public void EndAbility()
    {
        shouldCheckHit = false;
        animator.SetLayerWeight(1, Mathf.MoveTowards(animator.GetLayerWeight(1), 0f, .04f));

        if (animator.GetLayerWeight(1) == 0f)
        {
            animator.SetBool("AbilityDone", true);
            abilityDone = true;
            endAbility = false;
            isAttacking = false;
            shouldCheckHit = false;
            hitObjects.Clear();
        }
    }

    public void ShouldCheckHit() => shouldCheckHit = true;

    public void EndHitCheck() => shouldCheckHit = false;

    public void HitCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(hitCheckPosition.position, .3f, hitMask, QueryTriggerInteraction.Ignore);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out IDamageable damageableObject) && collider.gameObject != gameObject && !hitObjects.Contains(collider.gameObject))
            {
                blockedByOther = damageableObject.CheckDamage(30, currentIndex);
                hitObjects.Add(collider.gameObject);
            }
        }
    }

    public bool CheckDamage(int damage, int swingDirection)
    {
        if (isBlocking && swingDirection == blockIndex)
            return true;       
        else
        {
            currentHealth -= damage;
            hitByOther = true;
            return false;
        }
    }

    public void BlockedByOther()
    {
        audioSource.PlayOneShot(audioClips[0], 1);
        endAbility = true;
        blockedByOther = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInProximity = true;
            detectedPlayer = other.gameObject.GetComponent<Player>();
        }
    }

    public void Debugging()
    {
       // Debug.Log(Vector3.Angle(transform.forward, detectedPlayer.transform.forward));
    }
}
