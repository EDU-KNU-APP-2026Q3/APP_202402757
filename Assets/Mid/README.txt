Mid - 점수/기록 화면
학번: 202402757 / 이름: 김민

씬 흐름
Mid_MainMenu → Mid → Mid_GameOver
- Mid.unity는 Assets/W06_HW/W06_HW.unity를 복사한 게임 씬입니다.
- W06의 캐릭터, 애니메이션, 적 프리팹, 공격 로직은 그대로 참조합니다.
- 시작 화면과 게임오버 화면의 '기록 보기'로 기록 패널을 엽니다.
- '닫기'는 패널만 닫고, '다시 하기'는 새 라운드를 시작합니다.

기록 표시
- 기존 ScoreManager의 HighScore와 TopScores를 읽습니다.
- TopScores가 비었으면 '아직 기록이 없습니다'와 순위별 '-'를 표시합니다.
- 0점으로 완료한 라운드는 정상 기록이며, 기록 없음과 구분합니다.
- 점수 저장은 기존 ScoreManager의 PlayerPrefs 기능을 사용합니다.

화면 대응
- Canvas Scaler: Scale With Screen Size / 1080×1920 / Expand
- Safe Area 적용
- 세로: 최고 기록 위, Top 3 아래
- 가로: 최고 기록 왼쪽, Top 3 오른쪽
- 제목·닫기 버튼은 각각 위·아래에 고정하며 텍스트는 영역 내 자동 크기 조절

과제 자산
Scripts: 기록 UI, 씬 초기화, 버튼 동작, 비율 배치
Prefabs: Mid 전용 Managers 및 ScoreBoard
UI: 한글 TMP 폰트, 테마, 패널 스프라이트
Editor: 최초 씬 생성 도구 및 별도 저장 공간의 검증 빌드 도구
Tests: opt-in 실행 검증 및 Unity UI 캡처

제출 PDF 및 캡처는 프로젝트 바깥 D:\APP_202402757\Mid_Submission에 저장합니다.
Editor/MidSceneSetup.Create는 최초 생성용이며 기존 씬이 있으면 덮어쓰지 않습니다.
기존 디자인을 Mid 브랜치 / Assets/Mid에 보존했습니다.
로컬 커밋 메시지: 중간고사 작업. 푸시는 수행하지 않았습니다.
새 기획서 기반 W08 디자인은 W08 브랜치의 Assets/W08_HW에 별도로 저장합니다.
