using System.Collections;
using System.Collections.Generic;
using AddressableImpl;
using BakingSheetImpl;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private List<EnemyInfo> enemies;
    public void ManualUpdate()
    {
        if (enemies == null || enemies.Count == 0) return;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].SpawnRemain <= 0)
            {
                enemies.RemoveAt(i);
                continue;
            }

            enemies[i].SpawnCD -= Time.deltaTime;
            if (enemies[i].SpawnCD <= 0)
            {
                enemies[i].SpawnCD = enemies[i].baseData.SpawnRate;
                enemies[i].SpawnRemain--;

                Vector3 ePos = GameManager.Instance.GetStartCell().GetWorldPosition();
                GameObject enemy = Instantiate(enemies[i].Prefab, ePos, Quaternion.identity, GameManager.Instance.GetGameContainer());
                if (enemy.TryGetComponent(out EnemyController controller))
                {
                    controller.Setup(enemies[i].baseData);
                }
                else
                {
                    enemy.AddComponent<EnemyController>().Setup(enemies[i].baseData);
                }
            }
        }
    }

    public bool IsHaveEnemyToSpawn()
    {
        return enemies != null && enemies.Count > 0;
    }

    public void LoadEnemies()
    {
        enemies = new List<EnemyInfo>();

        // Load enemies from sheet
        Level levelData = GameManager.Instance.GetLevelData();
        int curWave = GameManager.Instance.GetCurWave();

        foreach (EnemyInWave e in levelData.Waves[curWave].Enemies)
        {
            EnemyInfo info = new EnemyInfo()
            {
                baseData = e,
                SpawnCD = e.SpawnRate,
                SpawnRemain = e.SpawnCount
            };

            // Load Enemy Prefab
            AddressableManager.LoadAssetAsync<GameObject>(e.Enemy.Ref.PrefabAddress, (result) =>
            {
                info.Prefab = result;
            });

            enemies.Add(info);
        }
    }

    public class EnemyInfo
    {
        public EnemyInWave baseData;
        public GameObject Prefab;
        public float SpawnCD;
        public int SpawnRemain;
    }
}
