using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/EnemyDataSO")]
public class EnemyDataSO : ScriptableObject
{
	public string enemyName;
	public int maxHP;
	public bool isBoss;
	public int attackPower;
	public float attackRange;
	public float moveSpeed;
	
	public string SfxAttackKey;
	public string SfxDamageKey;
	public string SfxDeadKey;
}
