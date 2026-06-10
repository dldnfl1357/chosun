#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using Roller.Grid;
using Roller.Characters;
using Roller.CameraCtrl;
using Roller.Demo;

namespace Roller.EditorTools
{
    /// <summary>
    /// 마일스톤 1 원클릭 셋업.
    /// 메뉴: Tools/Roller/M1 — Setup Demo Scene
    /// 절차: Sprite import 설정 → Tile 자산 4종 → GridConfig 자산 → M1Demo 씬 구성·저장.
    /// 이미 존재하는 자산은 덮어쓰지 않고 재사용한다.
    /// </summary>
    public static class M1SetupTool
    {
        const string SpriteFolder = "Assets/Art/Sprites/placeholder";
        const string TileFolder = "Assets/Art/Tiles";
        const string GridConfigFolder = "Assets/Data/GridConfigs";
        const string ScenesFolder = "Assets/Scenes";
        const string ScenePath = "Assets/Scenes/M1Demo.unity";
        const string GridConfigPath = "Assets/Data/GridConfigs/M1TestGrid.asset";

        [MenuItem("Tools/Roller/M1 — Setup Demo Scene")]
        public static void SetupAll()
        {
            try
            {
                EnsureFolders();
                ConfigurePlaceholderSprites();
                var tiles = EnsureTiles();
                var config = EnsureGridConfig();
                BuildScene(tiles, config);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog(
                    "M1 Setup",
                    "완료!\n\n" +
                    "1) 플레이스홀더 스프라이트 import 설정 적용\n" +
                    "2) Tile 자산 4종 생성/확인 (Assets/Art/Tiles)\n" +
                    "3) GridConfig 생성 (Assets/Data/GridConfigs/M1TestGrid)\n" +
                    "4) M1Demo 씬 생성·저장 (Assets/Scenes/M1Demo.unity)\n\n" +
                    "이제 ▶ Play 버튼을 눌러 확인.",
                    "OK");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[M1Setup] 실패: {e}");
                EditorUtility.DisplayDialog("M1 Setup", $"실패: {e.Message}\n\nConsole 확인.", "OK");
            }
        }

        [MenuItem("Tools/Roller/M1 — Reimport Placeholder Sprites")]
        public static void ReimportSpritesOnly()
        {
            ConfigurePlaceholderSprites();
            AssetDatabase.Refresh();
        }

        // ---------- folders ----------

        static void EnsureFolders()
        {
            EnsureFolder("Assets/Art");
            EnsureFolder(SpriteFolder);
            EnsureFolder(TileFolder);
            EnsureFolder("Assets/Data");
            EnsureFolder(GridConfigFolder);
            EnsureFolder(ScenesFolder);
        }

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parent = Path.GetDirectoryName(path).Replace('\\', '/');
            var leaf = Path.GetFileName(path);
            if (!AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }

        // ---------- sprite import ----------

        static void ConfigurePlaceholderSprites()
        {
            string[] names = { "grass", "forest", "water", "wall", "character" };
            foreach (var n in names)
            {
                var path = $"{SpriteFolder}/{n}.png";
                if (!File.Exists(path))
                {
                    Debug.LogWarning($"[M1Setup] {path} 없음. PowerShell로 미리 생성했어야 함.");
                    continue;
                }
                var imp = AssetImporter.GetAtPath(path) as TextureImporter;
                if (imp == null)
                {
                    AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                    imp = AssetImporter.GetAtPath(path) as TextureImporter;
                }
                if (imp == null) continue;

                imp.textureType = TextureImporterType.Sprite;
                imp.spritePixelsPerUnit = 16f;
                imp.filterMode = FilterMode.Point;
                imp.textureCompression = TextureImporterCompression.Uncompressed;
                imp.mipmapEnabled = false;
                imp.wrapMode = TextureWrapMode.Clamp;
                imp.SaveAndReimport();
            }
        }

        // ---------- tiles ----------

        struct TileSet
        {
            public Tile Grass, Forest, Water, Wall;
            public Sprite Character;
        }

        static TileSet EnsureTiles()
        {
            var set = new TileSet
            {
                Grass = EnsureTile("GrassTile", "grass"),
                Forest = EnsureTile("ForestTile", "forest"),
                Water = EnsureTile("WaterTile", "water"),
                Wall = EnsureTile("WallTile", "wall"),
                Character = AssetDatabase.LoadAssetAtPath<Sprite>($"{SpriteFolder}/character.png"),
            };
            return set;
        }

