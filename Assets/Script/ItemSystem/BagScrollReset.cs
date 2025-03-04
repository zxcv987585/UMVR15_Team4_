using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BagScrollReset : MonoBehaviour
{
    void Start()
    {
        ScrollRect scrollRect = GetComponentInParent<ScrollRect>();
        if (scrollRect != null)
        {
            // 設為 1 代表滑到最上面
            scrollRect.verticalNormalizedPosition = 1; 
        }
    }
}
