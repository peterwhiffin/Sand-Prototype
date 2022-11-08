using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : EnemyAbilityState
{
    public EnemyHitState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        enemy.animator.Play("HitReact", 1, 0f);
        enemy.audioSource.PlayOneShot(enemy.audioClips[1], 1);
        enemy.hitByOther = false;
    }

    public override void Exit()
    {
        base.Exit();
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
