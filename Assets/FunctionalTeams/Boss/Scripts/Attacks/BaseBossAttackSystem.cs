using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseBossAttackSystem : MonoBehaviour
{
    [SerializeField] protected GameObject _attackObject;
    private UnityEvent<BaseBossAttackSystem> _attackBegin = new();
    private UnityEvent<BaseBossAttackSystem> _attackEnd = new();




    protected virtual void AttackBegin()
    {
        InvokeAttackBegin();
    }
    protected virtual void AttackEnd()
    {
        InvokeAttackEnd();
    }
    #region Events
    private void InvokeAttackBegin()
    {
        _attackBegin?.Invoke();
    }
    private void InvokeAttackEnd()
    {
        _attackEnd?.Invoke();
    }
    #endregion
    #region Getters
    public UnityEvent<BaseBossAttackSystem> GetAttackBegin() => _attackBegin;
    public UnityEvent<BaseBossAttackSystem> GetAttackEnd() => _attackEnd;
    #endregion
}
