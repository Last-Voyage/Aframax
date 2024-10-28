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
    [SerializeField] private Transform _target;
    [SerializeField] private Transform _pole;

    [Header("Solver Parameters")]
    [SerializeField] private int _iterations = 10;
    [SerializeField] private float _delta = 0.001f;

    [Range(0, 1)]
    [SerializeField] private float _snapBackStrength = 1f;

    private float[] _bonesLength; //Target to Origin
    private float _completeLength;
    private Transform[] _bones;
    private Vector3[] _positions;
    private Vector3[] _startDirectionSucc;
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
        _positions = new Vector3[_chainLength + 1];
        _bonesLength = new float[_chainLength];
        _startDirectionSucc = new Vector3[_chainLength + 1];
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
        if (_target == null)
        {
            _target = new GameObject(gameObject.name + " Target").transform;
            SetPositionRootSpace(_target, GetPositionRootSpace(transform));
        }
        _startRotationTarget = GetRotationRootSpace(_target);

        //init data
        var current = transform;
        _completeLength = 0;
        for (var i = _bones.Length - 1; i >= 0; i--)
        {
            _bones[i] = current;
            _startRotationBone[i] = GetRotationRootSpace(current);

            if (i == _bones.Length - 1)
            {
                //leaf
                _startDirectionSucc[i] = GetPositionRootSpace(_target) - GetPositionRootSpace(current);
            }
            else
            {
                //mid bone
                _startDirectionSucc[i] = GetPositionRootSpace(_bones[i + 1]) - GetPositionRootSpace(current);
                _bonesLength[i] = _startDirectionSucc[i].magnitude;
                _completeLength += _bonesLength[i];
            }
            current = current.parent;
        }
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
        if (_target == null) { return; }

        if (_bonesLength.Length != _chainLength) { Init(); }

        //Fabric

        //  root
        //  (bone0) (bonelen 0) (bone1) (bonelen 1) (bone2)...
        //   x--------------------x--------------------x---...

        //get position
        for (int i = 0; i < _bones.Length; i++)
        {
            _positions[i] = GetPositionRootSpace(_bones[i]);
        }

        var targetPosition = GetPositionRootSpace(_target);
        var targetRotation = GetRotationRootSpace(_target);

        //1st is possible to reach?
        if ((targetPosition - GetPositionRootSpace(_bones[0])).sqrMagnitude >= _completeLength * _completeLength)
        {
            //just strech it
            var direction = (targetPosition - _positions[0]).normalized;
            //set everything after root
            for (int i = 1; i < _positions.Length; i++)
            {
                _positions[i] = _positions[i - 1] + direction * _bonesLength[i - 1];
            }
        }
        else
        {
            for (int i = 0; i < _positions.Length - 1; i++)
            {
                _positions[i + 1] = Vector3.Lerp(_positions[i + 1], _positions[i] + _startDirectionSucc[i], _snapBackStrength);
            }

            for (int iteration = 0; iteration < _iterations; iteration++)
            {
                //https://www.youtube.com/watch?v=UNoX65PRehA
                //back
                for (int i = _positions.Length - 1; i > 0; i--)
                {
                    if (i == _positions.Length - 1) 
                    {
                        _positions[i] = targetPosition; //set it to target
                    }
                    else
                    {
                        _positions[i] = _positions[i + 1] + (_positions[i] - _positions[i + 1]).normalized * _bonesLength[i]; //set in line on distance
                    }
                }

                //forward
                for (int i = 1; i < _positions.Length; i++)
                {
                    _positions[i] = _positions[i - 1] + (_positions[i] - _positions[i - 1]).normalized * _bonesLength[i - 1];
                }

                //close enough?
                if ((_positions[_positions.Length - 1] - targetPosition).sqrMagnitude < _delta * _delta) { break; }
            }
        }

        //move towards pole
        if (_pole != null)
        {
            var polePosition = GetPositionRootSpace(_pole);
            for (int i = 1; i < _positions.Length - 1; i++)
            {
                var plane = new Plane(_positions[i + 1] - _positions[i - 1], _positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(polePosition);
                var projectedBone = plane.ClosestPointOnPlane(_positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - _positions[i - 1], projectedPole - _positions[i - 1], plane.normal);
                _positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (_positions[i] - _positions[i - 1]) + _positions[i - 1];
            }
        }

        //set position & rotation
        for (int i = 0; i < _positions.Length; i++)
        {
            if (i == _positions.Length - 1)
            {
                SetRotationRootSpace(_bones[i], Quaternion.Inverse(targetRotation) * _startRotationTarget * Quaternion.Inverse(_startRotationBone[i]));
            }
            else
            {
                SetRotationRootSpace(_bones[i], Quaternion.FromToRotation(_startDirectionSucc[i], _positions[i + 1] - _positions[i]) * Quaternion.Inverse(_startRotationBone[i]));
            }
            SetPositionRootSpace(_bones[i], _positions[i]);
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
    private void OnDrawGizmos()
    {
    #if UNITY_EDITOR
        var current = this.transform;
        for (int i = 0; i < _chainLength && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    #endif
    }
}