using System;
using DG.Tweening;
using Factories;
using UnityEngine;

namespace Classes.Skills
{
    public class OrbOfDeception : Skill
    {
        private float _APDamage => _baseSkillValue[0][Math.Max(0, _skillLevel - 1)] + 0.5f * owner.abilityPower;
        private float _RealDamage => _baseSkillValue[1][Math.Max(0, _skillLevel - 1)] + 0.5f * owner.abilityPower;
        
        public OrbOfDeception()
        {
            ReadSkillConfig("OrbOfDeception");
            
            _skillLevel = 0;
            _maxSkillLevel = 5;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription,
                _APDamage, _RealDamage);
        }

        public override void SkillEffect()
        {
            Debug.Log(skillName + ": Skill effective");
            owner.OnQSkillRelease += (_, _) =>
            {
                owner.RotateToMousePoint();
                if (_skillLevel < 0)
                {
                    Debug.Log("Skill level too low to use.");
                    return;
                }

                var deceptionOrb = BulletFactory.Instance.CreateBullet(owner);

                // 状态变量（由闭包捕获）
                var hasReachedTarget = false;
                var targetPosition = Vector3.zero;
                const float flyDuration = 0.8f;
                const float returnDuration = 0.8f;
                var flyTimer = 0f;
                var returnTimer = 0f;

                deceptionOrb.OnBulletAwake += (self) =>
                {
                    self.gameObject.transform.position = owner.gameObject.transform.position;
                    self.gameObject.SetActive(true);

                    // 计算飞出目标点
                    var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var direction = ((Vector2)mouseWorld - (Vector2)self.gameObject.transform.position).normalized;
                    targetPosition = (Vector2)self.gameObject.transform.position + direction * actualSkillRange;

                    // 自定义每帧更新逻辑
                    self.OnBulletUpdate += (bullet) =>
                    {
                        if (hasReachedTarget)
                        {
                            // 返回阶段：实时追踪 owner 当前位置
                            returnTimer += Time.deltaTime;
                            var t = Mathf.Clamp01(returnTimer / returnDuration);
                            t = t * t; // Ease.InQuad

                            if (owner?.gameObject != null)
                            {
                                bullet.gameObject.transform.position = Vector3.Lerp(
                                    targetPosition,
                                    owner.gameObject.transform.position,
                                    t
                                );
                            }

                            if (returnTimer >= returnDuration || owner?.gameObject == null)
                            {
                                bullet.Destroy();
                            }
                        }
                        else
                        {
                            // 飞出阶段
                            flyTimer += Time.deltaTime;
                            var t = Mathf.Clamp01(flyTimer / flyDuration);
                            t = t * (2f - t); // Ease.OutQuad

                            bullet.gameObject.transform.position = Vector3.Lerp(
                                owner.gameObject.transform.position,
                                targetPosition,
                                t
                            );

                            if (flyTimer >= flyDuration)
                            {
                                hasReachedTarget = true;
                                returnTimer = 0f;
                            }
                        }
                    };
                };

                deceptionOrb.Awake();
            };
        }
    }
}