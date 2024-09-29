using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalHealth : UniversalHealthSystem
{
    [Header("Health Description")]
    [Tooltip("Health at the beginning of game")]
    [SerializeField] private float _maxHealth;
    [Tooltip("Determines if player or not")]
    [SerializeField] private bool _isPlayer;

    private Health health;
    void Start()
    {
        health = new Health(_maxHealth, _maxHealth, _isPlayer);
        Debug.Log(health.MaxHealth);
        Debug.Log(health.CurrentHealth);
        Debug.Log(health.IsPlayer);
    }
    // Update is called once per frame
    void Update()
    {
        if(health.WaveClear == true)
        {
            health.CurrentHealth = health.MaxHealth;
            health.WaveClear = false;
        }
        if(health.CurrentHealth <= 0)
        {
            Destroy(gameObject);
        }
        if(health.CurrentHealth > health.MaxHealth)
        {
            health.CurrentHealth = health.MaxHealth;
        }
    }
    public void HealthDecrease(float _damage)
    {
        health.CurrentHealth -= _damage;
    }
    public void HealthIncrease(float _heal)
    {
        health.CurrentHealth += _heal;
    }
}
