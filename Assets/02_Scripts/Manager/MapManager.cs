using System.Collections.Generic;
using UnityEngine;
using StarDefense.Data;

namespace StarDefense.Managers
{
    /// <summary>
    /// GameArea 오브젝트 위에 7x10 그리드를 배치
    /// GameArea의 Position = 좌하단, gameAreaSize = 영역 크기
    /// Scene 뷰에서 Gizmo로 그리드를 시각적으로 확인
    /// </summary>
    public class MapManager : MonoBehaviour
    {
        [Header("Game Area")]
        [Tooltip("게임 영역 좌하단에 배치")]
        [SerializeField] private Transform gameArea;

        [Header("Game Area Size")]
        [SerializeField] private Vector2 gameAreaSize = new Vector2(12.5f, 17f);

        [Header("Chapter Themes")]
        [Tooltip("index 0 = chapter 1, index 1 = chapter 2, ...")]
        [SerializeField] private TileTheme[] chapterThemes;

        [Header("Background")]
        [SerializeField] private SpriteRenderer background;

        private StageData stageData;
        private MapData mapData;
        private TileTheme currentTheme;
        private int[,] logicGrid;
        private bool[,] occupiedGrid;
        private GameObject[,] tileObjects;
        private int playableWidth;
        private int playableHeight;
        private Vector2 cellSize;
        private List<List<Vector3>> waypointsList = new List<List<Vector3>>();
        private List<Vector2Int> spawnGridPosList = new List<Vector2Int>();
        private Vector2Int commanderGridPos;
        private Transform tileParent;

        public int PlayableWidth => playableWidth;
        public int PlayableHeight => playableHeight;
        public Vector2 CellSize => cellSize;
        public List<List<Vector3>> WaypointsList => waypointsList;
        public List<Vector2Int> SpawnGridPosList => spawnGridPosList;
        public Vector3 CommanderWorldPosition => GridToWorldPosition(commanderGridPos.x, commanderGridPos.y);

        /// <summary>
        /// StageInitManager에서 호출 배경 + 맵 + 경로 전부 세팅
        /// </summary>
        public void InitMap(int mStageId)
        {
            stageData = DataManager.GetTable<StageData>().Get(mStageId);
            if (stageData == null) return;

            LoadBackground(stageData.chapter);
            LoadStage(mStageId);
        }

        /// <summary>
        /// 챕터에 맞는 배경을 Resources/BG/에서 로드
        /// BG파일명: BG_Stage_1, BG_Stage_2...
        /// </summary>
        private void LoadBackground(int chapter)
        {
            Sprite bg = Resources.Load<Sprite>($"BG/BG_Stage_{chapter}");

            if (bg != null && background != null)
            {
                background.sprite = bg;
            }
            else
            {
                Debug.LogWarning($"BG찾을 수 없음: BG/BG_Chapter_{chapter}");
            }
        }

        private void LoadStage(int mStageId)
        {
            mapData = DataManager.GetTable<MapData>().Get(stageData.mapId);
            if (mapData == null) return;

            // 챕터에 맞는 테마 선택
            int themeIndex = stageData.chapter - 1;
            if (chapterThemes != null && themeIndex >= 0 && themeIndex < chapterThemes.Length)
            {
                currentTheme = chapterThemes[themeIndex];
            }
            else if (chapterThemes != null && chapterThemes.Length > 0)
            {
                currentTheme = chapterThemes[0];
            }

            playableWidth = mapData.mapWidth - 2;
            playableHeight = mapData.mapHeight - 2;

            cellSize = new Vector2(
                gameAreaSize.x / playableWidth,
                gameAreaSize.y / playableHeight
            );

            InitializeLogicGrid();
            BuildTiles();
            BuildWaypoints();
        }

