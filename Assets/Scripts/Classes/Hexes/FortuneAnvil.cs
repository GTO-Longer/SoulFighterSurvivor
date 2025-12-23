using Managers;
using Managers.EntityManagers;

namespace Classes.Hexes
{
    public class FortuneAnvil : Hex
    {
        public FortuneAnvil() : base("FortuneAnvil")
        {
            
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            if (!hexGetEventTriggered)
            {
                HeroManager.hero.coins.Value += 1000;
                hexGetEventTriggered = true;
            }

            EquipmentManager.Instance.GetEquipment("AttributeAnvil")._cost -= 100;
        }

        public override void OnHexRemove()
        {
            EquipmentManager.Instance.GetEquipment("AttributeAnvil")._cost += 100;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail);
            return true;
        }
    }
}