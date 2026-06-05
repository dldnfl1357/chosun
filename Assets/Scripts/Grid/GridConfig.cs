using UnityEngine;

namespace Roller.Grid
{
    [CreateAssetMenu(fileName = "GridConfig", menuName = "Roller/Grid Config")]
    public class GridConfig : ScriptableObject
    {
        [Tooltip("그리드 가로 칸 수 (모바일 가로 기본 8)")]
        public int Width = 8;

        [Tooltip("그리드 세로 칸 수 (모바일 가로 기본 6)")]
        public int Height = 6;

        [Tooltip("월드 단위 셀 크기")]
        public float CellSize = 1f;

        [Tooltip("초기 지형 (Width*Height, 행 우선 순서). 비어있으면 모두 Grass")]
        public TerrainType[] InitialTerrain;

#if UNITY_EDITOR
        [ContextMenu("Generate Test Layout 8x6")]
        void GenerateTestLayout()
        {
            Width = 8;
            Height = 6;
            int total = Width * Height;
            InitialTerrain = new TerrainType[total];
            // 기본은 모두 풀밭
            for (int i = 0; i < total; i++) InitialTerrain[i] = TerrainType.Grass;

            // 테스트용 장애물 배치
            // (x, y) -> idx = y * Width + x
            InitialTerrain[Idx(2, 1)] = TerrainType.Water;
            InitialTerrain[Idx(2, 2)] = TerrainType.Water;
            InitialTerrain[Idx(2, 3)] = TerrainType.Water;
            InitialTerrain[Idx(5, 2)] = TerrainType.Wall;
            InitialTerrain[Idx(5, 3)] = TerrainType.Wall;
            InitialTerrain[Idx(3, 4)] = TerrainType.Forest;
            InitialTerrain[Idx(4, 4)] = TerrainType.Forest;
            InitialTerrain[Idx(0, 5)] = TerrainType.Forest;

            UnityEditor.EditorUtility.SetDirty(this);
            Debug.Log("Test layout generated: 8x6 with water/wall/forest obstacles.");
        }

        int Idx(int x, int y) => y * Width + x;
#endif
    }
}