        private void InitializeLogicGrid()
        {
            int w = mapData.mapWidth;
            int h = mapData.mapHeight;

            logicGrid = new int[w, h];
            occupiedGrid = new bool[playableWidth, playableHeight];

            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    int type = mapData.layout[x][y];
                    logicGrid[x, y] = type;

                    if (type == 5)
                    {
                        commanderGridPos = new Vector2Int(x - 1, y - 1);
                    }
                }
            }
        }

        /// <summary>
        /// 게임 영역(Wall 제외)에 테마 프리팹을 배치
        /// </summary>
        private void BuildTiles()
        {
            if (currentTheme == null) return;

            if (tileParent != null) Destroy(tileParent.gameObject);

            tileParent = new GameObject("Tiles").transform;
            tileParent.SetParent(gameArea);
            tileParent.localPosition = Vector3.zero;
            tileObjects = new GameObject[playableWidth, playableHeight];

            for (int x = 0; x < playableWidth; x++)
            {
                for (int y = 0; y < playableHeight; y++)
                {
                    int type = logicGrid[x + 1, y + 1];

                    GameObject prefab = GetPrefabFromTheme(type);
                    if (prefab == null) continue;

                    Vector3 worldPos = GridToWorldPosition(x, y);
                    GameObject tile = Instantiate(prefab, worldPos, Quaternion.identity, tileParent);
                    tile.name = $"Tile_{x}_{y}";

                    if (tile.TryGetComponent(out SpriteRenderer sr))
                    {
                        sr.sortingOrder = y + 1;
                    }

                    tileObjects[x, y] = tile;
                }
            }
        }

        private GameObject GetPrefabFromTheme(int type)
        {
            return type switch
            {
                1 => currentTheme.block,
                2 => currentTheme.fixBlock,
                3 => currentTheme.buff,
                4 => currentTheme.spawn,
                5 => currentTheme.commander,
                6 => currentTheme.obstacle,
                _ => null
            };
        }

        /// <summary>
        /// MapData의 paths 데이터에서 각 경로를 생성
        /// </summary>
        private void BuildWaypoints()
        {
            waypointsList.Clear();
            spawnGridPosList.Clear();

            if (mapData.paths == null) return;

            foreach (EnemyPath enemyPath in mapData.paths)
            {
                if (enemyPath.corners == null || enemyPath.corners.Length < 2) continue;

                Vector2Int spawnPos = new Vector2Int(enemyPath.corners[0][0], enemyPath.corners[0][1]);
                spawnGridPosList.Add(spawnPos);

                List<Vector3> path = BuildPathFromCorners(enemyPath.corners);
                waypointsList.Add(path);

                Debug.Log($"[MapManager] Path {enemyPath.pathId}: {enemyPath.corners.Length} corners → {path.Count} waypoints");
            }
        }

        /// <summary>
        /// 코너 포인트 사이를 직선으로 보간하여 전체 경로를 생성
        /// </summary>
        private List<Vector3> BuildPathFromCorners(int[][] corners)
        {
            List<Vector3> path = new List<Vector3>();

            for (int i = 0; i < corners.Length - 1; i++)
            {
                Vector2Int from = new Vector2Int(corners[i][0], corners[i][1]);
                Vector2Int to = new Vector2Int(corners[i + 1][0], corners[i + 1][1]);

                int dx = to.x - from.x;
                int dy = to.y - from.y;
                int stepX = dx != 0 ? (dx > 0 ? 1 : -1) : 0;
                int stepY = dy != 0 ? (dy > 0 ? 1 : -1) : 0;

                Vector2Int current = from;

                while (current != to)
                {
                    path.Add(GridToWorldPosition(current.x, current.y));

                    if (current.x != to.x)
                    {
                        current.x += stepX;
                    }
                    else if (current.y != to.y)
                    {
                        current.y += stepY;
                    }
                }
            }

            int[] last = corners[corners.Length - 1];
            path.Add(GridToWorldPosition(last[0], last[1]));

            return path;
        }

        // ── Public API ──
        public Vector3 GridToWorldPosition(int x, int y)
        {
            Vector3 origin = gameArea != null ? gameArea.position : Vector3.zero;

            return new Vector3(
                origin.x + (x + 0.5f) * cellSize.x,
                origin.y + (playableHeight - 1 - y + 0.5f) * cellSize.y,
                0f
            );
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPos)
        {
            Vector3 origin = gameArea != null ? gameArea.position : Vector3.zero;

            int x = Mathf.FloorToInt((worldPos.x - origin.x) / cellSize.x);
            int y = playableHeight - 1 - Mathf.FloorToInt((worldPos.y - origin.y) / cellSize.y);
            return new Vector2Int(x, y);
        }

        /// <summary>
        /// 스폰 인덱스에 해당하는 월드 좌표를 반환
        /// </summary>
        public Vector3 GetSpawnPosition(int spawnIndex = 0)
        {
            if (spawnIndex < 0 || spawnIndex >= spawnGridPosList.Count)
                return Vector3.zero;

            Vector2Int pos = spawnGridPosList[spawnIndex];
            return GridToWorldPosition(pos.x, pos.y);
        }

        /// <summary>
        /// 스폰 인덱스에 해당하는 웨이포인트 경로를 반환
        /// </summary>
        public List<Vector3> GetWaypoints(int spawnIndex = 0)
        {
            if (spawnIndex < 0 || spawnIndex >= waypointsList.Count)
                return null;

            return waypointsList[spawnIndex];
        }

        public int GetTileType(int x, int y)
        {
            if (x < 0 || x >= playableWidth || y < 0 || y >= playableHeight)
                return -1;

            return logicGrid[x + 1, y + 1];
        }

        public bool CanPlaceHero(int x, int y)
        {
            int type = GetTileType(x, y);
            if (type < 0) return false;
            return (type == 1 || type == 3) && !occupiedGrid[x, y];
        }

        public void SetOccupied(int x, int y, bool occupied)
        {
            if (x < 0 || x >= playableWidth || y < 0 || y >= playableHeight) return;
            occupiedGrid[x, y] = occupied;
        }

        public bool IsBuffTile(int x, int y) => GetTileType(x, y) == 3;

        public void RepairTile(int x, int y)
        {
            if (GetTileType(x, y) != 2) return;

            logicGrid[x + 1, y + 1] = 1;

            if (tileObjects[x, y] != null)
            {
                Destroy(tileObjects[x, y]);
            }

            if (currentTheme != null && currentTheme.block != null)
            {
                Vector3 worldPos = GridToWorldPosition(x, y);
                GameObject tile = Instantiate(currentTheme.block, worldPos, Quaternion.identity, tileParent);
                tile.name = $"Tile_{x}_{y}";
                tileObjects[x, y] = tile;
            }
        }
    }
}