using System;
using DG.Tweening;
using Factories;
using UnityEngine;
using UnityEngine.AI;

namespace Classes.Skills
{
    public class SpiritRush : Skill
    {
        private const float DashDuration = 0.5f;

        private float _damage => _baseSkillValue[0][Math.Max(0, _skillLevel - 1)] + 0.35f * owner.abilityPower;
        
        public SpiritRush()
        {
            ReadSkillConfig("SpiritRush");
            
            _skillLevel = 1;
            _maxSkillLevel = 3;
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

                var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = owner.gameObject.transform.position.z;

                var direction = (mouseWorldPos - owner.gameObject.transform.position).normalized;
                var targetPosition = owner.gameObject.transform.position + direction * destinationDistance;

                // 使用NavMesh确保目标点有效
                if (NavMesh.SamplePosition(targetPosition, out var hit, 2.0f, NavMesh.AllAreas))
                {
                    targetPosition = hit.position;
                }
                else
                {
                    for (var d = destinationDistance * 0.9f; d > 0.5f; d -= 0.5f)
                    {
                        var fallbackPos = owner.gameObject.transform.position + direction * d;
                        if (NavMesh.SamplePosition(fallbackPos, out hit, 1.0f, NavMesh.AllAreas))
                        {
                            targetPosition = hit.position;
                            break;
                        }
                    }
                }

                // 消耗魔法值
                owner.magicPoint.Value -= _baseSkillCost[_skillLevel];

                // 设置面向
                owner.gameObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
                
                // 使用 DOTween 平滑位移
                owner.gameObject.transform.DOMove(targetPosition, DashDuration)
                    .OnUpdate(() =>
                    {
                        // 关闭agent
                        owner.agent.isStopped = true;
                    })
                    .OnComplete(() =>
                    {
                        // 恢复agent
                        owner.agent.isStopped = false;
                        owner.agent.velocity = Vector3.zero;

                        var spiritOrb = BulletFactory.Instance.CreateBullet(owner);

                        spiritOrb.OnBulletAwake += (self) =>
                        {
                            self.target = null;
                            self.gameObject.transform.position = owner.gameObject.transform.position;
                            self.gameObject.SetActive(true);

                            self.OnBulletUpdate += (bullet) =>
                            {
                                
                            };
                        };

                        spiritOrb.OnBulletHit += (self) =>
                        {
                            self.target.TakeDamage(self.target.CalculateAPDamage(self.owner, _damage));
                            self.AbilityEffectActivate();
                        };

                        spiritOrb.Awake();
                    });
            };
        }
    }
}