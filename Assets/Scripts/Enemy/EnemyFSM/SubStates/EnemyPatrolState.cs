using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : EnemyGroundedState
{
    public EnemyPatrolState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
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

        enemy.MoveEnemy(enemy.destinationPositions[enemy.currentDestination]);

        if (Vector3.Distance(enemy.transform.position, enemy.destinationPositions[enemy.currentDestination]) < 1f)
        {
            stateMachine.ChangeState(enemy.IdleState);
        }

        if (enemy.playerDetected)
            stateMachine.ChangeState(enemy.PursueState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
