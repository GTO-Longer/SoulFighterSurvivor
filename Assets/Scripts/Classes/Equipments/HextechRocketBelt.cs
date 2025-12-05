using System.Collections.Generic;
using Classes.Entities;
using Factories;
using Managers.EntityManagers;
using Systems;
using UnityEngine;
using Utilities;

namespace Classes.Equipments
{
    public class HextechRocketBelt : Equipment
    {
        private float damageCount => 100 + 0.1f * HeroManager.hero.abilityPower;
        private static readonly float[] angles = new float[] {-120f, -105f, -90f, -75f, -60f };

        public HextechRocketBelt() : base("HextechRocketBelt")
        {
            ActiveSkillEffective += () =>
            {
                if (!_activeSkillActive) return;
                
                var mouseWorldPos = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
                mouseWorldPos.z = owner.gameObject.transform.position.z;
                var direction = (mouseWorldPos - owner.gameObject.transform.position).normalized;
                var hero = owner as Hero;
                
                hero?.Dash(400, 0.5f, direction, () =>
                {
                    _activeSkillCDTimer = 0;
                    var hexRocketBeltBullet = new List<Bullet>
                    {
                        BulletFactory.Instance.CreateBullet(hero),
                        BulletFactory.Instance.CreateBullet(hero),
                        BulletFactory.Instance.CreateBullet(hero),
                        BulletFactory.Instance.CreateBullet(hero),
                        BulletFactory.Instance.CreateBullet(hero),
                        BulletFactory.Instance.CreateBullet(hero)
                    };

                    for (var index = 0; index < hexRocketBeltBullet.Count; index++)
                    {
                        var hexBullet = hexRocketBeltBullet[index];
                        var bulletIndex = index;
                        
                        hexBullet.OnBulletAwake += (self) =>
                        {
                            self.gameObject.SetActive(true);
                            hexBullet.gameObject.transform.position = hero.gameObject.transform.position;

                            // 预设方向
                            var ownerPos = owner.gameObject.transform.position;
                            mouseWorldPos.z = ownerPos.z;
                            var fireDirs = new Vector2[6];
                            var baseDir = (mouseWorldPos - ownerPos).normalized;
                            for (var i = 0; i < 5; i++)
                                fireDirs[i] = Quaternion.Euler(0, 0, angles[i]) * baseDir;
                            fireDirs[5] = Quaternion.Euler(0, 0, 90) * baseDir;
                            
                            // 当前子弹的方向
                            var dir = fireDirs[bulletIndex];
                            self.gameObject.transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
                            
                            self.OnBulletUpdate += (_) =>
                            {
                                // 子弹的移动逻辑
                                self.gameObject.transform.position += self.gameObject.transform.up * (2000 * Time.deltaTime);

                                // 子弹的销毁逻辑
                                if (ToolFunctions.IsOverlappingOtherTag(self.gameObject, owner.gameObject.tag,
                                        out var target))
                                {
                                    self.BulletHit(target);
                                    self.Destroy();
                                }

                                // 最大飞行距离
                                if (Vector3.Distance(self.gameObject.transform.position, ownerPos) > 800f)
                                {
                                    self.Destroy();
                                }
                            };
                        };

                        hexBullet.OnBulletHit += (self) =>
                        {
                            var damage = self.target.CalculateAPDamage(self.owner, damageCount);
                            self.target.TakeDamage(damage, DamageType.AP, owner);
                        };

                        hexBullet.Awake();
                    }
                });
            };
        }
        
        public override void OnEquipmentGet(Entity entity)
        {
            base.OnEquipmentGet(entity);

            owner.EntityUpdateEvent += equipmentTimerUpdate;
        }

        public override void OnEquipmentRemove()
        {
            owner.EntityUpdateEvent -= equipmentTimerUpdate;
            
            base.OnEquipmentRemove();
        }

        public override bool GetActiveSkillDescription(out string description)
        {
            description = string.Format(_activeSkillName + "\n冷却时间:" + _activeSkillCD + "秒\n" + _activeSkillDescription, damageCount);
            return true;
        }
    }
}