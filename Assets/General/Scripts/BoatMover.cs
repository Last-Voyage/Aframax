using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatMover : MonoBehaviour
{
    #region Directional Variables
    private static Vector3 ForwardDirection = Vector3.forward;
    private static Vector3 BackwardDirection = Vector3.back;
    private static Vector3 LeftDirection = Vector3.left;
    private static Vector3 RightDirection = Vector3.right;
    private static Vector3 ForwardLeftDirection = Vector3.forward + Vector3.left;
    private static Vector3 ForwardRightDirection = Vector3.forward + Vector3.right;
    private static Vector3 BackLeftDirection = Vector3.back + Vector3.left;
    private static Vector3 BackRightDirection = Vector3.back + Vector3.right;

    //private Vector3 

    [Tooltip("Named pointers for the background direction")]
    public enum EDirectionalInserts
    {
        Forward,
        Backward,
        Left,
        Right,
        ForwardLeft,
        ForwardRight,
        BackwardLeft,
        BackwardRight
    }

    [Tooltip("Array that holds directions for the background")]
    private Vector3[] _directionalMovements =
        { ForwardDirection, BackwardDirection, LeftDirection, RightDirection,
        ForwardLeftDirection, ForwardRightDirection, BackLeftDirection, BackRightDirection };
    #endregion


    private List<GameObject> onTrain = new List<GameObject>();

    public List<GameObject> TrainPassengers { get => onTrain; private set => onTrain = value; }

    [Tooltip("The pointer for the background direction")]
    private int _currentDirectional =
        (int)EDirectionalInserts.Forward;
    [Tooltip("The speed multiplier for the background")] public float VelocityMultiplier = 1;/*{ get; private set; }*/

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.position += _directionalMovements[_currentDirectional] *
            VelocityMultiplier * Time.deltaTime;

        if (TrainPassengers.Count != 0 && TrainPassengers != null)
        {
            foreach (GameObject passenger in TrainPassengers)
            {
                passenger.transform.position += _directionalMovements[_currentDirectional] *
                    VelocityMultiplier * Time.deltaTime;
            }
        }
    }
}
