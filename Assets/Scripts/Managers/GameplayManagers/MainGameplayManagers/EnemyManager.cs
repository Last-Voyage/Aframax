/******************************************************************************
// File Name:       EnemyManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 15, 2024
//
// Description:     Provides other scripts with access to the boss.
                    Manager to be developed as I know specifics
******************************************************************************/

/// <summary>
/// Provides other scripts with access to the boss
/// Manager to be developed as I know specifics
/// </summary>
public class EnemyManager : MainGameplayManagerFramework
{
    public static EnemyManager Instance;

    #region Base Manager
    /// <summary>
    /// Establishes the instance for the enemy manager
    /// </summary>
    public override void SetUpInstance()
    {
        base.SetUpInstance();
        Instance = this;
    }

    #endregion
}
