using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        player.grounded = player.GroundCheck();

        player.Gravity();

        if (player.currentHealth <= 0)
            stateMachine.ChangeState(player.DeathState);
        
        if (!player.controller.isGrounded)
            stateMachine.ChangeState(player.InAirState);

        if (player.hitByOther)
            stateMachine.ChangeState(player.HitState);

        if (player.abilityDone)
        {
            if (attackInput != 0 && blockInput == 0 && canAttack)
            {
                player.usingIndex = player.directionIndex;
                stateMachine.ChangeState(player.AttackState);
            }

            if (jumpInput != 0 && canJump)
                stateMachine.ChangeState(player.JumpState);
        }

        if (attackInput == 0)
            player.directionIndex = player.CombatDirection(lookInput);

        if (blockInput != 0)
        {
            player.CameraLockToCharacter();
            player.usingIndex = player.directionIndex;
            player.networkAnimator.Animator.SetFloat("BlockIndex", player.usingIndex);
            player.animator.SetBool("AbilityDone", true);
            player.isBlocking = true;
            player.endAbility = true;
            player.animator.SetBool("Block", true);
            player.networkAnimator.Animator.SetLayerWeight(1, 1f);
            player.Block();
        }
        else
        {
            //player.networkAnimator.Animator.SetLayerWeight(1, 0f);
            player.isBlocking = false;
            player.animator.SetBool("Block", false);
            player.animator.SetBool("AbilityDone", false);
            //player.animator.SetBool("AbilityDone", true);
        }

        if (!player.abilityDone && !player.endAbility)
            player.CheckAbilityDone();

        if (player.endAbility)
            player.EndAbility();

        if (player.shouldCheckHit)
            player.HitCheck();
        
        if (player.blockedOther)
            player.BlockedOther();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
