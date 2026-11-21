using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MidAudio : MonoBehaviour
{
    private const string MuteKey = "mid_audio_muted";

    public AudioClip effectsClip;
    private AudioSource music;
    private AudioSource effects;
    private int lastScore;

    public static bool Muted => PlayerPrefs.GetInt(MuteKey) != 0;

    private void Awake()
    {
        music = GetComponent<AudioSource>();
        effects = gameObject.AddComponent<AudioSource>();
        effects.playOnAwake = false;
        ApplySettings();
    }

    private void OnEnable()
    {
        GameEvents.OnSettingsChanged += ApplySettings;
        GameEvents.OnScoreChanged += OnScoreChanged;
    }

    private void Start() => music.Play();

    private void OnDisable()
    {
        GameEvents.OnSettingsChanged -= ApplySettings;
        GameEvents.OnScoreChanged -= OnScoreChanged;
    }

    public void SetMuted(bool muted)
    {
        PlayerPrefs.SetInt(MuteKey, muted ? 1 : 0);
        PlayerPrefs.Save();
        GameEvents.RaiseSettingsChanged();
    }

    public void PreviewEffects()
    {
        if (effectsClip != null && !Muted)
            effects.PlayOneShot(effectsClip);
    }

    private void ApplySettings()
    {
        var settings = SettingsManager.Instance;
        music.volume = settings == null ? 1f : settings.MasterVolume;
        effects.volume = settings == null ? 1f : settings.SfxVolume;
        music.mute = effects.mute = Muted;
        if (Muted) effects.Stop();
    }

    private void OnScoreChanged(int score)
    {
        if (score > lastScore && Time.timeScale > 0f)
            PreviewEffects();
        lastScore = score;
    }
}
