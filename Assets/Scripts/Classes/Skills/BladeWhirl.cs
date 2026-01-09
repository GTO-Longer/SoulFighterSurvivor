using Classes.Buffs;
using Factories;
using Managers;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class BladeWhirl : Skill
    {
        private float _damage => _baseSkillValue[0][skillLevelToIndex] + 0.6f * owner.attackDamage;
        public BladeWhirl() : base("BladeWhirl")
        {
            _skillLevel = 0;
            _maxSkillLevel = 5;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription, _damage);
        }

        public override bool SkillEffect(out string failMessage)
        {
            failMessage = string.Empty;

            // 吟唱时间
            Async.SetAsync(_castTime, null, null, () =>
            {
                var bladeWhirl = BulletFactory.Instance.CreateBullet(owner);
                var duration = 0.75f;
                bladeWhirl.OnBulletAwake += (self) =>
                {
                    self.target = null;
                    
                    self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    self.gameObject.SetActive(true);
                    self.bulletDamageCD = 0.7f;
                
                    // 创建环形斩击特效
                    var effect = EffectManager.Instance.CreateEffect("BladeWhirl", bladeWhirl.gameObject);
                    effect.EffectUpdateEvent += () =>
                    {
                        effect.effect.transform.rotation = bladeWhirl.gameObject.transform.rotation;
                    };

                    // 自定义每帧更新逻辑
                    self.OnBulletUpdate += (_) =>
                    {
                        self.gameObject.transform.position = owner.gameObject.transform.position;

                        // 持续时间结束
                        if (duration <= 0)
                        {
                            EffectManager.Instance.DestroyEffect(effect);
                            self.Destroy();
                        }
                        else
                        {
                            duration -= Time.deltaTime;
                        }
                        
                        // 检测是否有敌人进入环形斩击区域
                        var targets = ToolFunctions.IsOverlappingWithOtherTagAll(owner.gameObject, bulletWidth);
                        if (targets == null) return;
                        foreach (var target in targets)
                        {
                            if (target.isAlive && target.team != owner.team)
                            {
                                self.bulletEntityDamageCDTimer.TryAdd(target, self.bulletDamageCD);

                                if (self.bulletEntityDamageCDTimer[target] > self.bulletDamageCD)
                                {
                                    self.bulletEntityDamageCDTimer[target] = 0;

                                    var damageValue = target.CalculateADDamage(self.owner, _damage);
                                    target.TakeDamage(damageValue, DamageType.AD, owner, Random.Range(0f, 1f) < owner.criticalRate.Value && owner.canSkillCritical);
                                    
                                    // 造成0.7秒50%减速
                                    var speedReduce = new SpeedReduce(target, owner, 0.7f, 0.5f);
                                    target.GainBuff(speedReduce);
                                    
                                    // 造成技能特效
                                    self.owner.AbilityEffectActivate(target, damageValue, this);
                                }
                            }
                        }
                    };
                };

                bladeWhirl.Awake();
            });

            return true;
        }
    }
}