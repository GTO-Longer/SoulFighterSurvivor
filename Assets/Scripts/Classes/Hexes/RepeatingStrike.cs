using System;
using System.Collections.Generic;
using Factories;
using Managers;
using Managers.EntityManagers;
using UnityEngine;
using Utilities;

namespace Classes.Hexes
{
    public class RepeatingStrike : Hex
    {
        private float leftDownTimer = directionCD;
        private float rightDownTimer = directionCD;
        private float leftUpTimer = directionCD;
        private float rightUpTimer = directionCD;
        private const float directionCD = 5;
        private const float attackBulletSpeed = 2000;
        private float damageCount => 25 + HeroManager.hero.attackDamage * 0.15f;
        private float damageSum = 0;
        
        private Action<Entity> HexCDTimer;
        private Action<Entity, Entity, bool> HexEffect;
        
        private Effect effect;
        
        public RepeatingStrike() : base("RepeatingStrike")
        {
            HexCDTimer = (_) =>
            {
                effect.effect.transform.localScale = owner.gameObject.transform.localScale;
                if (leftDownTimer < directionCD)
                {
                    leftDownTimer += Time.deltaTime;
                }
                else if(!effect.effect.transform.Find("LeftDown").gameObject.activeSelf)
                {
                    effect.effect.transform.Find("LeftDown").gameObject.SetActive(true);
                }

                if (rightDownTimer < directionCD)
                {
                    rightDownTimer += Time.deltaTime;
                }
                else if(!effect.effect.transform.Find("RightDown").gameObject.activeSelf)
                {
                    effect.effect.transform.Find("RightDown").gameObject.SetActive(true);
                }

                if (leftUpTimer < directionCD)
                {
                    leftUpTimer += Time.deltaTime;
                }
                else if(!effect.effect.transform.Find("LeftUp").gameObject.activeSelf)
                {
                    effect.effect.transform.Find("LeftUp").gameObject.SetActive(true);
                }

                if (rightUpTimer < directionCD)
                {
                    rightUpTimer += Time.deltaTime;
                }
                else if(!effect.effect.transform.Find("RightUp").gameObject.activeSelf)
                {
                    effect.effect.transform.Find("RightUp").gameObject.SetActive(true);
                }
            };

            HexEffect = (_, target, _) =>
            {
                if (target == null) return;

                var ownerPos = owner.gameObject.transform.position;
                var targetPos = target.gameObject.transform.position;

                Vector2 dir = targetPos - ownerPos;

                var isUp = dir.y > 0f;
                var isRight = dir.x > 0f;

                if (isUp && isRight)
                {
                    if (rightUpTimer < directionCD)
                    {
                        return;
                    }

                    rightUpTimer = 0;
                    effect.effect.transform.Find("RightUp").gameObject.SetActive(false);
                }
                else if (isUp && !isRight)
                {
                    if (leftUpTimer < directionCD)
                    {
                        return;
                    }

                    leftUpTimer = 0;
                    effect.effect.transform.Find("LeftUp").gameObject.SetActive(false);
                }
                else if (!isUp && isRight)
                {
                    if (rightDownTimer < directionCD)
                    {
                        return;
                    }

                    rightDownTimer = 0;
                    effect.effect.transform.Find("RightDown").gameObject.SetActive(false);
                }
                else
                {
                    if (leftDownTimer < directionCD)
                    {
                        return;
                    }

                    leftDownTimer = 0;
                    effect.effect.transform.Find("LeftDown").gameObject.SetActive(false);
                }

                var bulletList = new List<Bullet>()
                {
                    BulletFactory.Instance.CreateBullet(owner),
                    BulletFactory.Instance.CreateBullet(owner),
                    BulletFactory.Instance.CreateBullet(owner),
                    BulletFactory.Instance.CreateBullet(owner),
                    BulletFactory.Instance.CreateBullet(owner)
                };

                foreach (var bullet in bulletList)
                {
                    var index = bulletList.IndexOf(bullet);

                    Async.SetAsync(index * 0.1f, null, null, () =>
                    {
                        bullet.OnBulletAwake += (self) =>
                        {
                            self.gameObject.transform.position = owner.gameObject.transform.position;
                            self.gameObject.SetActive(true);

                            // 设置目标
                            self.target = target;

                            // 每帧追踪目标
                            self.OnBulletUpdate += (_) =>
                            {
                                // 锁定目标死亡则清除子弹
                                if (self.target == null || !self.target.isAlive)
                                {
                                    self.Destroy();
                                    return;
                                }

                                var currentPosition = self.gameObject.transform.position;
                                var targetPosition = self.target.gameObject.transform.position;

                                var direction = (targetPosition - currentPosition).normalized;
                                var nextPosition = currentPosition + direction * (attackBulletSpeed * Time.deltaTime);

                                self.gameObject.transform.position = nextPosition;
                                self.gameObject.transform.rotation =
                                    Quaternion.LookRotation(Vector3.forward, direction);

                                // 子弹的销毁逻辑
                                const float destroyDistance = 30f;
                                if (Vector3.Distance(self.gameObject.transform.position,
                                        self.target.gameObject.transform.position) <= destroyDistance)
                                {
                                    self.BulletHit();
                                    self.Destroy();
                                }
                            };
                        };

                        bullet.OnBulletHit += (self) =>
                        {
                            // 计算攻击伤害
                            var damage = target.CalculateADDamage(owner, damageCount);
                            target.TakeDamage(damage, DamageType.AD, owner);
                            damageSum += damage;

                            // 造成攻击特效
                            if (target.isAlive)
                            {
                                owner.AttackEffectActivate(target, damage, 0.2f);
                            }
                        };

                        bullet.Awake();
                    });
                }
            };
        }

        public override void OnHexGet(Entity entity)
        {
            base.OnHexGet(entity);
            effect = EffectManager.Instance.CreateEffect("RepeatingStrike", owner.gameObject);
            owner.EntityUpdateEvent += HexCDTimer;
            owner.OnAttackHit += HexEffect;
        }

        public override void OnHexRemove()
        {
            owner.EntityUpdateEvent -= HexCDTimer;
            owner.OnAttackHit -= HexEffect;
            EffectManager.Instance.DestroyEffect(effect);
            effect = null;
            base.OnHexRemove();
        }

        public override bool GetHexDetail(out string detail)
        {
            detail = string.Format(hexDetail, damageCount, damageCount * 5, damageSum);
            return true;
        }
    }
}