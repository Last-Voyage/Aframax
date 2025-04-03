using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class TriggerDatamoshingEffect : MonoBehaviour
{
    private DatamoshEffectManager _datamoshEffectManager;

    [SerializeField] private float _effectDuration = 1.0F;

    private void Start()
    {
        _datamoshEffectManager = DatamoshEffectManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GetComponent<BoxCollider>().enabled = false;
            _datamoshEffectManager.PlayDatamoshEffect(_effectDuration);
        }
    }
}
