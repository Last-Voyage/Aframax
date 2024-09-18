using UnityEngine;

public class Spring {
    private float _strength;
    private float _damper;
    private float _target;
    private float _velocity;
    private float _value;

    public void Update(float deltaTime) {
        var direction = _target - _value >= 0 ? 1f : -1f;
        var force = Mathf.Abs(_target - _value) * _strength;
        _velocity += (force * direction - _velocity * _damper) * deltaTime;
        _value += _velocity * deltaTime;
    }

    public void Reset() {
        _velocity = 0f;
        _value = 0f;
    }
        
    public void SetValue(float _value) {
        this._value = _value;
    }
        
    public void SetTarget(float _target) {
        this._target = _target;
    }

    public void SetDamper(float _damper) {
        this._damper = _damper;
    }
        
    public void SetStrength(float _strength) {
        this._strength = _strength;
    }

    public void SetVelocity(float _velocity) {
        this._velocity = _velocity;
    }
        
    public float Value => _value;
}