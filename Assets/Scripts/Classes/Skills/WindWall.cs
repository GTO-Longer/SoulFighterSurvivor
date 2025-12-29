using Factories;
using Systems;
using UnityEngine;
using Utilities;

namespace Classes.Skills
{
    public class WindWall : Skill
    {
        public WindWall() : base("WindWall")
        {
            _skillLevel = 0;
            _maxSkillLevel = 5;
            
            coolDownTimer = 999;
        }

        public override string GetDescription()
        {
            return string.Format(_skillDescription);
        }

        public override bool SkillEffect()
        {
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
                windWall.OnBulletAwake += (self) =>
                {
                    self.target = null;
                    self.gameObject.transform.position = owner.gameObject.transform.position;
                    self.gameObject.SetActive(true);
                    var hasInitialized = false;
                    var speed = Vector2.zero;

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
                        if (Vector2.Distance(self.gameObject.transform.position, owner.gameObject.transform.position) > skillRange)
                        {
                            self.Destroy();
                        }
                        else
                        {
                            // 控制子弹位置和面向
                            self.gameObject.transform.position += (Vector3)(speed * Time.deltaTime);
                            var angle = Vector2.SignedAngle(Vector2.up, speed.normalized);
                            self.gameObject.transform.rotation = Quaternion.Euler(0, 0, angle);
                        }
                    };
                };

                windWall.Awake();
            });

            return true;
        }
    }
}