using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class HarpoonBoltVfx : MonoBehaviour
{
    private Dictionary<Material, uint> _materialVfxRefs = new();
    
    private List<Material> _materialsInCollision;

    private SpecificVisualEffect[] _harpoonVisualEffects;

    [SerializeField]
    private Material[] _vfxCollisionMaterials;
    
    private uint _functionRef;
    private const uint _noVFX = 0;

    private void Awake()
    {
        // Adds the vfx to the array for later referencing;
        // 0 is null for preventing vfx
        _harpoonVisualEffects = new []
            {null, VfxManager.Instance.GetMetalSparksVfx(), 
                VfxManager.Instance.GetDeckSplintersVfx(), VfxManager.Instance.GetTreeSplintersVfx()};
        
        // Adds the material to the vfx
        _materialVfxRefs.Add(_vfxCollisionMaterials[0], 1);
        _materialVfxRefs.Add(_vfxCollisionMaterials[1], 1);
        _materialVfxRefs.Add(_vfxCollisionMaterials[2], 2);
        _materialVfxRefs.Add(_vfxCollisionMaterials[3], 3);
        _materialVfxRefs.Add(_vfxCollisionMaterials[4], 3);
        _materialVfxRefs.Add(_vfxCollisionMaterials[5], 3);
    }


    private void OnTriggerExit(Collider other)
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        /*if (other.gameObject.GetComponent<MeshRenderer>() == 
            gameObject.GetComponentInChildren<MeshRenderer>())
        {
            return;
        }*/
        _functionRef = _noVFX;
        
        if (other.gameObject.TryGetComponent<MeshRenderer>(out var theRenderer))
        {
            _materialsInCollision = null;

            _materialsInCollision = theRenderer.sharedMaterials.ToList();
            
            /*_materialsInCollision = other.gameObject.GetComponent<MeshRenderer>
                ().materials.Where(addedMaterials => _materialVfxRefs.ContainsKey(addedMaterials)).ToList();*/

            Debug.Log("gravy");
            
            foreach (var material in _materialsInCollision)
            {
                if (_materialVfxRefs[material] != null)
                {
                    _functionRef = _materialVfxRefs[material];
                }
                if (_materialVfxRefs.TryGetValue(material, out uint friend))
                {
                    _functionRef = friend;
                    break;
                }
                print(material);
            }
            
            /*if (/*_materialsInCollision != null &&#1# _materialsInCollision.Count > 0)
            {
                print("Firey2" + " " + _functionRef);
                print(_materialVfxRefs.Count);
                print(_materialsInCollision.Count);
                _functionRef = _materialVfxRefs[_materialsInCollision[0]]; // What?
                Debug.Log(_functionRef);
            }*/
        }

        if (_harpoonVisualEffects[_functionRef] != null)
        {
            RealVfx();
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        /*if (other.gameObject.GetComponent<MeshRenderer>() == 
            gameObject.GetComponentInChildren<MeshRenderer>())
        {
            print("Firey");
            return;
        }
        _functionRef = _noVFX;
        
        if (other.gameObject.GetComponent<MeshRenderer>() != null)
        {
            _materialsInCollision = null;

            _materialsInCollision = other.gameObject.GetComponent<MeshRenderer>
                ().materials.Where(addedMaterials => _materialVfxRefs.ContainsKey(addedMaterials)).ToList();

            if (_materialsInCollision != null)
            {
                print("Firey2");
                _functionRef = _materialVfxRefs[_materialsInCollision[0]];
                Debug.Log(_functionRef);
            }
        }

        if (_harpoonVisualEffects[_functionRef] != null)
        {
            SummonNewVfx(other.GetContact(0));
        }*/
    }

    private void SummonNewVfx(ContactPoint pointOfCollision)
    {
        _harpoonVisualEffects[_functionRef].PlayNextVfxInPool(pointOfCollision.point,
            pointOfCollision.thisCollider.transform.rotation);
    }

    private void RealVfx()
    {
        _harpoonVisualEffects[_functionRef].PlayNextVfxInPool
            (gameObject.transform.position, Quaternion.Inverse(gameObject.transform.rotation));
    }
}