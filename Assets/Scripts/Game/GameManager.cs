using System.Collections.Generic;
using AddressableImpl;
using BakingSheetImpl;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoSingleton<GameManager>
{
    [Header("References")]
    [SerializeField] private Transform gameContainer;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private MapCellBG mapCellBGPrefab;
    [SerializeField] private TowerPreview towerPreviewPrefab;

    [Header("Play Resources")]
    [SerializeField] private Dictionary<ResourcesKey, float> resources = new Dictionary<ResourcesKey, float>();

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    #region Runtime Variables
    private GameState gameState = GameState.PAUSE;
    private GameState prePauseState = GameState.NONE;
    private Grid<MapCell> grid;
    private string curLevel = GloblaConstants.Default.LEVEL;
    private Level levelData;
    private EnemySpawner enemySpawner;
    private MapCell Start_Cell;
    private MapCell End_Cell;
    private int curWave = 0;
    private List<EnemyController> enemies;
    private List<TowerController> towers;
    private List<Projectile> projectiles;
    private TowerPreview towerPreview;
    #endregion

    private void OnEnable()
    {
        NotificationService.Instance.Add(ResourcesKey.LEVEL_HP.ToString(), OnLevelHPChanged);
    }

    private void OnDisable()
    {
        NotificationService.Instance.Remove(ResourcesKey.LEVEL_HP.ToString(), OnLevelHPChanged);
    }

    #region Game Control

    public void LoadGame()
    {
        ClearGame();

        SetGameState(GameState.LOADING);
        levelData = DataManager.Instance.GetData<Level>(curLevel);
        AddressableManager.LoadAssetAsync<Map>(levelData.MapDataAddress, (result) =>
        {
            Map map = result;
            grid = new Grid<MapCell>(map.gridSize.x, map.gridSize.y, map.cellSize, new Vector3(-map.gridSize.x, -map.gridSize.y) * 0.5f * map.cellSize, (Grid<MapCell> g, int x, int y) => new MapCell(g, x, y, map.GetValue(x, y)));
            SetResources(ResourcesKey.COIN, levelData.StartCoin);
            SetResources(ResourcesKey.LEVEL_HP, levelData.TotalHP);

            CreateEnemySpawner();

            SetGameState(GameState.PREPARE);
        });
    }

    public void ClearGame()
    {
        SetGameState(GameState.NONE);
        grid = null;
        levelData = null;
        enemySpawner = null;
        SetCurWave(0);

        Utilities.ClearChildren(GetGameContainer());
    }

    private void CreateEnemySpawner()
    {
        GameObject enemySpawnerObj = new GameObject("EnemySpawner");
        enemySpawnerObj.transform.SetParent(GetGameContainer());
        enemySpawner = enemySpawnerObj.AddComponent<EnemySpawner>();
    }

    private void Update()
    {
        if (GetGameState() != GameState.PLAYING && GetGameState() != GameState.PREPARE) return;

        if (cameraController != null) cameraController.ManualUpdate();
        if (towerPreview != null) towerPreview.ManualUpdate();
        PlaceTower();
        SellTower();

        if (GetGameState() == GameState.PLAYING)
        {
            if (enemySpawner != null) enemySpawner.ManualUpdate();

            UpdateProjectiles();
            UpdateEnemies();
            UpdateTowers();
        }
    }

    private void UpdateEnemies()
    {
        if (enemies != null && enemies.Count > 0)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i] == null)
                {
                    enemies.RemoveAt(i);
                    continue;
                }

                enemies[i].ManualUpdate();
            }
        }
    }

    private void UpdateTowers()
    {
        if (towers != null && towers.Count > 0)
        {
            for (int i = towers.Count - 1; i >= 0; i--)
            {
                if (towers[i] == null)
                {
                    towers.RemoveAt(i);
                    continue;
                }

                towers[i].ManualUpdate();
            }
        }
    }

    private void UpdateProjectiles()
    {
        if (projectiles != null && projectiles.Count > 0)
        {
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                if (i >= projectiles.Count || i < 0) continue;

                if (projectiles[i] == null)
                {
                    projectiles.RemoveAt(i);
                    continue;
                }

                projectiles[i].ManualUpdate();
            }
        }
    }

    public void PauseGame()
    {
        prePauseState = GetGameState();
        SetGameState(GameState.PAUSE);
    }

    public void ResumeGame()
    {
        SetGameState(prePauseState);
    }

    public void OnReady()
    {
        SetGameState(GameState.PLAYING);
        enemySpawner.LoadEnemies();
    }

    public void OnWin()
    {
        SSSceneManager.Instance.PopUp(PopupNames.WIN);
        curLevel = GetNextLevel();

        string GetNextLevel()
        {
            List<Level> levels = DataManager.Instance.GetDatas<Level>();
            int nextLevelIndex = levels.IndexOf(levelData) + 1;
            return levels[Mathf.Clamp(nextLevelIndex, 0, levels.Count - 1)].Id;
        }
    }

    public void OnLose()
    {
        SSSceneManager.Instance.PopUp(PopupNames.LOSE);
    }

    private void OnLevelHPChanged()
    {
        if (GetResources(ResourcesKey.LEVEL_HP) <= 0) OnLose();
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
    #endregion
    #region Resources

    public float GetResources(ResourcesKey key)
    {
        if (resources.ContainsKey(key))
        {
            return resources[key];
        }
        else
        {
            resources.Add(key, 0);
            return resources[key];
        }
    }

    public void SetResources(ResourcesKey key, float value)
    {
        if (resources.ContainsKey(key))
        {
            resources[key] = value;
        }
        else
        {
            resources.Add(key, value);
        }

        NotificationService.Instance.Post(key.ToString());
    }

    public void AddResources(ResourcesKey key, float value)
    {
        float curRes = GetResources(key);
        SetResources(key, curRes + value);
    }

    #endregion

    #region Getters & Setters
    #region Tower
    public void OnTowerSelected(Tower tower)
    {
        if (towerPreview == null)
        {
            towerPreview = Instantiate(towerPreviewPrefab, GetGameContainer());
        }

        towerPreview.Setup(tower);
    }

    public void OnRemoveTowerPreview()
    {
        if (towerPreview != null)
        {
            Destroy(towerPreview.gameObject);
            towerPreview = null;
        }
    }

    public void PlaceTower()
    {
        if (towerPreview == null) return;

        if (Input.GetMouseButtonDown(0) && !IsMouseOverUI())
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MapCell cell = GetCell(mousePos);

            if (!IsCanPlaceTower()) return;

            TowerController towerInCell = cell.GetTower();
            Tower towerInfo = towerPreview.GetTowerInfo();
            // Place new tower  
            if (towerInCell == null)
            {
                bool isEnoughCoin = GetResources(ResourcesKey.COIN) >= towerInfo.Upgrades[0].Price;
                if (!isEnoughCoin)
                {
                    NotificationService.Instance.Post(GloblaConstants.Noti.NOT_ENOUGH_COIN);
                    return;
                }

                AddressableManager.LoadAssetAsync<GameObject>(towerInfo.PrefabAddress, (result) =>
                {
                    TowerController tower = Instantiate(result, cell.GetCellBG().transform).GetComponent<TowerController>();
                    tower.Setup(towerInfo);
                    cell.PlaceTower(tower);

                    AddResources(ResourcesKey.COIN, -towerInfo.Upgrades[0].Price);
                });
            }
            // Upgrade tower
            else
            {
                bool isEnoughCoin = GetResources(ResourcesKey.COIN) >= towerInfo.Upgrades[towerInCell.GetLevel() + 1].Price;
                if (!isEnoughCoin)
                {
                    NotificationService.Instance.Post(GloblaConstants.Noti.NOT_ENOUGH_COIN);
                    return;
                }

                towerInCell.Upgrade();
                AddResources(ResourcesKey.COIN, -towerInfo.Upgrades[towerInCell.GetLevel()].Price);
            }
        }
    }

    public void SellTower()
    {
        if (Input.GetMouseButtonDown(1) && !IsMouseOverUI())
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            MapCell cell = GetCell(mousePos);

            cell?.RemoveTower();
        }
    }

    public void AddTower(TowerController tower)
    {
        towers ??= new List<TowerController>();
        towers.Add(tower);
    }

    public void RemoveTower(TowerController tower)
    {
        if (towers == null || towers.Count == 0) return;

        if (towers.Contains(tower))
        {
            towers.Remove(tower);
        }
    }

    public void RemoveTowerAt(int index)
    {
        if (towers == null || towers.Count == 0) return;

        if (index >= 0 && index < towers.Count)
        {
            towers.RemoveAt(index);
        }
    }

    public bool IsCanPlaceTower()
    {
        if (towerPreview == null) return false;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        MapCell cell = GetCell(mousePos);

        if (cell == null) return false;

        TowerController towerInCell = cell.GetTower();
        int level = towerInCell == null ? 0 : towerInCell.GetLevel() + 1;
        float price = towerPreview.GetTowerInfo().Upgrades[Mathf.Clamp(level, 0, towerPreview.GetTowerInfo().Upgrades.Count - 1)].Price;
        bool isEnoughCoin = GetResources(ResourcesKey.COIN) >= price;
        bool isCanPlaceInCell = cell.IsCanPlaceTower();
        bool isPreviewMatchCell = towerInCell == null ? true : towerInCell.GetTowerID() == towerPreview.GetTowerID();

        return isEnoughCoin && isCanPlaceInCell && isPreviewMatchCell;
    }

    #endregion
    #region Projectile

    public void AddProjectile(Projectile projectile)
    {
        projectiles ??= new List<Projectile>();
        projectiles.Add(projectile);
    }

    public void RemoveProjectile(Projectile projectile)
    {
        if (projectiles == null || projectiles.Count == 0) return;

        if (projectiles.Contains(projectile))
        {
            projectiles.Remove(projectile);
        }
    }

    public void RemoveProjectileAt(int index)
    {
        if (projectiles == null || projectiles.Count == 0) return;

        if (index >= 0 && index < projectiles.Count)
        {
            projectiles.RemoveAt(index);
        }
    }

    public void ClearProjectiles()
    {
        if (projectiles == null || projectiles.Count == 0) return;

        for (int i = projectiles.Count - 1; i >= 0; i--)
        {
            if (projectiles[i] == null)
            {
                projectiles.RemoveAt(i);
                continue;
            }

            Destroy(projectiles[i].gameObject);
        }

        projectiles = new List<Projectile>();
    }

    #endregion
    #region Enemies
    public void AddEnemy(EnemyController enemy)
    {
        enemies ??= new List<EnemyController>();
        enemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyController enemy)
    {
        if (enemies == null || enemies.Count == 0) return;

        if (enemies.Contains(enemy))
        {
            enemies.Remove(enemy);
            IsWaveCompleted();
        }
    }

    public void RemoveEnemyAt(int index)
    {
        if (enemies == null || enemies.Count == 0) return;

        if (index >= 0 && index < enemies.Count)
        {
            enemies.RemoveAt(index);
            IsWaveCompleted();
        }
    }
    #endregion

    #region State
    public GameState GetGameState()
    {
        return gameState;
    }

    public void SetGameState(GameState state)
    {
        gameState = state;
        NotificationService.Instance.Post(GloblaConstants.Noti.ON_UPDATE_GAME_STATE);
    }
    #endregion
    #region Wave & Level
    public int GetCurWave()
    {
        return curWave;
    }

    public void SetCurWave(int wave)
    {
        curWave = wave;
        NotificationService.Instance.Post(GloblaConstants.Noti.ON_UPDATE_WAVE);
    }

    public Level GetLevelData()
    {
        return levelData;
    }

    public bool IsWaveCompleted()
    {
        enemies ??= new List<EnemyController>();
        bool isWaveCompleted = enemies.Count <= 0 && enemySpawner != null && !enemySpawner.IsHaveEnemyToSpawn();
        if (isWaveCompleted) OnWaveComplete();
        return isWaveCompleted;
    }

    public void OnWaveComplete()
    {
        ClearProjectiles();
        if (curWave < levelData.Waves.Count - 1)
        {
            SetCurWave(curWave + 1);
            SetGameState(GameState.PREPARE);
        }
        else // Win Game
        {
            SetGameState(GameState.NONE);
            OnWin();
        }
    }

    #endregion
    #region Calculate Stats

    public Dictionary<Stat, float> ScaleStats(Dictionary<Stat, float> baseStats, float scaleIndex)
    {
        Dictionary<Stat, float> result = new Dictionary<Stat, float>();

        foreach (var stat in baseStats)
        {
            if (stat.Key == Stat.ATK_SPEED)
            {
                result.Add(stat.Key, stat.Value / scaleIndex);
                continue;
            }

            result.Add(stat.Key, stat.Value * scaleIndex);
        }

        return result;
    }

    #endregion
    #region MapCell
    public float GetCellSize()
    {
        return grid.GetCellSize();
    }

    public MapCell GetStartCell()
    {
        return Start_Cell;
    }

    public void SetStartCell(MapCell cell)
    {
        Start_Cell = cell;
    }

    public MapCell GetEndCell()
    {
        return End_Cell;
    }

    public MapCell GetCell(int x, int y)
    {
        if (grid == null)
        {
            Debug.LogError("<color=red>Grid is not set!</color>");
            return null;
        }

        return grid.GetValue(x, y);
    }

    public MapCell GetCell(Vector3 worldPos)
    {
        if (grid == null)
        {
            Debug.LogError("<color=red>Grid is not set!</color>");
            return null;
        }

        return grid.GetValue(worldPos);
    }

    public void SetEndCell(MapCell cell)
    {
        End_Cell = cell;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        if (grid == null)
        {
            Debug.LogError("<color=red>Grid is not set!</color>");
            return Vector3.zero;
        }

        return grid.GetWorldPosition(x, y);
    }

    public void GetGridPos(Vector3 worldPos, out int x, out int y)
    {
        if (grid == null)
        {
            Debug.LogError("<color=red>Grid is not set!</color>");
            x = 0;
            y = 0;
            return;
        }

        grid.GetXY(worldPos, out x, out y);
    }

    public MapCellBG GetMapCellBGPrefab()
    {
        return mapCellBGPrefab;
    }
    #endregion

    public Transform GetGameContainer()
    {
        if (gameContainer == null)
        {
            gameContainer = new GameObject("Game_Container").transform;
            gameContainer.SetParent(transform.parent);

            Debug.Log("<color=yellow>Game Container was not set. Creating a new one.</color>");
        }
        return gameContainer;
    }

    #endregion

    #region Debug
    public bool IsCanShowDebug()
    {
        return showDebug;
    }

    private void OnDrawGizmos()
    {
        if (grid == null || !IsCanShowDebug()) return;

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                Gizmos.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x, y + 1));
                Gizmos.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x + 1, y));
            }
        }

        Gizmos.DrawLine(grid.GetWorldPosition(0, grid.GetHeight()), grid.GetWorldPosition(grid.GetWidth(), grid.GetHeight()));
        Gizmos.DrawLine(grid.GetWorldPosition(grid.GetWidth(), 0), grid.GetWorldPosition(grid.GetWidth(), grid.GetHeight()));
    }

    #endregion
}

