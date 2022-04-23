using System;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Core;

namespace GameAILab.Sandbox
{

    public class AISquad : MonoBehaviour
    {
        protected class BattlePoint
        {
            public Vector3 position;
            public bool isClaimed;
            public GameObject owner;
        }


        [Header("Battle Points")]
        public float[] radiuses = { 3, 6 };
        public int[] angleStepDegrees = { 30, 20 };
        [Range(0, 1)]
        public float bpNpcDstWeight = 0.8f;
        [Range(0, 1)]
        public float bpPlayerDstWeight = 0.2f;
        [Range(0, 1)]
        public float bpPlayerAngleWeight = 0.8f;

        [Header("Target Track")]
        public float forgetTargetTime = 3.0f;

        [Header("Debug")]
        public Color dbg_bpClr = Color.yellow;

        public List<AIActor> Members { get; protected set; } = new List<AIActor>();

        public IActor Target 
        {
            get => m_target;
            set
            {
                m_target = value;
                if (value != null)
                {
                    LastTargetPos = m_target.Go.transform.position;
                }
            }
        }
        protected IActor m_target;

        public Vector3 LastTargetPos { get; set; }

        protected int m_seeTargetMemberCount = 0;
        protected List<BattlePoint> m_battlePoints = new List<BattlePoint>();
        protected float[] m_bpNpcDstScores    = new float[20];
        protected float[] m_bpPlayerDstScores = new float[20];
        protected float[] m_bpPlayerCosScores = new float[20];


        #region Member to Squad Communication

        public void OnMemberSeePlayer(IActor player)
        {
            if (player is null)
                throw new ArgumentException();

            ++m_seeTargetMemberCount;
            CancelInvoke(nameof(CancelTarget));

            if (Target is null)
                Target = player;
        }

        public void OnMemberLosePlayer()
        {
            if (Target is null)
                return;

            --m_seeTargetMemberCount;
            if (m_seeTargetMemberCount == 0)
            {
                Debug.Log("all ais lost the player");
                Invoke(nameof(CancelTarget), forgetTargetTime);                
            }
        }

        protected void CancelTarget()
        {
            Target = null;
        }

        #endregion


        #region Squad to Member Communication

        public void BuildUpBattlePoints()
        {
            if (radiuses.Length != angleStepDegrees.Length)
            {
                Debug.LogError("AISquad radiuses.Length != angleStepDegrees.Length");
            }

            Transform player = Target.Go.transform;
            m_battlePoints.Clear();

            for (int i=0; i<radiuses.Length; ++i)
            {
                float r = radiuses[i];
                int astep = angleStepDegrees[i];

                for (int a = astep; a <= 360; a += astep)
                {
                    Vector3 p = Quaternion.AngleAxis(a, Vector3.up) * player.forward * r;
                    BattlePoint bp = new BattlePoint { position = p + player.position };
                    m_battlePoints.Add(bp);
                }
            }
        }

        public void ClearBattlePoints()
        {
            m_battlePoints.Clear();
        }

        public void UpdateMemberBattlePoints()
        {
            Transform player = Target.Go.transform;

            CalculateBpPlayerDstScores(player);
            CalculateBpPlayerAngleScores(player);

            foreach (AIActor npcActor in Members)
            {
                CalculateBpNpcDstScores(npcActor.transform);

                BattlePoint bp = SelectHighestScoreBp();
                if (bp is null)
                {
                    continue;
                }

                bp.isClaimed = true;
                bp.owner = npcActor.gameObject;
                npcActor.BattlePoint = bp.position;
            }
        }

        protected BattlePoint SelectHighestScoreBp()
        {
            BattlePoint result = null;

            float maxScore = float.MinValue;
            for (int i = 0; i < m_battlePoints.Count; ++i)
            {
                BattlePoint bp = m_battlePoints[i];
                if (bp.isClaimed)
                {
                    continue;
                }

                float totalScore = m_bpPlayerDstScores[i] + m_bpPlayerCosScores[i] + m_bpNpcDstScores[i];

                if (totalScore > maxScore)
                {
                    maxScore = totalScore;
                    result = bp;
                }
            }

            return result;
        }

        protected void CalculateBpNpcDstScores(Transform npc)
        {
            if (m_bpNpcDstScores.Length < m_battlePoints.Count)
            {
                m_bpNpcDstScores = new float[m_battlePoints.Count];
            }

            // calculate sqr-distances
            float maxSqrDst = float.MinValue;
            for (int i = 0; i < m_battlePoints.Count; ++i)
            {
                BattlePoint bp = m_battlePoints[i];
                if (bp.isClaimed)
                    continue;

                float sqrDst = Vector3.SqrMagnitude(npc.position - bp.position);
                m_bpNpcDstScores[i] = sqrDst;
                if (sqrDst > maxSqrDst)
                    maxSqrDst = sqrDst;
            }

            for (int i = 0; i < m_battlePoints.Count; ++i)
            {
                BattlePoint bp = m_battlePoints[i];
                if (bp.isClaimed)
                    continue;

                // normalize
                m_bpNpcDstScores[i] /= maxSqrDst;
                // final score
                m_bpNpcDstScores[i] = (1 - m_bpNpcDstScores[i]) * bpNpcDstWeight;
            }
        }

