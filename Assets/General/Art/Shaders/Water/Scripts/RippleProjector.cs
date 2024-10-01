using UnityEngine;

public class RippleProjector : MonoBehaviour
{
    [SerializeField] private MeshRenderer waterMesh;

    private Camera _camera;
    private Material _waterMaterial;
    
    private Vector3 _lastPosition;
    private float _lastScale;

    private int _positionID = Shader.PropertyToID("_BoxCenter");
    private int _scaleID = Shader.PropertyToID("_BoxSize");

    private void Start()
    {
        _camera = GetComponent<Camera>();
        
        _waterMaterial = waterMesh.sharedMaterial;
        _lastPosition = transform.position;
        _lastScale = _camera.orthographicSize;

        UpdateShaderData();
    }

    private void Update()
    {
        if (transform.position != _lastPosition)
        {
            _lastPosition = transform.position;
            UpdateShaderData();
        }

        if (Mathf.Abs(_camera.orthographicSize - _lastScale) > 0.01F)
        {
            _lastScale = _camera.orthographicSize;
            UpdateShaderData();
        }
    }

    private void UpdateShaderData()
    {
        _waterMaterial.SetVector(
            _positionID, 
            new Vector4(
                _lastPosition.x, 
                _lastPosition.y, 
                _lastPosition.z, 
                0.0F
        ));
        
        _waterMaterial.SetVector(
            _scaleID, 
            new Vector4(
                _lastScale,
                _lastScale,
                _lastScale,
                0.0F
        ));
    }
}
