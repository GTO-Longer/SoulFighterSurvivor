using Factories;
using Managers;
using Systems;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class SteelTempest : Skill
    {
        private float _damage => _baseSkillValue[0][skillLevelToIndex] + 1.05f * owner.attackDamage;
        private float _critDamage => _baseSkillValue[0][skillLevelToIndex] + 1.05f * owner.attackDamage * owner.criticalDamage;
        private int continuousReleaseCount;
        private const float continuousReleaseTime = 6;
        private float continuousReleaseTimer;
        private const float controlTime = 1;
        public override float actualSkillCoolDown => 4 * (1 - Mathf.Min(owner.attackSpeed - owner._baseAttackSpeed, 1.11111f) / 1.67f);
        private bool skillUsed;

        public SteelTempest() : base("SteelTempest")
        {
            _skillLevel = 0;
            _maxSkillLevel = 5;
            
            coolDownTimer = 999;
            
            PassiveAbilityEffective += () =>
            {
                owner.attackSpeed.PropertyChanged += (_, _) =>
                {
                    _castTime = 0.35f - Mathf.Min(owner.attackSpeed - owner._baseAttackSpeed, 1.2f) / 24 * 3.5f;
                };
                
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
                };
            };
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription, _damage, _critDamage);
        }

        public override bool SkillEffect()
        {
            skillUsed = true;
            var sweepingBlade = owner.skillList[(int)SkillType.ESkill] as SweepingBlade;
            if (sweepingBlade == null) return false;

            if (sweepingBlade.isSweeping)
            {
                var damageArea = BulletFactory.Instance.CreateBullet(owner, 0.3f, 1);
                damageArea.OnBulletAwake += (self) =>
                {
                    self.gameObject.transform.position = owner.gameObject.transform.position;
                    self.gameObject.SetActive(true);
                    self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    self.effect = EffectManager.Instance.CreateEffect("CircleSlash", self.gameObject);

                    self.OnBulletUpdate += (_) =>
                    {
                        damageArea.gameObject.transform.position = owner.gameObject.transform.position;
                        
                        var targets = ToolFunctions.IsOverlappingOtherTagAll(self.gameObject, 215);
                        if (targets != null)
                        {
                            var wind = false;
                            foreach (var entity in targets)
                            {
                                if (!entity.isAlive) continue;

                                self.bulletEntityDamageCDTimer.TryAdd(entity, self.bulletDamageCD);

                                if (self.bulletEntityDamageCDTimer[entity] > self.bulletDamageCD)
                                {
                                    self.bulletEntityDamageCDTimer[entity] = 0;
                                    
                                    var isCritical = Random.Range(0f, 1f) < owner.criticalRate.Value;
                            
                                    var damageCount = isCritical ? 
                                        entity.CalculateADDamage(self.owner, _critDamage) : 
                                        entity.CalculateADDamage(self.owner, _damage);
                            
                                    entity.TakeDamage(damageCount, DamageType.AD, owner,
                                        isCritical && owner.canSkillCritical);

                                    // 若有3层风则眩晕并且创建额外特效
                                    if (continuousReleaseCount >= 2 && skillUsed)
                                    {
                                        // TODO:额外特效
                                        
                                        entity.GetControlled(controlTime);
                                        wind = true;
                                    }
                                    
                                    if (continuousReleaseCount < 2 && skillUsed)
                                    {
                                        skillUsed = false;
                                        continuousReleaseCount += 1;
                                        continuousReleaseTimer = continuousReleaseTime;
                                    }

                                    // 造成攻击特效
                                    self.owner.AttackEffectActivate(entity, damageCount);
                                }
                            }

                            if (continuousReleaseCount >= 2 && wind)
                            {
                                continuousReleaseCount = 0;
                            }
                        }

                        // 子弹的销毁逻辑
                        if (self.bulletContinuousTimer >= self.bulletContinuousTime)
                        {
                            self.bulletEntityDamageCDTimer.Clear();
                            EffectManager.Instance.DestroyEffect(self.effect);
                            self.Destroy();
                        }
                    };
                };

                damageArea.Awake();
            }
            else
            {
                if (continuousReleaseCount < 2)
                {
                    // 计算飞出目标点
                    var mouseWorld = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    var direction = ((Vector2)mouseWorld - (Vector2)owner.gameObject.transform.position).normalized;

                    // 吟唱时间
                    Async.SetAsync(_castTime, null, () =>
                    {
                        owner.canUseSkill = false;
                        owner.canMove = false;
                        owner.RotateTo(ref direction);
                    }, () =>
                    {
                        owner.canUseSkill = true;
                        owner.canMove = true;
                        owner.agent.SetStop(false);

                        var slash = BulletFactory.Instance.CreateBullet(owner);
                        slash.OnBulletAwake += (self) =>
                        {
                            self.target = null;
                            self.gameObject.transform.position = owner.gameObject.transform.position;
                            self.gameObject.SetActive(true);
                            var hasInitialized = false;
                            var speed = Vector2.zero;

                            // 自定义每帧更新逻辑
                            self.OnBulletUpdate += (_) =>
                            {
                                // 初始化
                                if (!hasInitialized)
                                {
                                    // 设置速度
                                    speed = direction * 4000;
                                    hasInitialized = true;
                                }

                                // 到达目标位置
                                if (Vector2.Distance(self.gameObject.transform.position, owner.gameObject.transform.position) > skillRange)
                                {
                                    self.Destroy();
                                }
                                else
                                {
                                    // 控制子弹位置和面向
                                    self.gameObject.transform.position += (Vector3)(speed * Time.deltaTime);
                                    var angle = Vector2.SignedAngle(Vector2.up, speed.normalized);
                                    self.gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                                }

                                // 技能命中判定
                                if (ToolFunctions.IsOverlappingOtherTag(self.gameObject, self.gameObject.tag, out var target, _bulletWidth))
                                {
                                    if (!target.Equals(self.target))
                                    {
                                        self.BulletHit(target);

                                        if (continuousReleaseCount < 2 && skillUsed)
                                        {
                                            skillUsed = false;
                                            continuousReleaseCount += 1;
                                            continuousReleaseTimer = continuousReleaseTime;
                                        }
                                    }
                                }
                            };
                        };

                        slash.OnBulletHit += (self) =>
                        {
                            var isCritical = Random.Range(0f, 1f) < owner.criticalRate.Value;
                            
                            var damageCount = isCritical ? 
                                self.target.CalculateADDamage(self.owner, _critDamage) : 
                                self.target.CalculateADDamage(self.owner, _damage);
                            
                            self.target.TakeDamage(damageCount, DamageType.AD, owner,
                                isCritical && owner.canSkillCritical);

                            // 造成攻击特效
                            self.owner.AttackEffectActivate(self.target, damageCount);
                        };

                        slash.Awake();
                    });
                }
                else
                {
                    continuousReleaseCount = 0;
                    
                    // 计算飞出目标点
                    var mouseWorld = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
                    var direction = ((Vector2)mouseWorld - (Vector2)owner.gameObject.transform.position).normalized;

                    // 吟唱时间
                    Async.SetAsync(_castTime, null, () =>
                    {
                        owner.canUseSkill = false;
                        owner.canMove = false;
                        owner.RotateTo(ref direction);
                    }, () =>
                    {
                        owner.canUseSkill = true;
                        owner.canMove = true;
                        owner.agent.SetStop(false);

                        var slash = BulletFactory.Instance.CreateBullet(owner);
                        slash.OnBulletAwake += (self) =>
                        {
                            self.target = null;
                            self.gameObject.transform.position = owner.gameObject.transform.position;
                            self.gameObject.SetActive(true);
                            var hasInitialized = false;
                            var speed = Vector2.zero;

                            // 自定义每帧更新逻辑
                            self.OnBulletUpdate += (_) =>
                            {
                                // 初始化
                                if (!hasInitialized)
                                {
                                    // 设置速度
                                    speed = direction * bulletSpeed;
                                    hasInitialized = true;
                                    self.bulletStateID = 1;
                                }

                                // 到达目标位置
                                if (Vector2.Distance(self.gameObject.transform.position, owner.gameObject.transform.position) > 1150)
                                {
                                    self.Destroy();
                                }
                                else
                                {
                                    // 控制子弹位置和面向
                                    self.gameObject.transform.position += (Vector3)(speed * Time.deltaTime);
                                    var angle = Vector2.SignedAngle(Vector2.up, speed.normalized);
                                    self.gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                                }

                                // 技能命中判定
                                if (ToolFunctions.IsOverlappingOtherTag(self.gameObject, self.gameObject.tag, out var target, 180))
                                {
                                    if (!target.Equals(self.target) && target.isAlive)
                                    {
                                        self.BulletHit(target);
                                    }
                                }
                            };
                        };

                        slash.OnBulletHit += (self) =>
                        {
                            var isCritical = Random.Range(0f, 1f) < owner.criticalRate.Value;
                            
                            var damageCount = isCritical ? 
                                self.target.CalculateADDamage(self.owner, _critDamage) : 
                                self.target.CalculateADDamage(self.owner, _damage);
                            
                            self.target.TakeDamage(damageCount, DamageType.AD, owner,
                                isCritical && owner.canSkillCritical);
                            
                            self.target.GetControlled(controlTime);

                            // 造成攻击特效
                            self.owner.AttackEffectActivate(self.target, damageCount);
                        };

                        slash.Awake();
                    });
                }
            }

            return true;
        }
    }
}