using UnityEngine;
using Roller.Characters;
using Roller.Grid;
using Roller.Pathfinding;

namespace Roller.Demo
{
    /// <summary>
    /// 마일스톤 1 데모 컨트롤러.
    /// 그리드 + 캐릭터 1명 + 탭으로 이동.
    /// </summary>
    public class M1DemoController : MonoBehaviour
    {
        public GridMap Map;
        public GridInput Input;
        public CharacterEntity Character;

        [Tooltip("모바일 60FPS 고정 (자율 전투 결정적 시뮬레이션이라 프레임률과 무관하지만 UI 부드러움 보장)")]
        public int TargetFps = 60;

        CharacterMover _mover;

        void Start()
        {
            Application.targetFrameRate = TargetFps;

            if (Map == null || Input == null || Character == null)
            {
                Debug.LogError("[M1DemoController] Map/Input/Character 참조가 누락되었다. Inspector에서 연결할 것.");
                enabled = false;
                return;
            }

            Character.Map = Map;
            Character.SnapToGrid();
            _mover = Character.GetComponent<CharacterMover>();
            if (_mover == null)
            {
                Debug.LogError("[M1DemoController] CharacterMover 컴포넌트가 Character에 없다.");
                enabled = false;
            }
        }

        void Update()
        {
            if (_mover == null || _mover.IsMoving) return;
            if (Input.TryGetTappedCell(out var target))
            {
                var path = AStarPathfinder.FindPath(Map, Character.Position, target);
                if (path != null && path.Count > 1)
                {
                    StartCoroutine(_mover.FollowPath(path));
                }
                else
                {
                    Debug.Log($"[M1Demo] Path not found from {Character.Position} to {target}");
                }
            }
        }
    }
}
