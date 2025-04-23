using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MissionSelectionButton : UI_Button
{
    private UI_MissionSelection missionSelection;
    private TextMeshProUGUI myText;

    [SerializeField] private Mission myMission;

    private void OnValidate()
    {
        gameObject.name = "Button - Select Mission: " + myMission.missionName;
    }

    public override void Start()
    {
        base.Start();
        missionSelection = GetComponentInParent<UI_MissionSelection>();
        myText = GetComponentInChildren<TextMeshProUGUI>();
        myText.text = myMission.missionName;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);
        missionSelection.SetMissionDescription(myMission.missionDescription);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        MissionManager.instance.SetCurrentMission(myMission);
        UI.instance.SwitchToInGameUI();
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        missionSelection.SetMissionDescription("Choose a mission");
    }
}
