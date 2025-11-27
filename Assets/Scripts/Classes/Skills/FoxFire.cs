using System;
using System.Collections.Generic;
using Factories;
using MVVM;
using MVVM.ViewModels;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class FoxFire : Skill
    {
        private float _firstDamage => _baseSkillValue[0][skillLevelToIndex] + 0.4f * owner.abilityPower;
        private float _secondDamage => _baseSkillValue[1][skillLevelToIndex] + 0.16f * owner.abilityPower;
        
        public FoxFire() : base("FoxFire")
        {
            _skillLevel = 0;
            _maxSkillLevel = 5;
            
            coolDownTimer = 999;
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
                if (_skillLevel <= 0)
                {
                    Binder.ShowText(SkillViewModel.instance.skillTips, "技能尚未解锁", 1);
                    return;
                }

                if (_baseSkillCost[skillLevelToIndex] > owner.magicPoint)
                {
                    Binder.ShowText(SkillViewModel.instance.skillTips, "施法资源不够，技能无法使用", 1);
                    return;
                }
                
                if (actualSkillCoolDown > coolDownTimer)
                {
                    Binder.ShowText(SkillViewModel.instance.skillTips, "技能正在冷却", 1);
                    return;
                }

                owner.magicPoint.Value -= _baseSkillCost[skillLevelToIndex];
                coolDownTimer = 0;

                // 获得2s内40%递减的加速
                owner._percentageMovementSpeedBonus.Value += 0.4f;
                Async.SetAsync(1, null, null, () =>
                {
                    // OnComplete时记得反向执行一次OnUpdate内容（因为会多运行1逻辑帧）
                    Async.SetAsync(1, null,
                        () => { owner._percentageMovementSpeedBonus.Value -= 0.4f * Time.fixedDeltaTime; },
                        () => owner._percentageMovementSpeedBonus.Value += 0.4f * Time.fixedDeltaTime);
                });

                var foxFire = new List<Bullet>
                {
                    BulletFactory.Instance.CreateBullet(owner),
                    BulletFactory.Instance.CreateBullet(owner),
                    BulletFactory.Instance.CreateBullet(owner)
                };

                const float rotationSpeed = Mathf.PI;
                const float r = 1.5f;

                for (var i = 0; i < foxFire.Count; i++)
                {
                    var fire = foxFire[i];
                    var index = i;
                    var continuousTime = 0f;

                    fire.OnBulletAwake += (self) =>
                    {
                        self.target = null;
                        self.gameObject.SetActive(true);
                        self.bulletStateID = 1;

                        var angleOffset = index * (2f * Mathf.PI / 3f);

                        var offset = new Vector3(
                            Mathf.Cos(angleOffset) * r,
                            Mathf.Sin(angleOffset) * r,
                            0f
                        );

                        var ownerPos = owner.gameObject.transform.position;
                        self.gameObject.transform.position = new Vector3(
                            ownerPos.x + offset.x,
                            ownerPos.y + offset.y,
                            ownerPos.z
                        );


                        self.OnBulletUpdate += (bullet) =>
                        {
                            if(self.bulletStateID == 1){
                                continuousTime += Time.deltaTime;
                                var center = owner.gameObject.transform.position;
                                var currentAngle = continuousTime * rotationSpeed + angleOffset;

                                var newPos = center + new Vector3(
                                    Mathf.Cos(currentAngle) * r,
                                    Mathf.Sin(currentAngle) * r,
                                    0f
                                );
                                
                                var directionAngle = (currentAngle) * Mathf.Rad2Deg;

                                bullet.gameObject.transform.position = newPos;
                                bullet.gameObject.transform.eulerAngles = new Vector3(0, 0, directionAngle);

                                if (continuousTime > 0.4f + index * 0.1f)
                                {
                                    self.target = ToolFunctions.IsOverlappingOtherTag(self.owner.gameObject, actualSkillRange);
                                    if (self.target != null)
                                    {
                                        self.bulletStateID = 2;
                                    }
                                }

                                // 若长时间未检索到敌人则消失
                                if (continuousTime > 5)
                                {
                                    self.Destroy();
                                }
                            }
                            else if (self.bulletStateID == 2)
                            {
                                var currentPosition = self.gameObject.transform.position;
                                var targetPosition = self.target.gameObject.transform.position;
            
                                var direction = (targetPosition - currentPosition).normalized;
                                var nextPosition = currentPosition + direction * (bulletSpeed * Time.deltaTime);
            
                                self.gameObject.transform.position = nextPosition;
                                self.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

                                // 子弹的销毁逻辑
                                if (Vector3.Distance(self.gameObject.transform.position, self.target.gameObject.transform.position) <= self.target.actualScale)
                                {
                                    self.BulletHit();
                                    self.Destroy();
                                }
                            }
                        };
                    };

                    fire.OnBulletHit += (self) =>
                    {
                        if (self.target != null)
                        {
                            if (index == 0)
                            {
                                // 对20%生命值以下的敌人造成200%伤害
                                if (self.target.healthPointProportion.Value <= 0.2f)
                                {
                                    self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, _firstDamage * 2), DamageType.AP);
                                }
                                else
                                {
                                    self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, _firstDamage), DamageType.AP);
                                }
                            }
                            else
                            {
                                if (self.target.healthPointProportion.Value <= 0.2f)
                                {
                                    self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, _secondDamage * 2), DamageType.AP);
                                }
                                else
                                {
                                    self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, _secondDamage), DamageType.AP);
                                }
                            }
                            
                            self.AbilityEffectActivate();
                        }
                    };

                    fire.Awake();
                }
            };
        }
    }
}