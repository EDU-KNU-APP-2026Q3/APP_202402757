using UnityEngine;

/// <summary>
/// 시간이 지날수록 적 스폰 빈도를 높이는 웨이브 매니저.
/// [정상 케이스] 게임 시작과 함께 자동으로 스폰 시작, 시간에 따라 간격이 점점 짧아짐.
/// [예외 케이스] spawnInterval이 minSpawnInterval 밑으로 내려가지 않도록 Clamp 처리.
/// </summary>
public class WaveSpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public Transform player;
    public float spawnRadius = 8f;

    public float startInterval = 2.5f;
    public float minSpawnInterval = 0.3f;
    public float difficultyRampPerSecond = 0.01f; // 시간이 지날수록 interval이 줄어드는 속도

    float _timer;
    float _elapsed;

    void Update()
    {
        _elapsed += Time.deltaTime;
        _timer += Time.deltaTime;

        float currentInterval = Mathf.Max(minSpawnInterval, startInterval - _elapsed * difficultyRampPerSecond);

        if (_timer >= currentInterval)
        {
            _timer = 0f;
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        if (player == null || enemyPrefab == null) return; // 예외 케이스: 참조 누락 시 조용히 무시(에러 로그만)

        Vector2 randomDir = Random.insideUnitCircle.normalized;
        Vector3 spawnPos = player.position + (Vector3)(randomDir * spawnRadius);
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}
