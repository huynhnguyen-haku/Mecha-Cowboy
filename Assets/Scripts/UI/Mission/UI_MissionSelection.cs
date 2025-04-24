using TMPro;
using UnityEngine;

public class UI_MissionSelection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionDescription;

    public void SetMissionDescription(string description)
    {
        missionDescription.text = description;
    }
}
