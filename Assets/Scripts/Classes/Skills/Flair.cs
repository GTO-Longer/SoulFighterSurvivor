using Factories;
using Managers;
using Systems;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class Flair : Skill
    {
        private float _damage => _baseSkillValue[0][skillLevelToIndex] + _baseSkillValue[1][skillLevelToIndex] * owner.attackDamage;

        public Flair() : base("Flair")
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

            var wildRush = owner.skillList[(int)SkillType.ESkill] as WildRush;
            if (wildRush == null) return false;
            
            // 计算朝向方向
            var mouseWorld = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var direction = ((Vector2)mouseWorld - (Vector2)owner.gameObject.transform.position).normalized;

            if (wildRush.isRushing)
            {
                if (owner.currentDash != null)
                {
                    owner.currentDash.onComplete += () =>
                    {
                        var angle = owner.gameObject.transform.eulerAngles.z;
                        var flair = BulletFactory.Instance.CreateBullet(owner, 0.2f, 1);
                        flair.OnBulletAwake += (self) =>
                        {
                            self.gameObject.transform.position = owner.gameObject.transform.position;
                            self.gameObject.SetActive(true);
                            self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                            self.effect = EffectManager.Instance.CreateCanvasEffect("SquareExplode", self.gameObject);
                            self.effect.EffectUpdateEvent += () =>
                            {
                                self.effect.effect.transform.localEulerAngles = new Vector3(0, 0, angle - 90);
                            };

                            self.OnBulletUpdate += (_) =>
                            {
                                if (ToolFunctions.IsOverlappingInBoxColliderAll(self.effect.effect, out var targets))
                                {
                                    foreach (var entity in targets.FindAll(target => target.team == Team.Enemy))
                                    {
                                        if (!entity.isAlive) continue;

                                        self.bulletEntityDamageCDTimer.TryAdd(entity, self.bulletDamageCD);

                                        if (self.bulletEntityDamageCDTimer[entity] > self.bulletDamageCD)
                                        {
                                            self.bulletEntityDamageCDTimer[entity] = 0;

                                            var isCritical = Random.Range(0f, 1f) < owner.criticalRate.Value;

                                            var damageCount = entity.CalculateADDamage(self.owner, _damage);
                                            entity.TakeDamage(damageCount, DamageType.AD, owner, isCritical);

                                            // 造成技能特效
                                            self.owner.AbilityEffectActivate(entity, damageCount, this);

                                            // 造成66.6%的生命窃取效果
                                            self.owner.TakeHeal(damageCount * owner.lifeSteal * 0.666f);
                                        }
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

                        flair.Awake();
                    };
                }
            }
            else if (ToolFunctions.IsOverlappingInSectorAll(120, 400, direction, owner.gameObject, out var sectorTargets))
            {
                // 吟唱时间
                Async.SetAsync(_castTime, null, () =>
                {
                    owner.canUseSkill = false;
                    owner.canMove = false;
                    owner.RotateTo(ref direction);
                }, () =>
                {
                    var flair = BulletFactory.Instance.CreateBullet(owner, 0.2f, 1);
                    flair.OnBulletAwake += (self) =>
                    {
                        self.gameObject.transform.position = owner.gameObject.transform.position;
                        self.gameObject.SetActive(true);
                        self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                        self.effect = EffectManager.Instance.CreateCanvasEffect("SectorSlash", self.gameObject);
                        self.effect.EffectUpdateEvent += () =>
                        {
                            self.effect.effect.transform.eulerAngles = new Vector3(0, 0,
                                self.owner.gameObject.transform.eulerAngles.z + 60);
                        };
                        self.effect.effect.SetActive(true);

                        self.OnBulletUpdate += (_) =>
                        {
                            owner.RotateTo(ref direction);
                            flair.gameObject.transform.position = owner.gameObject.transform.position;

                            foreach (var entity in sectorTargets)
                            {
                                if (!entity.isAlive) continue;

                                self.bulletEntityDamageCDTimer.TryAdd(entity, self.bulletDamageCD);

                                if (self.bulletEntityDamageCDTimer[entity] > self.bulletDamageCD)
                                {
                                    self.bulletEntityDamageCDTimer[entity] = 0;

                                    var isCritical = Random.Range(0f, 1f) < owner.criticalRate.Value;

                                    var damageCount = entity.CalculateADDamage(self.owner, _damage);
                                    entity.TakeDamage(damageCount, DamageType.AD, owner, isCritical);

                                    // 造成技能特效
                                    self.owner.AbilityEffectActivate(entity, damageCount, this);

                                    // 造成66.6%的生命窃取效果
                                    self.owner.TakeHeal(damageCount * owner.lifeSteal * 0.666f);
                                }
                            }

                            // 子弹的销毁逻辑
                            if (self.bulletContinuousTimer >= self.bulletContinuousTime)
                            {
                                self.bulletEntityDamageCDTimer.Clear();
                                EffectManager.Instance.DestroyEffect(self.effect);
                                self.Destroy();
                                
                                owner.canUseSkill = true;
                                owner.canMove = true;
                                owner.agent.SetStop(false);
                            }
                        };
                    };

                    flair.Awake();
                });
            }
            else
            {
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
                    Vector2 startPosition;
                    slash.OnBulletAwake += (self) =>
                    {
                        self.target = null;
                        self.gameObject.transform.position = owner.gameObject.transform.position;
                        startPosition = owner.gameObject.transform.position;
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
                            }

                            // 到达目标位置
                            if (Vector2.Distance(self.gameObject.transform.position, startPosition) > skillRange)
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
                            if (ToolFunctions.IsOverlappingOtherTag(self.gameObject, self.gameObject.tag,
                                    out var target, _bulletWidth))
                            {
                                if (!target.Equals(self.target))
                                {
                                    self.BulletHit(target);
                                    self.Destroy();
                                }
                            }
                        };
                    };

                    slash.OnBulletHit += (self) =>
                    {
                        var isCritical = Random.Range(0f, 1f) < owner.criticalRate.Value;

                        var damageCount = self.target.CalculateADDamage(self.owner, _damage);
                        self.target.TakeDamage(damageCount, DamageType.AD, owner, isCritical);

                        // 造成技能特效
                        self.owner.AbilityEffectActivate(self.target, damageCount, this);
                                    
                        // 造成66.6%的生命窃取效果
                        self.owner.TakeHeal(damageCount * owner.lifeSteal * 0.666f);
                    };

                    slash.Awake();
                });
            }

            return true;
        }
    }
}