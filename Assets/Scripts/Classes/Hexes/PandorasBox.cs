using Managers;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Hexes
{
    public class PandorasBox : Hex
    {
        public PandorasBox() : base("PandorasBox")
        {
            
        }

        public override void OnHexGet(Entity entity)
        {
            canChoose = false;
            
            var hexCount = HeroManager.hero.hexList.Count;
            HeroManager.hero.RemoveAllHex();
            ToolFunctions.GetRandomUniqueItems(HexManager.Instance.hexList.FindAll(hex => hex.canChoose && hex.hexQuality == Quality.Prismatic), hexCount, out var results);
            for (var index = 0; index < hexCount; index++)
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