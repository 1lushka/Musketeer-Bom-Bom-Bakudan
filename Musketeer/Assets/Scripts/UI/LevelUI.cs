using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("UI элементов")]
    [Tooltip("Круг перезарядки (Image с типом Filled)")]
    public Image cooldownCircle;

    public void SetCooldownProgress(float value)
    {
        if (cooldownCircle != null)
            cooldownCircle.fillAmount = Mathf.Clamp01(value);
    }

    public void SetReady()
    {
        SetCooldownProgress(1f);
    }

    public void SetCooldownStart()
    {
        SetCooldownProgress(0f);
    }
}
