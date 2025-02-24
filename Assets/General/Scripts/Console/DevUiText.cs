/*****************************************************************************
// File Name :         DevUiText.cs
// Author :            Nabil Tagba
// Creation Date :     2/23/2025
//
// Brief Description : determins what the dev ui text should display
*****************************************************************************/
using TMPro;
using UnityEngine;

public class DevUiText : MonoBehaviour
{
    enum DevUITextType
    {
        none,
        medkitUses,
        harpoonUses
    }

    [SerializeField] private DevUITextType _devUITextType;
    [SerializeField] private GameObject _owningObject;

    /// <summary>
    /// happens every frame
    /// </summary>
    private void Update()
    {
        switch (_devUITextType)
        {
            case DevUITextType.medkitUses:
                if (_owningObject.TryGetComponent<HealthPackInteractable>(out HealthPackInteractable h))
                {
                    GetComponent<TMP_Text>().text = h.GetNumOfUses().ToString();
                }
                break;
            case DevUITextType.harpoonUses:
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
