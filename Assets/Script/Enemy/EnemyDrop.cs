using System.Collections;
using System.Collections.Generic;
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
            Vector3 direction = (_endPosition.position - (transform.position + Vector3.down)).normalized;

            transform.position += direction * (_moveSpeed * Time.deltaTime);

            if(Vector3.Distance(_endPosition.position, transform.position + Vector3.down) < 0.1f)
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
        if(_dropWeightList.Count != 0)
        {
            foreach(DropWeight dropWeight in _dropWeightList)
            {
                if(dropWeight.weight > Random.Range(0, 100))
                {
                    foreach(ItemData itemData in InventoryManager.instance.myBag.itemList)
                    {
                        if(dropWeight.itemData.itemID == itemData.itemID)
                        {
                            itemData.itemNum++;
                            InventoryManager.instance.RefreshUI();

                            AudioManager.Instance.PlaySound("GetItem", transform.position);
                            break;
                        }
                    }
                }
            }
        }

        Destroy(gameObject);
    }

}
