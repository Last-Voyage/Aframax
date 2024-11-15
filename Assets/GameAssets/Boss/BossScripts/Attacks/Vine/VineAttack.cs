/*****************************************************************************
// File Name :         VineAttack.cs
// Author :            Ryan Swanson
//Contributor:      Mark Hanson
// Creation Date :     11/7/2024
//
// Brief Description : Controls the functionality for the bosses vine attack
*****************************************************************************/

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Functionality for the spawned tentacles for the vine attack
/// </summary>
[RequireComponent(typeof(AttackWarningZone))]
[System.Serializable]
public class VineAttack : MonoBehaviour
{
    [Tooltip("How long the attack hitbox remains active")]
    [SerializeField] private float _attackHitboxDuration;

    [SerializeField] private Transform _vineAttackSpawnLocation;
    [SerializeField] private GameObject _vineAttackPrefab;

    private AttackWarningZone _warningZone;

    private GameObject _spawnedVine;

    private GameObject _attackGameObject;

    private VineAttackController _associatedController;

    /// <summary>
    /// Sets up the vine attack
    /// </summary>
    public void PerformSetUp(VineAttackController vineAttackController)
    {
        SetStartingValues(vineAttackController);
        SubscribeToEvents();
    }

    /// <summary>
    /// Sets any values that are needed before functionality begins
    /// </summary>
    private void SetStartingValues(VineAttackController vineAttackController)
    {
        _associatedController = vineAttackController;

        _warningZone = GetComponent<AttackWarningZone>();

        _attackGameObject = transform.GetChild(0).gameObject;
        _attackGameObject.SetActive(false);
    }

    /// <summary>
    /// Spawns the visual vine
    /// </summary>
    public void SpawnVine()
    {
        _spawnedVine = Instantiate(_vineAttackPrefab, _vineAttackSpawnLocation);
    }

    /// <summary>
    /// Causes the vine to attack
    /// </summary>
    public void StartAttackProcess()
    {
        _warningZone.StartWarningZone();
        _warningZone.GetOnWarningEndEvent().AddListener(BeginAttackDamage);
    }

    /// <summary>
    /// Activates the hitbox after the warning has concluded
    /// </summary>
    private void BeginAttackDamage()
    {
        _attackGameObject.SetActive(true);
        StartCoroutine(AttackDamageProcess());
        RuntimeSfxManager.APlayOneShotSfx?.Invoke(FmodSfxEvents.Instance.LimbAttack, transform.position);
    }

    /// <summary>
    /// The duration of the attack
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackDamageProcess()
    {
        yield return new WaitForSeconds(_attackHitboxDuration);
        AttackDamageEnd();
    }

    /// <summary>
    /// Concludes the attack
    /// </summary>
    private void AttackDamageEnd()
    {
        _attackGameObject.SetActive(false);
    }

    /// <summary>
    /// Destroys all vines when the attack ends
    /// </summary>
    private void AttackEnd()
    {
        Destroy(_spawnedVine);
    }

    /// <summary>
    /// Subscribes to all needed events
    /// </summary>
    public void SubscribeToEvents()
    {
        //Please rename this getter to include on. Not going to do so in this to avoid merge conflicts just in case
        _associatedController.GetAttackEndEvent().AddListener(AttackEnd);
    }

    /// <summary>
    /// Unsubscribes to all subscribed events
    /// </summary>
    private void UnsubscribeToEvents()
    {
        _associatedController.GetAttackEndEvent().RemoveListener(AttackEnd);
    }

    /// <summary>
    /// Unsubscribes to events on destruction
    /// </summary>
    private void OnDestroy()
    {
        UnsubscribeToEvents();
    }


    #region Getters

    #endregion
}
