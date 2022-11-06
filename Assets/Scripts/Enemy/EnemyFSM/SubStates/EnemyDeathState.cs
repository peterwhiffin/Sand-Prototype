using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyAbilityState
{
    public EnemyDeathState(Enemy enemy, EnemyStateMachine stateMachine, EnemyData enemyData, string animBoolName) : base(enemy, stateMachine, enemyData, animBoolName)
    {
    }

    public override void Enter()
    {
        enemy.enemySwordArmConstraint.weight = 0f;
        enemy.animator.SetLayerWeight(0, 0f);
        enemy.animator.SetLayerWeight(1, 0f);
        enemy.animator.SetLayerWeight(2, 1f);
        enemy.animator.Play("Death", 2, 0f);
    }
}
