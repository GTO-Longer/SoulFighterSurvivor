using System.ComponentModel;

namespace Classes.Equipments
{
    public class Bloodthirster : Equipment
    {
        private PropertyChangedEventHandler EquipmentEffect;
        private bool effectTriggered;
        public Bloodthirster() : base("Bloodthirster")
        {
            effectTriggered = false;
            
            EquipmentEffect = (_, _) =>
            {
                if (owner.healthPointProportion >= 1 && !effectTriggered)
                {
                    effectTriggered = true;
                    owner._percentageAttackDamageBonus.Value += 0.1f;
                }
                else if(effectTriggered)
                {
                    effectTriggered = false;
                    owner._percentageAttackDamageBonus.Value -= 0.1f;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner.healthPointProportion.PropertyChanged += EquipmentEffect;
            EquipmentEffect?.Invoke(null, null);
        }

        public override void OnEquipmentRemove()
        {
            owner.healthPointProportion.PropertyChanged -= EquipmentEffect;
            if (effectTriggered)
            {
                effectTriggered = false;
                owner._percentageAttackDamageBonus.Value -= 0.1f;
            }
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}