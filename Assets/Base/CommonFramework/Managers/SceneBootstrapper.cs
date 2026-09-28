using UnityEngine;

/// <summary>
/// GamePlay(Survivor) 씬처럼 Managers 없이 단독 실행될 수 있는 씬에 빈 GameObject로 하나 추가해두면,
/// MainMenu를 거치지 않고 이 씬만 바로 Play해도 "Managers가 없어서" 나는
/// NullReferenceException(SettingsManager.Instance 등이 null)을 방지합니다.
///
/// 원리:
/// - 씬 시작 시 GameManager.Instance가 이미 있으면(MainMenu를 거쳐 온 정상 흐름) 아무것도 안 함
/// - 없으면(이 씬을 단독으로 바로 실행한 경우) Resources 폴더의 Managers 프리팹을 자동 생성
///
/// 준비물: "Managers" 프리팹을 Assets 안 아무 위치의 "Resources"라는 이름의 폴더 안에 넣어둘 것
/// (예: Assets/_Base/CommonFramework/Resources/Managers.prefab — 폴더 이름이 정확히 "Resources"여야 함)
///
/// 사용법: 이 스크립트를 GamePlay(Survivor) 씬에 빈 GameObject를 하나 만들어 붙여두기만 하면 됨.
/// MainMenu 씬에는 필요 없음 (거기엔 이미 Managers가 있으므로).
/// </summary>
[DefaultExecutionOrder(-200)] // Managers 쪽 스크립트들(-100)보다도 먼저 실행되어야 함
public class SceneBootstrapper : MonoBehaviour
{
    const string ManagersPrefabName = "Managers";

    void Awake()
    {
        if (GameManager.Instance != null) return; // 이미 있으면(정상 흐름) 아무것도 안 함

        var prefab = Resources.Load<GameObject>(ManagersPrefabName);
        if (prefab == null)
        {
            Debug.LogError($"[SceneBootstrapper] Resources 폴더에서 '{ManagersPrefabName}' 프리팹을 찾을 수 없습니다. " +
                            "Managers 프리팹이 정확히 'Resources'라는 이름의 폴더 안에 있는지 확인하세요.");
            return;
        }

        Instantiate(prefab);
    }
}
