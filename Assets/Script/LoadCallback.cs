using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCallback : MonoBehaviour
{
	[SerializeField] private bool isNeedFalseLoad = true;
	[SerializeField] private float falseLoadSecond;

	private void Update()
	{
		if(isNeedFalseLoad)
		{
			//做些假加載處理, 如果需要的話
			StartCoroutine(FalseLoadProcess());
		}
		else
		{
			LoadManager.LoadCallback();
		}
	}

	private IEnumerator FalseLoadProcess()
	{	
		yield return new WaitForSeconds(falseLoadSecond);

		LoadManager.LoadCallback();
	}
}
