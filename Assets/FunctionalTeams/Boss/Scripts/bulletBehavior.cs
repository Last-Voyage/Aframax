/*****************************************************************************
// File Name :         BulletBehavior.cs
// Author :            Mark Hanson
// Contributors:       Andrea Swihart-DeCoster
// Creation Date :     9/16/2024
//
// Brief Description : THIS IS A PLACEHOLDER THIS IS NOTHING BUT A PLACEHOLDER AND IS TO BE DELETED WHEN THE REAL GUN AND BULLET IS IN (CERTAIN ASPECTS ARE TO BE TAKEN FROM IT THOUGH)
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    //The UI component that serves as feedback of how much damage was taken
    [SerializeField] private GameObject _damageCount;
    //The ever changing game object that reflects the instantiated UI component that is made upon contact of an enemy
    private GameObject _exactCounter;
    //Adjustible number for designers outside of code (default damage is 15)
    [SerializeField] private int _actualDamage = 15;

    [Space]
    [SerializeField] private bool _damageNumberToggle;

    void Start()
    {
        //StartCoroutine(delayDespawn());
    }

    /// <summary>
    /// What causes the bullet to move
    /// </summary>
    void Update()
    {
        //transform.position -= new Vector3(0, 0, 0.1f);
    }

    IEnumerator delayDespawn()
    {
        yield return new WaitForSeconds(0.6f);
        Destroy(gameObject);
    }
    
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col)
    {
        DamageNumBehavior dc = null;
        TextMeshPro damageText = null;

        if(_damageNumberToggle)
        {
            Instantiate(_damageCount, this.transform);
            _exactCounter = this.transform.GetChild(0).gameObject;
            dc = _exactCounter.GetComponent<DamageNumBehavior>();
            damageText = _exactCounter.GetComponent<TextMeshPro>();
        }
        
        //If it touches the weak spot the bullet sets the UI count awake and then detaches from it once the correct number is applied to the specific number while despawning
        if (col.gameObject.TryGetComponent<WeakPoint>(out WeakPoint weakPoint))
        {
            if(_damageNumberToggle)
            {
                dc.DamageNumber = _actualDamage;
                damageText.text = _actualDamage.ToString();
            }
            
            this.gameObject.transform.DetachChildren();

            col.GetComponent<WeakPoint>().DamageWeakPoint(_actualDamage);

            //destroy after taking out weak spot
            //Destroy(gameObject, 0.1f);
        }
        //If it touches the invulnerable spot the bullet sets the UI count awake and then detaches from it once it applies the correct number while despawning
        else if (col.gameObject.tag == "Invulnerable Spot")
        {
            if (_damageNumberToggle)
            {
                dc.DamageNumber = 0;
                this.gameObject.transform.DetachChildren();
            }
                
        }
    }
}
