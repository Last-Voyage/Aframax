using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FakeWindowManager : MonoBehaviour
{
    public static FakeWindowManager Instance { get; private set; }
    
    [SerializeField] private GameObject _fakeWindowCaptureContainer;
    [SerializeField] private Vector3 _worldSpawnPos = new(0.0F, 0.0F, 1500.0F);
    
    private void Awake()
    {
        // Global static registry
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
        }

        Instantiate(
            _fakeWindowCaptureContainer,
            _worldSpawnPos,
            Quaternion.identity
        );
    }
}
