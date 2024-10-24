using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossAttackPhaseSystem : MonoBehaviour
{
    //The current phase it is on
    protected private int _phaseCounter = 0;
    //A switch bool that works in tandem with the phase counter
    protected private bool _isPhaseOver;
    //Ideally have a empty hold 2 or more attacks in the one empty and match it up with the phase number
    [Tooltip("Through in attack empty holder that aligns with phase")]
    [SerializeField] private GameObject[] _attackCollection;
    private UnityEvent<BossAttackPhaseSystem> _phaseBegin;
    private UnityEvent<BossAttackPhaseSystem> _phaseEnd;

    private void Awake()
    {
        _isPhaseOver = false;
        _attackCollection[_phaseCounter].SetActive(true);
        InvokePhaseBegin();
    }



    /// <summary>
    /// Phase Management works as a way to listen to attack events and elaborate on what phase it should be on
    /// And when it is over.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PhaseManagement()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            if(_isPhaseOver)
            {
                InvokePhaseEnd();
                _attackCollection[_phaseCounter].SetActive(false);
                _phaseCounter++;
                _attackCollection[_phaseCounter].SetActive(true);
                InvokePhaseBegin();
                _isPhaseOver = false;
            }
            if(_phaseCounter > _attackCollection.Length)
            {
                //Does game end here??
            }
        }
    }

    #region Events
    private void InvokePhaseBegin()
    {
        _phaseBegin?.Invoke(this);
    }
    private void InvokePhaseEnd()
    {
        _phaseEnd?.Invoke(this);
    }
    #endregion
    #region Getters
    public UnityEvent<BossAttackPhaseSystem> GetPhaseBegin() => _phaseBegin;
    public UnityEvent<BossAttackPhaseSystem> GetPhaseEnd() => _phaseEnd;
    #endregion
}
