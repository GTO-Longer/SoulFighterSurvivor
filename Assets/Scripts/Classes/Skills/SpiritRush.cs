using System;
using System.Collections.Generic;
using Factories;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class SpiritRush : Skill
    {
        private const float dashDuration = 0.5f;

        private float _damage => _baseSkillValue[0][Math.Max(0, _skillLevel - 1)] + 0.35f * owner.abilityPower;
        
        public SpiritRush() : base("SpiritRush")
        {
            _skillLevel = 1;
            _maxSkillLevel = 3;
            maxSkillChargeCount = 3;
            
            coolDownTimer = 999;

            OnSpecialTimeOut += () => coolDownTimer = 0;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription,
                _damage);
        }

        public override void SkillEffect()
        {
            Debug.Log(skillName + ": Skill effective");
            owner.OnRSkillRelease += (_, _) =>
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
                
                // 设置充能
                if (skillChargeCount == 0 && specialTimer == 0)
                {
                    // 消耗魔法值
                    owner.magicPoint.Value -= _baseSkillCost[_skillLevel];
                    skillChargeCount = maxSkillChargeCount;
                    specialTimer = 10;
                }
                
                if (skillChargeCount > 0)
                {
                    skillChargeCount -= 1;
                    coolDownTimer = actualSkillCoolDown - 0.5f;
                }
                else
                {
                    Debug.Log("Charge time no enough.");
                    return;
                }

                var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = owner.gameObject.transform.position.z;
                var direction = (mouseWorldPos - owner.gameObject.transform.position).normalized;
                const float r = 1.5f;

                owner.Dash(destinationDistance, dashDuration, direction, () =>
                {
                    // 到目的地检测是否有敌人
                    var targets = ToolFunctions.IsOverlappingOtherTagAll(owner.gameObject, actualSkillRange);
                    if (targets != null)
                    {
                        var spiritOrbList = new List<Bullet>
                        {
                            BulletFactory.Instance.CreateBullet(owner),
                            BulletFactory.Instance.CreateBullet(owner),
                            BulletFactory.Instance.CreateBullet(owner)
                        };

                        for (var index = 0; index < spiritOrbList.Count; index++)
                        {
                            var spiritOrb = spiritOrbList[index];
                            var bulletIndex = index;
                            
                            spiritOrb.OnBulletAwake += (self) =>
                            {
                                self.target = targets[bulletIndex % targets.Length];
                                self.gameObject.SetActive(true);

                                var angleOffset = bulletIndex * (2f * Mathf.PI / 3f);

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

                                self.OnBulletUpdate += (_) =>
                                {
                                    var bulletCurrentPosition = self.gameObject.transform.position;
                                    var bulletTargetPosition = self.target.gameObject.transform.position;

                                    var bulletDirection = (bulletTargetPosition - bulletCurrentPosition).normalized;
                                    var nextPosition = bulletCurrentPosition +
                                                       bulletDirection * (bulletSpeed * Time.deltaTime);

                                    self.gameObject.transform.position = nextPosition;
                                    self.gameObject.transform.rotation =
                                        Quaternion.LookRotation(Vector3.forward, bulletDirection);

                                    // 子弹的销毁逻辑
                                    if (Vector3.Distance(self.gameObject.transform.position,
                                            self.target.gameObject.transform.position) <= self.target.actualScale)
                                    {
                                        self.BulletHit();
                                        self.Destroy();
                                    }
                                };
                            };

                            spiritOrb.OnBulletHit += (self) =>
                            {
                                self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, _damage));
                                self.AbilityEffectActivate();
                            };

                            spiritOrb.Awake();
                        }
                    }
                });
            };
        }
    }
}