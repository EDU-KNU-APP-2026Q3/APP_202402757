using UnityEngine;

/// <summary>
/// 설정값(볼륨, 조작 민감도, 진동)을 저장/불러오는 매니저. PlayerPrefs로 로컬 저장.
/// [정상 케이스] 값 범위: MasterVolume/SfxVolume = 0~1, TouchSensitivity = 0.5~2.0
/// [예외 케이스] 최초 실행 시 PlayerPrefs에 값이 없으면 기본값(1.0, 1.0, 1.0, true)을 사용해야 함 — 여기서 이미 처리됨
/// </summary>
[DefaultExecutionOrder(-100)]
public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance { get; private set; }

    const string KEY_MASTER = "settings_master_volume";
    const string KEY_SFX = "settings_sfx_volume";
    const string KEY_SENSITIVITY = "settings_touch_sensitivity";
    const string KEY_VIBRATION = "settings_vibration";

    public float MasterVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public float TouchSensitivity { get; private set; }
    public bool VibrationEnabled { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // DontDestroyOnLoad는 부모(Managers)의 ManagerRoot.cs가 대표로 한 번만 호출합니다.
        LoadSettings();
    }

    void LoadSettings()
    {
        // 예외 케이스 처리: 저장된 값이 없으면 기본값 사용
        MasterVolume = PlayerPrefs.GetFloat(KEY_MASTER, 1.0f);
        SfxVolume = PlayerPrefs.GetFloat(KEY_SFX, 1.0f);
        TouchSensitivity = PlayerPrefs.GetFloat(KEY_SENSITIVITY, 1.0f);
        VibrationEnabled = PlayerPrefs.GetInt(KEY_VIBRATION, 1) == 1;
    }

    public void SetMasterVolume(float value)
    {
        MasterVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(KEY_MASTER, MasterVolume);
        PlayerPrefs.Save();
        GameEvents.RaiseSettingsChanged();
    }

    public void SetSfxVolume(float value)
    {
        SfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(KEY_SFX, SfxVolume);
        PlayerPrefs.Save();
        GameEvents.RaiseSettingsChanged();
    }

    public void SetTouchSensitivity(float value)
    {
        TouchSensitivity = Mathf.Clamp(value, 0.5f, 2.0f);
        PlayerPrefs.SetFloat(KEY_SENSITIVITY, TouchSensitivity);
        PlayerPrefs.Save();
        GameEvents.RaiseSettingsChanged();
    }

    public void SetVibrationEnabled(bool enabled)
    {
        VibrationEnabled = enabled;
        PlayerPrefs.SetInt(KEY_VIBRATION, enabled ? 1 : 0);
        PlayerPrefs.Save();
        GameEvents.RaiseSettingsChanged();
    }
}
