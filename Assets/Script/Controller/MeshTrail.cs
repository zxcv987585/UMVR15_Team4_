using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float ActiveTime = 2f;

    [Header("Ÿk‰e“Iç¨Œ¸ŽžŠÔ")]
    public float meshRefrenchRate = 0.1f;

    private bool isTrailActive;
    private SkinnedMeshRenderer[] skinnedMeshRenderers;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L) && !isTrailActive)
        {
            isTrailActive = true;
            StartCoroutine(ActivateTrail(ActiveTime));
        }
    }

    private IEnumerator ActivateTrail(float activeTime)
    {
        while (activeTime > 0f)
        {
            activeTime -= meshRefrenchRate;

            if(skinnedMeshRenderers == null)
            {
                skinnedMeshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            yield return new WaitForSeconds(meshRefrenchRate);
        } 

        isTrailActive = false;
    }
}
