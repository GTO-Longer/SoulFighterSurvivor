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
        
        public Charm()
        {
            ReadSkillConfig("Charm");
            
            _skillLevel = 0;
            _maxSkillLevel = 5;
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

                owner.RotateToMousePoint();
                owner.magicPoint.Value -= _baseSkillCost[_skillLevel];
                var deceptionOrb = BulletFactory.Instance.CreateBullet(owner);
                
                const float flyDuration = 1f;
                var flyTimer = 0f;

                deceptionOrb.OnBulletAwake += (self) =>
                {
                    self.target = null;
                    self.gameObject.transform.position = owner.gameObject.transform.position;
                    self.gameObject.SetActive(true);
                    
                    // TODO:完成Buff系统设计，通过赋予“魅惑”debuff的方式控制敌人

                    // 计算飞出目标点
                    var mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    var direction = ((Vector2)mouseWorld - (Vector2)self.gameObject.transform.position).normalized;
                    var targetPosition = (Vector2)self.gameObject.transform.position + direction * actualSkillRange;

                    // 自定义每帧更新逻辑
                    self.OnBulletUpdate += (bullet) =>
                    {
                        // 技能运动轨迹
                        self.bulletStateID = 1;

                        flyTimer += Time.deltaTime;
                        var t = Mathf.Clamp01(flyTimer / flyDuration);

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
                            self.Destroy();
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

                deceptionOrb.OnBulletHit += (self) =>
                {
                    self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, _damage));
                    
                    // 造成技能特效
                    self.AbilityEffectActivate();
                };

                deceptionOrb.Awake();
            };
        }
    }
}