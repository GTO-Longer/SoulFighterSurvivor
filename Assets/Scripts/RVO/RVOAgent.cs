using Classes;
using DataManagement;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.AI;

namespace RVO
{
    public class RVOAgent : MonoBehaviour
    {
        private RVOManager manager;
        public Entity entity;

        public bool isStopped;
        private bool changeGoal;
        public float2 goal;

        private int agentId = -1;

        private NavMeshPath navPath;
        private int currentCornerIndex;
        public bool forceNavWhenPathIncomplete = true;

        public float neighborMultiplier = 3.0f;
        public float neighborMinFactor = 1.5f;
        public float neighborMax = 800f;

        public float navSwitchDistanceFactor = 5f;

        public LayerMask unitsLayer;
        public LayerMask navObstacleLayer;

        public bool debugDrawPath;
        public bool enableDebugLogs = true;

        public bool enableMinSpeedFallback = true;
        public float minPrefVelThresholdFactor = 0.05f;
        public float minSpeedFactor = 0.25f;

        private float lastNavPathLength;
        /// <summary>
        /// 是否使用NavMesh寻路
        /// </summary>
        private bool useNavMeshMovement;

        public bool UsingNavMeshMovement => useNavMeshMovement;
        public float LastNavPathLength => lastNavPathLength;

        private void Start()
        {
            manager = RVOManager.Instance;
            agentId = manager.AddAgent(this);
            entity = GetComponent<EntityData>().entity;

            var navAgent = GetComponent<NavMeshAgent>();
            if (navAgent != null)
            {
                navAgent.updateRotation = false;
                navAgent.updateUpAxis = false;
                navAgent.updatePosition = false;
            }

            var p = transform.position;
            transform.position = new Vector3(p.x, p.y, 0f);

            navPath = new NavMeshPath();
            currentCornerIndex = 0;
            lastNavPathLength = 0f;
            useNavMeshMovement = false;
        }
        
        /// <summary>
        /// 更新Agent位置和旋转
        /// </summary>
        public void AgentUpdate()
        {
            CheckReachTarget();
            
            if (!isStopped)
            {
                var pos = manager.simulator.GetAgentPosition(agentId);
                transform.position = new Vector3(pos.x, pos.y, 0f);

                var velocity = manager.simulator.GetAgentVelocity(agentId);
                transform.eulerAngles = new Vector3(0, 0, Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg);
            }
        }

        /// <summary>
        /// 设置目标位置
        /// </summary>
        public void SetDestination(Vector2 position)
        {
            if (NavMesh.SamplePosition(position, out var hit, 1000f, NavMesh.AllAreas))
            {
                goal = new float2(hit.position.x, hit.position.y);
            }
            else
            {
                goal = new float2(position.x, position.y);
            }
            
            changeGoal = true;
        }

        // 检查是否需要更换寻路方式
        public void ChangePathFindMethod(float2 pos)
        {
            // 只有更新目标的时候或者使用RVO寻路时才需要重算路线
            if (changeGoal || !useNavMeshMovement)
            {
                // 刷新路径
                RegeneratePath(pos);

                changeGoal = false;
            }

            Vector2 targetPoint;

            if (navPath.corners != null &&
                navPath.corners.Length > 0 &&
                currentCornerIndex < navPath.corners.Length)
            {
                targetPoint = new Vector2(navPath.corners[currentCornerIndex].x,
                                          navPath.corners[currentCornerIndex].y);
            }
            else
            {
                targetPoint = new Vector2(goal.x, goal.y);
            }

            // 发射前向CircleCastAll判定障碍物
            var origin = new Vector2(pos.x, pos.y);
            var dir = (targetPoint - origin).normalized;
            origin += dir * entity.scale;
            
            var rayLen = entity.scale.Value * 2f;
            var results = new RaycastHit2D[10];
            Physics2D.CircleCastNonAlloc(origin, entity.scale.Value * 0.9f, dir, results, rayLen, unitsLayer | navObstacleLayer);
            Debug.DrawLine(origin, origin + dir * rayLen, Color.red);
            
            foreach (var h in results)
            {
                if (!h.collider) continue;

                var go = h.collider.gameObject;

                // 忽略自己
                if (go == gameObject)
                    continue;

                // 检测到敌人切换RVO
                if (go.CompareTag("Enemy"))
                {
                    useNavMeshMovement = false;
                    return;
                }

                // 被墙阻挡自动设置Nav
                if (go.CompareTag("Wall"))
                {
                    useNavMeshMovement = true;
                    return;
                }
            }

            useNavMeshMovement = true;
        }
        
