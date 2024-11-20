/*****************************************************************************
// File Name :         FastIKFabric.cs
// Author :            Tommy Roberts
// Creation Date :     10/28/2024
//
// Brief Description : All of the FABRIK functionality
*****************************************************************************/
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

/// <summary>
/// Fabrik IK Solver
/// </summary>
public class FastIKFabric : MonoBehaviour
{
    [SerializeField] private int _chainLength = 2;
    [SerializeField] internal Transform Target;
    [SerializeField] private Transform _pole;

    [Header("Solver Parameters")]
    [SerializeField] private int _iterations = 10;
    [SerializeField] private float _delta = 0.001f;

    [Range(0, 1)]
    [SerializeField] private float _snapBackStrength = 1f;

    private float[] _bonesLength; //Target to Origin
    private float _completeLength;
    private Transform[] _bones;
    private Vector3[] _jointPositions;
    private Vector3[] _startDirection; // start direction of all the joins
    private Quaternion[] _startRotationBone;
    private Quaternion _startRotationTarget;
    private Transform _root;

    /// <summary>
    /// Initialize the line of bones
    /// </summary>
    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Initialize the FABRIK chain
    /// </summary>
    private void Init()
    {
        //initial array
        _bones = new Transform[_chainLength + 1];
        _jointPositions = new Vector3[_chainLength + 1];
        _bonesLength = new float[_chainLength];
        _startDirection = new Vector3[_chainLength + 1];
        _startRotationBone = new Quaternion[_chainLength + 1];

        //find root
        _root = transform;
        for (var i = 0; i <= _chainLength; i++)
        {
            if (_root == null)
                throw new UnityException("The chain value is longer than the ancestor chain!");
            _root = _root.parent;
        }

        //init target
        if (Target == null)
        {
            Target = new GameObject(gameObject.name + " Target").transform;
            SetPositionRootSpace(Target, GetPositionRootSpace(transform));
        }
        _startRotationTarget = GetRotationRootSpace(Target);

        GetFullLength();
    }

    /// <summary>
    /// gets the full length from root to tip of the IK system
    /// </summary>
    private float GetFullLength()
    {
        //init data
        var currentJoint = transform;
        _completeLength = 0;
        for (var i = _bones.Length - 1; i >= 0; i--)
        {
            _bones[i] = currentJoint;
            _startRotationBone[i] = GetRotationRootSpace(currentJoint);

            if (i == _bones.Length - 1)
            {
                //tip of system
                _startDirection[i] = GetPositionRootSpace(Target) - GetPositionRootSpace(currentJoint);
            }
            else
            {
                //middle of system(not start or end)
                _startDirection[i] = GetPositionRootSpace(_bones[i + 1]) - GetPositionRootSpace(currentJoint);
                _bonesLength[i] = _startDirection[i].magnitude;
                _completeLength += _bonesLength[i];
            }
            currentJoint = currentJoint.parent;
        }
        return _completeLength;
    }

    /// <summary>
    /// Calls the IK Solver
    /// </summary>
    private void LateUpdate()
    {
        ResolveIK();
    }

