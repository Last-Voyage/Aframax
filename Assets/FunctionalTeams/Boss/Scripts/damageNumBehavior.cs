/*****************************************************************************
// File Name :         DamageNumBehavior.cs
// Author :            Mark Hanson
// Creation Date :     9/16/2024
//
// Brief Description : This a quality of life UI element just to show a bit of feedback when you hit an enemy
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageNumBehavior : MonoBehaviour
{
    //The actual text of the game object to be edited 
    [SerializeField] private TextMeshPro _damageText;
    //The number which the text reflects visually
    private float _damageNumber = 0;
    //The number that gives the illusion of the number disappearing
    [SerializeField] private float _opacityScaler = 255;

    private void Start()
    {
        StartCoroutine(ColorsAndMovement());
    }

    /// <summary>
    /// The holder of changing the color and updating the movemont of the number text along with the opacity
    /// </summary>
    /// <returns></returns>
    private IEnumerator ColorsAndMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            //disappears quickly
            _opacityScaler -= ((255f * 6f) * Time.deltaTime);
            //update the color of the text to match the intesity of which it has been hit(more damage more red) along with opacity going clear
            _damageText.color = new Color(255f / 255f, ((255f - (_damageNumber * 17f))) / 255f, (255f - (_damageNumber * 17f)) / 255f, (_opacityScaler / 255));
            //move Upwards slowly
            transform.position += new Vector3(0, 0.06f, 0);
            //Once text is completely clear destory object
            if (_damageText.color.a <= 0)
            {
                StartCoroutine(Despawn());
            }
        }
    }

    private IEnumerator Despawn()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    public float DamageNumber
    {
        get { return _damageNumber; }
        set { _damageNumber = value; }
    }
}
