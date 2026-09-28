using UnityEngine;

/// <summary>
/// 타이머 기반 자동 공격. 조준/발사 버튼 없음 — 일정 주기로 가장 가까운 적에게 투사체를 자동 발사.
/// [정상 케이스] 사거리 안에 적이 1마리 이상 있으면 가장 가까운 적에게 발사.
/// [예외 케이스] 사거리 안에 적이 하나도 없으면 발사하지 않고 조용히 대기 (에러 없음).
/// </summary>
public class AutoAttack : MonoBehaviour
{
    public GameObject projectilePrefab;
    public float attackInterval = 1.0f;
    public float attackRange = 5f;
    public int damage = 10;
    public string enemyTag = "Enemy";

    float _timer;

    void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= attackInterval)
        {
            _timer = 0f;
            TryAttack();
        }
    }

    void TryAttack()
    {
        GameObject nearest = FindNearestEnemy();
        if (nearest == null) return; // 예외 케이스: 적 없으면 그냥 넘어감

        float dist = Vector2.Distance(transform.position, nearest.transform.position);
        Debug.Log($"[DEBUG][AutoAttack] 발사! 대상={nearest.name}, 거리={dist:0.00}, 방향으로 직선 발사 (추적 아님)");

        Vector2 direction = (nearest.transform.position - transform.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        var p = proj.GetComponent<Projectile>();
        if (p != null) p.Init(direction, damage);
        else Debug.LogWarning("[DEBUG][AutoAttack] 생성된 프리팹에 Projectile 컴포넌트가 없습니다!");
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject nearest = null;
        float nearestDist = attackRange;

        foreach (var e in enemies)
        {
            float dist = Vector2.Distance(transform.position, e.transform.position);
            if (dist <= nearestDist)
            {
                nearestDist = dist;
                nearest = e;
            }
        }
        return nearest;
    }
}
