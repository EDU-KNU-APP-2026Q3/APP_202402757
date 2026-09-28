using UnityEngine;
using TMPro;

/// <summary>
/// GameOver 씬 — 최고 점수 표시
/// 배치: GameOver 씬에 TMP_Text 1개 추가
/// 필드 연결: highScoreText에 TMP_Text 드래그
/// 패턴: 읽기(Read) — 이 씬은 매번 새로 로드되므로, 진입 시(OnEnable) 현재 값을
///       한 번만 확인하면 됨. 구독할 필요 없음(이 씬에 있는 동안 값이 바뀔 일이 없으므로).
/// </summary>
public class GameOverHighScoreDisplay : MonoBehaviour
{
    public TMP_Text highScoreText;

    void OnEnable()
    {
        highScoreText.text = $"BestScore: {ScoreManager.Instance.HighScore}";
    }
}
