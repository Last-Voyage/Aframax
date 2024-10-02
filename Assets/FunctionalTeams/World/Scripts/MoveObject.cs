using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3(0, 0, 1f) * Time.deltaTime;
    }
}
