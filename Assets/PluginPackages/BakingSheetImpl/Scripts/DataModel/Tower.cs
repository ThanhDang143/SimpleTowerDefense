using System.Collections.Generic;
using Cathei.BakingSheet;
using Cathei.BakingSheet.Unity;

namespace BakingSheetImpl
{
    public class Tower : BaseModel
    {
        public string PrefabAddress { get; set; }
        public Dictionary<Stat, float> Stats { get; set; }
        public List<TowerInfo> Upgrades { get; set; }

        public float GetStat(Stat stat)
        {
            if (!Stats.ContainsKey(stat))
            {
                Stats.Add(stat, 0);
            }

            return Stats[stat];
        }
    }

    public class TowerSheet : Sheet<Tower> { }
}

public struct TowerInfo
{
    public DirectAssetPath Icon { get; set; }
    public int Price { get; set; }
    public float PowerMultiplier { get; set; }
    public string ProjectileAddress { get; set; }
}
