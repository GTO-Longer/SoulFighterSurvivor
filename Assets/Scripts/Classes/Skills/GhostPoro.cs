using Managers;

namespace Classes.Skills
{
    public class GhostPoro : Skill
    {
        public GhostPoro() : base("GhostPoro")
        {
            _skillLevel = 1;
            _maxSkillLevel = 1;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription);
        }

        public override bool SkillEffect(out string failMessage)
        {
            failMessage = string.Empty;
            
            AudioManager.Instance.Play("SummonerSkills/GhostPoro", "GhostPoro");
            
            var ghostPoro = new Buffs.GhostPoro(owner, owner);
            ghostPoro.buffIcon = skillIcon.sprite;
            owner.GainBuff(ghostPoro);
            coolDownTimer = 0;

            return true;
        }
    }
}
