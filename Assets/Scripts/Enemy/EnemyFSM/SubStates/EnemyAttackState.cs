using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyAbilityState
{
    public EnemyAttackState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.animator.Play(enemy.enemyAttackDirection[enemy.currentIndex = enemy.SwingChoice()], 1, 0);
        enemy.isAttacking = true;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.attackTimer = 0f;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        stateMachine.ChangeState(enemy.PursueState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
