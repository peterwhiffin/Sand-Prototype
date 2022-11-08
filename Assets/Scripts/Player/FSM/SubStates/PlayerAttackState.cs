using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerAbilityState
{
    public PlayerAttackState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.inputHandler.canAttack = false;
        player.animLayer = 1;       
        player.animator.Play(player.attackDirection[player.usingIndex], 1, 0f);
        player.isAttacking = true; 
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        stateMachine.ChangeState(stateMachine.PreviousState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
