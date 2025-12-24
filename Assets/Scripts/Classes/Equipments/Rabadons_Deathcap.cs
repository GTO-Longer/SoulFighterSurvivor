namespace Classes.Equipments
{
    public class Rabadons_Deathcap : Equipment
    {
        public Rabadons_Deathcap() : base("Rabadons_Deathcap")
        {
            
        }

        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);
            owner._percentageAbilityPowerBonus.Value += 0.3f;
        }

        public override void OnEquipmentRemove()
        {
            owner._percentageAbilityPowerBonus.Value -= 0.3f;
            base.OnEquipmentRemove();
        }

        public override bool GetPassiveSkillDescription(out string description)
        {
            description = string.Format(_passiveSkillName + "\n" + _passiveSkillDescription);
            return true;
        }
    }
}