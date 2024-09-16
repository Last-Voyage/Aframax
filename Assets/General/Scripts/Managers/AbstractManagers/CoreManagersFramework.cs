using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CoreManagersFramework : MonoBehaviour
{
    protected virtual void Awake()
    {
        if(EstablishInstance())
        {
            SetupMainManagers();
        }
        else
        {
            Destroy(gameObject);
        }

    }

    protected abstract bool EstablishInstance();

    protected abstract void SetupMainManagers();
}
