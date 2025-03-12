using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemy
{
    Health Health {get;}
    void EnemyUpdate();
    void SetIsPause(bool isPause);
}
