using System.Collections.Generic;
using Cathei.BakingSheet;
using Cathei.BakingSheet.Unity;
using Sirenix.Utilities;

namespace BakingSheetImpl
{
    public class SheetContainer : SheetContainerBase
    {
        public SheetContainer() : base(UnityLogger.Default) { }

        #region Declare
        public TowerSheet Towers { get; private set; }
        public EnemySheet Enemies { get; private set; }
        public LevelSheet Levels { get; private set; }
        #endregion

        public Dictionary<string, IDataModel> CacheData()
        {
            Dictionary<string, IDataModel> result = new();

            #region CacheData
            Towers.ForEach(d => result.Add(d.Id, d));
            Enemies.ForEach(d => result.Add(d.Id, d));
            Levels.ForEach(d => result.Add(d.Id, d));
            #endregion

            return result;
        }
    }
}