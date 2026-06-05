# 마일스톤 1 구현 계획서 — 그리드 시스템 + 캐릭터 이동 (모바일)

## 목표

**Unity 학습 완료 + 1개 캐릭터가 그리드 위 탭으로 이동. Android 실기에서 동작.**

검증 기준 (마일스톤 통과 조건):
- Unity 에디터에서 데모 씬을 열고 실행 (가로 모드 해상도 1920×1080)
- 8×6 그리드가 화면 가운데에 표시됨
- 캐릭터 스프라이트 1개가 그리드 위 한 칸에 있음
- 빈 칸을 **터치/탭**하면 캐릭터가 그 칸까지 자동 경로로 부드럽게 이동
- 벽/물 칸은 통과 불가, 우회 경로 자동 생성
- 핀치 줌, 두 손가락 드래그 팬 동작
- **Android 빌드 → 실기에서 동일하게 동작**

이 문서는 한 달 내 위 검증 기준을 만족하는 작업 계획.

---

## 1. 사전 준비

### 1.1 Unity 설치

- **Unity Hub** 설치 (unity.com/download)
- **Unity 6 LTS** (현재 최신 안정판, Unity 6000.0.x) 설치
  - Universal Render Pipeline (URP) 모듈 포함
  - **Android Build Support** 모듈 (필수)
    - OpenJDK + Android SDK & NDK + Gradle 모두 체크
- **Visual Studio 2022 Community** 또는 **JetBrains Rider** (선호)
  - Unity 통합을 위해 「Game development with Unity」 워크로드 설치

### 1.1.1 Android 개발 환경

- **Android 실기** 1대 권장 (Android 10+ 추천)
- **USB 디버깅 활성화**:
  - 설정 → 휴대전화 정보 → 빌드 번호 7회 탭 (개발자 옵션 활성화)
  - 설정 → 개발자 옵션 → USB 디버깅 ON
- USB 케이블로 PC 연결 → Unity가 자동 인식 (또는 ADB로 확인)
- 실기 없으면 **Android Studio 에뮬레이터** 사용 (느림 주의)

### 1.2 Unity Learn 학습 (3~5일)

세 개 완주 권장:

1. **Roll-a-Ball Tutorial** (3시간) — 기본 GameObject, Component, MonoBehaviour, Input 이해
2. **2D Beginner: Adventure Game** (4시간) — Tilemap, Sprite, Camera, 2D 충돌 이해
3. **Mobile Notifications & Build** 가이드 훑기 (1시간) — Android 빌드 절차, Player Settings 핵심
   - 또는 「First Android Build in Unity」 YouTube 튜토리얼 (Brackeys 등)

빠르게 훑어도 됨. 익숙해진 뒤 다음 단계.

### 1.3 프로젝트 생성

- Unity Hub → New Project
- Template: **2D (URP)** (Mobile 2D 템플릿은 일부 Unity 버전에만 존재. 2D URP면 충분)
- Name: `roller`
- Location: `C:\projects\` (즉 `C:\projects\roller`로 생성)
- 「Create project」

**프로젝트 생성 직후 초기 설정**:

1. **Build Target → Android**
   - File → Build Settings → Platform: Android → Switch Platform
2. **Player Settings → Resolution and Presentation**
   - Default Orientation: **Landscape Left** (가로 고정, 회전 차단)
   - Allowed Orientations: Landscape Left + Landscape Right만 체크
3. **Player Settings → Other Settings**
   - Package Name: `com.{개인}.roller`
   - Minimum API Level: **24 (Android 7.0)** (현재 90%+ 기기 커버)
   - Target API Level: Automatic (Highest installed)
   - Scripting Backend: **IL2CPP** (Google Play 64bit 요구사항)
   - Target Architectures: **ARM64** 체크 (ARMv7 추가 가능)
4. **Player Settings → Splash Image**
   - Unity 로고 표시 (개인 무료 라이선스)
5. **Quality Settings**
   - Android 기본 품질에서 Anti Aliasing OFF (모바일 성능)
   - V Sync Count: Don't Sync (대신 Application.targetFrameRate로 60 고정 예정)

### 1.4 Git 초기화

```powershell
cd C:\projects\roller
git init
```

`.gitignore` 작성:
```gitignore
# Unity standard ignores
[Ll]ibrary/
[Tt]emp/
[Oo]bj/
[Bb]uild/
[Bb]uilds/
[Ll]ogs/
[Mm]emoryCaptures/
[Uu]ser[Ss]ettings/
*.csproj
*.unityproj
*.sln
*.suo
*.tmp
*.user
*.userprefs
*.pidb
*.booproj
*.svd
*.pdb
*.mdb
*.opendb
*.VC.db
sysinfo.txt
*.apk
*.aab
*.unitypackage
*.app
.vs/
.idea/
```

(선택) **Git LFS 설정** — 아트 에셋이 늘어나면 필수:
```powershell
git lfs install
git lfs track "*.png" "*.psd" "*.fbx" "*.wav" "*.mp3" "*.ogg"
git add .gitattributes
```

### 1.5 폴더 구조 생성 (Unity 에디터에서)

```
Assets/
  Scripts/
    Core/
    Grid/
    Pathfinding/
    Characters/
    Camera/
    Demo/
  Scenes/
  Data/
    GridConfigs/
  Art/
    Sprites/
      placeholder/
