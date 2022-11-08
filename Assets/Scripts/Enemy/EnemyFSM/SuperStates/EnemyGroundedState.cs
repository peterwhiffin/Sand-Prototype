using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGroundedState : EnemyState
{
    public EnemyGroundedState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
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

        if (enemy.playerInProximity && !enemy.playerDetected)
            enemy.SightLineCheck(enemy.detectedPlayer.transform.position);

        if (enemy.currentHealth <= 0)
            stateMachine.ChangeState(enemy.DeathState);

        if (!enemy.abilityDone && !enemy.endAbility)
            enemy.CheckAbilityDone();

        if (enemy.endAbility)
            enemy.EndAbility();

        if (enemy.shouldCheckHit)
            enemy.HitCheck();

        if (enemy.hitByOther)
            stateMachine.ChangeState(enemy.HitState);  

        if(enemy.blockedByOther)
            enemy.BlockedByOther();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
