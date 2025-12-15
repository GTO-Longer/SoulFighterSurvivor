namespace Classes.Skills
{
    public class GoustPoro : Skill
    {
        public GoustPoro() : base("GoustPoro")
        {
            _skillLevel = 1;
            _maxSkillLevel = 1;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription);
        }

        public override void SkillEffect()
        {
            var goustPoro = new Buffs.GoustPoro(owner, owner);
            owner.GetBuff(goustPoro);
            coolDownTimer = 0;
        }
    }
}