```

`Assets/Scripts/` 안의 각 하위 폴더에는 빈 `_keep.cs` 또는 `README.txt`를 두어 Git에 폴더가 포함되도록 함 (Unity는 빈 폴더 .meta를 생성하지 않음).

---

## 2. 핵심 데이터 구조

### 2.1 `Coord` (좌표 구조체)

`Assets/Scripts/Core/Coord.cs`:

```csharp
using System;

namespace Roller.Core {
    [Serializable]
    public readonly struct Coord : IEquatable<Coord> {
        public readonly int X;
        public readonly int Y;
        public Coord(int x, int y) { X = x; Y = y; }

        public static Coord Zero => new(0, 0);
        public static Coord operator +(Coord a, Coord b) => new(a.X + b.X, a.Y + b.Y);
        public static Coord operator -(Coord a, Coord b) => new(a.X - b.X, a.Y - b.Y);

        // 8방향 인접 거리 (체비셰프)
        public int ChebyshevTo(Coord other) =>
            Math.Max(Math.Abs(X - other.X), Math.Abs(Y - other.Y));

        public bool Equals(Coord other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is Coord c && Equals(c);
        public override int GetHashCode() => (X << 16) | (Y & 0xFFFF);
        public override string ToString() => $"({X},{Y})";
    }
}
```

### 2.2 `Direction` (8방향 enum)

`Assets/Scripts/Core/Direction.cs`:

```csharp
using System.Collections.Generic;

namespace Roller.Core {
    public enum Direction {
        N, NE, E, SE, S, SW, W, NW
    }

    public static class Directions {
        public static readonly Coord[] Offsets = new[] {
            new Coord( 0,  1), new Coord( 1,  1), new Coord( 1,  0), new Coord( 1, -1),
            new Coord( 0, -1), new Coord(-1, -1), new Coord(-1,  0), new Coord(-1,  1),
        };

        public static IEnumerable<Coord> Neighbors(Coord origin) {
            foreach (var offset in Offsets)
                yield return origin + offset;
        }
    }
}
```

### 2.3 `TerrainType` (지형 enum)

`Assets/Scripts/Grid/TerrainType.cs`:

```csharp
namespace Roller.Grid {
    public enum TerrainType {
        Grass,   // 통과 가능, 이동 비용 10
        Forest,  // 통과 가능, 이동 비용 15, 시야 차단(추후)
        Water,   // 통과 불가
        Wall     // 통과 불가
    }

    public static class TerrainExtensions {
        public static bool IsPassable(this TerrainType t) =>
            t == TerrainType.Grass || t == TerrainType.Forest;

        public static int MoveCost(this TerrainType t) => t switch {
            TerrainType.Grass => 10,
            TerrainType.Forest => 15,
            _ => int.MaxValue
        };
    }
}
```

### 2.4 `GridCell` (셀 데이터)

`Assets/Scripts/Grid/GridCell.cs`:

```csharp
using Roller.Core;

namespace Roller.Grid {
    public class GridCell {
        public Coord Position;
        public TerrainType Terrain;
        public CharacterEntity Occupant; // 임시 참조, 추후 ICombatant로 변경

