using System;
using DG.Tweening;
using Factories;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class OrbOfDeception : Skill
    {
        private float _APDamage => _baseSkillValue[0][Math.Max(0, _skillLevel - 1)] + 0.5f * owner.abilityPower;
        private float _RealDamage => _baseSkillValue[1][Math.Max(0, _skillLevel - 1)] + 0.5f * owner.abilityPower;
        private bool hasReachedTarget = false;
        
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
                
                if (_baseSkillCost[_skillLevel] > owner.magicPoint)
                {
                    Debug.Log("Magic point too low to use.");
                    return;
                }

                owner.magicPoint.Value -= _baseSkillCost[_skillLevel];
                var deceptionOrb = BulletFactory.Instance.CreateBullet(owner);
                
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
                    var targetPosition = (Vector2)self.gameObject.transform.position + direction * actualSkillRange;

                    // 自定义每帧更新逻辑
                    self.OnBulletUpdate += (bullet) =>
                    {
                        // 技能运动轨迹
                        if (hasReachedTarget)
                        {
                            returnTimer += Time.deltaTime;
                            var t = Mathf.Clamp01(returnTimer / returnDuration);
                            t = t * t;

                            bullet.gameObject.transform.position = Vector3.Lerp(
                                targetPosition,
                                owner.gameObject.transform.position,
                                t
                            );

                            // 旋转子弹使其朝向运动方向（返回时）
                            if (returnTimer < returnDuration && (Vector2)owner.gameObject.transform.position != targetPosition)
                            {
                                var directionToOwner = ((Vector2)owner.gameObject.transform.position - (Vector2)bullet.gameObject.transform.position).normalized;
                                var angle = Vector2.SignedAngle(Vector2.up, directionToOwner);
                                bullet.gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                            }

                            if (returnTimer >= returnDuration)
                            {
                                bullet.Destroy();
                            }
                        }
                        else
                        {
                            flyTimer += Time.deltaTime;
                            var t = Mathf.Clamp01(flyTimer / flyDuration);
                            t = t * (2f - t);

                            bullet.gameObject.transform.position = Vector3.Lerp(
                                owner.gameObject.transform.position,
                                targetPosition,
                                t
                            );

                            // 旋转子弹使其朝向运动方向（飞出时）
                            if (flyTimer < flyDuration && (Vector2)owner.gameObject.transform.position != targetPosition)
                            {
                                var currentDirection = ((Vector2)targetPosition - (Vector2)bullet.gameObject.transform.position).normalized;
                                var angle = Vector2.SignedAngle(Vector2.up, currentDirection);
                                bullet.gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                            }

                            if (flyTimer >= flyDuration)
                            {
                                hasReachedTarget = true;
                                returnTimer = 0f;
                            }
                        }
                        
                        // 技能命中判定
                        var target = ToolFunctions.IsOverlappingOtherTag(self.gameObject);
                        if (target != null)
                        {
                            self.BulletHit(target);
                        }
                    };
                };

                deceptionOrb.OnBulletHit += (self) =>
                {
                    if (!hasReachedTarget)
                    {
                        // 第一段造成魔法伤害
                        self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, 0.5f));
                    }
                    else
                    {
                        // 第二段造成真实伤害
                        self.target.TakeDamage(self.target.abilityPower * 0.5f);
                    }
                    
                    // 造成技能特效
                    self.AbilityEffectActivate();
                };

                deceptionOrb.Awake();
            };
        }
    }
}