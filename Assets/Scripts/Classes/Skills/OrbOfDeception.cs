using System;
using Factories;
using MVVM;
using MVVM.ViewModels;
using Systems;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class OrbOfDeception : Skill
    {
        private float _APDamage => _baseSkillValue[0][Math.Max(0, _skillLevel - 1)] + 0.5f * owner.abilityPower;
        private float _realDamage => _baseSkillValue[1][Math.Max(0, _skillLevel - 1)] + 0.5f * owner.abilityPower;
        
        public OrbOfDeception() : base("OrbOfDeception")
        {
            _skillLevel = 0;
            _maxSkillLevel = 5;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription,
                _APDamage, _realDamage);
        }

        public override void SkillEffect()
        {
            Debug.Log(skillName + ": Skill effective");
            owner.OnQSkillRelease += (_, _) =>
            {
                if (_skillLevel <= 0)
                {
                    Binder.ShowText(SkillViewModel.instance.skillTips, "技能尚未解锁", 1);
                    return;
                }
                
                if (_baseSkillCost[_skillLevel] > owner.magicPoint)
                {
                    Binder.ShowText(SkillViewModel.instance.skillTips, "施法资源不够，技能无法使用", 1);
                    return;
                }
                
                if (actualSkillCoolDown > coolDownTimer)
                {
                    Binder.ShowText(SkillViewModel.instance.skillTips, "技能正在冷却", 1);
                    return;
                }

                owner.RotateToMousePoint();
                owner.magicPoint.Value -= _baseSkillCost[_skillLevel];
                coolDownTimer = 0;
                
                // 计算飞出目标点
                var mouseWorld = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
                var direction = ((Vector2)mouseWorld - (Vector2)owner.gameObject.transform.position).normalized;
                var targetPosition = (Vector2)owner.gameObject.transform.position + direction * actualSkillRange;
                
                // 吟唱时间
                Async.SetAsync(_castTime, null, () =>
                {
                    owner.canUseSkill = false;
                    owner.canCancelTurn = false;
                }, () =>
                {
                    owner.canUseSkill = true;
                    owner.canCancelTurn = true;
                    var deceptionOrb = BulletFactory.Instance.CreateBullet(owner);

                    deceptionOrb.OnBulletAwake += (self) =>
                    {
                        self.target = null;
                        self.gameObject.transform.position = owner.gameObject.transform.position;
                        self.gameObject.SetActive(true);
                        var hasReachedTarget = false;
                        var hasInitialized = false;

                        var speed = Vector2.zero;
                        var returnSpeed = 0f;
                        float acceleration = 0;

                        self.OnBulletUpdate += (_) =>
                        {
                            // 初始化
                            if (!hasInitialized)
                            {
                                // 计算加速度
                                acceleration = (bulletSpeed * bulletSpeed) / (2f * actualSkillRange);
                                
                                // 初速度
                                speed = direction * bulletSpeed;
                                hasInitialized = true;
                                self.bulletStateID = 1;
                            }

                            // 技能子弹飞出
                            if (!hasReachedTarget)
                            {
                                speed -= direction * (acceleration * Time.deltaTime);

                                // 到达目标位置
                                if ((speed / direction).x < 0.01f)
                                {
                                    speed = Vector2.zero;
                                    self.gameObject.transform.position = targetPosition;
                                    hasReachedTarget = true;
                                    self.bulletStateID = 2;
                                    self.target = null;
                                }
                                else
                                {
                                    // 控制子弹位置和面向
                                    self.gameObject.transform.position += (Vector3)(speed * Time.deltaTime);
                                    var angle = Vector2.SignedAngle(Vector2.up, speed.normalized);
                                    self.gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                                }
                            }
                            // 技能子弹返回
                            else
                            {
                                // 持续锁定英雄位置
                                var currentPosition = (Vector2)self.gameObject.transform.position;
                                var ownerPos = (Vector2)owner.gameObject.transform.position;
                                var returnDirection = (ownerPos - currentPosition).normalized;
                                
                                returnSpeed += acceleration * Time.deltaTime;
                                speed = returnDirection * returnSpeed;

                                // 更新位置
                                self.gameObject.transform.position += (Vector3)(speed * Time.deltaTime);

                                // 旋转
                                if (speed != Vector2.zero)
                                {
                                    var angle = Vector2.SignedAngle(Vector2.up, speed.normalized);
                                    self.gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                                }

                                // 到达英雄位置
                                if (Vector2.Distance(currentPosition, ownerPos) < 1f)
                                {
                                    self.Destroy();
                                    return;
                                }
                            }

                            // 命中检测
                            var target = ToolFunctions.IsOverlappingOtherTag(self.gameObject);
                            if (target != null)
                            {
                                if (self.target == null || !target.Equals(self.target))
                                {
                                    self.BulletHit(target);
                                }
                            }
                        };
                    };

                    deceptionOrb.OnBulletHit += (self) =>
                    {
                        if (self.bulletStateID == 1)
                        {
                            // 第一段造成魔法伤害
                            self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, _APDamage), DamageType.AP);
                        }
                        else if (self.bulletStateID == 2)
                        {
                            // 第二段造成真实伤害
                            self.target.TakeDamage(_realDamage, DamageType.Real);
                        }

                        // 造成技能特效
                        self.AbilityEffectActivate();
                    };

                    deceptionOrb.Awake();
                });
            };
        }
    }
}