        public bool IsPassable => Terrain.IsPassable() && Occupant == null;
        public int MoveCost => Terrain.MoveCost();
    }
}
```

### 2.5 `GridConfig` (ScriptableObject로 그리드 정의)

`Assets/Scripts/Grid/GridConfig.cs`:

```csharp
using UnityEngine;

namespace Roller.Grid {
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Roller/Grid Config")]
    public class GridConfig : ScriptableObject {
        public int Width = 8;        // 모바일 가로 8칸
        public int Height = 6;       // 모바일 세로 6칸
        public float CellSize = 1f;  // 월드 단위
        public TerrainType[] InitialTerrain; // 길이 = Width*Height, 행 우선
    }
}
```

> **Unity 팁**: ScriptableObject는 데이터 정의 방식의 핵심. 코드 변경 없이 에디터에서 다양한 그리드 레이아웃을 만들 수 있음.

### 2.6 `GridMap` (그리드 상태 보관 MonoBehaviour)

`Assets/Scripts/Grid/GridMap.cs`:

```csharp
using UnityEngine;
using Roller.Core;

namespace Roller.Grid {
    public class GridMap : MonoBehaviour {
        public GridConfig Config;
        public GridCell[,] Cells { get; private set; }

        void Awake() {
            BuildFromConfig();
        }

        public void BuildFromConfig() {
            Cells = new GridCell[Config.Width, Config.Height];
            for (int y = 0; y < Config.Height; y++) {
                for (int x = 0; x < Config.Width; x++) {
                    int idx = y * Config.Width + x;
                    var terrain = (Config.InitialTerrain != null && idx < Config.InitialTerrain.Length)
                        ? Config.InitialTerrain[idx]
                        : TerrainType.Grass;
                    Cells[x, y] = new GridCell { Position = new Coord(x, y), Terrain = terrain };
                }
            }
        }

        public bool InBounds(Coord c) =>
            c.X >= 0 && c.Y >= 0 && c.X < Config.Width && c.Y < Config.Height;

        public GridCell Get(Coord c) =>
            InBounds(c) ? Cells[c.X, c.Y] : null;

        // 월드 좌표 ↔ 그리드 좌표
        public Vector3 CellToWorld(Coord c) =>
            new(c.X * Config.CellSize, c.Y * Config.CellSize, 0);

