using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IDamageable
{  
    void CheckDamage(int damage, int colliderID);
}
