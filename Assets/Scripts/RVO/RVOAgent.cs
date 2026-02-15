using Classes;
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
        
        // 处于幽灵模式
        public bool isGhost;
        
        public float2 goal;
        private int agentId = -1;

        private bool changeGoal;
        private int currentCornerIndex;

        private const float keepRVOTime = 0.5f;
        private float keepRVOTimer;
        
        // 随机扰动控制
        public float stuckTimer;
        public float2 activeNoise = float2.zero;
        public float noiseDuration;

        public LayerMask unitsLayer;
        public LayerMask navObstacleLayer;

        /// <summary>
        /// 是否使用NavMesh寻路
        /// </summary>
        private bool useNavMeshMovement;
        public bool UsingNavMeshMovement => useNavMeshMovement;

        /// <summary>
        /// 初始化Agent
        /// </summary>
        public void AgentInitialization(Entity agentEntity)
        {
            manager = RVOManager.Instance;
            agentId = manager.AddAgent(this);
            entity = agentEntity;
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
        /// 获取Agent的朝向
        /// </summary>
        public Vector2 GetAgentDirection()
        {
            return manager.simulator.GetAgentVelocity(agentId);
        }
        
        /// <summary>
        /// 更新Agent
        /// </summary>
        public void AgentUpdate()
        {
            manager.simulator.SetAgentRadius(agentId, entity.scale);
            
            if (!isStopped)
            {
                CheckReachTarget();
                
                // 幽灵模式下不避障
                if (isGhost)
                {
                    manager.simulator.SetAgentNeighborDist(agentId, 0f);
                    manager.simulator.SetAgentNeighborDist(agentId, 0);
                }
                else
                {
                    manager.simulator.SetAgentNeighborDist(agentId, 30f);
                    manager.simulator.SetAgentNeighborDist(agentId, entity.movementSpeed.Value);
                }
                
                manager.simulator.SetAgentTimeHorizonObst(agentId, 0.5f);
                manager.simulator.SetAgentTimeHorizon(agentId, 0.5f);
                manager.simulator.SetAgentMaxSpeed(agentId, 10000f);
                
                var pos = manager.simulator.GetAgentPosition(agentId);
                transform.position = new Vector3(pos.x, pos.y, 0f);
                
                var direction = GetAgentDirection();
                entity.RotateTo(ref direction);
            }
            
            entity.isMoving = !_isStopped;
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
            // 只有更新目标的时候才需要重算路线
            if (changeGoal)
            {
                // 刷新路径
                RegeneratePath(pos);
                changeGoal = false;
            }
            
            if (isGhost)
            {
                useNavMeshMovement = true;
                keepRVOTimer = 0;
                return;
            }
            
            if (keepRVOTimer > 0)
            {
                keepRVOTimer -= Time.fixedDeltaTime;
                return;
            }

            Vector2 targetPoint;

            if (navPath.corners != null && navPath.corners.Length > 0 && currentCornerIndex < navPath.corners.Length)
            {
                targetPoint = new Vector2(navPath.corners[currentCornerIndex].x, navPath.corners[currentCornerIndex].y);
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
            Physics2D.CircleCastNonAlloc(origin, entity.scale.Value * 0.99f, dir, results, rayLen, unitsLayer | navObstacleLayer);
            
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
                    keepRVOTimer = keepRVOTime;
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
        public bool TryWallSideAvoidance(float2 pos, out bool left)
        {
            left = false;
            
            if (useNavMeshMovement) return false;
            
            Vector2 targetPoint;

            if (navPath.corners != null && navPath.corners.Length > 0 && currentCornerIndex < navPath.corners.Length)
            {
                targetPoint = new Vector2(navPath.corners[currentCornerIndex].x, navPath.corners[currentCornerIndex].y);
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
            var leftDirection = new Vector2(-dir.y, dir.x);
            var rightDirection = new Vector2(dir.y, -dir.x);
            
            // 侧边检测长度
            var sideRayLen = entity.scale.Value * 2f;

            bool hitLeft  = Physics2D.Raycast(origin, leftDirection, sideRayLen, navObstacleLayer);
            bool hitRight = Physics2D.Raycast(origin, rightDirection, sideRayLen, navObstacleLayer);

            if (!hitLeft && !hitRight)
            {
                return false;
            }
            
            // 向没有墙的方向给予强制速度
            if (!hitRight)
            {
                left = false;
                return true;
            }
            
            if (!hitLeft)
            {
                left = true;
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
            gameObject.transform.position = pos;
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
                // 恢复时回到物体的当前所在位置
                Warp(transform.position);
            }
            
            _isStopped = stopped;
        }

        /// <summary>
        /// 注销Agent
        /// </summary>
        public void RemoveAgent()
        {
            manager.RemoveAgent(agentId);
        }
        
        /// <summary>
        /// 检查Agent是否抵达目标点
        /// </summary>
        private void CheckReachTarget()
        {
            if (!isStopped)
            {
                _isStopped = Vector2.Distance(transform.position, new Vector2(goal.x, goal.y)) <= entity.movementSpeed.Value * Time.fixedDeltaTime;

                if (isStopped)
                {
                    Warp(goal);
                }
            }
        }
        
        /// <summary>
        /// 获取期望速度
        /// </summary>
        public float2 GetDesiredVelocity(Simulator sim, int id)
        {
            var pos = sim.GetAgentPosition(id);
            var speed = entity.movementSpeed.Value;

            // Nav模式
            if (useNavMeshMovement && navPath != null && navPath.corners != null && navPath.corners.Length > 0)
            {
                // 确保索引安全
                if (currentCornerIndex >= navPath.corners.Length)
                {
                    currentCornerIndex = navPath.corners.Length - 1;
                }

                var corner = navPath.corners[currentCornerIndex];
                var target = new float2(corner.x, corner.y);
                var dis = Vector2.Distance(target, pos);

                // 接近当前corner，推进index
                if (dis <= speed * Time.fixedDeltaTime)
                {
                    currentCornerIndex++;
                    
                    if (currentCornerIndex >= navPath.corners.Length)
                    {
                        currentCornerIndex = navPath.corners.Length - 1;
                    }

                    // 联动到下一个拐点
                    corner = navPath.corners[currentCornerIndex];
                    target = new float2(corner.x, corner.y);
                }

                // 返回速度向量
                return ((Vector2)target - (Vector2)pos).normalized * speed;
            }

            // 使用RVO
            if (navPath != null && navPath.corners != null && navPath.corners.Length > 1)
            {
                return new Vector2(navPath.corners[1].x - pos.x, navPath.corners[1].y - pos.y).normalized * speed;
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
        
        public float2 GetTangentDirection(bool turnLeft)
        {
            var pos = manager.simulator.GetAgentPosition(agentId);
            float2 target;

            // 获取当前正在前往的目标点
            if (navPath != null && navPath.corners != null && currentCornerIndex < navPath.corners.Length)
            {
                target = new float2(navPath.corners[currentCornerIndex].x, navPath.corners[currentCornerIndex].y);
            }
            else
            {
                target = goal;
            }

            var dir = (new Vector2(target.x, target.y) - new Vector2(pos.x, pos.y)).normalized;

            return turnLeft ? new float2(-dir.y, dir.x) : new float2(dir.y, -dir.x);
        }
    }
}
