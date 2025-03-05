using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAttack
{
    public event Action OnAttackHit;
    
    public void StartAttack();
    public void ResetHasAttack();
}
