/**********************************************************************************************************************
// File Name :         BoxDroppingScript.cs
// Author :            Nick Rice
// Creation Date :     
// 
// Brief Description : Sample script that spawns a cube to simulate objects on the boat
**********************************************************************************************************************/
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class BoxDroppingScript : MonoBehaviour
{
    GameObject theBox;

    /// <summary>
    /// starts Hugo() coroutine
    /// </summary>
    void Start()
    {

        StartCoroutine("Hugo");
    }

    /// <summary>
    /// Waits 2 secconds, then spawns a cube above the boat
    /// </summary>
    private IEnumerator Hugo()
    {
        yield return new WaitForSeconds(2f);
        GameObject temp;
        temp = theBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Rigidbody rigid = theBox.AddComponent<Rigidbody>();
        //Destroy(temp);
        //rigid.collisionDetectionMode = CollisionDetectionMode.Discrete;
        theBox.transform.position = new Vector3(-10, -10, -10);
        Instantiate(theBox, gameObject.transform.position, Quaternion.identity);
        yield return new WaitForSeconds(4f);
        //Instantiate(theBox, gameObject.transform.position, Quaternion.identity);
    }
}