using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalManagers : CoreManagersFramework
{
    public static UniversalManagers Instance;

    protected override bool EstablishInstance()
    {
        if (Instance == null)
        {
            Instance = this;
            return true;
        }
        return false;
    }

    protected override void SetupMainManagers()
    {
        throw new System.NotImplementedException();
    }
}
