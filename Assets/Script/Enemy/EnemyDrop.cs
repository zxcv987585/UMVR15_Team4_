using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DropWeight
{
    [Header("掉落機率 0~100%")]public int weight;
    public ItemData itemData;
}

public class EnemyDrop : MonoBehaviour
{
    [SerializeField] private float _moveSpeed;
    [SerializeField] private List<DropWeight> _dropWeightList;
    private Transform _endPosition;

    private void Start()
    {
        _endPosition = FindObjectOfType<PlayerController>().transform;
        
        StartCoroutine(MoveToUp(transform.position + Vector3.up * 2f));
    }
    
    // 先往上飄
    private IEnumerator MoveToUp(Vector3 targetPosition)
    {
        while(transform.position.y < targetPosition.y)
        {
            transform.position += Vector3.up * (_moveSpeed * Time.deltaTime);
    
            yield return null;
        }
    
        StartCoroutine(MoveToPlayer());
    }

    // 往玩家方向前進
    private IEnumerator MoveToPlayer()
    {
        while(true)
        {
            Vector3 targetPosition = _endPosition.position + Vector3.up * 0.5f;
            Vector3 direction = (targetPosition - transform.position ).normalized;

            transform.position += direction * (_moveSpeed * Time.deltaTime);

            if(Vector3.Distance(targetPosition, transform.position) < 0.1f)
            {
                GetDropItem();
                break;
            }
            
            yield return null;
        }
    }
    
    // 根據道具的掉落來給玩家對應的物品, 並刪除該物件
    private void GetDropItem()
    {
        if(_dropWeightList == null || _dropWeightList.Count == 0) return;

        float nowWeight = 0f;
        float totalWeight = _dropWeightList.Sum(drop => drop.weight);

        float r = Random.Range(0f, totalWeight);

        foreach(DropWeight dropWeight in _dropWeightList)
        {
            nowWeight += dropWeight.weight;

            if(nowWeight >= r)
            {
                InventoryManager.instance.AddItem(dropWeight.itemData);
                InventoryManager.instance.RefreshUI();

                AudioManager.Instance.PlaySound("GetItem", transform.position);

                break;
            }
        }
        
        Destroy(gameObject);
    }

}
