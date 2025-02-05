/*********************************************************************************************************************
// File Name :         BaseDamage.cs
// Author :            Andrea Swihart-DeCoster
// Creation Date :     10/23/24
//
// Brief Description : Controls the default damage functionality
*********************************************************************************************************************/

using UnityEngine;

/// <summary>
/// Controls the default damage functionality
/// </summary>
public class BaseDamage : MonoBehaviour, IBaseDamage
{
    [Tooltip("Damage to be applied")]
    [field: SerializeField] public float Damage { get; private set; }

    /// <summary>
    /// Whether or not the game object can apply damage to a receiving health script.
    /// Default value is true.
    /// </summary>
    public bool CanApplyDamage { get; protected set; }

    private void Start()
    {
        CanApplyDamage = true;
    }

    protected virtual void OnTriggerEnter(Collider col)
    {
        if(col.CompareTag("Enemy"))
        {
            ApplyDamage(col.gameObject);
        }
        
    }

    /// <summary>
    /// Applies damage amount to the receiving health script
    /// </summary>
    /// <param name="damageRecipient"> The target we are dealing damage to </param>
    public virtual void ApplyDamage(GameObject damageRecipient)
    {
        if (!CanApplyDamage)
        {
            return;
        }

        if (damageRecipient.TryGetComponent<IBaseHealth>(out IBaseHealth health))
        {
            ApplyDamageToHealth(health);
        }
    }

    /// <summary>
    /// Applies damage taking in the health interface
    /// </summary>
    /// <param name="health"></param>
    protected virtual void ApplyDamageToHealth(IBaseHealth health)
    {
        health.TakeDamage(Damage,this);
    }
}
