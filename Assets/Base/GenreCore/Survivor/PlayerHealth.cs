using UnityEngine;

/// <summary>
/// 플레이어 체력 관리. 0이 되면 GameEvents.OnPlayerDied 발생 + GameOver 흐름 트리거.
/// [정상 케이스] TakeDamage는 라운드 중 여러 번 호출.
/// [예외 케이스] 이미 사망한 상태에서 추가로 TakeDamage 호출되어도 중복으로 죽지 않도록 방지.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int CurrentHealth { get; private set; }

    bool _isDead = false;

    void OnEnable()
    {
        CurrentHealth = maxHealth;
        _isDead = false;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return; // 예외 케이스 방지

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);

        if (CurrentHealth <= 0)
        {
            _isDead = true;
            ScoreManager.Instance.FinalizeRound();
            GameEvents.RaisePlayerDied();
            GameManager.Instance.SetState(GameState.GameOver);
            SceneFlowManager.Instance.GoToGameOver();
        }
    }
}
