using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour, IDamageable
{
    public bool CheckDamage(int d, int s)
    {
        return true;
    }

    public bool AttackBlocked()
    {
        return true;
    }
}
