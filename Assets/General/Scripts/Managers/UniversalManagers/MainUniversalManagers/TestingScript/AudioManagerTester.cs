using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerTester : MonoBehaviour
{
    [field: SerializeField] public EventReference OneShot { get; private set; }
    [field: SerializeField] public EventReference Ambient { get; private set; }

    [SerializeField] AudioManager Manager;
    EventInstance _audioEvent;

    public void PlayOneshotSound()
    {
        Manager.PlayOneShotSFX(OneShot);
    }
    public void PlayAmbient()
    {
        _audioEvent = Manager.PlayAmbientSFX(Ambient);
    }
    public void StopAmbient()
    {
        Manager.StopAmbientSFX(_audioEvent);
    }

}
