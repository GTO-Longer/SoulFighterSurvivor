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
        private NavMeshPath navPath;
        public NavMeshAgent navAgent;

        /// <summary>
        /// 是否停止寻路
        /// </summary>
        private bool _isStopped;
        public bool isStopped => _isStopped;
        
        public float2 goal;
        private int agentId = -1;

        private bool changeGoal;
        private int currentCornerIndex;

        public LayerMask unitsLayer;
        public LayerMask navObstacleLayer;

        /// <summary>
        /// 是否使用NavMesh寻路
        /// </summary>
        private bool useNavMeshMovement;
        public bool UsingNavMeshMovement => useNavMeshMovement;

        private void Start()
        {
            manager = RVOManager.Instance;
            agentId = manager.AddAgent(this);
            entity = GetComponent<EntityData>().entity;
            navAgent = GetComponent<NavMeshAgent>();

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
            useNavMeshMovement = false;
        }
        
        /// <summary>
        /// 更新Agent位置和旋转
        /// </summary>
        public void AgentUpdate()
        {
            if (!isStopped)
            {
                CheckReachTarget();
                
                var pos = manager.simulator.GetAgentPosition(agentId);
                transform.position = new Vector3(pos.x, pos.y, 0f);
                
                var velocity = (Vector2)manager.simulator.GetAgentVelocity(agentId);
                entity.RotateTo(velocity);
            }
        }

        /// <summary>
        /// 设置目标位置
        /// </summary>
        public void SetDestination(Vector2 position)
        {
            if (isStopped) return;
            
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

        /// <summary>
        /// 检查是否需要更换寻路方式
        /// </summary>
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
        
        /// <summary>
        /// RVO寻路中向没墙体的方向逃逸
        /// </summary>
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

        /// <summary>
        /// 瞬移到指定地点
        /// </summary>
        /// <param name="pos"></param>
        public void Warp(Vector2 pos)
        {
            manager.simulator.SetAgentPosition(agentId, new float2(pos.x, pos.y));
        }

        /// <summary>
        /// 设置是否停止
        /// </summary>
        public void SetStop(bool stopped)
        {
            if (stopped)
            {
                // 停止时设置Agent属性
                manager.simulator.SetAgentTimeHorizon(agentId, 1000f);
                manager.simulator.SetAgentTimeHorizonObst(agentId, 1000f); 
                manager.simulator.SetAgentNeighborDist(agentId, 0f);
                manager.simulator.SetAgentMaxSpeed(agentId, 0f);
                
                manager.simulator.SetAgentPrefVelocity(agentId, float2.zero);
            }
            else
            {
                manager.simulator.SetAgentTimeHorizon(agentId, 3f);
                manager.simulator.SetAgentTimeHorizonObst(agentId, 60f);
                manager.simulator.SetAgentNeighborDist(agentId, 600f);
                manager.simulator.SetAgentMaxSpeed(agentId, 3000f);
                
                // 恢复时回到物体所在位置
                Warp(transform.position);
            }
            
            _isStopped = stopped;
        }
        
        /// <summary>
        /// 检查Agent是否抵达目标点
        /// </summary>
        private void CheckReachTarget()
        {
            _isStopped = Vector2.Distance(transform.position, new Vector2(goal.x, goal.y)) <= entity.scale.Value * 0.3f;
        }
        
        /// <summary>
        /// 获取期望速度
        /// </summary>
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
    }
}
