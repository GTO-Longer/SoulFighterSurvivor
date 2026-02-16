using System.Collections.Generic;
using System.Linq;
using DataManagement;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
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
        private TweenerCore<Vector3, Vector3, VectorOptions> tweener;
        
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
                        if (!key.isAlive)
                        {
                            EffectManager.Instance.DestroyEffect(entityCDEffect[key]);
                            entityCDTimer.Remove(key);
                            entityCDEffect.Remove(key);
                            continue;
                        }
                        
                        if (entityCDTimer[key] > 0)
                        {
                            entityCDTimer[key] -= Time.deltaTime;
                            entityCDEffect[key].effect.gameObject.GetComponent<Image>().fillAmount = entityCDTimer[key] / _targetCD;
                        }
                        
                        if (entityCDTimer[key] <= 0)
                        {
                            EffectManager.Instance.DestroyEffect(entityCDEffect[key]);
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

        public override bool SkillEffect(out string failMessage)
        {
            failMessage = string.Empty;
            
            if (!ToolFunctions.IsObjectAtMousePoint(out var obj, "Enemy", true) || isSweeping)
            {
                failMessage = "无有效目标";    
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
                if (target != null && Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) > skillRange && tweener == null && !entityCDTimer.ContainsKey(target))
                {
                    tweener = Async.SetAsync(20, null, () =>
                    {
                        owner.agent.SetDestination(target.gameObject.transform.position);

                        if (Input.GetMouseButton(1))
                        {
                            tweener.Kill();
                            tweener = null;
                        }
                        
                        if (owner.canMove && Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) <= skillRange && target.isAlive && !entityCDTimer.ContainsKey(target))
                        {
                            Sweep(target);
                            tweener.Kill();
                            tweener = null;
                        }
                    });
                }

                failMessage = "无有效目标";
                return false;
            }
            
            Sweep(target);

            return true;
        }

        private void Sweep(Entity target)
        {
            HeroModelManager.Instance.ESkillAnimation();
            
            // 当QCD小于0.6秒时可以刷新CD直接使用
            var steelTempest = owner.skillList[(int)SkillType.QSkill] as SteelTempest;
            if (steelTempest != null && steelTempest.actualSkillCoolDown - steelTempest.coolDownTimer <= 0.6f)
            {
                steelTempest.coolDownTimer = steelTempest.actualSkillCoolDown;
            }
            
            var direction = (target.gameObject.transform.position - owner.gameObject.transform.position).normalized;
            owner.Dash(destinationDistance, dashDuration, direction, () =>
            {
                isSweeping = false;
            }, () =>
            {
                if (!isSweeping)
                {
                    isSweeping = true;
                    AudioManager.Instance.Play("Hero/Yasuo/E_OnCast", "Yasuo_E_OnCast");
                }

                owner.energy.Value += Time.deltaTime * 50;
                
                if (Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) < 100)
                {
                    if (!entityCDTimer.ContainsKey(target) && target.isAlive)
                    {
                        AudioManager.Instance.Play("Hero/Yasuo/E_OnHit", "Yasuo_E_OnHit");
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
            }, true, true);
        }
    }
}