        public Coord WorldToCell(Vector3 world) =>
            new(Mathf.RoundToInt(world.x / Config.CellSize),
                Mathf.RoundToInt(world.y / Config.CellSize));
    }
}
```

---

## 3. 그리드 렌더링

### 3.1 접근 방식 선택

**선택**: Unity의 **Tilemap** 컴포넌트 사용. 이유:
- 빌트인 지원, 16×12 그리드에 충분
- 타일 스프라이트 교체로 지형 표현
- 추후 커스텀 메쉬가 필요해지면 교체

### 3.2 데모 씬 셋업 (에디터 작업)

`Assets/Scenes/M1Demo.unity` 생성:

1. **Main Camera**
   - Projection: **Orthographic**
   - Orthographic Size: 4 (8×6 그리드가 화면에 꽉 차게)
   - Position: (3.5, 2.5, -10) — 그리드 중심 정렬 (그리드는 0~7, 0~5 범위)
2. **Game 뷰 해상도 설정**
   - Game 뷰 상단 드롭다운 → +로 새 해상도 추가
   - 「Landscape 1920×1080」 (16:9 기본)
   - 추가로 「Landscape 2400×1080 (19.5:9)」도 등록 (요즘 폰)
3. **Grid GameObject** (메뉴: GameObject → 2D Object → Tilemap → Rectangular)
   - 자동 생성: Grid + Tilemap 자식
   - Cell Size: (1, 1, 0)
4. **GridMap** 컴포넌트를 Grid GameObject에 추가
   - `GridConfig` 참조 연결 (Data/GridConfigs/M1TestGrid.asset 생성 후 할당)
5. **GridView** 컴포넌트 (아래 코드) 동일 GameObject에 추가
6. **임시 타일 스프라이트** 4종 만들기:
   - 16×16 픽셀 단색 PNG: 풀(녹), 숲(짙은 녹), 물(파랑), 벽(회색)
   - `Assets/Art/Sprites/placeholder/` 에 저장
   - Unity Tile Asset 4개 생성 (각 색깔별)
   - Import 설정: Filter Mode = **Point (no filter)**, Compression = None

### 3.3 `GridView.cs`

`Assets/Scripts/Grid/GridView.cs`:

```csharp
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Roller.Grid {
    [RequireComponent(typeof(GridMap))]
    public class GridView : MonoBehaviour {
        public Tilemap Tilemap;
        public TileBase GrassTile;
        public TileBase ForestTile;
        public TileBase WaterTile;
        public TileBase WallTile;

        GridMap _map;

        void Start() {
            _map = GetComponent<GridMap>();
            Render();
        }

        public void Render() {
            for (int y = 0; y < _map.Config.Height; y++) {
                for (int x = 0; x < _map.Config.Width; x++) {
                    var cell = _map.Cells[x, y];
                    Tilemap.SetTile(new Vector3Int(x, y, 0), TileFor(cell.Terrain));
                }
            }
        }

        TileBase TileFor(TerrainType t) => t switch {
            TerrainType.Grass => GrassTile,
            TerrainType.Forest => ForestTile,
            TerrainType.Water => WaterTile,
            TerrainType.Wall => WallTile,
            _ => GrassTile
        };
    }
}
```

---

## 4. A* 패스파인딩

### 4.1 `AStarPathfinder.cs`

`Assets/Scripts/Pathfinding/AStarPathfinder.cs`:

```csharp
using System.Collections.Generic;
using Roller.Core;
using Roller.Grid;

namespace Roller.Pathfinding {
    public static class AStarPathfinder {
        // 카디널 = 10, 대각선 = 14
        const int CardinalCost = 10;
        const int DiagonalCost = 14;

        public static List<Coord> FindPath(GridMap map, Coord start, Coord goal) {
            if (!map.InBounds(start) || !map.InBounds(goal)) return null;
            if (!map.Get(goal).IsPassable && goal != start) return null;

            var open = new PriorityQueue<Coord, int>();
            var cameFrom = new Dictionary<Coord, Coord>();
            var gScore = new Dictionary<Coord, int> { [start] = 0 };

            open.Enqueue(start, Heuristic(start, goal));

            while (open.Count > 0) {
                var current = open.Dequeue();
                if (current.Equals(goal)) return Reconstruct(cameFrom, current);

                foreach (var neighbor in Directions.Neighbors(current)) {
                    if (!map.InBounds(neighbor)) continue;
                    var cell = map.Get(neighbor);
                    if (!cell.IsPassable) continue;

                    bool isDiagonal = (neighbor.X != current.X) && (neighbor.Y != current.Y);
                    int stepCost = (isDiagonal ? DiagonalCost : CardinalCost) * cell.MoveCost / 10;
                    int tentativeG = gScore[current] + stepCost;

                    if (!gScore.TryGetValue(neighbor, out int existingG) || tentativeG < existingG) {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        int f = tentativeG + Heuristic(neighbor, goal);
                        open.Enqueue(neighbor, f);
                    }
                }
            }
            return null; // 경로 없음
        }

        static int Heuristic(Coord a, Coord b) {
            int dx = System.Math.Abs(a.X - b.X);
            int dy = System.Math.Abs(a.Y - b.Y);
            return CardinalCost * (dx + dy) + (DiagonalCost - 2 * CardinalCost) * System.Math.Min(dx, dy);
        }

