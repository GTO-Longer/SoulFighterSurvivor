using System.Collections.Generic;
using Factories;
using Systems;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class SpiritRush : Skill
    {
        private const float dashDuration = 0.5f;

        private float _damage => _baseSkillValue[0][skillLevelToIndex] + 0.35f * owner.abilityPower;
        
        public SpiritRush() : base("SpiritRush")
        {
            _skillLevel = 0;
            _maxSkillLevel = 3;
            maxSkillChargeCount = 0;
            
            coolDownTimer = 999;

            OnSpecialTimeOut += () =>
            {
                coolDownTimer = 0;
                specialCoolDown = 0;
                maxSkillChargeCount = 0;
                skillChargeCount = 0;
            };
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription,
                _damage);
        }

        public override void SkillEffect()
        {
            // 设置充能
            if (skillChargeCount == 0 && specialTimer == 0)
            {
                maxSkillChargeCount = 3;
                skillChargeCount = maxSkillChargeCount - 1;
                specialTimer = 10;
                specialCoolDown = 1f;
            }

            var mouseWorldPos = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = owner.gameObject.transform.position.z;
            var direction = (mouseWorldPos - owner.gameObject.transform.position).normalized;
            const float r = 150f;

            owner.Dash(destinationDistance, dashDuration, direction, () =>
            {
                // 到目的地检测是否有敌人
                var targets = ToolFunctions.IsOverlappingOtherTagAll(owner.gameObject, skillRange);
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
                                // 锁定目标死亡则清除子弹
                                if (self.target == null || !self.target.isAlive)
                                {
                                    self.Destroy();
                                    return;
                                }

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
                                        self.target.gameObject.transform.position) <= self.target.scale)
                                {
                                    self.BulletHit();
                                    self.Destroy();
                                }
                            };
                        };

                        spiritOrb.OnBulletHit += (self) =>
                        {
                            var damageCount = self.target.CalculateAPDamage(self.owner, _damage);
                            self.target.TakeDamage(damageCount, DamageType.AP, owner, Random.Range(0f, 1f) < owner.criticalRate.Value && owner.canSkillCritical);
                            self.owner.AbilityEffectActivate(self.target, damageCount, this);
                        };

                        spiritOrb.Awake();
                    }
                }
            });
        }
    }
}