        static Tile EnsureTile(string tileAssetName, string spriteName)
        {
            var tilePath = $"{TileFolder}/{tileAssetName}.asset";
            var tile = AssetDatabase.LoadAssetAtPath<Tile>(tilePath);
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{SpriteFolder}/{spriteName}.png");
            if (sprite == null)
                Debug.LogWarning($"[M1Setup] {spriteName}.png Sprite를 로드 못 함. import 설정 확인.");

            if (tile == null)
            {
                tile = ScriptableObject.CreateInstance<Tile>();
                tile.sprite = sprite;
                AssetDatabase.CreateAsset(tile, tilePath);
            }
            else if (tile.sprite != sprite)
            {
                tile.sprite = sprite;
                EditorUtility.SetDirty(tile);
            }
            return tile;
        }

        // ---------- grid config ----------

        static GridConfig EnsureGridConfig()
        {
            var cfg = AssetDatabase.LoadAssetAtPath<GridConfig>(GridConfigPath);
            if (cfg == null)
            {
                cfg = ScriptableObject.CreateInstance<GridConfig>();
                AssetDatabase.CreateAsset(cfg, GridConfigPath);
            }
            cfg.Width = 8;
            cfg.Height = 6;
            cfg.CellSize = 1f;
            // ContextMenu 「Generate Test Layout 8x6」와 동일한 레이아웃을 코드로 적용
            int total = cfg.Width * cfg.Height;
            var terrain = new TerrainType[total];
            for (int i = 0; i < total; i++) terrain[i] = TerrainType.Grass;
            int W = cfg.Width;
            terrain[1 * W + 2] = TerrainType.Water;
            terrain[2 * W + 2] = TerrainType.Water;
            terrain[3 * W + 2] = TerrainType.Water;
            terrain[2 * W + 5] = TerrainType.Wall;
            terrain[3 * W + 5] = TerrainType.Wall;
            terrain[4 * W + 3] = TerrainType.Forest;
            terrain[4 * W + 4] = TerrainType.Forest;
            terrain[5 * W + 0] = TerrainType.Forest;
            cfg.InitialTerrain = terrain;
            EditorUtility.SetDirty(cfg);
            return cfg;
        }

        // ---------- scene ----------

        static void BuildScene(TileSet tiles, GridConfig config)
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Main Camera
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            var cam = camGO.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 4f;
            cam.backgroundColor = new Color(0.13f, 0.13f, 0.13f, 1f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 100f;
            camGO.transform.position = new Vector3(3.5f, 2.5f, -10f);
            camGO.AddComponent<CameraController>();
            camGO.AddComponent<AudioListener>();

            // Grid + Tilemap
            var gridGO = new GameObject("Grid");
            var unityGrid = gridGO.AddComponent<UnityEngine.Grid>();
            unityGrid.cellSize = new Vector3(1f, 1f, 0f);

            var tilemapGO = new GameObject("Tilemap");
            tilemapGO.transform.SetParent(gridGO.transform, false);
            var tilemap = tilemapGO.AddComponent<Tilemap>();
            tilemapGO.AddComponent<TilemapRenderer>();

            // GridMap + GridView + GridInput on Grid GameObject
            var gridMap = gridGO.AddComponent<GridMap>();
            gridMap.Config = config;

            var gridView = gridGO.AddComponent<GridView>();
            gridView.Tilemap = tilemap;
            gridView.GrassTile = tiles.Grass;
            gridView.ForestTile = tiles.Forest;
            gridView.WaterTile = tiles.Water;
            gridView.WallTile = tiles.Wall;

            var gridInput = gridGO.AddComponent<GridInput>();
            gridInput.Cam = cam;

            // TestCharacter
            var charGO = new GameObject("TestCharacter");
            charGO.transform.position = new Vector3(4f, 3f, 0f);
            var sr = charGO.AddComponent<SpriteRenderer>();
            sr.sprite = tiles.Character;
            sr.sortingOrder = 5;
            var entity = charGO.AddComponent<CharacterEntity>();
            entity.Position = new Roller.Core.Coord(4, 3);
            entity.Map = gridMap;
            charGO.AddComponent<CharacterMover>();

            // GameController
            var ctrlGO = new GameObject("GameController");
            var ctrl = ctrlGO.AddComponent<M1DemoController>();
            ctrl.Map = gridMap;
            ctrl.Input = gridInput;
            ctrl.Character = entity;

            // Save scene
            EditorSceneManager.SaveScene(scene, ScenePath);

            // 빌드 설정에 씬 추가
            var scenes = EditorBuildSettings.scenes;
            bool already = false;
            foreach (var s in scenes) if (s.path == ScenePath) { already = true; break; }
            if (!already)
            {
                var list = new System.Collections.Generic.List<EditorBuildSettingsScene>(scenes);
                list.Add(new EditorBuildSettingsScene(ScenePath, true));
                EditorBuildSettings.scenes = list.ToArray();
            }
        }
    }
}
#endif
