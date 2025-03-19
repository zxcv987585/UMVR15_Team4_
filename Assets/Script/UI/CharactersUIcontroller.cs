using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharactersUIcontroller : MonoBehaviour
{
    public UnityEvent openAction;

    void Start()
    {
        StartCoroutine(UIAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private IEnumerator UIAnimation()
    {
        yield return new WaitForSeconds(2f);
        gameObject.GetComponent<CharactersUIcontroller>().openAction.Invoke();
    }
}
