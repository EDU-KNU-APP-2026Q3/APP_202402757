using UnityEngine;

/// <summary>
/// GamePlay 씬 — 종료 버튼
/// 배치: GamePlay 씬에 Button 1개 ("종료") 추가
/// 필드 연결: 버튼 On Click()에 OnEndGameClicked() 연결
/// 패턴: 호출(Call) — 플레이어가 실제로 죽지 않아도 수업/실습 중 바로 게임을 끝낼 수 있도록,
///       실제 사망 시나리오(PlayerHealth.TakeDamage)와 같은 순서로 직접 호출해줌.
///       FinalizeRound()를 호출해야 GameOver 씬에서 보여줄 최고기록이 정확히 갱신됨.
/// </summary>
public class EndGameButtonHandler : MonoBehaviour
{
    public void OnEndGameClicked()
    {
        ScoreManager.Instance.FinalizeRound();
        GameManager.Instance.SetState(GameState.GameOver);
        SceneFlowManager.Instance.GoToGameOver();
    }
}
