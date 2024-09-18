/*****************************************************************************
// File Name :         bulletBehavior.cs
// Author :            Mark Hanson
// Creation Date :     9/16/2024
//
// Brief Description : THIS IS A PLACEHOLDER THIS IS NOTHING BUT A PLACEHOLDER AND IS TO BE DELETED WHEN THE REAL GUN AND BULLET IS IN (CERTAIN ASPECTS ARE TO BE TAKEN FROM IT THOUGH)
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletBehavior : MonoBehaviour
{
    //The UI component that serves as feedback of how much damage was taken
    [SerializeField] private GameObject _damCount;
    //Adjustible number for designers outside of code (default damage is 15)
    [SerializeField] private int _actualDamage = 15;

    void Start()
    {
        StartCoroutine(delayDespawn());
    }

    /// <summary>
    /// What causes the bullet to move
    /// </summary>
    void Update()
    {
        transform.position -= new Vector3(0, 0, 0.1f);
    }

    IEnumerator delayDespawn()
    {
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider col)
    {
        //If it touches the weak spot the bullet sets the UI count awake and then detaches from it once the correct number is applied to the specific number while despawning
        if (col.gameObject.tag == "Weak Spot")
        {
            _damCount.SetActive(true);
            //Getting the script from the serialized ui component to change the number
            damageNumBehavior dc = _damCount.GetComponent<damageNumBehavior>();
            dc._damageNumber = _actualDamage;
            this.gameObject.transform.DetachChildren();
            Destroy(gameObject ,0.1f);
        }
        //If it touches the invulnerable spot the bullet sets the UI count awake and then detaches from it once it applies the correct number while despawning
        if (col.gameObject.tag == "Invulnerable Spot")
        {
            _damCount.SetActive(true);
            //Getting the script from the serialized ui component to change the number
            damageNumBehavior dc = _damCount.GetComponent<damageNumBehavior>();
            dc._damageNumber = 0;
            this.gameObject.transform.DetachChildren();
            Destroy(gameObject, 0.1f);
        }
    }
}
