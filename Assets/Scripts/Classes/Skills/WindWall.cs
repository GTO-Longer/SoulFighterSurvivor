using Classes.Buffs;
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
            HeroModelManager.Instance.WSkillAnimation();
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
                Vector2 startPosition;
                
                windWall.OnBulletAwake += (self) =>
                {
                    AudioManager.Instance.Play("Hero/Yasuo/W_OnCast", "Yasuo_W_OnCast");
                    AudioManager.Instance.Play("Hero/Yasuo/W_Voice", "Yasuo_W_Voice");
                    AudioManager.Instance.Play("Hero/Yasuo/W_Background", "Yasuo_W_Background", true);
                    
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
                            AudioManager.Instance.Stop("Yasuo_W_Background");
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
                        if (ToolFunctions.IsOverlappingInBoxColliderAll(effect.effect, out var results))
                        {
                            foreach (var result in results)
                            {
                                if (result.isAlive && result.team != owner.team)
                                {
                                    self.bulletEntityDamageCDTimer.TryAdd(result, self.bulletDamageCD);

                                    if (self.bulletEntityDamageCDTimer[result] > self.bulletDamageCD)
                                    {
                                        self.bulletEntityDamageCDTimer[result] = 0;

                                        var damageValue = result.CalculateAPDamage(self.owner, _damage);
                                        result.TakeDamage(damageValue, DamageType.AP, owner,
                                            Random.Range(0f, 1f) < owner.criticalRate.Value &&
                                            owner.canSkillCritical);

                                        // 造成1秒40%减速
                                        var speedReduce = new SpeedReduce(result, owner, 1f, 0.4f);
                                        result.GainBuff(speedReduce);
                                        
                                        // 造成技能特效
                                        self.owner.AbilityEffectActivate(result, damageValue, this);

                                        AudioManager.Instance.Play("Hero/Yasuo/W_OnHit", "Yasuo_W_OnHit");
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