using UnityEngine;
using UnityEngine.UI;

public class LevelUI : MonoBehaviour
{
    [Header("UI элементов")]
    [Tooltip(" руг перезар€дки (Image с типом Filled)")]
    public Image cooldownCircle;

    /// <summary>
    /// ”станавливает прогресс перезар€дки (0 Ч пусто, 1 Ч полностью готов)
    /// </summary>
    public void SetCooldownProgress(float value)
    {
        if (cooldownCircle != null)
            cooldownCircle.fillAmount = Mathf.Clamp01(value);
    }

    /// <summary>
    /// ”станавливает, что пушка полностью готова к стрельбе
    /// </summary>
    public void SetReady()
    {
        SetCooldownProgress(1f);
    }

    /// <summary>
    /// ќбнул€ет индикатор перезар€дки
    /// </summary>
    public void SetCooldownStart()
    {
        SetCooldownProgress(0f);
    }
}
