using System;
using DataManagement;
using Utilities;

namespace Classes{
    public class Skill
    {
        public string skillName;
        public string heroName;
        public int skillLevel => _skillLevel;
        public float skillCost => _baseSkillCost[skillLevel - 1];
        
        protected Entity _entity;
        protected int _skillLevel = 0;
        protected int _maxSkillLevel = 0;
        
        protected string _skillDescription;
        protected SkillType _skillType;
        protected float[] _baseSkillCost;
        protected float[] _baseSkillCoolDown;
        protected float[][] _baseSkillValue;
        protected float _skillRange;
        protected BulletType[] _skillBulletType;
        protected SkillUsageType[] _skillUsageType;

        protected void ReadSkillConfig(string name)
        {
            var config = ConfigReader.ReadSkillConfig(name);
            heroName = config.heroName;
            skillName = config.skillName;
            
            _skillDescription = config._skillDescription;
            _skillType = (SkillType)Enum.Parse(typeof(SkillType), config._skillType);
            _baseSkillCost = config._baseSkillCost;
            _baseSkillCoolDown = config._baseSkillCoolDown;
            _baseSkillValue = config._baseSkillValue;
            _skillRange = config._skillRange;
        }

        public bool SkillUpdate()
        {
            if (_skillLevel < _maxSkillLevel)
            {
                _skillLevel++;
                return true;
            }

            return false;
        }
    }
}
