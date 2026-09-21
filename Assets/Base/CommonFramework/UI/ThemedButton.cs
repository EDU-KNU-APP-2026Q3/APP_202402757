using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 버튼 배경 Image에 붙이는 컴포넌트. theme 필드에 프로젝트의 UITheme 자산을 드래그해 넣으세요.
/// </summary>
[RequireComponent(typeof(Image))]
public class ThemedButton : MonoBehaviour
{
    public UITheme theme;
    Image _bg;

    void OnEnable()
    {
        _bg = GetComponent<Image>();
        if (theme == null)
        {
            Debug.LogWarning($"[ThemedButton] {name}에 UITheme가 연결되어 있지 않습니다.");
            return;
        }
        theme.OnThemeChanged += Apply;
        Apply();
    }

    void OnDisable()
    {
        if (theme != null) theme.OnThemeChanged -= Apply;
    }

    public void Apply()
    {
        if (theme == null || _bg == null) return;
        _bg.color = theme.buttonColor;
    }
}
