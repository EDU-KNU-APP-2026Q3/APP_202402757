using System;
using UnityEngine;
using TMPro;

/// <summary>
/// 중앙 테마 자산. Project 창에서 우클릭 > Create > Game > UI Theme 로 하나 생성.
/// 12주차에 이 자산 하나의 값만 바꾸면, ThemedText/ThemedButton이 붙은 모든 화면에 자동 반영됩니다.
/// 8~10주차(개발 스프린트)엔 기본값 그대로 두고 진행하세요 — 스타일은 12주차 몫입니다.
/// </summary>
[CreateAssetMenu(fileName = "UITheme", menuName = "Game/UI Theme")]
public class UITheme : ScriptableObject
{
    [Header("Typography")]
    public TMP_FontAsset primaryFont;
    public TMP_FontAsset headingFont;

    [Header("Colors")]
    public Color accentColor = new Color(0.3f, 0.6f, 1f);
    public Color backgroundColor = Color.white;
    public Color textColor = Color.black;
    public Color buttonColor = new Color(0.85f, 0.85f, 0.85f);

    [Header("Spacing")]
    public float cornerRadius = 8f;

    public event Action OnThemeChanged;

#if UNITY_EDITOR
    // 에디터에서 Inspector 값을 바꾸는 즉시 화면에 반영되도록 함 (12주차 작업 시 매우 유용)
    void OnValidate() => OnThemeChanged?.Invoke();
#endif

    public void NotifyChanged() => OnThemeChanged?.Invoke(); // 런타임에서 강제로 갱신하고 싶을 때 호출
}