        static List<Coord> Reconstruct(Dictionary<Coord, Coord> cameFrom, Coord current) {
            var path = new List<Coord> { current };
            while (cameFrom.TryGetValue(current, out var prev)) {
                current = prev;
                path.Add(current);
            }
            path.Reverse();
            return path;
        }
    }
}
```

> **참고**: `PriorityQueue<TElement, TPriority>`는 .NET 6+에 내장. Unity 2022.3+는 .NET Standard 2.1 기반이므로 사용 가능. 만약 Unity 버전 문제로 안 되면 `SortedDictionary` 기반으로 교체 (1시간 작업).

---

## 5. 캐릭터 엔티티 + 이동

### 5.1 `CharacterEntity.cs`

`Assets/Scripts/Characters/CharacterEntity.cs`:

```csharp
using UnityEngine;
using Roller.Core;
using Roller.Grid;

namespace Roller.Characters {
    public class CharacterEntity : MonoBehaviour {
        public Coord Position;
        public GridMap Map;

        public void SnapToGrid() {
            transform.position = Map.CellToWorld(Position);
            Map.Get(Position).Occupant = this;
        }
    }
}
```

### 5.2 `CharacterMover.cs`

`Assets/Scripts/Characters/CharacterMover.cs`:

```csharp
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Roller.Core;
using Roller.Grid;

namespace Roller.Characters {
    [RequireComponent(typeof(CharacterEntity))]
    public class CharacterMover : MonoBehaviour {
        public float SpeedCellsPerSec = 4f;
        public bool IsMoving { get; private set; }

        CharacterEntity _entity;

        void Awake() { _entity = GetComponent<CharacterEntity>(); }

        public IEnumerator FollowPath(List<Coord> path) {
            if (path == null || path.Count < 2) yield break;
            IsMoving = true;

            // 출발 칸에서 occupant 제거
            _entity.Map.Get(_entity.Position).Occupant = null;

            for (int i = 1; i < path.Count; i++) {
                var next = path[i];
                var nextCell = _entity.Map.Get(next);
                if (!nextCell.IsPassable) { IsMoving = false; yield break; } // 도중 막힘

                var startWorld = transform.position;
                var endWorld = _entity.Map.CellToWorld(next);
                float t = 0;
                float duration = 1f / SpeedCellsPerSec;
                while (t < duration) {
                    t += Time.deltaTime;
                    transform.position = Vector3.Lerp(startWorld, endWorld, t / duration);
                    yield return null;
                }
                transform.position = endWorld;
                _entity.Position = next;
            }

            // 도착 칸에 occupant 등록
            _entity.Map.Get(_entity.Position).Occupant = _entity;
            IsMoving = false;
        }
    }
}
```

### 5.3 캐릭터 프리팹 생성 (에디터 작업)

1. 빈 GameObject 생성 → 이름 「TestCharacter」
2. SpriteRenderer 추가 → 임시 캐릭터 스프라이트 (32×32 단색 PNG) 할당
3. `CharacterEntity` + `CharacterMover` 컴포넌트 추가
4. 프리팹으로 저장: `Assets/Art/Prefabs/TestCharacter.prefab`

---

## 6. 입력 처리 + 데모 컨트롤러

### 6.1 `GridInput.cs` (터치/마우스 통합 입력)

`Assets/Scripts/Grid/GridInput.cs`:

```csharp
using UnityEngine;
using Roller.Core;

namespace Roller.Grid {
    [RequireComponent(typeof(GridMap))]
    public class GridInput : MonoBehaviour {
        public Camera Cam;
        public float DragThresholdPx = 10f; // 이 이상 움직이면 탭이 아닌 드래그로 판단

        GridMap _map;
        Vector2 _touchStartPos;
        bool _touchValid;

        void Awake() { _map = GetComponent<GridMap>(); }

