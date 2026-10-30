using UnityEngine;

/// <summary>
/// [W06 도우미] 플레이어의 움직임을 읽어서 Animator와 그림 방향에 알려 주는 스크립트.
/// PlayerController(코어)는 건드리지 않습니다. Player 오브젝트에 이 스크립트를 붙이기만 하면 됩니다.
///
/// ■ 기본 단계 (서기/걷기 + 좌우 보기)
///   - Animator Controller에 Float 파라미터 "Speed" 하나만 만들면 됩니다.
///   - Idle → Walk : Speed Greater 0.1 / Walk → Idle : Speed Less 0.1
///     (두 화살표 모두 Has Exit Time 체크 해제, Transition Duration 0)
///   - 그림은 오른쪽을 보는 그림만 있으면 됩니다. 왼쪽으로 갈 때는 그림을 뒤집어 씁니다.
///
/// ■ 심화 단계 (상하좌우 4방향)
///   - Float 파라미터 "MoveX", "MoveY"를 더 만들면 자동으로 4방향 모드가 됩니다.
///   - Blend Tree(2D Simple Directional)에 MoveX/MoveY를 연결하고,
///     (0,-1) 아래 / (0,1) 위 / (1,0) 오른쪽 / (-1,0) 왼쪽 그림을 넣습니다.
///   - 왼쪽 그림을 따로 그리지 않았다면 왼쪽 자리에도 오른쪽 그림을 넣으세요. 뒤집기는 이 스크립트가 합니다.
///
/// [정상 케이스] 움직이는 동안 Speed > 0, 방향은 상하좌우 중 하나로 정리되어 전달됨.
/// [예외 케이스] 멈추면 Speed = 0이 되고, 마지막으로 보던 방향을 그대로 유지함.
/// [예외 케이스] Animator가 없거나 파라미터가 없어도 에러 없이 동작함 (있는 것만 사용).
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerAnimator2D : MonoBehaviour
{
    [Header("연결 (비워 두면 자동으로 찾음)")]
    public Animator animator;
    public SpriteRenderer spriteRenderer;

    [Header("설정")]
    [Tooltip("켜 두면 왼쪽으로 갈 때 그림을 좌우로 뒤집습니다. 왼쪽 그림을 따로 그렸다면 끄세요.")]
    public bool flipWhenMovingLeft = true;

    [Tooltip("이 속도보다 느리면 '서 있음'으로 봅니다.")]
    public float moveThreshold = 0.05f;

    static readonly int SpeedId = Animator.StringToHash("Speed");
    static readonly int MoveXId = Animator.StringToHash("MoveX");
    static readonly int MoveYId = Animator.StringToHash("MoveY");

    Rigidbody2D _rb;
    Vector2 _facing = Vector2.down; // 처음에는 아래(화면 쪽)를 보고 시작
    bool _hasSpeed, _hasMoveX, _hasMoveY;

    bool FourDirectionMode => _hasMoveX && _hasMoveY;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        // Animator에 어떤 파라미터가 있는지 한 번만 확인 (없는 파라미터를 쓰면 경고가 계속 나기 때문)
        if (animator == null || animator.runtimeAnimatorController == null) return;

        foreach (var p in animator.parameters)
        {
            if (p.type != AnimatorControllerParameterType.Float) continue;
            if (p.nameHash == SpeedId) _hasSpeed = true;
            else if (p.nameHash == MoveXId) _hasMoveX = true;
            else if (p.nameHash == MoveYId) _hasMoveY = true;
        }
    }

    void Update()
    {
        Vector2 velocity = _rb.linearVelocity;
        float speed = velocity.magnitude;
        bool isMoving = speed > moveThreshold;

        if (isMoving) _facing = SnapToFourDirections(velocity);

        UpdateAnimator(isMoving ? speed : 0f);
        UpdateFlip();
    }

    /// <summary>대각선 이동도 상하좌우 중 하나로 정리. 정확히 대각선이면 좌우를 우선.</summary>
    static Vector2 SnapToFourDirections(Vector2 v)
    {
        if (Mathf.Abs(v.x) >= Mathf.Abs(v.y))
            return new Vector2(Mathf.Sign(v.x), 0f);
        return new Vector2(0f, Mathf.Sign(v.y));
    }

    void UpdateAnimator(float speed)
    {
        if (animator == null) return;
        if (_hasSpeed) animator.SetFloat(SpeedId, speed);
        if (_hasMoveX) animator.SetFloat(MoveXId, _facing.x);
        if (_hasMoveY) animator.SetFloat(MoveYId, _facing.y);
    }

    void UpdateFlip()
    {
        if (spriteRenderer == null || !flipWhenMovingLeft) return;

        bool facingSideways = _facing.x != 0f;
        if (facingSideways)
        {
            spriteRenderer.flipX = _facing.x < 0f;
        }
        else if (FourDirectionMode)
        {
            // 4방향 모드에서 위/아래 그림은 뒤집지 않음
            spriteRenderer.flipX = false;
        }
        // 기본 모드에서 위/아래로만 움직이면, 직전에 보던 좌우 방향을 그대로 유지
    }
}