    /// <summary>
    /// The function to actually solve the IK system
    /// </summary>
    private void ResolveIK()
    {
        if (Target == null) { return; }

        if (_bonesLength.Length != _chainLength) { Init(); }

        //Fabric

        //  root
        //  (bone0) (bonelen 0) (bone1) (bonelen 1) (bone2)...
        //   x--------------------x--------------------x---...

        //get position
        for (int i = 0; i < _bones.Length; i++)
        {
            _jointPositions[i] = GetPositionRootSpace(_bones[i]);
        }

        var targetPosition = GetPositionRootSpace(Target);
        var targetRotation = GetRotationRootSpace(Target);

        //1st is possible to reach?
        if ((targetPosition - GetPositionRootSpace(_bones[0])).sqrMagnitude >= _completeLength * _completeLength)
        {
            //just strech it
            var direction = (targetPosition - _jointPositions[0]).normalized;
            //set everything after root
            for (int i = 1; i < _jointPositions.Length; i++)
            {
                _jointPositions[i] = _jointPositions[i - 1] + direction * _bonesLength[i - 1];
            }
        }
        else
        {
            for (int i = 0; i < _jointPositions.Length - 1; i++)
            {
                _jointPositions[i + 1] = Vector3.Lerp(_jointPositions[i + 1], _jointPositions[i] + _startDirection[i], _snapBackStrength);
            }

            for (int iteration = 0; iteration < _iterations; iteration++)
            {
                //https://www.youtube.com/watch?v=UNoX65PRehA
                //backward reaching IK iteration
                for (int i = _jointPositions.Length - 1; i > 0; i--)
                {
                    if (i == _jointPositions.Length - 1) 
                    {
                        _jointPositions[i] = targetPosition; //set it to target
                    }
                    else
                    {
                        _jointPositions[i] = _jointPositions[i + 1] + (_jointPositions[i] - _jointPositions[i + 1]).normalized * _bonesLength[i]; //set in line on distance
                    }
                }

                //forward reaching IK iteration
                for (int i = 1; i < _jointPositions.Length; i++)
                {
                    _jointPositions[i] = _jointPositions[i - 1] + (_jointPositions[i] - _jointPositions[i - 1]).normalized * _bonesLength[i - 1];
                }

                //check if the tip of the tendril is close enough to target
                if ((_jointPositions[_jointPositions.Length - 1] - targetPosition).sqrMagnitude < _delta * _delta) { break; }
            }
        }

        //move entire system slightly towards pole
        if (_pole != null)
        {
            var polePosition = GetPositionRootSpace(_pole);
            for (int i = 1; i < _jointPositions.Length - 1; i++)
            {
                var plane = new Plane(_jointPositions[i + 1] - _jointPositions[i - 1], _jointPositions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(polePosition);
                var projectedBone = plane.ClosestPointOnPlane(_jointPositions[i]);
                var angleToRotate = Vector3.SignedAngle(projectedBone - _jointPositions[i - 1], projectedPole - _jointPositions[i - 1], plane.normal);
                _jointPositions[i] = Quaternion.AngleAxis(angleToRotate, plane.normal) * (_jointPositions[i] - _jointPositions[i - 1]) + _jointPositions[i - 1];
            }
        }

        //set position & rotation
        for (int i = 0; i < _jointPositions.Length; i++)
        {
            if (i == _jointPositions.Length - 1)
            {
                SetRotationRootSpace(_bones[i], Quaternion.Inverse(targetRotation) * _startRotationTarget * Quaternion.Inverse(_startRotationBone[i]));
            }
            else
            {
                SetRotationRootSpace(_bones[i], Quaternion.FromToRotation(_startDirection[i], _jointPositions[i + 1] - _jointPositions[i]) * Quaternion.Inverse(_startRotationBone[i]));
            }
            SetPositionRootSpace(_bones[i], _jointPositions[i]);
        }
    }

    /// <summary>
    /// Get the position of the current bones root
    /// </summary>
    private Vector3 GetPositionRootSpace(Transform current)
    {
        if (_root == null)
        {
            return current.position;
        }
        else
        {
            return Quaternion.Inverse(_root.rotation) * (current.position - _root.position);
        }
    }

    /// <summary>
    /// Set the position of the current bones root
    /// </summary>
    private void SetPositionRootSpace(Transform current, Vector3 position)
    {
        if (_root == null)
        {
            current.position = position;
        }
        else
        {
            current.position = _root.rotation * position + _root.position;
        }
    }

    /// <summary>
    /// Get the rotation of the current bones root
    /// </summary>
    private Quaternion GetRotationRootSpace(Transform current)
    {
        //inverse(after) * before => rot: before -> after
        if (_root == null)
        {
            return current.rotation;
        }
        else
        {
            return Quaternion.Inverse(current.rotation) * _root.rotation;
        }
    }

    /// <summary>
    /// Set the rotation of the current bones root
    /// </summary>
    private void SetRotationRootSpace(Transform current, Quaternion rotation)
    {
        if (_root == null)
        {
            current.rotation = rotation;
        }
        else
        {
            current.rotation = _root.rotation * rotation;
        }
    }

    /// <summary>
    /// draws the bones in scene view editor
    /// </summary>
    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        var currentJoint = this.transform;
        //draw each bone in editor
        for (int i = 0; i < _chainLength && currentJoint != null && currentJoint.parent != null; i++)
        {
            //set size of the current bones gizmo
            var scale = Vector3.Distance(currentJoint.position, currentJoint.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(currentJoint.position, Quaternion.FromToRotation(Vector3.up, currentJoint.parent.position - currentJoint.position), new Vector3(scale, Vector3.Distance(currentJoint.parent.position, currentJoint.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            currentJoint = currentJoint.parent;
        }
    }
    #endif
}