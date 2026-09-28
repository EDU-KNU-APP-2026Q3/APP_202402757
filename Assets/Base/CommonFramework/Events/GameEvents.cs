using System;

/// <summary>
/// 게임 전역에서 발생하는 이벤트를 한 곳에 모아둔 클래스.
/// 학생 UI는 이 이벤트들을 구독(subscribe)해서 화면을 갱신하면 됩니다.
/// 게임 코어(Survivor 등)는 이 이벤트들을 발생(invoke)시키기만 합니다.
///
/// [정상 케이스 / 예외 케이스 정의 — 12주차 자가진단 판정 기준]
/// OnScoreChanged: 정상 = 점수가 0 이상 정수로 전달됨. 예외 = 없음(항상 유효한 값만 전달)
/// OnPlayerDied: 정상 = 라운드 중 1회만 호출됨. 예외 = 라운드 시작 직후(0초) 호출되는 경우 UI가 깨지지 않아야 함
/// OnNewHighScore: 정상 = 기존 최고점보다 클 때만 호출. 예외 = 최초 플레이(최고점 없음)일 때도 호출됨
/// </summary>
public static class GameEvents
{
    public static event Action<int> OnScoreChanged;
    public static event Action OnPlayerDied;
    public static event Action OnNewHighScore;
    public static event Action OnSettingsChanged;

    // 게임 코어 스크립트에서 호출하는 발생 메서드
    public static void RaiseScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
    public static void RaisePlayerDied() => OnPlayerDied?.Invoke();
    public static void RaiseNewHighScore() => OnNewHighScore?.Invoke();
    public static void RaiseSettingsChanged() => OnSettingsChanged?.Invoke();
}
