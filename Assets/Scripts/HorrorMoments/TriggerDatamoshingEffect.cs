using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TriggerDatamoshingEffect : MonoBehaviour
{
    private DatamoshEffectManager _datamoshEffectManager;

    private void Start()
    {
        _datamoshEffectManager = DatamoshEffectManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().enabled = false;
            _datamoshEffectManager.PlayDatamoshEffect();
        }
    }
}
