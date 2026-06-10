using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Roller.Core;
using Roller.Grid;

namespace Roller.Characters
{
    [RequireComponent(typeof(CharacterEntity))]
    public class CharacterMover : MonoBehaviour
    {
        [Tooltip("초당 이동 칸 수")]
        public float SpeedCellsPerSec = 4f;

        public bool IsMoving { get; private set; }

        CharacterEntity _entity;

        void Awake()
        {
            _entity = GetComponent<CharacterEntity>();
        }

        public IEnumerator FollowPath(List<Coord> path)
        {
            if (path == null || path.Count < 2) yield break;
            if (_entity.Map == null) yield break;

            IsMoving = true;

            // 출발 칸의 occupant 해제
            var startCell = _entity.Map.Get(_entity.Position);
            if (startCell != null && ReferenceEquals(startCell.Occupant, _entity)) startCell.Occupant = null;

            // 첫 칸(현재)은 건너뛰고 다음 칸부터 이동
            for (int i = 1; i < path.Count; i++)
            {
                Coord next = path[i];
                var nextCell = _entity.Map.Get(next);
                if (nextCell == null || !nextCell.Terrain.IsPassable())
                {
                    // 도중 막힘
                    break;
                }
                if (nextCell.Occupant != null && i != path.Count - 1)
                {
                    // 누가 점유 중 (최종 칸은 도착 후 점유 체크)
                    break;
                }

                Vector3 startWorld = transform.position;
                Vector3 endWorld = _entity.Map.CellToWorld(next);
                float duration = 1f / Mathf.Max(0.1f, SpeedCellsPerSec);
                float t = 0f;
                while (t < duration)
                {
                    t += Time.deltaTime;
                    transform.position = Vector3.Lerp(startWorld, endWorld, t / duration);
                    yield return null;
                }
                transform.position = endWorld;
                _entity.Position = next;
            }

            // 도착 칸에 occupant 등록
            var arriveCell = _entity.Map.Get(_entity.Position);
            if (arriveCell != null) arriveCell.Occupant = _entity;
            IsMoving = false;
        }
    }
}
