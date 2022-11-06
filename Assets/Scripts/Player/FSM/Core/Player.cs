using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class Player : MonoBehaviour, IDamageable
{
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerAttackState AttackState { get; private set; }
    public PlayerDeathState DeathState { get; private set; }
    public PlayerHitState HitState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerInAirState InAirState { get; private set; }


    [SerializeField] private PlayerData playerData;
    public InputHandler inputHandler;
    public Animator animator;
    public Camera mainCam;
    public CharacterController controller;
    public Rigidbody playerRB;
    public Transform cameraFollowTarget;
    public Transform swordArmTarget;
    public Transform hitCheckPosition;
    public TwoBoneIKConstraint swordArmConstraint;
    public AudioSource audioSource;
    public List<string> attackDirection;
    public List<Transform> blockingPositions;
    public List<AudioClip> audioClips;
    public List<GameObject> hitObjects;
    public LayerMask hitMask;
    public LayerMask groundCheckMask;
   
    public int blockIndex;
    public int currentHealth;
    public int animLayer;
    public int currentIndex;
    
    public float blockChoiceX;
    public float blockChoiceY;
    public float turnSmoothVelocity;
    public float turnSmoothTime = .01f;
    public float cameraYRot;
    public float groundHeight;
    public float jumpHeight;
    
    public Vector3 startingCameraRotation;

    public bool abilityDone;
    public bool endAbility;
    public bool isAttacking;
    public bool blocking;
    public bool attackBlocked;
    public bool attackHit;
    public bool shouldCheckHit;
    public bool dead;
    public bool hitByOther;
    public bool grounded;
    public bool jumping;

    public void Awake()
    {
        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine, playerData, "Idle");
        WalkState = new PlayerWalkState(this, StateMachine, playerData, "Walk");
        RunState = new PlayerRunState(this, StateMachine, playerData, "Run");
        AttackState = new PlayerAttackState(this, StateMachine, playerData, "NoAnimate");
        DeathState = new PlayerDeathState(this, StateMachine, playerData, "Death");
        HitState = new PlayerHitState(this, StateMachine, playerData, "NoAnimate");
        JumpState = new PlayerJumpState(this, StateMachine, playerData, "NoAnimate");
        InAirState = new PlayerInAirState(this, StateMachine, playerData, "InAir");
    }

    public void Start()
    {
        currentHealth = playerData.maxHealth;
        inputHandler = GetComponent<InputHandler>();
        StateMachine.Initialize(IdleState);
        startingCameraRotation = cameraFollowTarget.eulerAngles;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        abilityDone = true;
        endAbility = false;
        dead = false;
    }

    public void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        Debugging();
    }

    public void FixedUpdate() => StateMachine.CurrentState.PhysicsUpdate();

    public void LateUpdate() => StateMachine.CurrentState.CameraUpdate();


    public void MoveCharacter(float speed, Vector2 input)
    {
        Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;
      
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        if (input.y >= 0 && !blocking)
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        else if(!blocking)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.LerpAngle(transform.eulerAngles.y, Mathf.LerpAngle(targetAngle, targetAngle + 180, 1f), .1f), transform.eulerAngles.z);

        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        controller.Move(speed * Time.deltaTime * moveDir.normalized);    
    }

    public void Gravity() => controller.Move(transform.up * -11f * Time.deltaTime);

    public bool GroundCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, playerData.groundCheckDistance, groundCheckMask);

        if (colliders.Length >= 1)
            return true;
        else
            return false;
    }

    public void CombatDirection(Vector2 lookInput)
    {
        blockChoiceX += lookInput.x;
        blockChoiceY += lookInput.y;

        if (Mathf.Abs(blockChoiceX) > 45 || blockChoiceY > 25)
        {
            if (blockChoiceY > Mathf.Abs(blockChoiceX))
                blockIndex = 2;
            else
            {
                if (blockChoiceX > 0)
                    blockIndex = 0;
                else
                    blockIndex = 1;
            }

            blockChoiceX = 0;
            blockChoiceY = 0;
        }
    }

    public void Block() => swordArmTarget.SetPositionAndRotation(Vector3.Lerp(swordArmTarget.position, blockingPositions[blockIndex].position, .2f), Quaternion.RotateTowards(swordArmTarget.rotation, blockingPositions[blockIndex].rotation, 6f));

    public bool CheckDamage(int damage, int swingDirection)
    {
        if(blocking && swingDirection == blockIndex)
        {
            BlockedAttack();
            return true;
        }
        else
        {
            currentHealth -= damage;
            hitByOther = true;

            if (currentHealth <= 0)
                dead = true;

            AttackHit();           
            return false;
        }
    }

    public void HitCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(hitCheckPosition.position, .3f, hitMask, QueryTriggerInteraction.Ignore);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out IDamageable damageableObject) && collider.gameObject != gameObject && !hitObjects.Contains(collider.gameObject))
            {
                damageableObject.CheckDamage(30, currentIndex);
                hitObjects.Add(collider.gameObject);
            }
        }
    }

    public void ShouldCheckHit() => shouldCheckHit = true;

    public void EndHitCheck() => shouldCheckHit = false;

    public void AttackHit()
    {
        audioSource.PlayOneShot(audioClips[1], 1);
        attackHit = false;
    }

    public void BlockedAttack()
    {
        endAbility = true;
        audioSource.PlayOneShot(audioClips[0], 1);
        attackBlocked = false;
    }

    public void CheckAbilityDone()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(animLayer);

        if (stateInfo.normalizedTime >= 1)
            endAbility = true;

        animator.SetLayerWeight(animLayer, Mathf.Lerp(animator.GetLayerWeight(animLayer), 1f, .1f));
    }

    public void EndAbility()
    {
        shouldCheckHit = false;
        animator.SetLayerWeight(animLayer, Mathf.MoveTowards(animator.GetLayerWeight(animLayer), 0f, .04f));

        if (animator.GetLayerWeight(1) == 0f)
        {            
            abilityDone = true;
            endAbility = false;
            isAttacking = false;
            hitObjects.Clear();
            animator.SetBool("AbilityDone", true);
        }
    }

    public void CameraLockToCharacter() => transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.LerpAngle(transform.eulerAngles.y, cameraFollowTarget.eulerAngles.y, .1f), transform.eulerAngles.z);

    public void UpdateCamera(Vector2 lookInput)
    {
        float camY;

        if (StateMachine.CurrentState == InAirState)
            camY = transform.position.y;
        else
            camY = 0;

        cameraYRot = Mathf.Clamp(Mathf.LerpAngle(cameraYRot, cameraYRot - lookInput.y, .1f), -38f, 46f);
        cameraFollowTarget.position = Vector3.Lerp(cameraFollowTarget.position, new Vector3(transform.position.x, camY, transform.position.z), 1);
        cameraFollowTarget.eulerAngles = new Vector3(Mathf.MoveTowardsAngle(cameraFollowTarget.eulerAngles.x, startingCameraRotation.x + cameraYRot, 10f), Mathf.LerpAngle(cameraFollowTarget.eulerAngles.y, cameraFollowTarget.eulerAngles.y + lookInput.x, .1f), cameraFollowTarget.eulerAngles.z);
    }

    public void Debugging()
    {
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if(shouldCheckHit)
            Gizmos.DrawWireSphere(hitCheckPosition.position, .3f);

        Gizmos.DrawWireSphere(transform.position, playerData.groundCheckDistance);
    }
}
