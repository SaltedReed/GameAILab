using System;
using System.Collections.Generic;
using UnityEngine;
using GameAILab.Core;

namespace GameAILab.Sandbox
{

    public class AISquad : MonoBehaviour
    {
        public List<AIActor> Members { get; protected set; } = new List<AIActor>();

        public void OnMemberSeePlayer(IActor player)
        {
            if (player is null)
                throw new ArgumentException();

            Debug.Log($"an ai see the player");

            foreach (AIActor m in Members)
            {
                m.Target = player;
            }

            UpdateBattlePoints();
        }

        public void OnMemberLosePlayer()
        {
            Debug.Log("ai lost the player");
            foreach (AIActor m in Members)
            {
                m.Target = null;
            }
        }

        protected void UpdateBattlePoints()
        {
            foreach (AIActor a in Members)
            {
                // temp
                a.BattlePoint = new Vector3(UnityEngine.Random.Range(0, 5), 0, 0);
            }
        }

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
        public AIActor[] mems;

        private void Awake()
        {
            foreach (AIActor a in mems)
                RegisterMember(a);
        }
    }

}