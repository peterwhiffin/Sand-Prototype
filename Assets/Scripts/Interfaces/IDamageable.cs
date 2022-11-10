using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    bool CheckHit(int damage, int swingDirection);    
}
