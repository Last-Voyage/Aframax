using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossAttackPhaseSystem : MonoBehaviour
{
    [Tooltip("Put all of the phases in here to be activated when needed")]
    [SerializeField] private GameObject[] _phaseCollection;
    //The current phase it is on
    protected private int _phaseCounter = 0;
    //A switch bool that works in tandem with the phase counter
    private bool _isPhaseOver;


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
                _phaseCollection[_phaseCounter].SetActive(false);
                _phaseCounter++;
                _phaseCollection[_phaseCounter].SetActive(true);
                _isPhaseOver = false;
            }
            if(_phaseCounter > _phaseCollection.Length)
            {
                //Does game end here??
            }
        }
    }

}
