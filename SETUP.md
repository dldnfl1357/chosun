# Unity 프로젝트 셋업 가이드 (마일스톤 1)

이 문서는 코드는 이미 작성된 상태에서 **사용자가 Unity GUI에서 해야 할 작업**을 단계별로 안내한다.

---

## 0. 사전 준비

### Unity 설치 (한 번만)
1. https://unity.com/download → Unity Hub 다운로드 및 설치
2. Unity Hub 실행 → Installs 탭 → Install Editor
3. **Unity 6 LTS** (또는 Unity 6000.0.x) 선택
4. 모듈 선택 (필수):
   - [x] Android Build Support
     - [x] OpenJDK
     - [x] Android SDK & NDK Tools
   - [x] Documentation (선택, 권장)
   - [x] Universal Windows Platform (선택, 데스크탑 빌드용)
5. 설치 시간 ~30분

### Visual Studio 또는 Rider
- Visual Studio 2022 Community (무료) — Unity Hub 설치 중 같이 설치 가능
- 또는 JetBrains Rider (유료, 학생 무료)

### Git 설치
- 이미 설치되어 있음 (`git --version`으로 확인)
- 없으면 https://git-scm.com 에서 설치

### Android 실기 (선택, 권장)
- Android 7.0+ 폰
- 설정 → 휴대전화 정보 → 빌드 번호 7회 탭 → 개발자 옵션 활성화
- 설정 → 개발자 옵션 → USB 디버깅 ON
- USB로 PC 연결

---

## 1. Unity 프로젝트 생성

⚠ **중요**: `C:\projects\roller` 폴더에는 이미 `Assets/Scripts/`, `docs/`, `.gitignore` 등이 있다. Unity Hub의 「New Project」는 이 폴더를 알아서 인식한다.

1. Unity Hub → Projects → **New Project**
2. Template: **Universal 2D** 또는 **2D (URP)**
3. Project Name: `roller`
4. Location: `C:\projects\`  (즉 `C:\projects\roller` 가 생성됨)
5. Unity Account 로그인 + 「Create Project」
6. Unity가 열리면서 `Assets/Scripts/`의 스크립트들이 자동으로 import 된다
7. Console 창에 컴파일 에러가 없는지 확인

### 만약 Unity Hub가 「폴더가 비어있지 않다」고 거절하면:

대안 A — 다른 곳에 새 프로젝트 만들고 옮기기:
1. `C:\temp\roller-empty\`에 새 프로젝트 생성
2. 생성된 `Library/`, `Packages/`, `ProjectSettings/`만 `C:\projects\roller\` 로 이동
3. `C:\projects\roller`에서 Unity Hub → Add → Open Project

대안 B — 새 프로젝트에 우리 코드 복사:
1. `C:\projects\roller`의 `Assets/`, `docs/`, `.gitignore`, `README.md`, `SETUP.md` 를 임시 폴더로 백업
2. 새 위치(`C:\projects\roller-v2`)에 Unity 프로젝트 생성
3. 백업한 파일들을 새 위치 안에 덮어쓰기 (Library 등은 건드리지 않음)

---

## 2. Project Settings 구성

Edit → Project Settings

### Player
- **Company Name**: 본인 또는 스튜디오 명
- **Product Name**: roller
- **Package Name** (Android): `com.{본인}.roller` — 영문 소문자만
- **Default Icon**: 일단 빈 칸 (M1엔 OK)
- **Resolution and Presentation**:
  - Default Orientation: **Landscape Left**
  - Allowed Orientations:
    - [x] Landscape Left
    - [x] Landscape Right
    - [ ] Portrait (해제)
    - [ ] Portrait Upside Down (해제)
- **Other Settings**:
  - Color Space: **Linear** (권장, 모바일은 Linear 지원 기기 많음)
  - Auto Graphics API: ON (자동 선택, 첫 빌드엔 권장)
  - Minimum API Level: **24 (Android 7.0)**
  - Target API Level: Automatic (Highest installed)
  - Scripting Backend: **IL2CPP**
  - Api Compatibility Level: **.NET Standard 2.1**
  - Target Architectures: [x] ARM64 ([ ] ARMv7 무시 가능)

### Quality
- Android 항목 선택 → Anti Aliasing: Disabled
- V Sync Count: Don't Sync

---

## 3. Build Settings → Android

File → Build Settings
1. Platform 리스트에서 **Android** 선택
2. **Switch Platform** 클릭
3. 전환 완료까지 대기 (수분 소요, 첫 회만)
4. (선택) 「Build And Run」으로 실기에 즉시 설치

---

## 4. 플레이스홀더 스프라이트 만들기

`Assets/Art/Sprites/placeholder/` 폴더에 16×16 픽셀 단색 PNG 5개 필요.

### 옵션 A — Paint/Photoshop으로 만들기

각 색상별 16×16 PNG:
- `grass.png` — 녹색 (예: `#4CAF50`)
- `forest.png` — 짙은 녹색 (예: `#1B5E20`)
- `water.png` — 파랑 (예: `#2196F3`)
- `wall.png` — 회색 (예: `#616161`)
- `character.png` — 빨강 (예: `#E53935`) — 캐릭터 표시용 32×32 권장

