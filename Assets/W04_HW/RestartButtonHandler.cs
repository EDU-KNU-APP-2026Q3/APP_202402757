using UnityEngine;

/// <summary>
/// GameOver 씬 — 재시작 버튼 (과제)
/// 배치: GameOver 씬에 Button 1개 ("재시작") 추가
/// 필드 연결: 버튼 On Click()에 OnRestartClicked() 연결
/// 패턴: 호출(Call) — RestartGame() 내부에서 점수 초기화 + GamePlay 씬 전환까지 자동 처리됨
///       (매니저 내부 구현은 블랙박스 — 몰라도 됨. 오늘 실습에서 다루지 않은 새로운 호출이니 직접 찾아 적용해볼 것)
/// </summary>
public class RestartButtonHandler : MonoBehaviour
{
    public void OnRestartClicked()
    {
        GameManager.Instance.RestartGame();
    }
}