        // RVO寻路中向没墙体的方向逃逸
        public bool TryWallSideAvoidance(float2 pos, out float2 escapeDir)
        {
            escapeDir = float2.zero;

            // 如果处于Nav寻路中直接返回
            if (useNavMeshMovement)
                return false;
            
            Vector2 targetPoint;

            if (navPath.corners != null &&
                navPath.corners.Length > 0 &&
                currentCornerIndex < navPath.corners.Length)
            {
                targetPoint = new Vector2(navPath.corners[currentCornerIndex].x,
                    navPath.corners[currentCornerIndex].y);
            }
            else
            {
                targetPoint = new Vector2(goal.x, goal.y);
            }

            // 确定最终前进方向
            var origin = new Vector2(pos.x, pos.y);
            var dir = (targetPoint - origin).normalized;
            origin += dir * entity.scale * 1.5f;

            // 左右侧方向
            var left = new Vector2(-dir.y, dir.x);
            var right = new Vector2(dir.y, -dir.x);
            
            // 侧边检测长度
            var sideRayLen = entity.scale.Value * 2f;

            bool hitLeftWall  = Physics2D.Raycast(origin, left, sideRayLen, navObstacleLayer);
            bool hitRightWall = Physics2D.Raycast(origin, right, sideRayLen, navObstacleLayer);

            Debug.DrawLine(origin, origin + left * sideRayLen, hitLeftWall  ? Color.red : Color.green);
            Debug.DrawLine(origin, origin + right * sideRayLen, hitRightWall ? Color.red : Color.green);

            if (!hitLeftWall && !hitRightWall)
            {
                return false;
            }
            
            // 向没有墙的方向给予强制速度
            if (!hitRightWall)
            {
                escapeDir = new float2(right.x, right.y);
                return true;
            }
            if (!hitLeftWall)
            {
                escapeDir = new float2(left.x, left.y);
                return true;
            }

            return false;
        }
        
        // 检查Agent是否抵达目标点
        private void CheckReachTarget()
        {
            isStopped = Vector2.Distance(transform.position, new Vector2(goal.x, goal.y)) <= entity.scale.Value * 0.3f;
        }
        
        // 获取期望速度
        public float2 GetDesiredVelocity(Simulator sim, int id)
        {
            var pos = sim.GetAgentPosition(id);

            // NAV 模式
            if (useNavMeshMovement && navPath != null && navPath.corners != null && navPath.corners.Length > 0)
            {
                // 确保索引安全
                if (currentCornerIndex >= navPath.corners.Length)
                    currentCornerIndex = navPath.corners.Length - 1;

                var corner = navPath.corners[currentCornerIndex];
                var target = new float2(corner.x, corner.y);

                var dis = Vector2.Distance(target, pos);

                // 接近当前 corner，推进 index
                if (dis < 10f)
                {
                    currentCornerIndex++;
                    
                    // 关键修复：推进后必须再次检查索引
                    if (currentCornerIndex >= navPath.corners.Length)
                    {
                        // 已经到最后一个点，直接朝最终目标移动
                        currentCornerIndex = navPath.corners.Length - 1;
                    }

                    // 联动到下一个拐点
                    corner = navPath.corners[currentCornerIndex];
                    target = new float2(corner.x, corner.y);
                }

                // 返回速度向量
                return ((Vector2)target - (Vector2)pos).normalized * entity.movementSpeed.Value;
            }

            // 使用RVO
            RegeneratePath(pos);
            if (navPath != null && navPath.corners != null && navPath.corners.Length > 1)
            {
                var vel = new float2(navPath.corners[1].x - pos.x, navPath.corners[1].y - pos.y);
                var speed = entity.movementSpeed.Value;
                if (math.lengthsq(vel) > 1f)
                    vel = math.normalize(vel) * speed;

                return vel;
            }
            
            return float2.zero;
        }

        /// <summary>
        /// 使用Nav重新计算路线
        /// </summary>
        private void RegeneratePath(float2 pos)
        {
            var start = new Vector3(pos.x, pos.y, 0f);
            var goalV = new Vector3(goal.x, goal.y, 0f);

            if (NavMesh.SamplePosition(start, out var hit, 100f, NavMesh.AllAreas))
            {
                start = new Vector3(hit.position.x, hit.position.y, 0f);
            }

            navPath ??= new NavMeshPath();
            NavMesh.CalculatePath(start, goalV, NavMesh.AllAreas, navPath);
        }
        
        private void OnDrawGizmosSelected()
        {
            if (!debugDrawPath) return;

            if (navPath != null && navPath.corners != null)
            {
                Gizmos.color = Color.cyan;
                for (var i = 0; i < navPath.corners.Length - 1; i++)
                {
                    var a = navPath.corners[i]; a.z = 0f;
                    var b = navPath.corners[i + 1]; b.z = 0f;
                    Gizmos.DrawLine(a, b);
                }

                if (currentCornerIndex < navPath.corners.Length)
                {
                    var cc = navPath.corners[currentCornerIndex];
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(new Vector3(cc.x, cc.y, 0f), 0.2f);
                }
            }
        }
    }
}
