using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 이동만 담당. 조준/발사 없음 — 로그라이트 서바이버 장르의 핵심 특징.
/// 키보드(개발용, New Input System)와 가상 조이스틱(모바일용) 둘 다 지원.
/// 가상 조이스틱 UI가 있다면 SetMoveInput(Vector2)를 매 프레임 호출해주면 됩니다.
///
/// ⚠️ 이 스크립트를 쓰려면 프로젝트에 Input System 패키지가 설치되어 있어야 하고,
///    Edit > Project Settings > Player > Active Input Handling이
///    "Input System Package (New)" 또는 "Both"로 설정되어 있어야 합니다.
///
/// [정상 케이스] 입력값은 항상 -1~1 범위의 Vector2
/// [예외 케이스] 입력이 전혀 없을 때(정지) Vector2.zero가 들어와도 안전하게 처리됨
/// [예외 케이스] 키보드가 없는 환경(모바일 등)에서는 조이스틱 입력만 사용, 에러 없이 동작
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 4f;

    Rigidbody2D _rb;
    Vector2 _moveInput;
    bool _usingKeyboard; // 키보드 입력 중이었는지 추적 — 조이스틱 값을 실수로 덮어쓰지 않기 위함

    void Awake() => _rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        var kb = Keyboard.current;
        if (kb == null) return; // 예외 케이스: 키보드가 없는 환경 — 조이스틱 입력만 사용

        // WASD + 방향키 모두 지원
        float x = 0f, y = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) x -= 1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) x += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed) y -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed) y += 1f;

        if (x != 0 || y != 0)
        {
            _moveInput = new Vector2(x, y).normalized;
            _usingKeyboard = true;
        }
        else if (_usingKeyboard)
        {
            // 키를 뗀 순간에만 정지 처리 (조이스틱만 쓰는 경우엔 이 분기를 타지 않아 값이 보존됨)
            _moveInput = Vector2.zero;
            _usingKeyboard = false;
        }
    }

    void FixedUpdate()
    {
        float sensitivity = SettingsManager.Instance != null ? SettingsManager.Instance.TouchSensitivity : 1f;
        _rb.linearVelocity = _moveInput * moveSpeed * sensitivity;
    }

    /// <summary>가상 조이스틱 UI에서 호출: 예) joystick.OnDrag -> player.SetMoveInput(dragVector)</summary>
    public void SetMoveInput(Vector2 input)
    {
        _moveInput = Vector2.ClampMagnitude(input, 1f);
        _usingKeyboard = false; // 조이스틱이 입력을 준 프레임부터는 키보드 쪽 리셋 로직이 이 값을 건드리지 않도록
    }
}
