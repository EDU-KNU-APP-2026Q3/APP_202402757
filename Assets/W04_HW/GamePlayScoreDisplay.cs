using UnityEngine;
using TMPro;

/// <summary>
/// GamePlay 씬 — 점수 표시
/// 배치: GamePlay 씬에 TMP_Text 1개 추가
/// 필드 연결: scoreText에 TMP_Text 드래그
/// 패턴: 구독(Subscribe) — 적 처치는 이미 만들어진 게임 로직(EnemyHealth)이 자동으로
///       ScoreManager.AddScore()를 호출하므로, 여기서는 결과만 받아서 표시하면 됨.
/// OnEnable에서 구독한 이벤트는 반드시 OnDisable에서 해제할 것
/// </summary>
public class GamePlayScoreDisplay : MonoBehaviour
{
    public TMP_Text scoreText;

    void OnEnable()
    {
        GameEvents.OnScoreChanged += UpdateScoreText;
    }

    void OnDisable()
    {
        GameEvents.OnScoreChanged -= UpdateScoreText;
    }

    void UpdateScoreText(int score)
    {
        scoreText.text = $"Score: {score}";
    }
}
