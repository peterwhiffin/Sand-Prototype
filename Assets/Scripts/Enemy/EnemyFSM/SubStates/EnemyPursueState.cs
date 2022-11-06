using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPursueState : EnemyGroundedState
{
    public EnemyPursueState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
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
        
        enemy.transform.LookAt(enemy.detectedPlayer.transform);
        enemy.enemyAgent.isStopped = Vector3.Distance(enemy.transform.position, enemy.detectedPlayer.transform.position) < enemyData.attackDistance;
        
        enemy.animator.SetBool("Run", !enemy.enemyAgent.isStopped);
        enemy.animator.SetBool("Idle", enemy.enemyAgent.isStopped);

        if (enemy.enemyAgent.isStopped && enemy.abilityDone)
        {
            if (enemy.ShouldBlock() && !enemy.isAttacking && !enemy.isBlocking)
            {
                stateMachine.ChangeState(enemy.BlockState);
            }
            else if (!enemy.isAttacking && enemy.attackTimer > 1.8f && !enemy.isBlocking)
            {
                stateMachine.ChangeState(enemy.AttackState);
            }
        }
        else
        {
            enemy.enemyAgent.destination = enemy.detectedPlayer.transform.position;
        }

        enemy.attackTimer += Time.deltaTime;
        enemy.blockTimer += Time.deltaTime;
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
