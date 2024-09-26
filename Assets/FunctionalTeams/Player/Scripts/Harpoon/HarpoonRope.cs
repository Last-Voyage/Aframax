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
    private Spring _spring;
    private LineRenderer _lr;
    private Vector3 _currentHitPos;
    [SerializeField] private HarpoonGun _harpoonGun;

    #region Inspector Variables
    [SerializeField] private int _quality;
    [SerializeField] private float _damper;
    [SerializeField] private float _strength;
    [SerializeField] private float _velocity;
    [SerializeField] private float _waveCount;
    [SerializeField] private float _waveHeight;
    [SerializeField] private AnimationCurve _affectCurve;

    #endregion
    /// <summary>
    /// gets the line renderer and sets up the spring
    /// </summary>
    void Awake() {
        _lr = GetComponent<LineRenderer>();
        _spring = new Spring();
        _spring.SetTarget(0);
    }

    private Coroutine _drawRopeCoroutine;
    /// <summary>
    /// Draws the rope and does the bounce animation when harpoon is shot/reeled in
    /// </summary>
    /// <returns></returns>
    IEnumerator DrawRopeCoroutine() {
        while (_harpoonGun.IsShooting) {
            // Spring settings
            if (_lr.positionCount != _quality + 1) 
            {
                _spring.SetVelocity(_velocity);
                _lr.positionCount = _quality + 1;  // Ensure position count matches the quality
            }

            _spring.SetDamper(_damper);
            _spring.SetStrength(_strength);
            _spring.Update(Time.deltaTime);

            var grapplePoint = _harpoonGun.HarpoonSpear.transform.position;
            var gunTipPosition = _harpoonGun.HarpoonTip.position;
            var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

            _currentHitPos = Vector3.Lerp(_currentHitPos, grapplePoint, Time.deltaTime * 12f);

            // Rope wave effect and drawing
            for (var i = 0; i < _quality + 1; i++) {
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
    public void StartDrawingRope() {
        if (_drawRopeCoroutine == null) {
            _drawRopeCoroutine = StartCoroutine(DrawRopeCoroutine());
        }
    }
    /// <summary>
    /// stops the draw rope coroutine and resets the rope for next spring
    /// </summary>
    public void StopDrawingRope() {
        if (_drawRopeCoroutine != null) {
            StopCoroutine(_drawRopeCoroutine);
            _drawRopeCoroutine = null;

            // Cleanup when coroutine stops
            _currentHitPos = _harpoonGun.HarpoonTip.position;
            _spring.Reset();
            if (_lr.positionCount > 0)
                _lr.positionCount = 0;
        }
    }

}