using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;

namespace RVO
{
    internal class RVOManager : MonoBehaviour
    {
        private List<int> agents;
        private Dictionary<int, RVOAgent> agentDic;

        public Simulator simulator;
        private CustomSampler sampler;
        private System.Random rng = new System.Random();

        public static RVOManager Instance;

        private void Awake()
        {
            Instance = this;

            simulator = new Simulator();
            simulator.SetTimeStep(Time.fixedDeltaTime);

            simulator.SetAgentDefaults(
                300f,   // neighborDist
                30,     // maxNeighbors
                3f,     // timeHorizon
                60f,    // timeHorizonObst
                100f,   // radius
                350f,   // maxSpeed
                new float2(0f, 0f)
            );

            agents = new List<int>();
            agentDic = new Dictionary<int, RVOAgent>();

            sampler = CustomSampler.Create("RVO Update");
        }

        private void FixedUpdate()
        {
            sampler.Begin();
            SetPreferredVelocities();
            simulator.DoStep();
            simulator.EnsureCompleted();
            sampler.End();

            AgentsPositionUpdate();
        }

        /// <summary>
        /// 添加新的Agent
        /// </summary>
        public int AddAgent(RVOAgent agent)
        {
            var id = simulator.AddAgent(new float2(agent.transform.position.x, agent.transform.position.y));
            agents.Add(id);
            agentDic[id] = agent;
            return id;
        }

        /// <summary>
        /// 删除Agent
        /// </summary>
        public void RemoveAgent(int agentId)
        {
            simulator.RemoveAgent(agentId);
            agents.Remove(agentId);
            agentDic.Remove(agentId);
        }

        /// <summary>
        /// 设置每个Agent的期望速度
        /// </summary>
        private void SetPreferredVelocities()
        {
            foreach (var id in agents)
            {
                var agent = agentDic[id];

                // 停止则跳过
                if (agent.isStopped)
                {
                    continue;
                }

                // 判断该使用什么寻路方式
                agent.ChangePathFindMethod(simulator.GetAgentPosition(id));
                simulator.SetAgentMaxSpeed(id, agent.entity.movementSpeed.Value);
                
                var prefVel = agent.GetDesiredVelocity(simulator, id);
                var currentVel = simulator.GetAgentVelocity(id);
                var maxSpeed = agent.entity.movementSpeed.Value;
                var stuckThresholdSq = math.pow(maxSpeed * 0.1f, 2); 
                var desireThresholdSq = math.pow(10f, 2);
                
                if (agent.noiseDuration > 0)
                {
                    agent.noiseDuration -= Time.fixedDeltaTime;
                    prefVel += agent.activeNoise;
                }
                else
                {
                    if (!agent.UsingNavMeshMovement && math.lengthsq(prefVel) > desireThresholdSq && math.lengthsq(currentVel) < stuckThresholdSq)
                    {
                        agent.stuckTimer += Time.fixedDeltaTime;
                        
                        // 如果连续卡住0.2秒以上
                        if (agent.stuckTimer > 0.2f)
                        {
                            var turnLeft = (id % 2 == 0); 
                            var tangentDir = agent.GetTangentDirection(turnLeft);
            
                            agent.activeNoise = tangentDir * agent.entity.movementSpeed.Value * 1.0f; 
                            agent.noiseDuration = 0.5f; 
                            agent.stuckTimer = 0f;
                        }
                    }
                }
                
                simulator.SetAgentPrefVelocity(id, prefVel);
            }
        }

        /// <summary>
        /// 更新所有Agent的Transform表现
        /// </summary>
        private void AgentsPositionUpdate()
        {
            foreach (var id in agents)
            {
                agentDic[id].AgentUpdate();
            }
        }

        private void OnDestroy()
        {
            simulator.Clear();
            simulator.Dispose();
        }
    }
}
