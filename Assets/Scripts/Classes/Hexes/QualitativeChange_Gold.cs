using Managers;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Hexes
{
    public class QualitativeChange_Gold : Hex
    {
        public QualitativeChange_Gold() : base("QualitativeChange_Gold")
        {
            
        }

        public override void OnHexGet(Entity entity)
        {
            canChoose = false;
            HeroManager.hero.RemoveHex(this);
            ToolFunctions.GetRandomUniqueItems(HexManager.Instance.hexList.FindAll(hex => hex.canChoose && !HeroManager.hero.hexList.Contains(hex) && hex.hexQuality == Quality.Gold), 1, out var results);
            Debug.Log("已质变：" + results[0].hexName);
            HeroManager.hero.GetHex(results[0]);
        }

        public override void OnHexRemove()
        {
            
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}