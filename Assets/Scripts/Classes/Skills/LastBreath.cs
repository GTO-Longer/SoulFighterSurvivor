using System.Linq;
using DataManagement;
using Factories;
using Managers;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class LastBreath : Skill
    {
        private const float attackBulletSpeed = 1500;
        private float _damage => _baseSkillValue[0][skillLevelToIndex] + 1.5f * owner.attackDamage;
        
        public LastBreath() : base("LastBreath")
        {
            _skillLevel = 0;
            _maxSkillLevel = 3;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription, _damage);
        }

        public override bool SkillEffect(out string failMessage)
        {
            failMessage = string.Empty;
            
            // 遍历寻找范围内的目标
            var targets = ToolFunctions.IsOverlappingOtherTagAll(owner.gameObject, skillRange)?.ToList().FindAll(entity => entity.isControlled.Value);

            if (targets is not { Count: > 0 })
            {
                failMessage = "无有效目标";
                return false;
            } 

            // 选取目标
            Entity target = null;

            if (ToolFunctions.IsObjectAtMousePoint(out var obj, "Enemy", true))
            {
                target = obj.FindAll(GameObject => GameObject.GetComponent<EntityData>())[0].GetComponent<EntityData>() .entity;
            }

            if (target == null || !targets.Contains(target))
            {
                target = targets[0];
            }
            
            AudioManager.Instance.Play("Hero/Yasuo/R_Voice", "Yasuo_R_Voice");
            AudioManager.Instance.Play("Hero/Yasuo/R_OnCast", "Yasuo_R_OnCast");

            // 闪现到目标身后
            var targetDirection = (Vector2)(target.gameObject.transform.position - owner.gameObject.transform.position).normalized;
            owner.Flash(targetDirection, Vector2.Distance(target.gameObject.transform.position, owner.gameObject.transform.position) + target.scale + owner.attackRange);
            
            // 面向敌人
            var angleRad = Mathf.Atan2(-targetDirection.y, -targetDirection.x);
            var angleDeg = angleRad * Mathf.Rad2Deg;
            owner.gameObject.transform.eulerAngles = new Vector3(0, 0, angleDeg);
            
            // 使附近范围内的受控制敌人的控制状态增加1秒
            foreach (var entity in targets)
            {
                entity.controlTime.Value += 1;
            }
            
            // 获得满额剑意
            owner.energy.Value = owner.maxEnergy.Value;
            
            // 获得15秒60%额外物理穿透
            var lastBreath = new Buffs.LastBreath(owner, owner);
            lastBreath.buffIcon = null;
            owner.GetBuff(lastBreath);
            
            // 斩击3次
            var continuousTime = 0f;
            var slashTime = 0;
            AudioManager.Instance.Play("Hero/Yasuo/R_OnHit", "Yasuo_R_OnHit");
            Async.SetAsync(0.75f, null, () =>
            {
                // 设置角色无法使用技能、无法移动
                owner.canUseSkill = false;
                owner.canMove = false;
                owner.canFlash = false;
                owner.agent.SetStop(true);
                continuousTime += Time.deltaTime;

                if (continuousTime >= 0.3f * slashTime)
                {
                    slashTime += 1;
                    
                    var bullet = BulletFactory.Instance.CreateBullet(owner);
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
                            self.gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

                            // 子弹的销毁逻辑
                            const float destroyDistance = 30f;
                            if (Vector3.Distance(self.gameObject.transform.position, self.target.gameObject.transform.position) <= destroyDistance)
                            {
                                self.BulletHit();
                                self.Destroy();
                            }
                        };
                    };

                    bullet.OnBulletHit += (_) =>
                    {
                        // 每段造成三分之一的大招伤害
                        var damageCount = target.CalculateADDamage(owner, _damage / 3f);
                        target.TakeDamage(damageCount, DamageType.AD, owner, Random.Range(0f, 1f) < owner.criticalRate.Value && owner.canSkillCritical);
                    };
                
                    bullet.Awake();
                }
            }, () =>
            {
                AudioManager.Instance.Play("Hero/Yasuo/R_OnFinish", "Yasuo_R_OnFinish");
                
                owner.canUseSkill = true;
                owner.canFlash = true;
                owner.canMove = true;
                owner.agent.SetStop(false);
            });

            return true;
        }
    }
}