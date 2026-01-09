using System.Collections.Generic;
using DataManagement;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class WildRush : Skill
    {
        private const float dashDuration = 0.5f;
        private float _damage => _baseSkillValue[0][skillLevelToIndex] + 0.2f * owner.attackDamage;
        private float _attackSpeedBonus => _baseSkillValue[1][skillLevelToIndex];
        public bool isRushing;
        private TweenerCore<Vector3, Vector3, VectorOptions> tweener;
        
        public WildRush() : base("WildRush")
        {
            _skillLevel = 0;
            _maxSkillLevel = 5;
            
            coolDownTimer = 999;

            PassiveAbilityEffective += () =>
            {
                owner.OnKillEntity += (_, _) =>
                {
                    coolDownTimer = 999f;
                };
            };
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription, _damage, _attackSpeedBonus);
        }

        public override bool SkillEffect(out string failMessage)
        {
            failMessage = string.Empty;
            
            if (!ToolFunctions.IsObjectAtMousePoint(out var obj, "Enemy", true) || isRushing)
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

            if (target == null || Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) > skillRange)
            {
                if (target != null && Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) > skillRange && tweener == null)
                {
                    tweener = Async.SetAsync(20, null, () =>
                    {
                        owner.agent.SetDestination(target.gameObject.transform.position);

                        if (Input.GetMouseButton(1))
                        {
                            tweener.Kill();
                            tweener = null;
                        }
                        
                        if (owner.canMove && Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) <= skillRange && target.isAlive)
                        {
                            Rush(target);
                            tweener.Kill();
                            tweener = null;
                        }
                    });
                }

                failMessage = "无有效目标";
                return false;
            }
            
            Rush(target);

            return true;
        }

        private void Rush(Entity target)
        {
            var direction = (target.gameObject.transform.position - owner.gameObject.transform.position).normalized;
            var damagedEntity = new List<Entity>();
            owner.Dash(destinationDistance, dashDuration, direction, () =>
            {
                // 获得攻速增益
                var wildRush = new Buffs.WildRush(owner, owner, _attackSpeedBonus);
                owner.GainBuff(wildRush);
                
                damagedEntity.Clear();
                isRushing = false;
            }, () =>
            {
                if (!isRushing)
                {
                    isRushing = true;
                }
                
                if (Vector2.Distance(owner.gameObject.transform.position, target.gameObject.transform.position) < 100)
                {
                    if (!damagedEntity.Contains(target))
                    {
                        damagedEntity.Add(target);

                        var damageCount = target.CalculateAPDamage(owner, _damage);
                        target.TakeDamage(damageCount, DamageType.AP, owner,
                            Random.Range(0f, 1f) < owner.criticalRate.Value && owner.canSkillCritical);

                        // 造成技能特效
                        owner.AbilityEffectActivate(target, damageCount, this);
                    }
                }
            }, true, true);
        }
    }
}