        public bool TryGetTappedCell(out Coord cell) {
            cell = default;

            // 멀티터치 진행 중이면 무시 (핀치/팬용)
            if (Input.touchCount >= 2) { _touchValid = false; return false; }

            // 모바일 터치
            if (Input.touchCount == 1) {
                var t = Input.GetTouch(0);
                switch (t.phase) {
                    case TouchPhase.Began:
                        _touchStartPos = t.position;
                        _touchValid = true;
                        return false;
                    case TouchPhase.Moved:
                        if (Vector2.Distance(t.position, _touchStartPos) > DragThresholdPx)
                            _touchValid = false;
                        return false;
                    case TouchPhase.Ended:
                        if (_touchValid && Vector2.Distance(t.position, _touchStartPos) <= DragThresholdPx) {
                            cell = ScreenToCell(t.position);
                            _touchValid = false;
                            return _map.InBounds(cell);
                        }
                        return false;
                }
                return false;
            }

            // 에디터 마우스 (Android 실기에선 사용 안 됨)
            if (Application.isEditor) {
                if (Input.GetMouseButtonDown(0)) _touchStartPos = Input.mousePosition;
                if (Input.GetMouseButtonUp(0) &&
                    Vector2.Distance(Input.mousePosition, (Vector3)_touchStartPos) <= DragThresholdPx) {
                    cell = ScreenToCell(Input.mousePosition);
                    return _map.InBounds(cell);
                }
            }
            return false;
        }

        Coord ScreenToCell(Vector2 screenPos) {
            var worldPos = Cam.ScreenToWorldPoint(screenPos);
            return _map.WorldToCell(worldPos);
        }
    }
}
```

### 6.2 `M1DemoController.cs`

`Assets/Scripts/Demo/M1DemoController.cs`:

```csharp
using UnityEngine;
using Roller.Characters;
using Roller.Grid;
using Roller.Pathfinding;

namespace Roller.Demo {
    public class M1DemoController : MonoBehaviour {
        public GridMap Map;
        public GridInput Input;
        public CharacterEntity Character;

        CharacterMover _mover;

        void Start() {
            Application.targetFrameRate = 60;  // 모바일 60FPS 고정
            Character.Map = Map;
            Character.SnapToGrid();
            _mover = Character.GetComponent<CharacterMover>();
        }

        void Update() {
            if (_mover.IsMoving) return;
            if (Input.TryGetTappedCell(out var target)) {
                var path = AStarPathfinder.FindPath(Map, Character.Position, target);
                if (path != null) StartCoroutine(_mover.FollowPath(path));
            }
        }
    }
}
```

씬에 빈 GameObject 「GameController」를 만들어 이 컴포넌트를 추가, 인스펙터에서 참조 연결.

### 6.3 `CameraController.cs` — 모바일 핀치 줌 + 두 손가락 팬

`Assets/Scripts/Camera/CameraController.cs`:

```csharp
using UnityEngine;

namespace Roller.CameraSys {
    public class CameraController : MonoBehaviour {
        public float MinSize = 2.5f;
        public float MaxSize = 6f;
        public float PinchSensitivity = 0.01f;
        public float TwoFingerPanSensitivity = 0.01f;

        Camera _cam;
        Vector2 _prevMidpoint;
        float _prevPinchDistance;

        void Awake() { _cam = GetComponent<Camera>(); }

