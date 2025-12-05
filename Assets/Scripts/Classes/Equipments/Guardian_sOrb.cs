using System;

namespace Classes.Equipments
{
    public class Guardian_sOrb : Equipment
    {
        private Action<Entity> equipmentEffect;
        public Guardian_sOrb() : base("Guardian_sOrb")
        {
            equipmentEffect = (hero) =>
            {
                if (_passiveSkillActive)
                {
                    hero.TakeMagicRecover(35);
                    _passiveSkillCDTimer = 0;
                }
            };
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);

            owner.EntityUpdateEvent += equipmentEffect;
            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner.EntityUpdateEvent -= equipmentEffect;
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n冷却时间:" + _passiveSkillCD + "秒\n" + _passiveSkillDescription);
            return true;
        }
    }
}