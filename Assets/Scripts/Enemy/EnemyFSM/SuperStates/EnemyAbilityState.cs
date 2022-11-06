using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAbilityState : EnemyState
{
    public EnemyAbilityState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.animator.SetBool("AbilityDone", false);
        enemy.abilityDone = false;
        enemy.animator.SetLayerWeight(1, 1f);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
