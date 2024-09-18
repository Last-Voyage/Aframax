using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarpoonRope : MonoBehaviour {
    private Spring spring;
    private LineRenderer lr;
    private Vector3 currentHitPosition;
    public HarpoonGun harpoonGun;
    public int quality;
    public float damper;
    public float strength;
    public float velocity;
    public float waveCount;
    public float waveHeight;
    public AnimationCurve affectCurve;
    
    void Awake() {
        lr = GetComponent<LineRenderer>();
        spring = new Spring();
        spring.SetTarget(0);
    }
    
    //Called after Update
    void LateUpdate() {
        DrawRope();
    }

    void DrawRope() {
        //If not grappling, don't draw rope
        if (!harpoonGun.isShooting) {
            currentHitPosition = harpoonGun.harpoonTip.position;
            spring.Reset();
            if (lr.positionCount > 0)
                lr.positionCount = 0;
            return;
        }

        if (lr.positionCount == 0) {
            spring.SetVelocity(velocity);
            lr.positionCount = quality + 1;
        }
        
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(Time.deltaTime);

        var grapplePoint = harpoonGun.harpoonInstance.transform.position;
        var gunTipPosition = harpoonGun.harpoonTip.position;
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        currentHitPosition = Vector3.Lerp(currentHitPosition, grapplePoint, Time.deltaTime * 12f);

        for (var i = 0; i < quality + 1; i++) {
            var delta = i / (float) quality;
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);
            
            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentHitPosition, delta) + offset);
        }
    }
}