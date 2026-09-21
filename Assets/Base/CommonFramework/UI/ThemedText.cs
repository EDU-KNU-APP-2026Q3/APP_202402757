using UnityEngine;
using TMPro;

/// <summary>
/// TextMeshPro 텍스트에 붙이는 컴포넌트. theme 필드에 프로젝트의 UITheme 자산을 드래그해 넣으세요.
/// 폰트나 색을 직접 지정하지 말고, 이 컴포넌트를 통해 테마를 적용하세요.
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class ThemedText : MonoBehaviour
{
    public UITheme theme;
    public bool isHeading = false;

    TMP_Text _text;

    void OnEnable()
    {
        _text = GetComponent<TMP_Text>();
        if (theme == null)
        {
            Debug.LogWarning($"[ThemedText] {name}에 UITheme가 연결되어 있지 않습니다.");
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
        if (theme == null || _text == null) return;
        _text.font = isHeading ? theme.headingFont : theme.primaryFont;
        _text.color = theme.textColor;
    }
}
