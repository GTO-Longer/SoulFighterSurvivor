using Classes.Buffs;
using DataManagement;
using Factories;
using Managers;
using Systems;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class WindWall : Skill
    {
        private float _damage => _baseSkillValue[0][skillLevelToIndex] + 0.4f * owner.abilityPower;
        public WindWall() : base("WindWall")
        {
            _skillLevel = 0;
            _maxSkillLevel = 5;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription, _damage);
        }

        public override bool SkillEffect(out string failMessage)
        {
            failMessage = string.Empty;
            
            // 计算飞出目标点
            var mouseWorld = CameraSystem._mainCamera.ScreenToWorldPoint(Input.mousePosition);
            var direction = ((Vector2)mouseWorld - (Vector2)owner.gameObject.transform.position).normalized;

            // 吟唱时间
            Async.SetAsync(_castTime, null, () =>
            {
                owner.canUseSkill = false;
                owner.canMove = false;
                owner.RotateTo(ref direction);
            }, () =>
            {
                owner.canUseSkill = true;
                owner.canMove = true;
                owner.agent.SetStop(false);

                var windWall = BulletFactory.Instance.CreateBullet(owner);
                var startPosition = new Vector2(0, 0);
                
                windWall.OnBulletAwake += (self) =>
                {
                    self.target = null;
                    self.gameObject.transform.position = (Vector2)owner.gameObject.transform.position + direction * 50;
                    startPosition = self.gameObject.transform.position;
                    
                    self.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                    self.gameObject.SetActive(true);
                    self.bulletDamageCD = 0.99f;
                    var hasInitialized = false;
                    var speed = Vector2.zero;
                
                    // 创建风墙特效
                    var effect = EffectManager.Instance.CreateEffect("WindWall", windWall.gameObject);
                    effect.EffectUpdateEvent += () =>
                    {
                        effect.effect.transform.rotation = windWall.gameObject.transform.rotation;
                        var rotZ = effect.effect.gameObject.transform.eulerAngles.z;
                        effect.effect.transform.Find("Effect").GetComponent<ParallelogramGenerator>().rotationAngle = 90 - rotZ;
                        effect.effect.transform.Find("Effect").GetComponent<ParallelogramGenerator>() .GenerateParallelogram();
                    };

                    // 自定义每帧更新逻辑
                    self.OnBulletUpdate += (_) =>
                    {
                        // 初始化
                        if (!hasInitialized)
                        {
                            // 设置速度
                            speed = direction * bulletSpeed;
                            hasInitialized = true;
                            self.bulletStateID = 1;
                        }

                        // 到达目标位置
                        if (Vector2.Distance(self.gameObject.transform.position, startPosition) > skillRange)
                        {
                            EffectManager.Instance.DestroyEffect(effect);
                            self.Destroy();
                        }
                        else
                        {
                            // 控制子弹位置和面向
                            self.gameObject.transform.position += (Vector3)(speed * Time.deltaTime);
                            var angle = Vector2.SignedAngle(Vector2.up, speed.normalized);
                            self.gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                        }
                        
                        // 检测是否有敌人进入风墙区域
                        var filter = new ContactFilter2D();
                        filter.useTriggers = true;
                        var results = new Collider2D[10];
                        int count = effect.effect.GetComponent<BoxCollider2D>().OverlapCollider(filter, results);

                        for (var index = 0; index < count; index++)
                        {
                            var result = results[index].gameObject.GetComponent<EntityData>();
                            if (result != null)
                            {
                                var entity = result.entity;

                                if (entity.isAlive && entity.team != owner.team)
                                {
                                    self.bulletEntityDamageCDTimer.TryAdd(entity, self.bulletDamageCD);

                                    if (self.bulletEntityDamageCDTimer[entity] > self.bulletDamageCD)
                                    {
                                        self.bulletEntityDamageCDTimer[entity] = 0;

                                        var damageValue = entity.CalculateAPDamage(self.owner, _damage);
                                        entity.TakeDamage(damageValue, DamageType.AP, owner, Random.Range(0f, 1f) < owner.criticalRate.Value && owner.canSkillCritical);
                                        
                                        // 造成1秒40%减速
                                        var speedReduce = new SpeedReduce(entity, owner, 1f, 0.4f);
                                        entity.GetBuff(speedReduce);
                                    }
                                }
                            }
                        }
                    };
                };

                windWall.Awake();
            });

            return true;
        }
    }
}