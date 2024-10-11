/*****************************************************************************
// File Name :         HarpoonRope.cs
// Author :            Tommy Roberts
// Creation Date :     9/22/24
//
// Brief Description : holds the bouncy harpoon line renderer funtionality
*****************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonRope : MonoBehaviour {
    //private variables
    private Spring _spring;
    private LineRenderer _lr;
    private Vector3 _currentHitPos;
    private Coroutine _drawRopeCoroutine;
    private HarpoonGun _harpoonGun;

    #region Inspector Variables
    [Tooltip("How many points are in the line renderer total")]
    [SerializeField] private int _quality;
    [Tooltip("How many instances are created (kind of just smooths out the effect)")]
    [SerializeField] private float _damper;
    [Tooltip("Strength of wave coming out of harpoon")]
    [SerializeField] private float _strength;
    [Tooltip("Speed of wave coming out of harpoon")]
    [SerializeField] private float _velocity;
    [Tooltip("How many times we want the wave to repeat")]
    [SerializeField] private float _waveCount;
    [Tooltip("How big the wave should be")]
    [SerializeField] private float _waveHeight;
    [Tooltip("The actual curve shape of the wave")]
    [SerializeField] private AnimationCurve _affectCurve;
    #endregion

    /// <summary>
    /// gets the line renderer and sets up the spring
    /// </summary>
    private void Awake()
    {
        _harpoonGun = GetComponentInParent<HarpoonGun>();
        _lr = GetComponentInChildren<LineRenderer>();
        _spring = new Spring();
        _spring.SetTarget(0);
    }

    /// <summary>
    /// sets up the events for the harpoon gun
    /// </summary>
    private void OnEnable()
    {
        PlayerManager.Instance.GetHarpoonFiredEvent().AddListener(StartDrawingRope);
        PlayerManager.Instance.GetHarpoonRetractEvent().AddListener(StopDrawingRope);
    }

    /// <summary>
    /// disables the events for the harpoon gun
    /// </summary>
    private void OnDisable()
    {
        PlayerManager.Instance.GetHarpoonFiredEvent().RemoveListener(StartDrawingRope);
        PlayerManager.Instance.GetHarpoonRetractEvent().RemoveListener(StopDrawingRope);
    }

    /// <summary>
    /// Draws the rope and does the bounce animation when harpoon is shot/reeled in
    /// </summary>
    /// <returns></returns>
    private IEnumerator DrawRopeCoroutine() 
    {
        while (_harpoonGun.GetHarpoonFiringState() == EHarpoonFiringState.Firing||
            _harpoonGun.GetHarpoonFiringState() == EHarpoonFiringState.Reeling) 
        {
            // Spring settings
            if (_lr.positionCount != _quality + 1) 
            {
                _spring.SetVelocity(_velocity);
                _lr.positionCount = _quality + 1;  // Ensure position count matches the quality
            }

            _spring.SetDamper(_damper);
            _spring.SetStrength(_strength);
            _spring.Update(Time.deltaTime);

            var grapplePoint = _harpoonGun.GetHarpoonSpear().transform.position;
            var gunTipPosition = _harpoonGun.GetHarpoonTip().position;
            var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;
            _currentHitPos = Vector3.Lerp(_currentHitPos, grapplePoint, Time.deltaTime * 12f);
            // Rope wave effect and drawing
            for (var i = 0; i < _quality + 1; i++) 
            {
                var delta = i / (float) _quality;
                var offset = up * _waveHeight * Mathf.Sin(delta * _waveCount * Mathf.PI) * _spring.Value *
                            _affectCurve.Evaluate(delta);
                
                _lr.SetPosition(i, Vector3.Lerp(gunTipPosition, _currentHitPos, delta) + offset);
            }
            yield return null; // Wait for the next frame
        }
    }

    /// <summary>
    /// starts the draw rope coroutine
    /// </summary>
    private void StartDrawingRope()
    {
        if (_drawRopeCoroutine == null) {
            _drawRopeCoroutine = StartCoroutine(DrawRopeCoroutine());
        }
    }

    /// <summary>
    /// stops the draw rope coroutine and resets the rope for next spring
    /// </summary>
    private void StopDrawingRope() 
    {
        if (_drawRopeCoroutine != null) 
        {
            StopCoroutine(_drawRopeCoroutine);
            _drawRopeCoroutine = null;
            // Cleanup when coroutine stops
            _currentHitPos = _harpoonGun.GetHarpoonTip().position;
            _spring.Reset();
            if (_lr.positionCount > 0)
                _lr.positionCount = 0;
        }
    }

}