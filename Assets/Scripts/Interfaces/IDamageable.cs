using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool CheckDamage(int damage, Collider colliderHit);    
}
