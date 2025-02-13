/*****************************************************************************
// File Name :         FullRoomAttack.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay
//                     Ryan Swanson
// Creation Date :     10/9/2024
//
// Brief Description : controls the full room attack for the boss
*****************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Provides functionality for attack warning areas seperated from any other attack
/// </summary>
public class AttackWarningZone : MonoBehaviour
{
    [Tooltip("A toggle for if you want the warning to begin on start or by manual activation")]
    [SerializeField] private bool _doesBeginWarningOnStart;
    [Tooltip("A toggle for if you want the object to destroy itself after the warning is complete")]
    [SerializeField] private bool _doesDestroyOnWarningEnd;
    [Space]
    [Tooltip("How fast the indicator blinks when it begins blinking")]
    [SerializeField] private float _startBlinkInterval = 1f;
    [Tooltip("How fast the indicator blinks right before it actually attacks")]
    [SerializeField] private float _endBlinkInterval = .1f;
    [Tooltip("How long the indicator will blink for")]
    [SerializeField] private float _blinkDuration = 3f;
    // [Tooltip("How long the actual attack will stay covering the room")]
    // [SerializeField] private float _hitBoxAppearDuration = 1f;                    // commented out because it was not being used

    [Space]
    //Left serialized as this may be used on multiple attacks and as
    //  such the child may not always be in the same location
    [Tooltip("Link to indicator object that flashes red currently")]
    [SerializeField] private GameObject _attackIndicator;

    private MeshRenderer _warningMeshRenderer;

    private Coroutine _warningZoneCoroutine;

    private UnityEvent _onWarningEnd = new UnityEvent();

    private void Start()
    {
        SetStartingValues();

        if(_doesBeginWarningOnStart)
        {
            StartWarningZone();
        }
    }

    /// <summary>
    /// Removes all listeners to prevent memory leaks
    /// </summary>
    private void OnDestroy()
    {
        _onWarningEnd.RemoveAllListeners();
    }

    /// <summary>
    /// Sets any values before functionalty begins
    /// </summary>
    private void SetStartingValues()
    {
        _warningMeshRenderer = _attackIndicator.GetComponent<MeshRenderer>();
        _warningMeshRenderer.enabled = false;
    }

    /// <summary>
    /// Starts the warning
    /// </summary>
    public void StartWarningZone()
    {
        if(_warningZoneCoroutine == null)
        {
            _warningZoneCoroutine = StartCoroutine(WarningZoneProcess());
        }
    }

    /// <summary>
    /// Cancels the warning zone early if needed
    /// </summary>
    public void CancelWarningZone()
    {
        if (_warningZoneCoroutine != null)
        {
            StopCoroutine(_warningZoneCoroutine);
            _warningZoneCoroutine = null;
        }
    }

    /// <summary>
    /// Displays the warning zone
    /// </summary>
    /// <returns></returns>
    private IEnumerator WarningZoneProcess()
    {
        //start blinking indicating attack will happen soon
        float elapsedTime = 0f;
        while (elapsedTime < _blinkDuration)
        {
            float currentBlinkInterval = Mathf.Lerp(_startBlinkInterval, _endBlinkInterval, elapsedTime / _blinkDuration);
            _warningMeshRenderer.enabled = true;
            yield return new WaitForSeconds(currentBlinkInterval);
            _warningMeshRenderer.enabled = false;
            yield return new WaitForSeconds(currentBlinkInterval);
            elapsedTime += currentBlinkInterval * 2;

            yield return null;
        }

        WarningComplete();
    }

    /// <summary>
    /// Called when the warning is completed
    /// </summary>
    private void WarningComplete()
    {
        _warningZoneCoroutine = null;
        InvokeOnWarningEnd();
        if(_doesDestroyOnWarningEnd)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Invokes event for when the warning has concluded
    /// </summary>
    private void InvokeOnWarningEnd()
    {
        _onWarningEnd?.Invoke();
    }

    #region Getters
    public UnityEvent GetOnWarningEndEvent() => _onWarningEnd;
    #endregion
}
