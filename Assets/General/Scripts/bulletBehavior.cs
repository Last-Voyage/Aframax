/*****************************************************************************
// File Name :         bulletBehavior.cs
// Author :            Mark Hanson
// Creation Date :     9/16/2024
//
// Brief Description : THIS IS A PLACEHOLDER THIS IS NOTHING BUT A PLACEHOLDER AND IS TO BE DELETED WHEN THE REAL GUN AND BULLET IS IN
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{
    [SerializeField] private GameObject _damCount;

    /// <summary>
    /// What causes the bullet to move
    /// </summary>
    void Update()
    {
        transform.position -= new Vector3(0, 0, 0.1f);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Weak Spot")
        {
            _damCount.SetActive(true);
            damageNumBehavior dc = _damCount.GetComponent<damageNumBehavior>();
            dc._damageNumber = 15;
            this.gameObject.transform.DetachChildren();
            Destroy(gameObject ,0.1f);
        }
        if (col.gameObject.tag == "Invulnerable Spot")
        {
            _damCount.SetActive(true);
            damageNumBehavior dc = _damCount.GetComponent<damageNumBehavior>();
            dc._damageNumber = 0;
            this.gameObject.transform.DetachChildren();
            Destroy(gameObject, 0.1f);
        }
    }
}