### 옵션 B — 온라인 생성기

https://www.pixilart.com 또는 https://www.piskelapp.com 에서 16×16 캔버스 단색 색상 추출.

### Sprite Import 설정

각 PNG를 Unity에 드래그한 뒤:
- Texture Type: **Sprite (2D and UI)**
- Pixels Per Unit: **16** (16px = 1 칸)
- Filter Mode: **Point (no filter)** ← 픽셀 아트 필수
- Compression: **None**
- Apply

### Tile Asset 만들기 (4개)

각 지형 스프라이트를 Tile로 변환:
1. Project 창에서 `grass.png` 우클릭 → Create → 2D → Tiles → **Tile**
2. 생성된 Tile 자산 이름: `GrassTile`
3. Inspector에서 Sprite 필드에 `grass`를 할당
4. 같은 방식으로 ForestTile, WaterTile, WallTile 생성

---

## 5. GridConfig 생성

1. `Assets/Data/GridConfigs/` 폴더 우클릭 → Create → Roller → **Grid Config**
2. 파일 이름: `M1TestGrid`
3. Inspector에서:
   - Width: 8
   - Height: 6
   - Cell Size: 1
4. **우클릭 → Generate Test Layout 8x6** (테스트용 장애물 자동 생성)

---

## 6. M1 Demo 씬 만들기

1. File → New Scene → 2D (URP) 또는 그냥 Empty
2. File → Save As → `Assets/Scenes/M1Demo.unity`

