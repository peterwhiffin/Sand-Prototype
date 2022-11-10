using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour, IDamageable
{
    public bool CheckHit(int damage, int swingDirection)
    {
        return true;
    }
}
