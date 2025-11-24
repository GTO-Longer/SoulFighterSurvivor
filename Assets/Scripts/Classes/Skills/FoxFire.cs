using System;
using System.Collections.Generic;
using Factories;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class FoxFire : Skill
    {
        private float _firstDamage => _baseSkillValue[0][Math.Max(0, _skillLevel - 1)] + 0.4f * owner.abilityPower;
        private float _secondDamage => _baseSkillValue[1][Math.Max(0, _skillLevel - 1)] + 0.16f * owner.abilityPower;
        
        public FoxFire()
        {
            ReadSkillConfig("FoxFire");
            
            _skillLevel = 0;
            _maxSkillLevel = 5;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription,
                _firstDamage, _secondDamage);
        }

        public override void SkillEffect()
        {
            Debug.Log(skillName + ": Skill effective");
            owner.OnWSkillRelease += (_, _) =>
            {
                if (_skillLevel < 0)
                {
                    Debug.Log("Skill level too low to use.");
                    return;
                }
                
                if (_baseSkillCost[_skillLevel] > owner.magicPoint)
                {
                    Debug.Log("Magic point too low to use.");
                    return;
                }

                owner.magicPoint.Value -= _baseSkillCost[_skillLevel];
                var foxFire = new List<Bullet>();
                foxFire.Add(BulletFactory.Instance.CreateBullet(owner));
                foxFire.Add(BulletFactory.Instance.CreateBullet(owner));
                foxFire.Add(BulletFactory.Instance.CreateBullet(owner));

                foreach (var fire in foxFire)
                {
                    fire.OnBulletAwake += (self) =>
                    {
                        self.target = null;
                        self.gameObject.transform.position = owner.gameObject.transform.position;
                        self.gameObject.SetActive(true);

                        // 自定义每帧更新逻辑
                        self.OnBulletUpdate += (bullet) =>
                        {
                        
                        };
                    };

                    fire.OnBulletHit += (self) =>
                    {
                        // 第一段造成魔法伤害
                        self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, _firstDamage));
                    
                        // 造成技能特效
                        self.AbilityEffectActivate();
                    };

                    fire.Awake();
                }
            };
        }
    }
}