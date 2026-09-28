using UnityEngine;

/// <summary>
/// 적 체력. 죽으면 점수 지급 후 파괴.
/// </summary>
public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 30;
    public int scoreValue = 10;

    int _currentHealth;
    bool _isDead = false;

    void OnEnable()
    {
        _currentHealth = maxHealth;
        _isDead = false;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead) return;

        _currentHealth -= amount;
        if (_currentHealth <= 0)
        {
            _isDead = true;
            if (ScoreManager.Instance != null) ScoreManager.Instance.AddScore(scoreValue);
            Destroy(gameObject);
        }
    }
}
