using UnityEngine;

/// <summary>
/// "Managers" 부모 오브젝트에 붙이는 컴포넌트입니다.
/// DontDestroyOnLoad는 루트(최상위) 오브젝트에만 적용되므로, 자식인 GameManager/SceneFlowManager/
/// SettingsManager/ScoreManager가 각자 호출하면 동작하지 않습니다 (경고 발생, 씬 전환 시 파괴됨).
/// 대신 부모인 이 오브젝트 하나가 대표로 호출하면, 자식들은 부모를 따라 함께 유지됩니다.
///
/// [DefaultExecutionOrder(-100)]로 다른 매니저 스크립트보다 먼저 Awake가 실행되도록 지정해,
/// 혹시 씬에 Managers가 중복으로 존재하는 경우(예: 다른 씬에도 실수로 배치) 먼저 정리합니다.
/// </summary>
[DefaultExecutionOrder(-100)]
public class ManagerRoot : MonoBehaviour
{
    static ManagerRoot _instance;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            // 이미 어딘가에 Managers가 살아있다면, 지금 로드된 이 씬의 중복 사본은 제거
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
