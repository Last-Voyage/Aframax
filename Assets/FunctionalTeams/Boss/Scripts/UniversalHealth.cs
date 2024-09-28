using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalHealth : UniversalHealthSystem
{
    [Tooltip("Used to Determine health numbers and status of character")]
    [SerializeField] private float _maxHealth;
    [SerializeField] private float _currentHealth;
    [SerializeField] private bool _isPlayer;
    private Health health;
    void Start()
    {
        health = new Health(_maxHealth, _currentHealth, _isPlayer);
        Debug.Log(health.MaxHealth);
        Debug.Log(health.CurrentHealth);
        Debug.Log(health.IsPlayer);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
