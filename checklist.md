# Progress Checklist

마일스톤별 진척도. 완료 항목 `[x]`로 체크. 작은 단위로 쪼개서 추적.

---

## 사전 준비 (개발 환경)

- [x] Unity Hub 설치
- [x] Unity 6 LTS Editor 설치
- [x] Android Build Support 모듈 (OpenJDK + SDK & NDK)
- [ ] Visual Studio 2022 Community 또는 Rider 설치
- [x] Git 설치
- [ ] Android 실기 USB 디버깅 활성화
- [ ] (선택) Android Studio (에뮬레이터용)

## 프로젝트 셋업

- [x] git init + .gitignore (Unity 표준)
- [x] 디자인 문서 작성 (6개)
- [x] M1 C# 스크립트 스캐폴드 (15개)
- [x] README.md / SETUP.md
- [x] Unity 프로젝트 생성
- [x] Unity 프로젝트 통합 (My project → roller 루트)
- [x] Android 빌드 타겟 전환 (Switch Platform)
- [ ] Unity 재오픈 + 콘솔 에러 없음 확인
- [ ] Player Settings — Default Orientation: Landscape Left
- [ ] Player Settings — Allowed Orientations: Landscape만
- [ ] Player Settings — Package Name `com.{본인}.roller`
- [ ] Player Settings — Min API Level 24 (Android 7.0)
- [ ] Player Settings — Scripting Backend IL2CPP
- [ ] Player Settings — Target Architectures ARM64
- [ ] Quality Settings — Anti Aliasing Disabled (Android)
- [ ] Game 뷰 해상도 1920×1080 가로 등록
- [x] git 원격 저장소 연결 (github.com/dldnfl1357/chosun)

---

## 마일스톤 1 — 그리드 + 캐릭터 이동 (목표: 4주)

### 코드
- [x] `Core/Coord.cs` (좌표 구조체)
- [x] `Core/Direction.cs` (8방향)
- [x] `Grid/TerrainType.cs`
- [x] `Grid/IGridOccupant.cs` (인터페이스)
- [x] `Grid/GridCell.cs`
- [x] `Grid/GridConfig.cs` (ScriptableObject + Test Layout ContextMenu)
- [x] `Grid/GridMap.cs`
- [x] `Grid/GridView.cs` (Tilemap 렌더)
- [x] `Grid/GridInput.cs` (터치+마우스, 탭/드래그 구분)
- [x] `Pathfinding/SimplePriorityQueue.cs`
- [x] `Pathfinding/AStarPathfinder.cs`
- [x] `Characters/CharacterEntity.cs`
- [x] `Characters/CharacterMover.cs` (코루틴 보간)
- [x] `Camera/CameraController.cs` (핀치 줌 + 두 손가락 팬)
- [x] `Demo/M1DemoController.cs` (씬 진입점)

### 자산
- [ ] 플레이스홀더 스프라이트 5종 (grass/forest/water/wall/character PNG)
- [ ] 각 스프라이트 import 설정 (Point filter, Pixels Per Unit 16)
- [ ] Tile 자산 4종 (GrassTile/ForestTile/WaterTile/WallTile)
- [ ] `Assets/Data/GridConfigs/M1TestGrid.asset` 생성
- [ ] M1TestGrid Generate Test Layout 8x6 실행

### 씬 구성
- [ ] `Assets/Scenes/M1Demo.unity` 새 씬 생성
- [ ] Main Camera 설정 (Orthographic, Size 4, Position 3.5/2.5/-10)
- [ ] Main Camera에 CameraController 컴포넌트
- [ ] Tilemap GameObject 생성 (Grid + Tilemap 자식)
- [ ] Grid GameObject에 GridMap + GridView + GridInput 컴포넌트
- [ ] 각 컴포넌트 Inspector 참조 연결 (Config, Tilemap, Cam, Tile 4개)
- [ ] TestCharacter GameObject 생성 (SpriteRenderer + CharacterEntity + CharacterMover)
- [ ] TestCharacter 초기 Position (4, 3)
- [ ] GameController GameObject + M1DemoController 컴포넌트
- [ ] M1DemoController에 Map/Input/Character 참조 연결
- [ ] Ctrl+S로 씬 저장

