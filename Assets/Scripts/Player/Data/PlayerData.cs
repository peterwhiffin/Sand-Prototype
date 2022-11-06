using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newPlayerData", menuName = "Data/Player Data/Base Data")]

public class PlayerData : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float attackMoveSpeed;
    public float jumpHeight;
    public float groundCheckDistance;

    [Header("Stats")]
    public int maxHealth;


}
