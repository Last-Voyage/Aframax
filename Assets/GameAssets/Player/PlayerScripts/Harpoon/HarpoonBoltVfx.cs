using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class HarpoonBoltVfx : MonoBehaviour
{
    private Dictionary<Material, uint> _materialVfxRefs = new();
    
    private List<Material> _materialsInCollision;

    private SpecificVisualEffect[] _harpoonVisualEffects = new SpecificVisualEffect[4];

    [SerializeField]
    private Material[] _vfxCollisionMaterials;

    private enum VFXType : uint
    {
        NOVFX,
        SPARKVFX,
        DECKVFX,
        TREEVFX
    }

    private uint _functionRef;

    private void Awake()
    {
        // Adds the vfx to the array for later referencing;
        _harpoonVisualEffects[(uint)VFXType.NOVFX] = null;
        _harpoonVisualEffects[(uint)VFXType.SPARKVFX] = VfxManager.Instance.GetMetalSparksVfx();
        _harpoonVisualEffects[(uint)VFXType.DECKVFX] = VfxManager.Instance.GetWoodenSparksVfx();
        _harpoonVisualEffects[(uint)VFXType.TREEVFX] = VfxManager.Instance.GetTreeSplintersVfx();
        
        // Adds the material to the vfx
        _materialVfxRefs.Add(_vfxCollisionMaterials[0], (uint)VFXType.SPARKVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[1], (uint)VFXType.SPARKVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[2], (uint)VFXType.DECKVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[3], (uint)VFXType.TREEVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[4], (uint)VFXType.TREEVFX);
        _materialVfxRefs.Add(_vfxCollisionMaterials[5], (uint)VFXType.TREEVFX);
    }

    private void OnTriggerEnter(Collider other)
    {
        // If the collided object does not have these components, it will exit this function
        if (!other.gameObject.TryGetComponent<MeshCollider>(out var anotherCollider)
            || !other.gameObject.TryGetComponent<MeshRenderer>(out var anotherRenderer))
        {
            return;
        }
        Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward),
            out RaycastHit hit, 2f);
        if (hit.collider.IsUnityNull() || !hit.collider.gameObject.TryGetComponent<MeshCollider>
                (out var freeza) || !freeza.gameObject.TryGetComponent<Renderer>(out var freezaRenderer))
        {
            return;
        }
        
        Debug.Log(anotherCollider);
        
        _functionRef = (uint)VFXType.NOVFX;
        
        _materialsInCollision = null;

        Mesh theMesh = anotherCollider.sharedMesh;
        int howManyMeshes = theMesh.subMeshCount;
        
        /*MeshCollider funny = hit.collider as MeshCollider;*/

        int whichTriangle = hit.triangleIndex;
        
        Debug.Log(whichTriangle);
        // Need the three rule; three indexes to get one triangle
        
        for (int i = 0; i < howManyMeshes; i++)
        {
            SubMeshDescriptor subMesh = theMesh.GetSubMesh(i);
            //Debug.log();
            Debug.Log(subMesh.indexStart);
            Debug.Log(subMesh.indexCount);
            if (whichTriangle > subMesh.indexStart && whichTriangle < subMesh.indexCount 
            /*&& funny.TryGetComponent<Renderer>(out var subMeshRenderer)*/)
            {
                _functionRef = _materialVfxRefs[freezaRenderer.sharedMaterial];
                Debug.Log(freezaRenderer.sharedMaterial);
            }
        }

        /*int fakeInt = theMesh.GetSubMesh(0).indexCount;
        
        _materialsInCollision = null;

        _materialsInCollision = anotherRenderer.sharedMaterials.ToList();

        Debug.Log("has renderer");
            
        foreach (var material in _materialsInCollision)
        { if (_materialVfxRefs.TryGetValue(material, out uint friend))
            { 
                _functionRef = friend;
                break;
            } 
            print(material);
        }*/

        if (_harpoonVisualEffects[_functionRef] != null)
        {
            RealVfx();
        }
    }

    private void RealVfx()
    {
        Debug.Log("Did vfx " + _functionRef);
        Debug.Log(_harpoonVisualEffects[_functionRef].ToString());
        _harpoonVisualEffects[_functionRef].PlayNextVfxInPool
            (gameObject.transform.position, Quaternion.Inverse(gameObject.transform.rotation));
    }
}