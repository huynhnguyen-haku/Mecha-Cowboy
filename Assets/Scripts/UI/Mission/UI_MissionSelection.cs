using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_MissionSelection : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI missionDescription;
    [SerializeField] private TextMeshProUGUI missionObjective;

    [SerializeField] private TextMeshProUGUI missionReward;
    [SerializeField] private Image missionPreviewImage; // Hình ảnh minh họa

    public void SetMissionDescription(string description)
    {
        missionDescription.text = description;
    }

    public void SetMissionObjective(string objective)
    {
        missionObjective.text = objective;
    }

    public void SetMissionReward(int reward)
    {
        missionReward.text = $"Reward: {reward} coins";
    }

    public void SetMissionPreview(Sprite preview)
    {
        if (preview != null)
        {
            missionPreviewImage.sprite = preview;
            missionPreviewImage.color = Color.white; // Hiển thị hình ảnh
        }
        else
        {
            missionPreviewImage.sprite = null;
            missionPreviewImage.color = Color.clear; // Ẩn hình ảnh nếu không có
        }
    }
}
