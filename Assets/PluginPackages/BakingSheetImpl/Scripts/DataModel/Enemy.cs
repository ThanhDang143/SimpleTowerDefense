using System.Collections.Generic;
using Cathei.BakingSheet;

namespace BakingSheetImpl
{
    public class Enemy : BaseModel
    {
        public string PrefabAddress { get; set; }
        public int Reward { get; set; }
        public Dictionary<Stat, float> Stats { get; set; }
    }

    public class EnemySheet : Sheet<Enemy> { }
}
