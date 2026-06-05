using UnityEngine;
using Roller.Core;
using Roller.Grid;

namespace Roller.Characters
{
    public class CharacterEntity : MonoBehaviour, IGridOccupant
    {
        [Tooltip("초기 그리드 좌표")]
        public Coord Position;

        [Tooltip("이 캐릭터가 속한 그리드. M1DemoController가 Start에서 할당해도 됨")]
        public GridMap Map;

        Coord IGridOccupant.Position => Position;

        public void SnapToGrid()
        {
            if (Map == null)
            {
                Debug.LogError($"[{nameof(CharacterEntity)}] Map not assigned.");
                return;
            }
            transform.position = Map.CellToWorld(Position);
            var cell = Map.Get(Position);
            if (cell != null) cell.Occupant = this;
        }
    }
}
