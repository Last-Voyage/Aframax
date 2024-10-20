using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumetricFog : MonoBehaviour
{
    void Start()
    {
        Camera mainCamera = Camera.main;
        Material fogMaterial = GetComponent<Renderer>().material;
        
        // Get near clip plane matrix
        fogMaterial.SetMatrix(
            Shader.PropertyToID("_NearClipPlaneCornersMatrix"),
            new Matrix4x4(
                mainCamera.ViewportToWorldPoint(new Vector3(0.0F, 1.0F, mainCamera.nearClipPlane)),
                mainCamera.ViewportToWorldPoint(new Vector3(1.0F, 1.0F, mainCamera.nearClipPlane)),
                mainCamera.ViewportToWorldPoint(new Vector3(0.0F, 0.0F, mainCamera.nearClipPlane)),
                mainCamera.ViewportToWorldPoint(new Vector3(1.0F, 0.0F, mainCamera.nearClipPlane))
        ));
        
        // Get far clip plane matrix
        fogMaterial.SetMatrix(
            Shader.PropertyToID("_FarClipPlaneCornersMatrix"),
            new Matrix4x4(
                mainCamera.ViewportToWorldPoint(new Vector3(0.0F, 1.0F, mainCamera.farClipPlane)),
                mainCamera.ViewportToWorldPoint(new Vector3(1.0F, 1.0F, mainCamera.farClipPlane)),
                mainCamera.ViewportToWorldPoint(new Vector3(0.0F, 0.0F, mainCamera.farClipPlane)),
                mainCamera.ViewportToWorldPoint(new Vector3(1.0F, 0.0F, mainCamera.farClipPlane))
        ));
    }
}
