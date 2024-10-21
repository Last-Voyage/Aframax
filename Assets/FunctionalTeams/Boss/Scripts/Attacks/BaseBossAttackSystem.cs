using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBossAttackSystem : MonoBehaviour
{
    [SerializeField] protected GameObject[] _attackList;
    protected bool _isAttacking = false;
    protected int _randomInt;

    IEnumerator attackCalled()
    {
        _randomInt = Random.Range(0, _attackList.Length);
        _isAttacking = true;
        yield return new WaitForSeconds(1f);
        _isAttacking = false;
    }
  private IEnumerator attackStatement()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            if (_isAttacking)
            {
                _attackList[_randomInt] = Instantiate(_attackList[_randomInt], this.transform.position, Quaternion.identity);
            }
        }
    }
}
