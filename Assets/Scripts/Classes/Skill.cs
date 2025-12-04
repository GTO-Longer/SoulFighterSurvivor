using System;
using System.Collections.Generic;
using Classes.Entities;
using DataManagement;
using Managers.EntityManagers;
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
        public float skillCost => _baseSkillCost[Math.Max(0, _skillLevel - 1)];
        public float skillRange => _skillRange;

        public float actualSkillCoolDown => skillType is SkillType.DSkill or SkillType.FSkill
            ? _baseSkillCoolDown[0]
            : specialCoolDown != 0
                ? specialCoolDown
                : _baseSkillCoolDown[Math.Max(0, _skillLevel - 1)] * owner.actualAbilityCooldown;
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
        public event Action OnSkillEnterCoolDown; 
        
        protected int _skillLevel;
        protected int _maxSkillLevel = 0;
        protected int skillLevelToIndex => Math.Max(0, _skillLevel - 1);

        public int skillChargeCount = 0;
        public int maxSkillChargeCount = 0;
        
        public Image skillCoolDownMask;
        public Image skillIcon;
        public TMP_Text skillCD;
        public TMP_Text skillCharge;
        public Button upgradeButton;
        public Transform levelBar;

        /// <summary>
        /// 技能冷却完成百分比
        /// </summary>
        public float skillCoolDownProportion => 1 - Mathf.Min(actualSkillCoolDown == 0 ? 1 : coolDownTimer / actualSkillCoolDown, 1);
        
        protected string _skillDescription;
        public SkillType skillType;
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
            
            skillIcon = GameObject.Find($"HUD/HeroAttributesHUD/MainStateBackground/SkillBarBackground/{skillType.ToString()}/SkillIcon").GetComponent<Image>();
            skillCoolDownMask = GameObject.Find($"HUD/HeroAttributesHUD/MainStateBackground/SkillBarBackground/{skillType.ToString()}/CDMask")?.GetComponent<Image>();
            skillCD = GameObject.Find($"HUD/HeroAttributesHUD/MainStateBackground/SkillBarBackground/{skillType.ToString()}/SkillCD")?.GetComponent<TMP_Text>();
            skillCharge = GameObject.Find($"HUD/HeroAttributesHUD/MainStateBackground/SkillBarBackground/{skillType.ToString()}/SkillCharge")?.GetComponent<TMP_Text>();
            upgradeButton = GameObject.Find($"HUD/HeroAttributesHUD/MainStateBackground/SkillBarBackground/{skillType.ToString()}/UpgradeButton")?.GetComponent<Button>();
            levelBar = GameObject.Find($"HUD/HeroAttributesHUD/MainStateBackground/SkillBarBackground/{skillType.ToString()}/LevelBar")?.transform;
            
            skillIcon.sprite = ResourceReader.ReadIcon(name);
        }
        
        protected void ReadSkillConfig(string name)
        {
            var config = ResourceReader.ReadSkillConfig(name);
            heroName = config.heroName;
            skillName = config.skillName;

            coolDownTimer = 0;
            
            _skillDescription = config._skillDescription;
            skillType = (SkillType)Enum.Parse(typeof(SkillType), config._skillType);
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
                owner._skillPoint -= 1;
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

        // 更新技能UI
        public void UpdateSkillUI()
        {
            // 设置技能冷却mask和文字
            if (skillCoolDownMask != null && skillCD != null)
            {
                skillCoolDownMask.fillAmount = skillCoolDownProportion;
                skillCD.text = actualSkillCoolDown - coolDownTimer >= 1 ? $"{actualSkillCoolDown - coolDownTimer:F0}" : $"{actualSkillCoolDown - coolDownTimer:F1}";
                skillCD.gameObject.SetActive(skillCoolDownProportion > 0);
                skillCharge.text = skillChargeCount.ToString();
                skillCharge.gameObject.SetActive(maxSkillChargeCount > 0);
            }
                    
            if (skillType is >= SkillType.QSkill and <= SkillType.RSkill)
            {
                upgradeButton.gameObject.SetActive(HeroManager.hero._skillPoint > 0);
                upgradeButton.interactable = SkillCanUpgrade();
                skillIcon.material = skillChargeCount <= 0 && maxSkillChargeCount > 0 || skillLevel == 0 ? Resources.Load<Material>("Materials/ImageGreyTexture") : null;
            }
            
            // 设置技能等级
            if (skillType is >= SkillType.QSkill and <= SkillType.RSkill)
            {
                for (var index = 0; index < levelBar.childCount; index++)
                {
                    if (index < _skillLevel)
                    {
                        levelBar.GetChild(index).GetComponent<Image>().color = new Color(1, 0.6f, 0);
                    }
                    else
                    {
                        levelBar.GetChild(index).GetComponent<Image>().color = new Color(0.425f, 0.425f, 0.425f);
                    }
                }
            }
        }

        /// <summary>
        /// 技能进入冷却
        /// </summary>
        public void SkillEnterCoolDown()
        {
            OnSkillEnterCoolDown?.Invoke();
        }
    }
}
