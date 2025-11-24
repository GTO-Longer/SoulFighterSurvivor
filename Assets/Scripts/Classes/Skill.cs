using System;
using System.Collections.Generic;
using Classes.Entities;
using DataManagement;
using Utilities;

namespace Classes{
    public class Skill
    {
        public string skillName;
        public string heroName;
        public Hero owner;
        public int skillLevel => _skillLevel;
        public float skillCost => _baseSkillCost[skillLevel - 1];
        public float actualSkillRange => _skillRange / 100f;
        public float actualSkillCoolDown => _baseSkillCoolDown[_skillLevel] * owner.actualAbilityCooldown;
        public float bulletWidth => _bulletWidth / 100f;
        public float bulletSpeed => _bulletSpeed / 100f;
        public float destinationDistance => _destinationDistance / 100f;
        public float coolDownTimer;
        
        protected int _skillLevel = 0;
        protected int _maxSkillLevel = 0;
        
        protected string _skillDescription;
        protected SkillType _skillType;
        protected float[] _baseSkillCost;
        protected float[] _baseSkillCoolDown;
        protected List<List<float>> _baseSkillValue;
        protected float _skillRange;
        protected BulletType[] _skillBulletType;
        protected SkillUsageType[] _skillUsageType;
        protected float _castTime;
        protected float _bulletWidth;
        protected float _bulletSpeed;
        protected float _destinationDistance;

        protected void ReadSkillConfig(string name)
        {
            var config = ConfigReader.ReadSkillConfig(name);
            heroName = config.heroName;
            skillName = config.skillName;

            coolDownTimer = 0;
            
            _skillDescription = config._skillDescription;
            _skillType = (SkillType)Enum.Parse(typeof(SkillType), config._skillType);
            _baseSkillCost = config._baseSkillCost;
            _baseSkillCoolDown = config._baseSkillCoolDown;
            _baseSkillValue = config._baseSkillValue;
            _skillRange = config._skillRange;

            _castTime = config._castTime;
            _bulletWidth = config._bulletWidth;
            _bulletSpeed = config._bulletSpeed;
            _destinationDistance = config._destinationDistance;
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

        public virtual string GetDescription()
        {
            return "";
        }

        public virtual void SkillEffect() { }
    }
}