### 카메라 셋업
1. Main Camera 선택
2. Inspector:
   - Projection: **Orthographic**
   - Size: 4
   - Position: (3.5, 2.5, -10) — 그리드(0~7, 0~5)의 중심
   - Background: 어두운 회색 또는 검정 (#222)
3. CameraController 컴포넌트 추가:
   - Add Component → Scripts → Roller → Camera Ctrl → **Camera Controller**

### Game 뷰 해상도
1. Game 탭 상단 드롭다운에서 + → New Resolution
   - Label: Landscape 1920×1080
   - Width 1920, Height 1080
2. 적용

### 그리드 GameObject
1. Hierarchy 우클릭 → 2D Object → Tilemap → **Rectangular**
2. 자동 생성된 Grid GameObject 선택
3. Cell Size: (1, 1, 0) 확인
4. Add Component:
   - **Grid Map**: Config = `M1TestGrid`
   - **Grid View**: Tilemap = 자식 Tilemap, 각 Tile 필드에 해당 Tile 자산 할당
   - **Grid Input**: Cam = Main Camera

### Character GameObject
1. Hierarchy 우클릭 → Create Empty → 이름 `TestCharacter`
2. SpriteRenderer 추가 → Sprite = `character.png`
3. Order in Layer: 5 (타일 위에 보이게)
4. Position: (4, 3, 0) — 그리드 중앙 부근
5. Add Component:
   - **Character Entity**: Position = (4, 3), Map = (Hierarchy의 Grid)
   - **Character Mover**: Speed = 4

### Demo Controller
1. Hierarchy → Create Empty → 이름 `GameController`
2. Add Component → **M1 Demo Controller**
3. Inspector에서 참조 연결:
   - Map → Grid
   - Input → Grid (같은 객체)
   - Character → TestCharacter

### 저장
- Ctrl+S로 씬 저장

---

## 7. 에디터에서 실행 (Play)

1. Hierarchy 가 위 구성과 같은지 확인:
   - Main Camera (CameraController 포함)
   - Grid (GridMap + GridView + GridInput)
     - Tilemap (자식)
   - TestCharacter (CharacterEntity + CharacterMover + SpriteRenderer)
   - GameController (M1DemoController)
2. Game 뷰 해상도 1920×1080 가로 선택
3. **▶ Play 버튼**
4. 확인:
   - 8×6 그리드가 화면에 표시됨
   - 풀밭(녹), 숲(짙은녹), 물(파랑), 벽(회색) 구분 가능
   - 빨간 캐릭터가 (4, 3)에 있음
   - 빈 풀밭 칸 클릭 → 캐릭터가 이동
   - 물 칸 클릭 → 캐릭터가 우회 또는 이동 불가
   - 벽 둘러싸인 칸 클릭 → 「Path not found」 콘솔 메시지

---

## 8. Android 빌드 & 실기 검증

### 빌드
1. File → Build Settings
2. **Add Open Scenes**: M1Demo.unity 추가
3. Platform: Android (이미 선택됨)
4. Texture Compression: **ASTC** (권장, 호환성 좋음)
5. **Build** 클릭 (또는 USB 연결 후 Build And Run)
6. 저장 위치: `C:\projects\roller\Build\roller-debug.apk`
7. 빌드 시간 ~5–15분 (첫 빌드, IL2CPP 컴파일)

### 실기 설치
- Build And Run 시 자동 설치
- 수동: APK 파일 → 폰에 옮김 → 설치 (개발자 옵션 → 「알 수 없는 소스」 허용 필요)

### 확인
- 앱 실행 → 가로 모드 + 그리드 + 캐릭터
- 빈 칸 탭 → 이동
- 두 손가락 핀치 → 줌
- 두 손가락 드래그 → 카메라 팬
- 회전 시도해도 가로 유지

---

## 9. 검증 체크리스트

마일스톤 1 통과 조건 (모두 체크되어야 함):

**에디터**
- [ ] M1Demo 씬 Play 시 콘솔 에러 없음
- [ ] 8×6 그리드 표시
- [ ] 캐릭터 표시
- [ ] 풀밭 클릭 → 이동
- [ ] 물 클릭 → 이동 안 함 또는 우회
- [ ] 벽 둘러싸인 칸 클릭 → 경로 없음

**Android 실기**
- [ ] APK 빌드 성공
- [ ] 폰에 설치 성공
- [ ] 가로 모드 강제
- [ ] 탭으로 이동
- [ ] 핀치 줌 / 두 손가락 팬
- [ ] 60FPS 유지 (Android Profiler 또는 GPU Watch)

---

## 10. 문제 해결

### 컴파일 에러 「Coord namespace not found」
- Console에서 정확한 에러 위치 확인
- 스크립트의 `using` 문이 올바른지 확인
- Assets 폴더 우클릭 → Reimport All

### Tilemap이 그려지지 않음
- GridView의 Tilemap 필드가 자식 Tilemap에 연결되었는지 확인
- Tile 자산이 실제로 Tile 타입인지 (Texture가 아닌)

### NullReferenceException
- M1DemoController에서 Map/Input/Character 참조 모두 연결되었는지
- 「\[GridMap\] Config is null」 에러: GridMap의 Config 필드 = M1TestGrid

### Android 빌드 실패 (Gradle)
- Player Settings → Publishing Settings → Custom Main Gradle Template ON
- File → Build Settings → Refresh
- 그래도 안 되면 Edit → Preferences → External Tools → JDK / SDK / NDK 경로 「Use embedded」 체크

### 실기에서 검은 화면 + 즉시 종료
- Android Logcat 창 열기 (Window → Analysis → Android Logcat)
- 폰 연결 → 앱 실행 → 에러 메시지 확인 (대부분 NullRef)

### 핀치/팬 너무 빠르거나 느림
- CameraController Inspector의 PinchSensitivity / TwoFingerPanSensitivity 조정

---

## 다음 단계 (M2)

마일스톤 1 통과 후:
- 틱 스케줄러 (`Combat/TickScheduler`)
- 일시정지/속도 토글 UI
- 평타 스킬 (단일 타겟 피해)
- HP 바 UI
- 1대1 데모 전투 씬

별도 문서로 안내 예정.
