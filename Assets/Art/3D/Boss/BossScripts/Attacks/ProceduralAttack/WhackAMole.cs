using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhackAMole : MonoBehaviour
{
    [SerializeField] private List<Transform> _whackAMolePoints = new List<Transform>();
    private bool _canAttack = false;
    [SerializeField] private float _attackInterval = 6f;
    private float _attackTimer = 0;
    [SerializeField] private GameObject _whackAMoleVine;
    private GameObject _currentActiveVine;
    private ProceduralVine _currentActiveVineScript;
    private Transform _playerTransform;

    private void Update()
    {
        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;

        if (_canAttack && _attackTimer <= 0) SpawnVineAtRandomHole(_playerTransform);
    }

    public void CallAttack(Transform p)
    {
        //if attack in progress or on cd don't call
        if (_attackTimer > 0) return;

        _canAttack = true;
        
        SpawnVineAtRandomHole(p);
    }

    private void SpawnVineAtRandomHole(Transform p)
    {
        _attackTimer = _attackInterval;
        _playerTransform = p;

        //spawn in vine and set variables
        var randTransform = _whackAMolePoints[Random.Range(0, _whackAMolePoints.Count)];
        _currentActiveVine = Instantiate(_whackAMoleVine, transform);
        _currentActiveVineScript = _currentActiveVine.transform.GetChild(2).GetComponent<ProceduralVine>();

        //set up vine and start the appearance
        _currentActiveVine.transform.position = randTransform.position;
        _currentActiveVine.transform.forward = randTransform.up;
        StartCoroutine(StartVineAppear());
    }

    private IEnumerator StartVineAppear()
    {
        yield return new WaitForSeconds(.1f);
        _currentActiveVineScript.StartAppear(_playerTransform);

        //wait for fully appeared 
        yield return new WaitUntil(() => _currentActiveVineScript.GetIsAppeared());

        //trigger attack
        _currentActiveVineScript.StartAttack(_playerTransform.position);
    }


    public bool CanAttack { get => _canAttack; set => _canAttack = value; }
}
