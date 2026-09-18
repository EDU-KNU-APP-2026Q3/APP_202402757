using UnityEngine;

/// <summary>
/// AutoAttack이 생성하는 투사체. 직선으로 날아가다 적과 충돌하면 데미지를 주고 사라짐.
/// Rigidbody2D(Body Type을 Kinematic으로 설정) + Collider2D(Is Trigger 체크) 필요.
/// </summary>
public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    public float lifeTime = 3f; // 예외 케이스: 아무것도 안 맞아도 3초 후 자동 소멸 (메모리 누수 방지)

    Vector2 _direction;
    int _damage;

    public void Init(Vector2 direction, int damage)
    {
        _direction = direction;
        _damage = damage;
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(_direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // [DEBUG] 뭐든 닿으면 일단 전부 로그로 확인 — 원인 파악 끝나면 이 로그 줄은 지우세요
        Debug.Log($"[DEBUG][Projectile] Trigger Enter: {other.name} (tag={other.tag})");

        if (other.CompareTag("Enemy"))
        {
            var health = other.GetComponent<EnemyHealth>();
            if (health == null)
            {
                Debug.LogWarning($"[DEBUG][Projectile] {other.name}에 EnemyHealth 컴포넌트가 없습니다!");
            }
            else
            {
                Debug.Log($"[DEBUG][Projectile] {other.name}에 데미지 {_damage} 적용");
                health.TakeDamage(_damage);
            }
            Destroy(gameObject);
        }
    }
}
