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
    public HarpoonGun harpoonGun;

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
    
    /// <summary>
    /// calls draw rope
    /// </summary>
    void LateUpdate() {
        DrawRope();
    }
    /// <summary>
    /// holds the functionality for drawing the bouncy rope
    /// </summary>
    void DrawRope() {
        //If not grappling, don't draw rope
        if (!harpoonGun.IsShooting) {
            _currentHitPos = harpoonGun.HarpoonTip.position;
            _spring.Reset();
            if (_lr.positionCount > 0)
                _lr.positionCount = 0;
            return;
        }

        if (_lr.positionCount == 0) {
            _spring.SetVelocity(_velocity);
            _lr.positionCount = _quality + 1;
        }
        
        _spring.SetDamper(_damper);
        _spring.SetStrength(_strength);
        _spring.Update(Time.deltaTime);

        var grapplePoint = harpoonGun.HarpoonSpear.transform.position;
        var gunTipPosition = harpoonGun.HarpoonTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        _currentHitPos = Vector3.Lerp(_currentHitPos, grapplePoint, Time.deltaTime * 12f);
        //not super sure about the math here (sorry charlie). In essence, this part just creates waves by adjusting
        //points positions on the line renderer. The affect curve is also important to make the wave look more natural
        for (var i = 0; i < _quality + 1; i++) {
            var delta = i / (float) _quality;
            var offset = up * _waveHeight * Mathf.Sin(delta * _waveCount * Mathf.PI) * _spring.Value *
                         _affectCurve.Evaluate(delta);
            
            _lr.SetPosition(i, Vector3.Lerp(gunTipPosition, _currentHitPos, delta) + offset);
        }
    }
}