/*****************************************************************************
// File Name :         FullRoomAttack.cs
// Author :            Tommy Roberts
// Contributor :       Andrew Stapay
// Creation Date :     10/9/2024
//
// Brief Description : controls the full room attack for the boss
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AttackWarningZone : MonoBehaviour
{
    [Tooltip("A toggle for if you want the warning to begin on start or by manual activation")]
    [SerializeField] private bool _doesBeginWarningOnStart;

    [Tooltip("How fast the indicator blinks when it begins blinking")]
    [SerializeField] private float _startBlinkInterval = 1f;
    [Tooltip("How fast the indicator blinks right before it actually attacks")]
    [SerializeField] private float _endBlinkInterval = .1f;
    [Tooltip("How long the indicator will blink for")]
    [SerializeField] private float _blinkDuration = 3f;
    [Tooltip("How long the actual attack will stay covering the room")]
    [SerializeField] private float _hitBoxAppearDuration = 1f;
    [Tooltip("Amount of time between attacks (in seconds)")]
    [SerializeField] private float _timeBetweenAttacks = 3f;

    [SerializeField] private Material _lowOpacity;
    [SerializeField] private Material _highOpacity;

    [Tooltip("Link to indicator object that flashes red currently")]
    [SerializeField] private GameObject _attackIndicator;

    private MeshRenderer _warningMeshRenderer;

    private Coroutine _warningZoneCoroutine;

    private UnityEvent _onWarningEnd = new UnityEvent();

    private void Start()
    {
        if(_doesBeginWarningOnStart)
        {
            StartWarningZone();
        }
    }

    public void StartWarningZone()
    {
        if(_warningZoneCoroutine == null)
        {
            GameObject newHitbox = Instantiate(_attackIndicator, transform.position, Quaternion.identity);

            // Need to be childed to move with the boat
            newHitbox.transform.parent = transform;

            _warningMeshRenderer = newHitbox.GetComponent<MeshRenderer>();
            _warningMeshRenderer.material = _lowOpacity;

            _warningZoneCoroutine = StartCoroutine(WarningZoneProcess());
        }
    }

    public void CancelWarningZone()
    {
        if (_warningZoneCoroutine != null)
        {
            StopCoroutine(_warningZoneCoroutine);
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
            _warningMeshRenderer.material = _highOpacity;
            yield return new WaitForSeconds(currentBlinkInterval);
            _warningMeshRenderer.material = _lowOpacity;
            yield return new WaitForSeconds(currentBlinkInterval);
            elapsedTime += currentBlinkInterval * 2;

            yield return null;
        }

        WarningComplete();
    }

    private void WarningComplete()
    {
        InvokeOnWarningEnd();
        Destroy(gameObject);
    }

    /// <summary>
    /// Invokes event for when the warning has concluded
    /// </summary>
    private void InvokeOnWarningEnd()
    {
        _onWarningEnd?.Invoke();
    }
    /*
    private void TempFunc()
    {
        if (TentaclesLeft(spawnIndex))
        {
            // after done blinking make the block solid and enable collider to hit player for a second
            attackMeshRenderer.material = _hitOpacity;

            // TODO in VS: Damage and timer should be handled by a new script on the indicator prefab, not here
            // https://bradleycapstone.atlassian.net/browse/LV-322?atlOrigin=eyJpIjoiZjM1ZGM1MTg5MTA3NDY0ZjlkMmRiYTRhMDViNDYwYjUiLCJwIjoiaiJ9
            attackCollider.enabled = true;

            attackMeshRenderer.enabled = true;

            //wait for hit time
            yield return new WaitForSeconds(_hitBoxAppearDuration);

            //disable the enabled
            attackMeshRenderer.enabled = false;
            attackCollider.enabled = false;

            // wait before attacking again
            yield return new WaitForSeconds(_timeBetweenAttacks);
        }

        Destroy(newHitbox);
    }*/

    #region Getters
    public UnityEvent GetOnWarningEndEvent() => _onWarningEnd;
    #endregion
}
