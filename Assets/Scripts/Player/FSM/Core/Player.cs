using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using FishNet.Object;
using FishNet.Component.Animating;
using FishNet.Object.Synchronizing;
using TMPro;
using FishNet.Connection;
using FishNet.Object.Prediction;

public class Player : NetworkBehaviour, IDamageable
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
    public NetworkAnimator networkAnimator;
    public Camera mainCam;
    public CharacterController controller;
    public Transform cameraFollowTarget;
    public Transform swordArmTarget;    
    public Transform hitCheckPosition;
    public Transform hitCheckPositionTwo;    
    public TwoBoneIKConstraint swordArmConstraint;
    public AudioSource audioSource;   
    public List<string> attackDirection;
    public List<Transform> blockingPositions;
    public List<AudioClip> audioClips;   
    public List<GameObject> hitObjects;   
    public List<BoxCollider> hitColliders;
    public LayerMask hitMask;
    public LayerMask groundCheckMask;  
    public TMP_Text healthValueUI;
    public TMP_Text debug;
   
    public int directionIndex;  
    public int currentHealth;
    public int animLayer;
    public int usingIndex;
    
    public float directionChoiceX;
    public float directionChoiceY;   
    public float turnSmoothVelocity;
    public float turnSmoothTime = .01f;    
    public float cameraYRot;    
    public float groundHeight;
    public float jumpHeight;
    public float blockIKWeight;
    
    public Vector3 startingCameraRotation;

    public bool abilityDone;
    public bool endAbility;
    public bool isAttacking;
    public bool isBlocking;
    public bool attackHit;
    public bool shouldCheckHit;
    public bool grounded;
    public bool jumping;
    public bool blockedOther;
    public bool hitByOther;

    //private void Awake()
    //{
    //    StateMachine = new PlayerStateMachine();
    //    IdleState = new PlayerIdleState(this, StateMachine, playerData, "idle");
    //    WalkState = new PlayerWalkState(this, StateMachine, playerData, "walk");
    //    RunState = new PlayerRunState(this, StateMachine, playerData, "run");
    //    AttackState = new PlayerAttackState(this, StateMachine, playerData, "noanimate");
    //    DeathState = new PlayerDeathState(this, StateMachine, playerData, "death");
    //    HitState = new PlayerHitState(this, StateMachine, playerData, "noanimate");
    //    JumpState = new PlayerJumpState(this, StateMachine, playerData, "noanimate");
    //    InAirState = new PlayerInAirState(this, StateMachine, playerData, "inair");
    //}

    //public void Start()
    //{
    //    StateMachine.Initialize(IdleState);
    //    currentHealth = playerData.maxHealth;
    //    inputHandler = GetComponent<InputHandler>();
    //    startingCameraRotation = cameraFollowTarget.eulerAngles;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    Cursor.visible = false;
    //    abilityDone = true;
    //    endAbility = false;
    //    blockedOther = false;
    //    hitByOther = false;
    //    healthValueUI.text = currentHealth.ToString();
    //}
    
    
    //moved state creation and setup to this method because it wasn't working in the awake and start method. Don't know why.
    public override void OnStartClient()
    {
        base.OnStartClient();
        
        if (base.IsOwner)
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

            StateMachine.Initialize(IdleState);
            currentHealth = playerData.maxHealth;
            inputHandler = GetComponent<InputHandler>();
            startingCameraRotation = cameraFollowTarget.eulerAngles;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            abilityDone = true;
            endAbility = false;
            blockedOther = false;
            hitByOther = false;
            healthValueUI.text = currentHealth.ToString();

            cameraFollowTarget.gameObject.SetActive(true);
            cameraFollowTarget.parent = null;
        }
    }


    //the base.isOwner just checks that the client running this method owns this object.
    private void Update()
    {
        if(!base.IsOwner)
            return;

        StateMachine.CurrentState.LogicUpdate();
        Debugging();       
    }


    private void FixedUpdate()
    {
        if (!base.IsOwner)
            return;

        StateMachine.CurrentState.PhysicsUpdate();
    }


    private void LateUpdate()
    {
        if (!base.IsOwner)
            return;

        StateMachine.CurrentState.CameraUpdate();
    }

    public void MoveCharacter(float speed, Vector2 movementInput)
    {
        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y).normalized;     
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);

        if (movementInput.y >= 0 && !isBlocking)
            transform.rotation = Quaternion.Euler(0f, angle, 0f);
        else if(!isBlocking)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.LerpAngle(transform.eulerAngles.y, Mathf.LerpAngle(targetAngle, targetAngle + 180, 1f), .1f), transform.eulerAngles.z);

        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        controller.Move(speed * Time.deltaTime * moveDir.normalized);    
    }

    public void Gravity() => controller.Move(transform.up * -9.81f * Time.deltaTime);

    public bool GroundCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, playerData.groundCheckDistance, groundCheckMask);

        if (colliders.Length >= 1)
            return true;
        else
            return false;
    }

    public int CombatDirection(Vector2 lookInput)
    {
        directionChoiceX += lookInput.x;
        directionChoiceY += lookInput.y;

        if (Mathf.Abs(directionChoiceX) > 45 || directionChoiceY > 15)
        {
            if (directionChoiceY > Mathf.Abs(directionChoiceX))
            {
                directionChoiceX = 0;
                directionChoiceY = 0;
                return 2;
            }

            else
            {
                if (directionChoiceX > 0)
                {
                    directionChoiceX = 0;
                    directionChoiceY = 0;
                    return 0;
                }
                else
                {
                    directionChoiceX = 0;
                    directionChoiceY = 0;
                    return 1;
                }
            }
        }
        else
            return directionIndex;
    }

    public void Block()
    {       
        swordArmTarget.SetPositionAndRotation(Vector3.Lerp(swordArmTarget.position, blockingPositions[directionIndex].position, .2f), Quaternion.RotateTowards(swordArmTarget.rotation, blockingPositions[directionIndex].rotation, 6f));
    }

    public void ShouldCheckHit() => shouldCheckHit = true;

    public void EndHitCheck() => shouldCheckHit = false;

    public void HitCheck()
    {
        if(Physics.Raycast(hitCheckPosition.position, hitCheckPositionTwo.position - hitCheckPosition.position, out RaycastHit hitInfo, Vector3.Distance(hitCheckPosition.position, hitCheckPositionTwo.position), hitMask))
        {
            if (hitInfo.transform.gameObject.GetInstanceID() != gameObject.GetInstanceID() && !hitObjects.Contains(hitInfo.transform.gameObject) && hitInfo.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
            {              
                hitObjects.Add(hitInfo.transform.gameObject);
                damageable.CheckDamage(30, hitInfo.colliderInstanceID);
            }
        }
    }
      
    public void CheckDamage(int damage, int colliderID)
    {

        Debug.Log("Checking DAmage!");
       
        if (isBlocking)

        {
            if (colliderID == hitColliders[0].GetInstanceID() && usingIndex == 0)
            {
                blockedOther = true;             
            }
            else if (colliderID == hitColliders[1].GetInstanceID() && usingIndex == 1)
            {
                blockedOther = true;                
            }
            else if (colliderID == hitColliders[2].GetInstanceID() && usingIndex == 2)
            {
                blockedOther = true;               
            }                     
            else
            {
                currentHealth -= damage;
                hitByOther = true;
                healthValueUI.text = currentHealth.ToString();
            }           
        }
        else
        {
            currentHealth -= damage;
            hitByOther = true;
            healthValueUI.text = currentHealth.ToString();
        }
    }

    public void BlockedOther()
    {
        audioSource.PlayOneShot(audioClips[0], 1);
        endAbility = true;
        blockedOther = false;
    }

    public void CheckAbilityDone()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(animLayer);

        if (stateInfo.normalizedTime >= 1)
            endAbility = true;

        networkAnimator.Animator.SetLayerWeight(animLayer, Mathf.Lerp(animator.GetLayerWeight(animLayer), 1f, .1f));
    }

    public void EndAbility()
    {
        shouldCheckHit = false;
        networkAnimator.Animator.SetLayerWeight(animLayer, Mathf.MoveTowards(animator.GetLayerWeight(animLayer), 0f, .04f));
        if (networkAnimator.Animator.GetLayerWeight(animLayer) == 0f)
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
            camY = transform.position.y;

        cameraYRot = Mathf.Clamp(Mathf.LerpAngle(cameraYRot, cameraYRot - lookInput.y, .1f), -38f, 46f);
        cameraFollowTarget.position = Vector3.Lerp(cameraFollowTarget.position, new Vector3(transform.position.x, camY, transform.position.z), 1);
        cameraFollowTarget.eulerAngles = new Vector3(Mathf.MoveTowardsAngle(cameraFollowTarget.eulerAngles.x, startingCameraRotation.x + cameraYRot, 10f), Mathf.LerpAngle(cameraFollowTarget.eulerAngles.y, cameraFollowTarget.eulerAngles.y + lookInput.x, .1f), cameraFollowTarget.eulerAngles.z);
    }

    public void Debugging()
    {      
        if(shouldCheckHit)
            Debug.DrawRay(hitCheckPosition.position, hitCheckPositionTwo.position - hitCheckPosition.position, Color.red, 5f);
    }

    private void OnDrawGizmos()
    {

    }
}
