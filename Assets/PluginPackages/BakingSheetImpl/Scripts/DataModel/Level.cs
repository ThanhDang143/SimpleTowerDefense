using System.Collections.Generic;
using BakingSheetImpl;
using Cathei.BakingSheet;

namespace BakingSheetImpl
{
    public class Level : BaseModel
    {
        public float TotalHP { get; set; }
        public int StartCoin { get; set; }
        public string MapDataAddress { get; set; }
        public List<TowerSheet.Reference> Towers { get; set; }
        public List<Wave> Waves { get; set; }
    }

    public class LevelSheet : Sheet<Level> { }
}

public struct Wave
{
    public List<EnemyInWave> Enemies { get; set; }
    public int Reward { get; set; }
}

public struct EnemyInWave
{
    public EnemySheet.Reference Enemy { get; set; }
    public float SpawnRate { get; set; }
    public int SpawnCount { get; set; }
    public float ScaleIndex { get; set; }
}
