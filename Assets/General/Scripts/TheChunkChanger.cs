using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheChunkChanger : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "TempObjForChunkLoading")
        {
            EnvironmentManager.Instance.SendChangeTheChunk()?.Invoke();
            Debug.Log("ITWORKDS");
        }
        Debug.Log("Dodsn't");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "TempObjForChunkLoading")
        {
            EnvironmentManager.Instance.SendChangeTheChunk()?.Invoke();
            Debug.Log("ITWORKDS");
        }
        Debug.Log("Dodsn't");
    }
}
