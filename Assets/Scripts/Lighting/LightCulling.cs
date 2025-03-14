using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightCulling : MonoBehaviour
{
    [SerializeField] private float _cullDistance = 100.0F;
    
    private Transform _cameraTransform;
    private Light _lightComponent;

    void Start()
    {
        _cameraTransform = Camera.main.transform;
        _lightComponent = GetComponent<Light>();
    }

    private void FixedUpdate()
    {
        float distanceToCamera = Vector3.Distance(
            _cameraTransform.position,
            transform.position
        );

        _lightComponent.enabled = distanceToCamera < _cullDistance;
    }
}
