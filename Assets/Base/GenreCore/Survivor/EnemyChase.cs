using UnityEngine;

/// <summary>
/// 플레이어를 향해 직진하는 가장 단순한 적 AI.
/// </summary>
public class EnemyChase : MonoBehaviour
{
    public float moveSpeed = 2f;
    public int contactDamage = 10;
    public float contactInterval = 1f; // 붙어있는 동안 계속 데미지 주지 않도록 간격 둠

    Transform _player;
    float _contactTimer;

    void Start()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) _player = playerObj.transform;
    }

    void Update()
    {
        if (_player == null) return; // 예외 케이스: 플레이어가 아직 씬에 없으면 대기

        Vector3 dir = (_player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        _contactTimer += Time.deltaTime;
    }

    // [DEBUG] 트리거에 뭐가 닿는지 전부 로그로 확인 — 원인 파악 끝나면 이 함수는 지우세요
    void OnTriggerEnter2D(Collider2D other)
    {
        float dist = Vector2.Distance(transform.position, other.transform.position);
        Debug.Log($"[DEBUG][EnemyChase] Trigger Enter: {other.name} (tag={other.tag}), 거리={dist:0.00}");
    }

    void OnTriggerStay2D(Collider2D other)
    {
        // [DEBUG] Player 태그가 아니어도, Stay 콜백 자체가 오는지 우선 확인
        if (!other.CompareTag("Player"))
        {
            //Debug.Log($"[DEBUG][EnemyChase] Stay 중이지만 Player 태그 아님: {other.name} (tag={other.tag})");
            return;
        }

        float dist = Vector2.Distance(transform.position, other.transform.position);
        Debug.Log($"[DEBUG][EnemyChase] Player와 Stay 중. 거리={dist:0.00}, contactTimer={_contactTimer:0.00}");

        if (_contactTimer >= contactInterval)
        {
            _contactTimer = 0f;
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
            {
                Debug.Log($"[DEBUG][EnemyChase] 데미지 적용: {contactDamage}, 거리={dist:0.00}");
                health.TakeDamage(contactDamage);
            }
        }
    }
}
