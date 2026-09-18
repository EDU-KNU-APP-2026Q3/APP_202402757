using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// [완성 예시 — 3단계 "완결"의 기준점]
/// 이 스크립트는 교수자가 미리 완성해 제공하는 설정 화면 예시입니다.
/// 학생은 이 코드를 참고해서 "점수/기록 화면" 등 다른 화면을 스스로 같은 완성도로 만들어보세요.
///
/// 이 예시가 "3단계 완결"인 이유:
/// 1) SettingsManager와 실제로 연결되어 값이 저장/반영됨 (기능 작동)
/// 2) 값이 바뀔 때 화면에 즉시 표시됨 (피드백)
/// 3) 최초 실행(저장된 값 없음)이라는 예외 상황도 자연스럽게 처리됨 (SettingsManager가 기본값을 주므로)
/// 4) 화면을 나갈 때 별도 "저장" 버튼 없이도 값이 안전하게 보존됨
/// </summary>
public class SettingsScreenUI : MonoBehaviour
{
    [Header("UI 참조")]
    public Slider masterVolumeSlider;
    public TMP_Text masterVolumeLabel;
    public Slider sfxVolumeSlider;
    public TMP_Text sfxVolumeLabel;
    public Slider sensitivitySlider;
    public TMP_Text sensitivityLabel;
    public Toggle vibrationToggle;
    public TMP_Text vibrationLabel;

    void OnEnable()
    {
        // 현재 저장된 값으로 UI 초기화 (예외 케이스: 최초 실행이어도 SettingsManager가 기본값을 줌)
        var s = SettingsManager.Instance;

        masterVolumeSlider.SetValueWithoutNotify(s.MasterVolume);
        UpdateLabel(masterVolumeLabel, s.MasterVolume);

        sfxVolumeSlider.SetValueWithoutNotify(s.SfxVolume);
        UpdateLabel(sfxVolumeLabel, s.SfxVolume);

        sensitivitySlider.SetValueWithoutNotify(s.TouchSensitivity);
        sensitivityLabel.text = $"{s.TouchSensitivity:0.0}x";

        vibrationToggle.SetIsOnWithoutNotify(s.VibrationEnabled);
        UpdateToggleLabel(s.VibrationEnabled);

        // 이벤트 연결 (중복 연결 방지를 위해 먼저 해제 후 연결)
        masterVolumeSlider.onValueChanged.RemoveAllListeners();
        masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);

        sfxVolumeSlider.onValueChanged.RemoveAllListeners();
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

        sensitivitySlider.onValueChanged.RemoveAllListeners();
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

        vibrationToggle.onValueChanged.RemoveAllListeners();
        vibrationToggle.onValueChanged.AddListener(OnVibrationChanged);
    }

    void OnMasterVolumeChanged(float value)
    {
        SettingsManager.Instance.SetMasterVolume(value);
        UpdateLabel(masterVolumeLabel, value);
    }

    void OnSfxVolumeChanged(float value)
    {
        SettingsManager.Instance.SetSfxVolume(value);
        UpdateLabel(sfxVolumeLabel, value);
    }

    void OnSensitivityChanged(float value)
    {
        SettingsManager.Instance.SetTouchSensitivity(value);
        sensitivityLabel.text = $"{value:0.0}x";
    }

    void OnVibrationChanged(bool enabled)
    {
        SettingsManager.Instance.SetVibrationEnabled(enabled);
        UpdateToggleLabel(enabled);
    }

    void UpdateLabel(TMP_Text label, float value01)
    {
        label.text = $"{Mathf.RoundToInt(value01 * 100)}%";
    }

    void UpdateToggleLabel(bool enabled)
    {
        vibrationLabel.text = enabled ? "켜짐" : "꺼짐";
    }
}
