using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBlockState : EnemyPursueState
{
    public EnemyBlockState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    private bool endBlock;
    private bool blockEnded;

    public override void Enter()
    {
        base.Enter();
        enemy.weaponCollider.enabled = true;
        endBlock = false;
        blockEnded = false;
        enemy.isBlocking = true;
        enemy.blockTimer = 0;
    }

    public override void Exit()
    {
        base.Exit();
        enemy.weaponCollider.enabled = false;
        enemy.enemySwordArmConstraint.weight = 0;
        enemy.isBlocking = false;
        enemy.blockTimer = 0;       
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(!endBlock)
            enemy.Block();

        if (enemy.blockTimer > 1f)
            endBlock = true;
        
        if (endBlock)
            blockEnded = enemy.EndBlock();
        
        if(blockEnded)
            stateMachine.ChangeState(enemy.PursueState);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
