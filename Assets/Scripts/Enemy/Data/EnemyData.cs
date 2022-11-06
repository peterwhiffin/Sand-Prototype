using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEnemyData", menuName = "Data/Enemy Data/Base Data")]

public class EnemyData : ScriptableObject
{
    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float attackMoveSpeed;

    [Header("Stats")]
    public int maxHealth;

    [Header("Behavior")]
    public float idleWaitTime;
    public float attackDistance;
}
