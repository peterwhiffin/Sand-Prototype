using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdleState : EnemyGroundedState
{
    public EnemyIdleState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    private float timer;
    private float idleWaitTime;
    private bool hasNewDestination;
    private int newDestination;

    public override void Enter()
    {
        base.Enter();
        timer = 0;
        idleWaitTime = Random.Range(2f, 6f);
        hasNewDestination = false;
        newDestination = enemy.currentDestination;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.currentDestination = newDestination;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        timer += Time.deltaTime;

        if (timer > idleWaitTime && hasNewDestination)
            stateMachine.ChangeState(enemy.PatrolState);

        if (enemy.playerDetected)
            stateMachine.ChangeState(enemy.PursueState);

        if (!hasNewDestination)
        {
            if (newDestination == enemy.currentDestination)
                newDestination = Random.Range(0, enemy.destinationPositions.Count);
            else
                hasNewDestination = true;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