        void Update() {
            // 핀치 + 두 손가락 팬 동시 처리
            if (Input.touchCount == 2) {
                var t0 = Input.GetTouch(0);
                var t1 = Input.GetTouch(1);
                var midpoint = (t0.position + t1.position) * 0.5f;
                var distance = Vector2.Distance(t0.position, t1.position);

                if (t0.phase == TouchPhase.Began || t1.phase == TouchPhase.Began) {
                    _prevMidpoint = midpoint;
                    _prevPinchDistance = distance;
                    return;
                }

                // 줌
                float deltaDist = distance - _prevPinchDistance;
                float newSize = _cam.orthographicSize - deltaDist * PinchSensitivity;
                _cam.orthographicSize = Mathf.Clamp(newSize, MinSize, MaxSize);

                // 팬
                Vector2 deltaMid = midpoint - _prevMidpoint;
                transform.position -= new Vector3(
                    deltaMid.x * _cam.orthographicSize * TwoFingerPanSensitivity,
                    deltaMid.y * _cam.orthographicSize * TwoFingerPanSensitivity,
                    0
                );

                _prevMidpoint = midpoint;
                _prevPinchDistance = distance;
            }

            // 에디터 마우스 휠 줌 (개발 편의)
            if (Application.isEditor) {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                if (Mathf.Abs(scroll) > 0.001f) {
                    _cam.orthographicSize = Mathf.Clamp(_cam.orthographicSize - scroll * 2f,
                        MinSize, MaxSize);
                }
            }
        }
    }
}
```

> **모바일 카메라 팁**: 8×6 그리드는 한 화면에 다 보이도록 기본 설계. 핀치 줌·팬은 디테일 보기·UI 회피용. 줌 범위(MinSize 2.5 ~ MaxSize 6)는 그리드 전체가 화면에 들어오는 정도로 제한.

---

## 7. 주별 작업 일정

| 주 | 작업 | 결과물 |
|---|------|--------|
| 1 | Unity 설치 + Android Build Support, 튜토리얼 3개, 프로젝트 생성 + Android 빌드 타겟 설정, Git init | `roller` 프로젝트 + 빈 씬 Android 빌드 성공 (실기에서 검은 화면 + 유니티 로고만 떠도 OK) |
| 2 | 데이터 구조 (Coord, Cell, GridConfig, GridMap), GridView, M1TestGrid 에셋, 가로 해상도 셋업 | 에디터에서 빈 그리드가 화면에 표시됨 |
| 3 | 캐릭터 엔티티, 카메라 컨트롤러(핀치 줌+팬), 탭으로 캐릭터 즉시 이동 | 에디터·실기 모두에서 탭 시 캐릭터 순간이동 |
| 4 | A* 패스파인딩, CharacterMover (코루틴), 길찾기 통합, **Android 실기 검증** | 실기에서 탭 시 부드럽게 경로 따라 이동, 벽 우회 |

---

## 8. 검증 체크리스트

마일스톤 1 완료 판정 (에디터 + Android 실기 양쪽 모두):

**에디터**
- [ ] Unity 에디터에서 `M1Demo.unity` 실행 시 에러 없음
- [ ] Game 뷰가 1920×1080 가로 모드로 표시
- [ ] 8×6 그리드가 화면에 표시됨 (4종 타일 색깔로 구분)
- [ ] 캐릭터 1개가 (4, 3)에 위치
- [ ] 빈 풀밭 칸 클릭 → 캐릭터가 그 칸까지 이동
- [ ] 숲 칸 클릭 → 이동
- [ ] 물 칸 클릭 → 이동 안 함 (또는 우회 가능 시 우회)
- [ ] 벽으로 둘러싸인 칸 클릭 → 경로 없음, 이동 안 함
- [ ] 이동 중 클릭 → 무시
- [ ] 마우스 휠 → 줌

**Android 실기**
- [ ] `roller-debug.apk` 빌드 성공 (File → Build → Build APK)
- [ ] APK 설치, 앱 실행 시 가로 모드 + 그리드 + 캐릭터 표시
- [ ] 빈 칸 탭 → 캐릭터 이동
- [ ] 드래그 했을 때 캐릭터 이동 안 함 (탭/드래그 구분 OK)
- [ ] 두 손가락 핀치 → 줌
- [ ] 두 손가락 드래그 → 카메라 팬
- [ ] 60FPS 유지 (Unity Profiler 또는 GPU Watch)
- [ ] 메모리 100MB 이하 (모바일 RAM 여유)
- [ ] 회전 시도해도 가로 유지

**개발 환경**
- [ ] 모든 코드 git 커밋, GitHub 푸시 (선택)
- [ ] README.md에 빌드 방법 간단 메모

---

## 9. 흔한 함정 (Unity 첫 사용자 + 모바일 첫 빌드 주의)

| 함정 | 회피 |
|------|------|
| Inspector에서 참조 까먹어서 NullReferenceException | `[SerializeField]` 또는 public 필드 확실히 연결. 빨간 에러 메시지 잘 읽기 |
| 코루틴(`IEnumerator`) 시작 안 됨 | `StartCoroutine(method())` 사용. 직접 호출은 동작 안 함 |
| Pixel art가 흐릿하게 보임 | 스프라이트 import 설정에서 Filter Mode: **Point (no filter)**, Compression: None |
| Tilemap에 타일이 안 그려짐 | 타일 자산을 정확히 만들었는지 확인. 스프라이트 → 우클릭 → Create → 2D → Tiles → Tile |
| 좌표가 뒤집힘 (Y) | Unity는 Y가 위로 +. 그리드 좌표와 일치하므로 신경 안 써도 됨. 단 화면 변환 시 주의 |
| `PriorityQueue` 없음 에러 | Project Settings → Player → Configuration → Api Compatibility Level → **.NET Standard 2.1** 확인 |
| Git에 빈 폴더 안 들어감 | 각 폴더에 `.gitkeep` 빈 파일 추가 |
| Android 빌드 실패 (SDK/NDK 경로) | Edit → Preferences → External Tools → Android SDK/NDK 경로 확인. Unity Hub 설치 시 자동 경로 권장 |
| Android 빌드 실패 (Gradle) | Player Settings → Publishing Settings → Custom Main Gradle Template ON 후 재빌드 |
| 실기에서 검은 화면 + 즉시 종료 | Android Logcat (Window → Analysis → Android Logcat) 으로 에러 확인. NullRef가 가장 흔함 |
| 터치 응답이 너무 작음 | 그리드 셀이 너무 작으면 fat-finger 문제. 최소 셀 크기 = 화면 짧은 변의 1/8 권장 |
| 「가로 락」 안 됨 | Player Settings → Resolution → Default Orientation: Landscape Left + Allowed Orientations에서 Portrait 체크 해제 |
| 화면 비율 차이로 그리드 잘림 | 카메라 Orthographic Size 계산을 화면 비율에 따라 보정. 또는 Safe Area 처리 (M1엔 생략 OK) |
| 빌드 시간 5분+ | IL2CPP는 첫 빌드만 느림. 캐시 활용 (Library 폴더 보존). 또는 Mono Backend로 개발 중 빠른 반복 |
| APK 설치 안 됨 (서명) | Debug 키스토어 자동 생성. Release 빌드시 키스토어 별도 생성 필요 (Post-M1) |

---

## 10. 마일스톤 1 → 2 다리

마일스톤 2(틱 시뮬레이션 + 일시정지)로 넘어가는 데 필요한 추가 작업 미리보기:

- `TickScheduler` (틱 진행 + 일시정지 + 속도 조절)
- `ICombatant` 인터페이스 (CharacterEntity가 구현)
- 기본 평타 스킬 (단일 표적 피해)
- HP/사기 표시 UI (간단한 막대)
- 1대1 데모 씬 (캐릭터 vs 적, 자동 평타 주고받기)

M1 코드 구조가 잘 잡혀 있어야 M2가 깔끔. 특히:
- `GridMap`/`CharacterEntity`/`AStarPathfinder`가 서로 인터페이스로 분리되어 있어야 M2의 `TickScheduler`가 끼어들기 좋음

---

## 11. 작업 시작 권장 순서

1. **오늘**: Unity Hub + Unity 6 LTS (+ Android Build Support) 설치, Roll-a-Ball 튜토리얼 시작
2. **이번 주 안**:
   - 2D Adventure 튜토리얼 완주
   - 「First Android Build」 가이드 따라 빈 프로젝트 → Android 실기 빌드 1회 성공
3. **2주차 시작**: `roller` 프로젝트 생성 + Android 빌드 타겟 설정 + Git init + 폴더 구조 + 위 코드 그대로 입력
4. **3주차 끝**: 에디터에서 탭으로 이동 동작
5. **4주차 끝**: A* + 부드러운 이동 + **Android 실기 빌드 검증**
6. **막히면**: 본 문서 + Unity 공식 문서 + Stack Overflow 순서로 검색. 막혀서 1시간 이상 걸리면 단순화:
   - A* 대신 BFS (단순한 경로, 비용 무시)
   - Tilemap 대신 SpriteRenderer 48개 (8×6)
   - 핀치 줌·팬 생략 (M1엔 옵션)
   - Android 실기 검증은 M2로 미룸 (단 빌드 자체는 M1에서 한 번은 성공해야 함)

---

## 12. 마일스톤 1 완료 후

- 데모 영상 녹화 (OBS 등) — 진행 기록·홍보용
- 진행 로그를 `docs/progress/m1-complete.md`에 작성
- 마일스톤 2 구현 계획서 작성 (틱 시뮬레이션 핵심 설계)
