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

        if (player.dead)
        {
            stateMachine.ChangeState(player.DeathState);
        }

        if (!player.controller.isGrounded)
            stateMachine.ChangeState(player.InAirState);

        if (player.hitByOther)
            stateMachine.ChangeState(player.HitState);

        if (player.abilityDone)
        {
            if (attackInput != 0 && blockInput == 0 && canAttack)
            {
                player.currentIndex = player.blockIndex;
                stateMachine.ChangeState(player.AttackState);
            }

            if (jumpInput != 0 && canJump)
                stateMachine.ChangeState(player.JumpState);
        }

        if (attackInput == 0)
            player.CombatDirection(lookInput);

        if (blockInput != 0)
        {
            player.CameraLockToCharacter();
            player.blocking = true;
            player.endAbility = true;
            player.swordArmConstraint.weight = Mathf.Lerp(player.swordArmConstraint.weight, 1, .2f);
            player.Block();

            player.blockCollider.enabled = true;
        }
        else
        {
            player.blockCollider.enabled = false;
            player.blocking = false;
            player.swordArmConstraint.weight = Mathf.Lerp(player.swordArmConstraint.weight, 0, .2f);
        }

        if (!player.abilityDone && !player.endAbility)
            player.CheckAbilityDone();

        if (player.endAbility)
            player.EndAbility();

        if (player.shouldCheckHit)
            player.HitCheck();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
