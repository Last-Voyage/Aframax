using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjectInteract : MonoBehaviour
{
    [SerializeField]
    string _objectTag;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("triggered");
        if(other.gameObject.CompareTag(_objectTag))
        {
            EnvironmentManager.Instance.CompletedTutorial()?.Invoke();
            gameObject.SetActive(false);
        }
    }
}
