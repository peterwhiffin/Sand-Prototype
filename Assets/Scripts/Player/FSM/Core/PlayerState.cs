using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected PlayerData playerData;
    protected string animBoolName;
   
    
    public Vector2 movementInput;
    public Vector2 lookInput;
    public float attackInput;
    public float blockInput;
    public float runInput;
    public float jumpInput;

    public bool canJump;
    public bool canAttack;

    public PlayerState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName)
    {
        this.player = player;
        this.stateMachine = stateMachine;
        this.playerData = playerData;
        this.animBoolName = animBoolName;
    }

    public virtual void Enter()
    {
        if (animBoolName != "NoAnimate")
            player.animator.SetBool(animBoolName, true); 
    }

    public virtual void Exit()
    {
        if (animBoolName != "NoAnimate")
            player.animator.SetBool(animBoolName, false);       
    }

    public virtual void LogicUpdate()
    {
        movementInput = player.inputHandler.MovementInput;
        lookInput = player.inputHandler.LookInput;
        attackInput = player.inputHandler.AttackInput;
        blockInput = player.inputHandler.BlockInput;
        runInput = player.inputHandler.RunInput;
        jumpInput = player.inputHandler.JumpInput;

        canAttack = player.inputHandler.canAttack;
        canJump = player.inputHandler.canJump;
    }

    public virtual void PhysicsUpdate()
    {

    }

    public virtual void CameraUpdate()
    {
        player.UpdateCamera(lookInput);
    }
}
