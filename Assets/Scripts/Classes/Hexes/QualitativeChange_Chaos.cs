using Managers;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Hexes
{
    public class QualitativeChange_Chaos : Hex
    {
        public QualitativeChange_Chaos() : base("QualitativeChange_Chaos")
        {
            
        }

        public override void OnHexGet(Entity entity)
        {
            canChoose = false;
            HeroManager.hero.RemoveHex(this);
            ToolFunctions.GetRandomUniqueItems(HexManager.Instance.hexList.FindAll(hex => hex.canChoose && !HeroManager.hero.hexList.Contains(hex)), 2, out var results);
            for (var index = 0; index < 2; index++)
            {
                Debug.Log("已质变：" + results[index].hexName);
                HeroManager.hero.GetHex(results[index]);
            }
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