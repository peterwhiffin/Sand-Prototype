using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using TMPro;

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
    public Transform hitCheckPositionTwo;
    public TwoBoneIKConstraint swordArmConstraint;
    public AudioSource audioSource;
    public List<string> attackDirections;
    public List<Transform> blockingPositions;
    public List<AudioClip> audioClips;
    public List<GameObject> hitObjects;
    public List<GameObject> hasBeenHit;
    public LayerMask hitMask;
    public LayerMask groundCheckMask;
    public Collider weaponCollider;
    public GameObject weapon;

    public TMP_Text FPSValue;
    public GameObject enemy;
   
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
    public float targetAngle;
    public float lookAngle;

    public Vector3 hitCheckSize;
    
    public Vector3 startingCameraRotation;

    public Vector3 oldHitCheckPosition;

    public bool abilityDone;
    public bool endAbility;   
    public bool isAttacking;
    public bool isBlocking;
    public bool attackHit;
    public bool shouldCheckHit;

    public bool grounded;
    public bool jumping;

    public bool blockedByOther;
    public bool hitByOther;

    
    //state creation
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
        StateMachine.Initialize(IdleState);
        currentHealth = playerData.maxHealth;
        inputHandler = GetComponent<InputHandler>();       
        startingCameraRotation = cameraFollowTarget.eulerAngles;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        abilityDone = true;
        endAbility = false;
        blockedByOther = false;
        hitByOther = false;
        usingIndex = 0;
        oldHitCheckPosition = hitCheckPosition.position;
    }


    //runs the logicUpdate method of the current state, which is set in the StateMachine class by calling stateMachine.ChangeState();
    public void Update()
    {
        StateMachine.CurrentState.LogicUpdate();
        Debugging();
        oldHitCheckPosition = hitCheckPosition.position;
    }

    public void FixedUpdate() => StateMachine.CurrentState.PhysicsUpdate();

    public void LateUpdate() => StateMachine.CurrentState.CameraUpdate();
   
    public void MoveCharacter(float speed, Vector2 movementInput)
    {
        //this gets the correct angle to rotate the character in reference to the camera so the character will rotate left, right, and forward when moving. This is just Brackeys. 
        Vector3 direction = new Vector3(movementInput.x, 0f, movementInput.y).normalized;     
         targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCam.transform.eulerAngles.y;
        //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        lookAngle = Mathf.LerpAngle(transform.eulerAngles.y, targetAngle, turnSmoothTime);


        //rotates the player forward if not blocking and trying to move forward.
        if (movementInput.y >= 0 && !isBlocking)
            transform.rotation = Quaternion.Euler(0f, lookAngle, 0f);
        else if(!isBlocking)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.LerpAngle(transform.eulerAngles.y, Mathf.LerpAngle(targetAngle, targetAngle + 180, 1f), .1f), transform.eulerAngles.z);

        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

        controller.Move(speed * Time.deltaTime * moveDir.normalized);    
    }

    //moves the character downward. This is a WIP. This is not how gravity works.
    public void Gravity() => controller.Move(transform.up * -9.81f * Time.deltaTime);

    public bool GroundCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, playerData.groundCheckDistance, groundCheckMask);

        if (colliders.Length >= 1)
            return true;
        else
            return false;
    }

    //This selects an integer based on mouse movement input. 
    //Just checks if vertical movement is greater than horizontal and sets directionIndex, which is used for block and swing direction.
    //The direction choices have minimum thresholds to be able to change the direction index, which should probably be variables.
    //Currently biased towards vertical mouse movement.
    public void CombatDirection(Vector2 lookInput)
    {
        directionChoiceX += lookInput.x;
        directionChoiceY += lookInput.y;

        if (Mathf.Abs(directionChoiceX) > 45 || directionChoiceY > 15)
        {
            if (directionChoiceY > Mathf.Abs(directionChoiceX))
                directionIndex = 2;
            else
            {
                if (directionChoiceX > 0)
                    directionIndex = 0;
                else
                    directionIndex = 1;
            }

            directionChoiceX = 0;
            directionChoiceY = 0;
        }
    }

    //moves and rotates the sword arm IK target based on direction index. 
    public void Block() => swordArmTarget.SetPositionAndRotation(Vector3.Lerp(swordArmTarget.position, blockingPositions[directionIndex].position, .2f), Quaternion.RotateTowards(swordArmTarget.rotation, blockingPositions[directionIndex].rotation, 6f));

    public void ShouldCheckHit() => shouldCheckHit = true;

    public void EndHitCheck() => shouldCheckHit = false;

    //This gets called while shouldCheckHit is true, which is set by animation events on the different swing clips.
    //Checks all colliders hit for the IDamageable interface and runs checkdamage method on that object, which returns a bool indicating if our attack was blocked.
    //also temporarily adds the hit object to a list so that we can only hit each object once per attack.
    // this list is cleared in the EndAbility method.
    public void HitCheck()
    {
        Collider[] colliders = Physics.OverlapSphere(hitCheckPosition.position, .4f, hitMask);

        foreach (Collider collider in colliders)
        {
            if (collider.TryGetComponent<IDamageable>(out IDamageable damageableObject) && collider.gameObject != gameObject && !hitObjects.Contains(collider.transform.root.gameObject))
            {
                //blockedByOther = damageableObject.CheckHit(30, usingIndex);

                hitObjects.Add(collider.gameObject);

                //if (blockedByOther)
                //{
                //    shouldCheckHit = false;
                    
                //}
            }
        }

        if (hitObjects.Contains(weapon))
        {
            blockedByOther = true;
        }
        else
            foreach(GameObject hitObject in hitObjects)
            {
                if (!hasBeenHit.Contains(hitObject))
                {
                    hitObject.GetComponent<IDamageable>().CheckHit(30, usingIndex);
                    hasBeenHit.Add(hitObject);
                }
            }
    }

    //this interface method is called by other objects' hit checks.
    //returns blocking bool and applies damage if not blocked.
    //may want to use a general damage method for all damage.
    public bool CheckHit(int damage, int swingDirection)
    {
            currentHealth -= damage;
            hitByOther = true;
            return false;      
    }

    //plays block audio when OUR attack is blocked and ends our current ability
    public void BlockedByOther()
    {
        audioSource.PlayOneShot(audioClips[0], 1);
        endAbility = true;
        blockedByOther = false;
    }

    //this is called while abilityDone is false, which is set to false when we enter an state that derives from the PlayerAbility class
    //moves the correct animation layer weight to 1 and sets endAbility bool once the animation is finished playing, which will call the EndAbility method.
    public void CheckAbilityDone()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(animLayer);

        if (stateInfo.normalizedTime >= 1)
            endAbility = true;

        animator.SetLayerWeight(animLayer, Mathf.Lerp(animator.GetLayerWeight(animLayer), 1f, .1f));
    }

    //called after the animation of current ability is finished.
    //basically just resets the ability-initiating variables.
    public void EndAbility()
    {
        shouldCheckHit = false;
        animator.SetLayerWeight(animLayer, Mathf.MoveTowards(animator.GetLayerWeight(animLayer), 0f, .04f));

        if (animator.GetLayerWeight(1) == 0f)
        {            
            abilityDone = true;
            endAbility = false;
            isAttacking = false;
            hasBeenHit.Clear();
            hitObjects.Clear();
            animator.SetBool("AbilityDone", true);
        }
    }

    //locks camera behind character. This will probably get updated/moved.
    public void CameraLockToCharacter() => transform.eulerAngles = new Vector3(transform.eulerAngles.x, Mathf.LerpAngle(transform.eulerAngles.y, cameraFollowTarget.eulerAngles.y, .1f), transform.eulerAngles.z);

    //basic camera controller.
    //The mainCam is a child of the cameraFollowTarget object.
    //cameraFollowTarget just lerps its position and rotation around based on player position and mouse input. WIP.
    public void UpdateCamera(Vector2 lookInput)
    {
        float camY;

        if (StateMachine.CurrentState == InAirState)
            camY = transform.position.y;
        else
            camY = 0;

        cameraYRot = Mathf.Clamp(Mathf.LerpAngle(cameraYRot, cameraYRot - lookInput.y, .1f), -38f, 46f);
        cameraFollowTarget.position = Vector3.Lerp(cameraFollowTarget.position, new Vector3(transform.position.x, camY, transform.position.z), 1);
        cameraFollowTarget.eulerAngles = new Vector3(Mathf.MoveTowardsAngle(cameraFollowTarget.eulerAngles.x, startingCameraRotation.x + cameraYRot, 10f), Mathf.Lerp(cameraFollowTarget.eulerAngles.y, cameraFollowTarget.eulerAngles.y + lookInput.x, .1f), cameraFollowTarget.eulerAngles.z);
    }

    //just a place to keep all my general debugs.
    public void Debugging()
    {
        FPSValue.text = Mathf.FloorToInt(1 / Time.deltaTime).ToString();

        if(shouldCheckHit)
            Debug.DrawRay(hitCheckPosition.position, hitCheckPosition.position - hitCheckPositionTwo.position, Color.red, 10f);
    }

    //draws gizmos.
    //currently draws the hitcheck and groundcheck spheres.
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
    }
}
