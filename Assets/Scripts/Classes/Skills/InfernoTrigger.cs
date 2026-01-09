using Classes.Buffs;
using Factories;
using Managers;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class InfernoTrigger : Skill
    {
        private float _damage => _baseSkillValue[0][skillLevelToIndex] + 0.45f * owner.attackDamage;
        public override float actualSkillCoolDown => 5;
        public InfernoTrigger() : base("InfernoTrigger")
        {
            _skillLevel = 0;
            _maxSkillLevel = 3;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription, _damage);
        }

        public override bool SkillEffect(out string failMessage)
        {
            failMessage = string.Empty;
            
            var daredevilImpulse = owner.skillList[(int)SkillType.PassiveSkill] as DaredevilImpulse;
            if (daredevilImpulse == null) return false;

            if (daredevilImpulse.comboLevel < 5)
            {
                failMessage = "连击等级不足";
                return false;
            }

            var infernoTrigger = BulletFactory.Instance.CreateBullet(owner);
            var duration = 2.1f;
            daredevilImpulse.comboLevel = 0;
            
            // 给自己减速30%
            var speedReduce = new SpeedReduce(owner, owner, duration, 0.3f);
            owner.GainBuff(speedReduce);
            
            infernoTrigger.OnBulletAwake += (self) =>
            {
                self.target = null;
                
                self.gameObject.transform.position = owner.gameObject.transform.position;
                self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                self.gameObject.SetActive(true);
                self.bulletDamageCD = 0.2f;
            
                // 创建技能特效
                var effect = EffectManager.Instance.CreateEffect("InfernoTrigger", infernoTrigger.gameObject);
                effect.EffectUpdateEvent += () =>
                {
                    effect.effect.transform.rotation = infernoTrigger.gameObject.transform.rotation;
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
                    
                    // 检测是否有敌人在技能范围内
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
                                target.TakeDamage(damageValue, DamageType.AD, owner, Random.Range(0f, 1f) < owner.criticalRate.Value);
                                
                                // 造成技能特效
                                self.owner.AbilityEffectActivate(target, damageValue, this);
                            }
                        }
                    }
                };
            };

            infernoTrigger.Awake();

            return true;
        }
    }
}