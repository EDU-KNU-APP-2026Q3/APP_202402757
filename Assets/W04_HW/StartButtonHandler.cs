using UnityEngine;

/// <summary>
/// 실습 1 — 시작 버튼 연결
/// 배치: 테스트 씬에 Button 1개 ("시작") → 이 스크립트를 빈 GameObject(또는 버튼 자신)에 붙이고
/// Button의 On Click()에 OnStartClicked()를 연결.
/// 패턴: 버튼 클릭 → 매니저 메서드를 직접 호출(Call)
/// </summary>
public class StartButtonHandler : MonoBehaviour
{
    public void OnStartClicked()
    {
        GameManager.Instance.SetState(GameState.Playing);
        SceneFlowManager.Instance.GoToGamePlay();
    }
}