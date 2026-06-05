# Work Log

작업 일지. 최신이 위.

---

## 2026-06-06 (금)

### 작업
- Unity Hub에서 Android Build Support 모듈 설치 안내 (OpenJDK + SDK & NDK Tools 포함)
- Build Profiles/Build Settings 메뉴 위치 안내 (Unity 6 메뉴 차이)
- 사용자가 Android 플랫폼 전환 완료 — Build 버튼이 Build And Run으로 표시 확인
- Unity Hub가 `C:\projects\roller\My project\` 안에 별도 프로젝트로 생성한 문제 해결
  - `My project/ProjectSettings/`, `Packages/`, `UserSettings/` → `C:\projects\roller\` 루트로 이동
  - `My project/Assets/*` (Scenes/SampleScene, Settings/, TutorialInfo/, InputSystem_Actions, Readme) → `C:\projects\roller\Assets\`로 병합
  - `My project/` 폴더 제거
  - 우리 `Assets/Scripts/`와 충돌 없음 (Scenes는 우리 쪽이 비어있어 그대로 흡수)
- 작업 진행도 추적용 `worklog.md`, `checklist.md` 추가

### 결과
- `C:\projects\roller\`가 단일 Unity 프로젝트 루트로 동작 가능 상태
- Android 빌드 타겟 활성화
- 통합 후 첫 커밋: Unity 자동 생성 파일 (ProjectSettings/, Packages/, Assets/Settings/, SampleScene 등)
- 작업 추적 문서 커밋
- `github.com/dldnfl1357/chosun.git` 원격 추가 + push

### 다음
- 사용자: Unity Hub에서 `C:\projects\roller` 열어 Library/ 재생성 + 콘솔 에러 없음 확인
- 사용자: Player Settings 가로 락 + Package Name + IL2CPP + Min API 24 설정 (SETUP.md §2)
- 사용자: 플레이스홀더 스프라이트 5종 만들기 (그림판 등) → Tile 자산 4종 생성
- 사용자: GridConfig 자산 생성 + Generate Test Layout 실행
- 사용자: M1Demo 씬 만들고 GameObject 4개 배치
- 사용자: ▶ Play → 8×6 그리드에서 캐릭터 탭 이동 검증

---

## 2026-06-05 (목)

### 작업

#### 게임 디자인 브레인스토밍
- 콘셉트 결정: 16세기 동아시아 (조선/명/일본) 다크 판타지 용병 RPG
- 핵심 결정 사항:
  - 장르: 2D 그리드 RTwP(실시간 일시정지) 파티 전술 RPG (Battle Brothers + Pillars + FF12 감빗)
  - 전투: 자율 전투 + 일시정지 시 명령 (감빗 시스템)
  - 주인공: 조선/명/일본 × 남/녀 = 6택, 클래스는 검사 1종
  - 용병: 9명 고정 명자 (3국 × 3직업), 영구 보존, 다운만(영구사망 없음)
  - 세계관: 위쳐 톤 다크 판타지, 임진왜란이 중반 분기점
  - 톤: 역사적 판타지 (실제 지리 + 설화 요괴)
  - 전체 구조: Battle Brothers 스타일 허브+오버맵+의뢰
- MVP 범위: 조선 지역만 (한양 허브 + 8개 노드), 3국 주인공 선택 가능

#### 설계 문서 작성 (`docs/design/`)
- `mercenaries.md` — 용병 9명 풀 디자인 (이름·출신·배경·고유 패시브·고용 조건·임진왜란 분기)
- `monsters.md` — 몬스터 8종 (설화 근거·전투 행동·고유 메커닉·전술적 해답·의뢰 매핑)
- `skills.md` — 스킬 시스템 + 9명+주인공 키트 (자동 3 + 수동 2) + 시너지 콤보 8개 + 기본 감빗
- `contracts.md` — 의뢰 25개 (입문/고유/호위/수색/절차생성 8템플릿/메인) + 평판 게이팅
- `imjin-war.md` — 임진왜란 트리거·컷씬·세계 상태 전환·일본 용병 4 이벤트·메인 25·엔딩 6분기
- `milestone-1-implementation.md` — M1 (그리드 + 캐릭터 이동) 상세 구현 가이드
- `README.md` — 디자인 문서 인덱스

#### 모바일 피벗
- 사용자 결정: 데스크탑 → 모바일 (Android 우선, 가로), 5–10분 세션, F2P + 광고, 전투 규모 축소 (시스템 유지)
- 영향 받은 변경:
  - 그리드 16×12 → 8×6
  - 파티 8명 → 5명 (주인공 + 용병 4)
  - 의뢰 20–40분 → 5–10분 (단일 전투 기본, 다단계는 ★ 표기)
  - 마우스 → 터치 (탭, 핀치 줌, 두 손가락 팬)
  - 메인 25 3-스테이지 1의뢰 → 25-A/B/C 3개 의뢰 분할
  - 광고: 인터스티셜 (3판마다), 보상 광고 (보수 2배, 무료 부활), 광고 제거 IAP 5,500원
  - 에너지 시스템 없음 (광고가 자연 페이스 조절)
- 모든 설계 문서 + `16-structured-karp.md` 플랜 파일 모바일에 맞춰 업데이트

#### M1 코드 스캐폴드 작성
- `Assets/Scripts/Core/` — Coord, Direction (8방향 enum)
- `Assets/Scripts/Grid/` — TerrainType, IGridOccupant, GridCell, GridConfig (8×6 기본 + 테스트 레이아웃 ContextMenu), GridMap, GridView (Tilemap 기반), GridInput (터치+마우스 통합)
- `Assets/Scripts/Pathfinding/` — SimplePriorityQueue (자체 구현), AStarPathfinder (옥타일 거리 휴리스틱, safety counter)
- `Assets/Scripts/Characters/` — CharacterEntity (IGridOccupant 구현), CharacterMover (코루틴 보간 이동)
- `Assets/Scripts/Camera/` — CameraController (핀치 줌 + 두 손가락 팬 + 에디터 마우스 휠)
- `Assets/Scripts/Demo/` — M1DemoController (씬 진입점, 60FPS 고정)
- 총 15개 .cs 파일

#### 프로젝트 셋업 문서
- 루트 `README.md` — 프로젝트 개요
- 루트 `SETUP.md` — Unity GUI 셋업 단계별 가이드 (설치부터 Android 빌드까지)
- 루트 `.gitignore` — Unity 표준 + 시크릿 보호 (CLAUDE.md, *.token, .env)
- `git init -b main` + 첫 커밋 (`6bac80f`)

### 보안 이슈 처리
- `CLAUDE.md`에 GitHub 토큰 평문 저장된 것 발견 → `.gitignore`에 추가 + 커밋에서 제외
- 사용자에게 토큰 재발급 권고 (이미 외부 공유했다면)

### 결과
- 디자인 문서 6개 (모바일 컨셉으로 정렬)
- M1 코드 스캐폴드 완성 (Unity Hub로 프로젝트 만들면 바로 컴파일 가능)
- 첫 커밋 25 files, 3245 insertions
