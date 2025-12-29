using System.Collections.Generic;
using System.Linq;
using DataManagement;
using Managers;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Classes.Skills
{
    public class SweepingBlade : Skill
    {
        private const float dashDuration = 0.5f;
        private float _damage => _baseSkillValue[0][skillLevelToIndex] + 0.2f * owner.attackDamage + 0.6f * owner.abilityPower;
        private float _targetCD => _baseSkillValue[1][skillLevelToIndex];
        private int continuousReleaseCount;
        private const float continuousReleaseTime = 5;
        private float continuousReleaseTimer;
        private Dictionary<Entity, float> entityCDTimer;
        private Dictionary<Entity, Effect> entityCDEffect;
        public bool isSweeping;
        
        public SweepingBlade() : base("SweepingBlade")
        {
            _skillLevel = 0;
            _maxSkillLevel = 5;
            
            coolDownTimer = 999;

            entityCDTimer = new Dictionary<Entity, float>();
            entityCDEffect = new Dictionary<Entity, Effect>();
            
            PassiveAbilityEffective += () =>
            {
                owner.EntityUpdateEvent += (_) =>
                {
                    // 连续释放计时器
                    if (continuousReleaseTimer > 0)
                    {
                        continuousReleaseTimer -= Time.deltaTime;
                    }
                    else
                    {
                        continuousReleaseCount = 0;
                    }

                    // 实体CD计时器
                    foreach (var key in entityCDTimer.Keys.ToList())
                    {
                        if (entityCDTimer[key] > 0)
                        {
                            entityCDTimer[key] -= Time.deltaTime;
                            entityCDEffect[key].effect.gameObject.GetComponent<Image>().fillAmount = entityCDTimer[key] / _targetCD;
                        }
                        
                        if (entityCDTimer[key] <= 0)
                        {
                            entityCDTimer.Remove(key);
                            entityCDEffect.Remove(key);
                        }
                    }
                };
            };
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription, _damage, _targetCD);
        }

        public override bool SkillEffect()
        {
            if (!ToolFunctions.IsObjectAtMousePoint(out var obj, "Enemy", true))
            {
                return false;
            }
            
            Entity target = null;
            
            foreach (var entity in obj)
            {
                if (entity.GetComponent<EntityData>())
                {
                    target = entity.GetComponent<EntityData>().entity;
                    break;
                }
            }

            if (target == null || Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) > skillRange || entityCDTimer.ContainsKey(target))
            {
                return false;
            }
            
            var direction = (target.gameObject.transform.position - owner.gameObject.transform.position).normalized;

            owner.Dash(destinationDistance, dashDuration, direction, () =>
            {
                isSweeping = false;
            }, () =>
            {
                isSweeping = true;
                
                if (Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) < 100)
                {
                    if (!entityCDTimer.ContainsKey(target))
                    {
                        entityCDTimer.Add(target, _targetCD);
                        entityCDEffect.Add(target, EffectManager.Instance.CreateCanvasEffect("SweepingBladeCD", target.gameObject));
                        
                        var damageCount = target.CalculateAPDamage(owner, _damage * (1 + 0.25f * continuousReleaseCount));
                        target.TakeDamage(damageCount, DamageType.AP, owner, Random.Range(0f, 1f) < owner.criticalRate.Value && owner.canSkillCritical);

                        // 造成技能特效
                        owner.AbilityEffectActivate(target, damageCount, this);

                        // 刷新重复释放计数
                        if (continuousReleaseCount < 4)
                        {
                            continuousReleaseCount += 1;
                        }
                        continuousReleaseTimer = continuousReleaseTime;
                    }
                }
            }, true);

            return true;
        }
    }
}