using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerTester : MonoBehaviour
{
    [SerializeField] public EventReference[] OneshotSounds;
    [SerializeField] public EventReference[] AmbientSounds;

    [SerializeField] AudioManager Manager;
    EventInstance _audioEvent;

    public void PlayOneshotSound(int index)
    {
        Manager.PlayOneShotSound(OneshotSounds[index]);
    }
    public void PlayAmbient(int index)
    {
        _audioEvent = Manager.PlayAmbientSound(AmbientSounds[index]);
    }
    public void StopAmbient()
    {
        Manager.StopAmbientSound(_audioEvent);
    }

}
