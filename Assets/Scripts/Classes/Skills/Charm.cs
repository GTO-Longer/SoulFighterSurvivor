using System;
using Factories;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class Charm : Skill
    {
        private float _controlTime => _baseSkillValue[0][Math.Max(0, _skillLevel - 1)];
        private float _damage => _baseSkillValue[1][Math.Max(0, _skillLevel - 1)] + 0.85f * owner.abilityPower;

        public Charm() : base("Charm")
        {
            _skillLevel = 1;
            _maxSkillLevel = 5;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription,
                _controlTime, _damage);
        }

        public override void SkillEffect()
        {
            Debug.Log(skillName + ": Skill effective");
            owner.OnESkillRelease += (_, _) =>
            {
                if (_skillLevel <= 0)
                {
                    Debug.Log("Skill level too low to use.");
                    return;
                }
                
                if (_baseSkillCost[_skillLevel] > owner.magicPoint)
                {
                    Debug.Log("Magic point too low to use.");
                    return;
                }
                
                if (actualSkillCoolDown > coolDownTimer)
                {
                    Debug.Log("Skill is in cooldown.");
                    return;
                }
                
                owner.RotateToMousePoint();
                owner.magicPoint.Value -= _baseSkillCost[_skillLevel];
                coolDownTimer = 0;
                
                // 计算飞出目标点
                var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var direction = ((Vector2)mouseWorld - (Vector2)owner.gameObject.transform.position).normalized;
                
                // 吟唱时间
                Async.SetAsync(_castTime, null, () => owner.canCancelTurn = false, () =>
                {
                    owner.canCancelTurn = true;
                    var charm = BulletFactory.Instance.CreateBullet(owner);

                    charm.OnBulletAwake += (self) =>
                    {
                        self.target = null;
                        self.gameObject.transform.position = owner.gameObject.transform.position;
                        self.gameObject.SetActive(true);
                        var hasInitialized = false;
                        var speed = Vector2.zero;

                        // TODO:完成Buff系统设计，通过赋予“魅惑”debuff的方式控制敌人

                        // 自定义每帧更新逻辑
                        self.OnBulletUpdate += (_) =>
                        {
                            // 初始化
                            if (!hasInitialized)
                            {
                                // 初速度
                                speed = direction * bulletSpeed;
                                hasInitialized = true;
                                self.bulletStateID = 1;
                            }

                            // 到达目标位置
                            if (Vector2.Distance(self.gameObject.transform.position, owner.gameObject.transform.position) > actualSkillRange)
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
                            var target = ToolFunctions.IsOverlappingOtherTag(self.gameObject);
                            if (target != null)
                            {
                                if (self.target == null || !target.Equals(self.target))
                                {
                                    self.BulletHit(target);
                                    self.Destroy();
                                }
                            }
                        };
                    };

                    charm.OnBulletHit += (self) =>
                    {
                        self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, _damage));

                        // 造成技能特效
                        self.AbilityEffectActivate();
                    };

                    charm.Awake();
                });
            };
        }
    }
}