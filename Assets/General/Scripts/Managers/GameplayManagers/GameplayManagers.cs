using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManagers : CoreManagersFramework
{
    public static GameplayManagers Instance;

    protected override bool EstablishInstance()
    {
        Instance = this;
        return true;
    }

    protected override void SetupMainManagers()
    {
        
    }
}
