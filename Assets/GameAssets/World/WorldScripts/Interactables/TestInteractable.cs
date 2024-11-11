using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestInteractable : MonoBehaviour, IPlayerInteractable
{
    public void OnInteractedByPlayer()
    {
        Debug.Log("Object has been interacted with");
    }
}
