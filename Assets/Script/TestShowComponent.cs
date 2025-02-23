using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShowComponent : MonoBehaviour
{
	void Start()
	{
		Component[] componentArray = GetComponents<Component>();
		
		foreach(Component component in componentArray)
		{
			Debug.Log("Component name is " + component.GetType().Name);
		}
	}
}
