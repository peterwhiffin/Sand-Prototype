using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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

        if (movementInput != Vector2.zero)
            stateMachine.ChangeState(player.WalkState);

        player.CameraLockToCharacter();
        player.MoveCharacter(0, movementInput);
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
