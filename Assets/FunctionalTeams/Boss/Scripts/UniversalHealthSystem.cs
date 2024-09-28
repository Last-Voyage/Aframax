using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalHealthSystem : MonoBehaviour
{
    
     public class Health
     {
           public float MaxHealth;
           public float CurrentHealth;
          public bool IsPlayer;
           public bool WaveClear = false;
        
         public Health (float _mHP, float _cHP, bool _iP)
         {
                MaxHealth = _mHP;
               CurrentHealth = _cHP;
               IsPlayer = _iP;
          }
         public Health()
         {
                MaxHealth = 1;
                CurrentHealth = 1;
                IsPlayer = false;
          }
     }
}
