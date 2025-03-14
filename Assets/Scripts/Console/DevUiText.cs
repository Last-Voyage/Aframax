/*****************************************************************************
// File Name :         DevUiText.cs
// Author :            Nabil Tagba
// Creation Date :     2/23/2025
//
// Brief Description : determines what the dev ui text should display
*****************************************************************************/
using TMPro;
using UnityEngine;

/// <summary>
/// determines what the dev ui text should display
/// </summary>
public class DevUiText : MonoBehaviour
{
    enum EDevUITextType
    {
        none,
        medkitUses,
        harpoonUses
    }

    [SerializeField] private EDevUITextType _devUITextType;
    [SerializeField] private GameObject _owningObject;

    /// <summary>
    /// happens every frame
    /// </summary>
    private void Update()
    {
        switch (_devUITextType)
        {
            case EDevUITextType.medkitUses:
                if (_owningObject.TryGetComponent<HealthPackInteractable>(out HealthPackInteractable h))
                {
                    GetComponent<TMP_Text>().text = h.GetNumOfUses().ToString();
                }
                break;
            case EDevUITextType.harpoonUses:
                if (_owningObject.TryGetComponent<AmmoRackInteractable>(out AmmoRackInteractable a))
                {
                    GetComponent<TMP_Text>().text = a.GetNumHarpoons().ToString();
                }
                break;
            default:
                break;
        }
    }
}
