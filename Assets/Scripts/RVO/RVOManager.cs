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
                600f,   // neighborDist
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

                // Nav寻路时RVO避障
                if (agent.UsingNavMeshMovement)
                {
                    simulator.SetAgentTimeHorizonObst(id, 0f);

                    simulator.SetAgentPrefVelocity(id, prefVel);
                }
                else
                {
                    // 恢复RVO避障
                    simulator.SetAgentTimeHorizonObst(id, 60f);
                    simulator.SetAgentPrefVelocity(id, prefVel);

                    // 防对称锁死随机扰动
                    var angle = (float)rng.NextDouble() * math.PI * 2f;
                    var noise = new float2(math.cos(angle), math.sin(angle)) * (float)rng.NextDouble() * 50f;

                    simulator.SetAgentPrefVelocity(id, simulator.GetAgentPrefVelocity(id) + noise);
                }
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
