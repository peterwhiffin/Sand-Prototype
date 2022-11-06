using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerState
{
    public PlayerDeathState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void CameraUpdate()
    {
        base.CameraUpdate();
    }

    public override void Enter()
    {
        base.Enter();
        player.animator.SetLayerWeight(1, 0f);
        player.animator.SetLayerWeight(2, 0f);
        player.swordArmConstraint.weight = 0f;
    }
}