### 검증
- [ ] 에디터 ▶ Play — 8×6 그리드 표시
- [ ] 풀밭 칸 탭 → 캐릭터 이동
- [ ] 숲 칸 탭 → 이동 (느림)
- [ ] 물 칸 탭 → 이동 안 함 또는 우회
- [ ] 벽으로 둘러싸인 칸 탭 → 경로 없음, 콘솔 메시지
- [ ] 이동 중 탭 → 무시
- [ ] 마우스 휠 → 줌

### Android 빌드
- [ ] File → Build Settings → Add Open Scenes (M1Demo)
- [ ] Texture Compression: ASTC
- [ ] Build APK 성공 (시간 ~5–15분)
- [ ] APK 실기 설치
- [ ] 실기에서 가로 모드 + 그리드 + 캐릭터 표시
- [ ] 탭으로 이동
- [ ] 두 손가락 핀치 → 줌
- [ ] 두 손가락 드래그 → 카메라 팬
- [ ] 회전해도 가로 유지
- [ ] 60FPS 유지 (Android Profiler)
- [ ] 메모리 100MB 이하

---

## 마일스톤 2 — 틱 시뮬레이션 + 일시정지 (목표: 1개월)

### 코드
- [ ] `Combat/Tick.cs` (틱 단위)
- [ ] `Combat/TickScheduler.cs` (틱 진행, 일시정지, 속도 1x/2x/4x)
- [ ] `Combat/Action.cs` (행동 캐스트타임)
- [ ] `Combat/ICombatant.cs` (HP, Acc, Eva, 등)
- [ ] `Combat/BasicAttack.cs` (평타 스킬)

### UI
- [ ] 일시정지 버튼 (우측 상단)
- [ ] 속도 토글 (1x/2x/4x)
- [ ] HP 바 (캐릭터 머리 위 또는 화면 하단)

### 씬
- [ ] `M2Demo.unity` — 1대1 데모
- [ ] 평타로 HP 깎이고 다운 가능

---

## 마일스톤 3 — 감빗 시스템 + 다수 캐릭터 (목표: 1개월)

- [ ] `AI/GambitRule.cs` (조건 + 행동)
- [ ] `AI/GambitCondition.cs` (HP %, 거리, 버프 보유 등)
- [ ] `AI/GambitEvaluator.cs` (우선순위 룰 평가)
- [ ] `AI/GambitSlot.cs` (캐릭터당 5개)
- [ ] 4 vs 4 자동 전투 동작
- [ ] 감빗 편집 UI (한양 화면용 별도 작업)

---

## 마일스톤 4 — 스킬 시스템 + 수동 발동 (목표: 1개월)

- [ ] `Combat/Skills/SkillDef.cs` (ScriptableObject)
- [ ] `Combat/Skills/SkillType.cs` (Auto/Manual)
- [ ] `Combat/Skills/RangePattern.cs` (Self/Melee/Tile/Line/Cone/AoE)
- [ ] `Combat/Skills/Effect.cs` (피해/회복/버프/디버프/이동/상태)
- [ ] `Combat/Skills/SkillResolver.cs` (스킬 시전 처리)
- [ ] `UI/SkillBar.cs` (화면 하단 스킬 바)
- [ ] 캐릭터 탭 → 스킬 버튼 부각 → 대상 셀 탭

---

## 마일스톤 5–6 — 오버맵 + 한양 허브 + 의뢰 보드 (목표: 2개월)

- [ ] `Hub/HubScene.cs` (한양 화면 진입점)
- [ ] `Hub/ContractBoard.cs` (의뢰 게시판)
- [ ] `Hub/MercenaryHall.cs` (용병 회관)
- [ ] `Hub/Inventory.cs` (장비/소지품)
- [ ] `Overworld/MapNode.cs` (지도 노드)
- [ ] `Overworld/PartyToken.cs` (파티 위치)
- [ ] `Overworld/Encounter.cs` (인카운터 트리거)
- [ ] 한양 → 의뢰 수락 → 노드 이동 → 전투 → 복귀 풀 사이클 동작 (단일 캐릭터)
- [ ] 한 사이클 5–10분 유지 확인

---

## 마일스톤 7 — 캐릭터 10명 데이터 + 영입/사기/회복 (목표: 1개월)

