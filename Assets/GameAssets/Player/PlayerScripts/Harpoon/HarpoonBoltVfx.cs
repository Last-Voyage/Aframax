using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HarpoonBoltVfx : MonoBehaviour
{
    private Dictionary<Material, uint> _materialVfxRefs = new();
    
    private List<Material> _materialsInCollision = new List<Material>();
    
    private SpecificVisualEffect[] _harpoonVisualEffects =
    {
       default, VfxManager.Instance.GetMetalSparksVfx(), VfxManager.Instance.GetDeckSplintersVfx(),
        VfxManager.Instance.GetTreeSplintersVfx()
    };

    private uint _functionRef;
    private const uint _noVFX = 0;

    private void OnCollisionEnter(Collision other)
    {
        _functionRef = _noVFX;
        
        if (other.gameObject.GetComponent<MeshRenderer>() != null)
        {
            _materialsInCollision = null;
            //_materialsInCollision = other.gameObject.GetComponent<MeshRenderer>().materials.ToList();
            
            _materialsInCollision = other.gameObject.GetComponent<MeshRenderer>
                ().materials.Where(addedMaterials => _materialVfxRefs.ContainsKey(addedMaterials)).ToList();
            
            foreach (Material material in _materialsInCollision)
            {
                if (_materialVfxRefs.TryGetValue(material, out uint value))
                {
                    _functionRef = value;
                }
            }
            
            /*_functionRef = _materialsInCollision.Where
                (addingMaterials => _materialVfxRefs.ContainsKey
                    (addingMaterials)).Equals(_materialVfxRefs.Keys) ?  : _noVFX;*/

            /*_functionRef = _materialsInCollision.Where
                (realMaterial => _materialVfxRefs.TryGetValue
                    (realMaterial, out uint funner)).Equals(_materialVfxRefs.Keys) ?  : _noVFX;*/

            /*_functionRef = _materialsInCollision.Where(material =>
                _materialVfxRefs.TryGetValue(material, out uint fun));*/

            /*_functionRef = (_materialsInCollision.FirstOrDefault(material =>
                _materialVfxRefs.TryGetValue(material, out uint grapes))) ? grapes : _noVFX;*/
        }

        if (_harpoonVisualEffects[_functionRef] != null)
        {
            SummonNewVfx(other.GetContact(0));
        }
    }

    private void SummonNewVfx(ContactPoint pointOfCollision)
    {
        _harpoonVisualEffects[_functionRef].PlayNextVfxInPool(pointOfCollision.point,
            pointOfCollision.thisCollider.transform.rotation);
    }
}