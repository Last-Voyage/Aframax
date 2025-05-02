/******************************************************************************
// File Name:       EnemyManager.cs
// Author:          Ryan Swanson
// Creation Date:   September 15, 2024
//
// Description:     Provides other scripts with access to the boss.
                    Manager to be developed as I know specifics
******************************************************************************/

using UnityEngine.Events;

/// <summary>
/// Provides other scripts with access to the boss
/// Manager to be developed as I know specifics
/// </summary>
public class EnemyManager : MainGameplayManagerFramework
{
    public static EnemyManager Instance;

    private UnityEvent _onChaseSequenceBegin = new UnityEvent();

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

    #region Events
    public void InvokeOnChaseSequenceBegin()
    {
        _onChaseSequenceBegin?.Invoke();
    }
    #endregion

    #region Getter
    public UnityEvent GetOnChaseSequenceBegin() => _onChaseSequenceBegin;
    #endregion
}
