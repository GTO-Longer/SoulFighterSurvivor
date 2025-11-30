using System;
using System.Collections.Generic;
using Classes.Entities;
using DataManagement;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Classes{
    public class Skill
    {
        public string skillName;
        public string heroName;
        public Hero owner;
        public int skillLevel => _skillLevel;
        public SkillType skillType => _skillType;
        public float skillCost => _baseSkillCost[Math.Max(0, _skillLevel - 1)];
        public float skillRange => _skillRange;
        public float actualSkillCoolDown => specialCoolDown != 0 ? specialCoolDown :_baseSkillCoolDown[Math.Max(0, _skillLevel - 1)] * owner.actualAbilityCooldown;
        public float actualSkillCost => _baseSkillCost[Math.Max(0, _skillLevel - 1)];
        public float bulletWidth => _bulletWidth;
        public float bulletSpeed => _bulletSpeed;
        public float destinationDistance => _destinationDistance;
        public float coolDownTimer;
        public float specialTimer;
        /// <summary>
        /// 特殊CD（比如内置CD）
        /// </summary>
        public float specialCoolDown = 0;
        public event Action OnSpecialTimeOut; 
        
        protected int _skillLevel = 0;
        protected int _maxSkillLevel = 0;
        protected int skillLevelToIndex => Math.Max(0, _skillLevel - 1);

        public int skillChargeCount = 0;
        public int maxSkillChargeCount = 0;
        
        public Image skillCoolDownMask;
        public Image skillIcon;
        public TMP_Text skillCD;
        public Button upgradeButton;

        /// <summary>
        /// 技能冷却完成百分比
        /// </summary>
        public float skillCoolDownProportion => 1 - Mathf.Min(actualSkillCoolDown == 0 ? 1 : coolDownTimer / actualSkillCoolDown, 1);
        
        protected string _skillDescription;
        protected SkillType _skillType;
        protected float[] _baseSkillCost;
        protected float[] _baseSkillCoolDown;
        protected List<List<float>> _baseSkillValue;
        protected float _skillRange;
        protected BulletType[] _skillBulletType;
        protected float _castTime;
        protected float _bulletWidth;
        protected float _bulletSpeed;
        protected float _destinationDistance;

        protected Skill(string name)
        {
            ReadSkillConfig(name);
            
            skillIcon = GameObject.Find($"HUD/HeroAttributesHUD/MainStateBackground/SkillBarBackground/{_skillType.ToString()}/SkillIcon").GetComponent<Image>();
            skillCoolDownMask = GameObject.Find($"HUD/HeroAttributesHUD/MainStateBackground/SkillBarBackground/{_skillType.ToString()}/CDMask")?.GetComponent<Image>();
            skillCD = GameObject.Find($"HUD/HeroAttributesHUD/MainStateBackground/SkillBarBackground/{_skillType.ToString()}/SkillCD")?.GetComponent<TMP_Text>();
            upgradeButton = GameObject.Find($"HUD/HeroAttributesHUD/MainStateBackground/SkillBarBackground/{_skillType.ToString()}/UpgradeButton")?.GetComponent<Button>();
            
            skillIcon.sprite = ResourceReader.ReadIcon(name);
            if (skillType is >= SkillType.QSkill and <= SkillType.RSkill)
            {
                skillIcon.material = Resources.Load<Material>("Materials/ImageGreyTexture");
            }
        }
        
        protected void ReadSkillConfig(string name)
        {
            var config = ResourceReader.ReadSkillConfig(name);
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

        public void SkillUpgrade()
        {
            if (SkillCanUpgrade())
            {
                skillIcon.material = null;
                _skillLevel++;
            }
        }

        public bool SkillCanUpgrade()
        {
            if (skillType == SkillType.RSkill)
            {
                return _skillLevel < _maxSkillLevel && (_skillLevel + 1) * 5 < owner.level;
            }
            if(skillType is >= SkillType.QSkill and <= SkillType.ESkill)
            {
                return _skillLevel < _maxSkillLevel && (_skillLevel + 1) * 2 < owner.level + 2;
            }
            
            return false;
        }

        public virtual string GetDescription()
        {
            return "";
        }

        public virtual void SkillEffect() { }

        public void SpecialTimeOut()
        {
            OnSpecialTimeOut?.Invoke();
        }
    }
}
