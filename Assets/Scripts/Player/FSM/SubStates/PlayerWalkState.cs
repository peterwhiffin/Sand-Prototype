using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWalkState : PlayerGroundedState
{
    public PlayerWalkState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    private float backSpeed;

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

        

        if (movementInput == Vector2.zero)
        {
            player.turnSmoothVelocity = 0;
            stateMachine.ChangeState(player.IdleState);
        }

        if (runInput != 0 && movementInput.y >= 0)
            stateMachine.ChangeState(player.RunState);

        player.MoveCharacter(playerData.walkSpeed + backSpeed, movementInput);

        if (movementInput.y < 0)
        {
            player.animator.SetFloat("MoveDirection", -.8f);
            backSpeed = -1f;
        }
        else if (movementInput.y > 0 || movementInput.x != 0)
        {
            player.animator.SetFloat("MoveDirection", 1f);
            backSpeed = 0f;
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
