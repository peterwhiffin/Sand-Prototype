using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void CameraUpdate()
    {
        base.CameraUpdate();
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

        player.grounded = player.GroundCheck();

        if (player.jumping)
        {
            player.controller.Move(new Vector3(player.controller.velocity.x, Mathf.Lerp(player.controller.velocity.y + 2, 1.5f, .2f), player.controller.velocity.z) * Time.deltaTime);
        }
        else
            player.Gravity();

        if (player.transform.position.y >= player.jumpHeight)
            player.jumping = false;
        
        if (player.grounded && !player.jumping)
            stateMachine.ChangeState(player.IdleState);


        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();       
    }
}
