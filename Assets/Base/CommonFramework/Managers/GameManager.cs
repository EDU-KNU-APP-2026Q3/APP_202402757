using System;
using UnityEngine;

public enum GameState { MainMenu, Playing, Paused, GameOver }

/// <summary>
/// 게임 전체 상태를 관리하는 최상위 매니저. 씬에 하나만 존재(싱글톤).
/// [정상 케이스] SetState는 MainMenu -> Playing -> (Paused <-> Playing) -> GameOver 순서로만 호출됩니다.
/// [예외 케이스] 이미 GameOver 상태에서 SetState(Playing) 호출 시 RestartGame()을 통해서만 허용합니다.
/// </summary>
[DefaultExecutionOrder(-100)] // Managers는 UI 스크립트(SettingsScreenUI 등, 기본 순서 0)보다 먼저 Awake가 실행되어야 함
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; } = GameState.MainMenu;
    public event Action<GameState> OnStateChanged;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // DontDestroyOnLoad는 부모(Managers)의 ManagerRoot.cs가 대표로 한 번만 호출합니다.
        // 이 스크립트는 Managers의 자식이므로 여기서 직접 호출하지 않습니다 (자식에는 적용 안 됨).
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(newState);
    }

    public void RestartGame()
    {
        ScoreManager.Instance.ResetCurrentScore();
        SetState(GameState.Playing);
        SceneFlowManager.Instance.GoToGamePlay();
    }

    public void QuitToMainMenu()
    {
        SetState(GameState.MainMenu);
        SceneFlowManager.Instance.GoToMainMenu();
    }
}
