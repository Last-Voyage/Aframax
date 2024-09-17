/*****************************************************************************
// File Name :         damageNumBehavior.cs
// Author :            Mark Hanson
// Creation Date :     9/16/2024
//
// Brief Description : This a quality of life UI element just to show a bit of feedback when you hit an enemy
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class damageNumBehavior : MonoBehaviour
{
    //The actual text of the game object to be edited 
    [SerializeField] private TextMeshPro _damText;
    //The number which the text reflects visually
    [SerializeField] public float _damageNumber = 0;
    //The number that gives the illusion of the number disappearing
    [SerializeField] private float _opacityScaler = 255;

    // Update is called once per frame
    void Update()
    {
        _damText.text = _damageNumber.ToString();
        //disappears quickly
        _opacityScaler -= 5f;
        //update the color of the text to match the intesity of which it has been hit(more damage more red) along with opacity going clear
        _damText.color = new Color(255f/255f, ((255f - (_damageNumber *17f)))/255f, (255f - (_damageNumber * 17f))/255f, _opacityScaler);
        //move Upwards slowly
        transform.position += new Vector3(0, 0.05f, 0);

        //Once text is completely clear destory object
        if (_damText.color.a <= 0)
        {
            StartCoroutine(despawn());
        }
    }

    IEnumerator despawn()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("despawned");
        Destroy(gameObject);
    }
}
