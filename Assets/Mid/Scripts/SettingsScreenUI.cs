using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsScreenUI : MonoBehaviour
{
    public Slider music;
    public Slider effects;
    public Toggle mute;
    public TMP_Text musicValue;
    public TMP_Text effectsValue;
    public TMP_Text muteValue;
    public MidAudio audioController;

    private void OnEnable()
    {
        var settings = SettingsManager.Instance;
        music.SetValueWithoutNotify(settings == null ? 1f : settings.MasterVolume);
        effects.SetValueWithoutNotify(settings == null ? 1f : settings.SfxVolume);
        mute.SetIsOnWithoutNotify(MidAudio.Muted);
        Refresh();
    }

    public void SetMusic(float value)
    {
        SettingsManager.Instance?.SetMasterVolume(value);
        Refresh();
    }

    public void SetEffects(float value)
    {
        SettingsManager.Instance?.SetSfxVolume(value);
        Refresh();
    }

    public void SetMute(bool value)
    {
        audioController.SetMuted(value);
        Refresh();
    }

    private void Refresh()
    {
        musicValue.text = music.value.ToString("P0");
        effectsValue.text = effects.value.ToString("P0");
        muteValue.text = mute.isOn ? "ON" : "OFF";
    }
}
