/*****************************************************************************
// File Name :         FullRoomAttack.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay
// Creation Date :     10/9/2024
//
// Brief Description : controls the full room attack for the boss
*****************************************************************************/
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// contains functionality for full room attack
/// </summary>
public class FullRoomAttack : MonoBehaviour //BaseBossAttackSystem
{
    //attack 1 ref
    [Header("Attack 1")]
    [Space]
    [Tooltip("Link to indicator object that flashes red currently")]
    [SerializeField] private GameObject _bossAttack1Indicator;
    [SerializeField] private Material _lowOpacity;
    [SerializeField] private Material _hitOpacity;
    [Tooltip("how fast the indicator blinks when it begins blinking")]
    [SerializeField] private float _startBlinkInterval = 1f;
    [Tooltip("How fast the indicator blinks right before it actually attacks")]
    [SerializeField] private float _endBlinkInterval = .1f;
    [Tooltip("How long the indicator will blink for")]
    [SerializeField] private float _blinkDuration = 5f;
    [Tooltip("how long the actual attack will stay covering the room")]
    [SerializeField] private float _hitBoxAppearDuration = 1f;

    /// <summary>
    /// links attack to boss attack manager
    /// </summary>
    private void OnEnable() 
    {
        BossAttacksManager.FullRoomAttack += ActivateThisAttack;
    }

    /// <summary>
    /// unlinks attack script from boss manager
    /// </summary>
    private void OnDisable()
    {
        BossAttacksManager.FullRoomAttack -= ActivateThisAttack;
    }
    
    /// <summary>
    /// just for testing
    /// </summary>
    private void Update() 
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            ActivateThisAttack();
        }
    }

    /// <summary>
    /// pretty self explanatory
    /// </summary>
    private void ActivateThisAttack()
    {
        StartCoroutine(DoAttack());
    }

    /// <summary>
    /// creates an indicator that the room is about to be attacked, and then attacks everything in room
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoAttack()
    {
        BossAttacksManager.Instance.AttackInProgress = true;
        _bossAttack1Indicator.GetComponent<MeshRenderer>().material = _lowOpacity;
        var attackMeshRenderer = _bossAttack1Indicator.GetComponent<MeshRenderer>();
        var attackCollider = _bossAttack1Indicator.GetComponent<Collider>();
        //start blinking indicating attack will happen soon
        float elapsedTime = 0f;
        while(elapsedTime < _blinkDuration)
        {
            float currentBlinkInterval = Mathf.Lerp(_startBlinkInterval, _endBlinkInterval, elapsedTime / _blinkDuration);
            attackMeshRenderer.enabled = true;
            yield return new WaitForSeconds(currentBlinkInterval);
            attackMeshRenderer.enabled = false;
            yield return new WaitForSeconds(currentBlinkInterval);
            elapsedTime+= currentBlinkInterval * 2;
        }

        // after done blinking make the block solid and enable collider to hit player for a second
        attackMeshRenderer.material = _hitOpacity;
        attackMeshRenderer.enabled = true;
        attackCollider.enabled = true;

        //wait for hit time
        yield return new WaitForSeconds(_hitBoxAppearDuration);

        //disable the enabled
        attackMeshRenderer.enabled = false;
        attackCollider.enabled = false;
        
        //end attack and cycle to another
        BossAttacksManager.Instance.AttackInProgress = false;
    }
}
