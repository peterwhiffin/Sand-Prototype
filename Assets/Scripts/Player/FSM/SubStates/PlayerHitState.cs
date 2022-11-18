using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitState : PlayerAbilityState
{
    public PlayerHitState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.animLayer = 1;
        player.networkAnimator.Play("HitReact", 1, 0f);
        player.audioSource.PlayOneShot(player.audioClips[1], 1);
        player.hitByOther = false;
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
