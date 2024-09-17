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
    [SerializeField] private TextMeshPro _damText;
    [SerializeField] public float _damageNumber = 0;
    [SerializeField] private float _opacityScaler = 255;

    // Update is called once per frame
    void Update()
    {
        _damText.text = _damageNumber.ToString();
        //slowly disappears
        _opacityScaler -= 5f;
        _damText.color = new Color(255f, (255f - (_damageNumber *17f)), (255f - (_damageNumber * 17f)), _opacityScaler);
        //move Upwards slowly
        transform.position += new Vector3(0, 0.05f, 0);

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