- [ ] `Characters/CharacterDef.cs` (ScriptableObject)
- [ ] 9 용병 + 주인공 ScriptableObject 자산 작성
- [ ] `Characters/Roster.cs` (보유 용병 관리)
- [ ] `Characters/Morale.cs` (사기 시스템)
- [ ] `Characters/RecoverySchedule.cs` (회복 기간)
- [ ] `Hub/RecruitmentUI.cs` (영입 화면)
- [ ] `Hub/PartySelectionUI.cs` (의뢰 출전 편성 — 5명 선발)

---

## 마일스톤 8 — 몬스터 8종 + 의뢰 25개 (목표: 1개월)

- [ ] `Combat/MonsterDef.cs`
- [ ] 8 몬스터 ScriptableObject 자산
- [ ] 8 몬스터 고유 메커닉 구현 (들도깨비 분산, 구미호 환영, 손각시 물리 면역 등)
- [ ] `Contracts/ContractDef.cs`
- [ ] `Contracts/ContractType.cs` (Kill/Escort/Search/Main/Generated)
- [ ] `Contracts/GeneratedContractTemplate.cs`
- [ ] 25개 의뢰 ScriptableObject 자산 (고유 10 + 호위 3 + 수색 2 + 메인 4 + 절차생성 8템플릿)

---

## 마일스톤 9 — 임진왜란 트리거 + 변화 (목표: 1개월)

- [ ] `World/Calendar.cs` (게임 시간)
- [ ] `World/WorldState.cs` (글로벌 상태, ImjinTriggered 등)
- [ ] `World/ImjinTrigger.cs` (메인 24 완료 시 발동)
- [ ] `World/ImjinEventDef.cs` (일본 용병 4 이벤트)
- [ ] 컷씬 「봉화의 밤」
- [ ] NPC 반응 변경
- [ ] 의뢰 보드 변화 (사라짐/등장)
- [ ] 지역 접근 잠금 (동래/부산)
- [ ] BGM 변경 시스템

---

## 마일스톤 10 — UI/UX 폴리시 + 튜토리얼 + AdMob (목표: 1개월)

- [ ] 튜토리얼 시퀀스 (의뢰 1 「청송 산적단 소탕」)
- [ ] UI 폴리시 (애니메이션, 사운드, 트랜지션)
- [ ] 한국어 UI 폰트 (예: 본명조)
- [ ] AdMob SDK 통합
- [ ] 인터스티셜 광고 (의뢰 3판마다)
- [ ] 보상 광고 — 보수 2배
- [ ] 보상 광고 — 무료 부활
- [ ] 보상 광고 — 회복 가속

---

## 마일스톤 11 — 밸런싱 + 버그 + 베타 + IAP (목표: 1개월)

- [ ] 전투 밸런스 (각 의뢰 클리어 가능 확인)
- [ ] Unity IAP 통합
- [ ] 광고 제거 IAP (5,500원)
- [ ] 베타 빌드 → 외부 5~10명 테스트
- [ ] 베타 피드백 반영
- [ ] 크래시 보고 (Crashlytics 또는 Sentry, 선택)

---

## 마일스톤 12 — 마무리·출시 (목표: 1개월)

- [ ] Google Play Console 계정 등록 ($25)
- [ ] 앱 정보 등록 (스크린샷, 설명, 아이콘, 권한 등)
- [ ] 개인정보처리방침 페이지 (필수)
- [ ] AdMob 광고 ID 운영용으로 교체
- [ ] Signed AAB (Android App Bundle) 빌드
- [ ] Internal Testing 트랙 출시
- [ ] (선택) Closed Testing → Open Testing → 정식 출시

---

## Out of Scope (Post-MVP 후보)

- [ ] 명·일본 플레이 지역
- [ ] iOS 출시
- [ ] 데스크탑 (Steam) 포팅
- [ ] 클라우드 저장 (Google Play Games Services)
- [ ] 다국어 (영어, 중국어, 일본어)
- [ ] 배틀패스/시즌제
- [ ] 가챠/캐릭터 코스튬

---

## 범례

- `[x]` 완료
- `[ ]` 미완료
- 굵게 표시한 사용자 작업은 Unity 에디터 GUI 필요 (자동화 불가)