        protected void CalculateBpPlayerDstScores(Transform player)
        {
            if (m_bpPlayerDstScores.Length < m_battlePoints.Count)
            {
                m_bpPlayerDstScores = new float[m_battlePoints.Count];
            }

            // calculate sqr-distances
            float maxSqrDst = float.MinValue;
            for (int i=0; i<m_battlePoints.Count; ++i)
            {
                BattlePoint bp = m_battlePoints[i];

                float sqrDst = Vector3.SqrMagnitude(player.position - bp.position);
                m_bpPlayerDstScores[i] = sqrDst;
                if (sqrDst > maxSqrDst)
                    maxSqrDst = sqrDst;
            }

            for (int i = 0; i < m_battlePoints.Count; ++i)
            {
                BattlePoint bp = m_battlePoints[i];
                if (bp.isClaimed)
                    continue;

                // normalize
                m_bpPlayerDstScores[i] /= maxSqrDst;
                // final score
                m_bpPlayerDstScores[i] = (1- m_bpPlayerDstScores[i]) * bpPlayerDstWeight;
            }
        }

        protected void CalculateBpPlayerAngleScores(Transform player)
        {
            if (m_bpPlayerCosScores.Length < m_battlePoints.Count)
            {
                m_bpPlayerCosScores = new float[m_battlePoints.Count];
            }

            // calculate cosines
            for (int i = 0; i < m_battlePoints.Count; ++i)
            {
                BattlePoint bp = m_battlePoints[i];

                float score = Vector3.Dot(player.forward, (bp.position - player.position).normalized);

                // normalize
                score = (score + 1.0f) * 0.5f;
                // final score
                score *= bpPlayerAngleWeight;

                m_bpPlayerCosScores[i] = score;
            }
        }

        public void UpdateMemberTargets(IActor target)
        {
            foreach (AIActor m in Members)
            {
                m.Target = target;
            }
        }

        #endregion


        #region Member Management

        public void RegisterMember(AIActor member)
        {
            if (member is null)
                throw new ArgumentNullException();

            if (!Members.Contains(member))
            {
                member.Squad = this;
                Members.Add(member);
            }
            else
            {
                Debug.LogError($"ai squad {gameObject.name} already contains ai actor {member.Id}");
            }
        }

        public void UnregisterMember(AIActor member)
        {
            if (member != null)
            {
                if (Members.Remove(member))
                {
                    member.Squad = null;
                }
            }
        }

        #endregion


        protected void OnDestroy()
        {
            for (int i = Members.Count - 1; i >= 0; --i)
            {
                UnregisterMember(Members[i]);
            }
        }

        // does not exist at runtime
        //[SerializeField]
        //protected SpawnPoint[] spawnPoints;

        /*protected void Awake()
        {
            for(int i=0; i<spawnPoints.Length; ++i)
            {
                GameObject go = Instantiate(spawnPoints[i].prefab);
                if (go is null)
                {
                    Debug.LogError("failed to instantiate an ai squad member");
                }
                else
                {
                    AIActor actor = go.GetComponent<AIActor>();
                    if (actor is null)
                    {
                        Debug.LogError("an ai squad member does not have an AIActor attached");
                    }
                    else
                    {
                        RegisterMember(actor);
                    }
                }

                Destroy(spawnPoints[i].gameObject);
            }
            Array.Clear(spawnPoints, 0, spawnPoints.Length);
        }*/

        // for test
        public AIActor[] mems_temp;

        private void Awake()
        {
            foreach (AIActor a in mems_temp)
                RegisterMember(a);
        }

        private void OnDrawGizmos()
        {
            // battle points ---------------------------
            Gizmos.color = dbg_bpClr;

            foreach (var v in m_battlePoints)
            {
                Gizmos.DrawWireCube(v.position, Vector3.one*0.3f);
            }
        }

        // strategic point generation
#if false
        List<Vector3> points = new List<Vector3>();
        public Transform ground;
        private void BuildUpStrategicPoints()
        {
            Vector3 origin = ground.position;
            Vector3 unsSize = ground.GetComponent<MeshFilter>().mesh.bounds.size;
            Vector3 scale = ground.localScale;

            float minx = origin.x - unsSize.x * scale.x * 0.5f;
            float minz = origin.z - unsSize.z * scale.z * 0.5f;
            float maxx = origin.x + unsSize.x * scale.x * 0.5f;
            float maxz = origin.z + unsSize.z * scale.z * 0.5f;

            for (int x = Mathf.CeilToInt(minx); x <= Mathf.FloorToInt(maxx); x += 5)
            {
                for (int z = Mathf.CeilToInt(minz); z <= Mathf.FloorToInt(maxz); z += 5)
                {
                    // todo: 检查点和障碍物的距离是否大于角色的半径
                    Vector3 p = new Vector3(x + UnityEngine.Random.Range(-1, 1), origin.y, z + UnityEngine.Random.Range(-1, 1));
                    points.Add(p);
                }
            }
        }
#endif
    }

}