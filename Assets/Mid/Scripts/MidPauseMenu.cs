using UnityEngine;
using UnityEngine.InputSystem;

public class MidPauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject confirmPanel;
    public CanvasGroup hud;

    private bool paused;
    private bool leaving;
    private float previousTimeScale;
    private void Update()
    {
        if (Keyboard.current?.escapeKey.wasPressedThisFrame != true || leaving) return;
        if (confirmPanel.activeSelf) CancelExit();
        else if (paused) Resume();
        else Pause();
    }

    public void Pause()
    {
        var game = GameManager.Instance;
        if (paused || leaving || game == null || game.CurrentState != GameState.Playing) return;
        paused = true;
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        game.SetState(GameState.Paused);
        hud.interactable = false;
        hud.blocksRaycasts = false;
        pausePanel.SetActive(true);
    }

    public void Resume()
    {
        if (!paused || leaving) return;
        RestoreTime();
        GameManager.Instance.SetState(GameState.Playing);
        hud.interactable = true;
        hud.blocksRaycasts = true;
        confirmPanel.SetActive(false);
        pausePanel.SetActive(false);
    }

    public void AskToExit()
    {
        if (!paused || leaving) return;
        pausePanel.SetActive(false);
        confirmPanel.SetActive(true);
    }

    public void CancelExit()
    {
        if (!paused || leaving) return;
        confirmPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    public void Exit()
    {
        if (!paused || leaving) return;
        leaving = true;
        RestoreTime();
        GameManager.Instance.QuitToMainMenu();
    }

    private void OnDisable() => RestoreTime();

    private void RestoreTime()
    {
        if (!paused) return;
        paused = false;
        Time.timeScale = previousTimeScale;
    }
}
