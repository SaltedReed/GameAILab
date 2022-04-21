using UnityEngine;


namespace GameAILab.Sandbox
{

    public class Patrolman : MonoBehaviour
    {
        [Header("Patrol")]
        // do not exist at runtime
        [SerializeField]
        protected GameObject[] patrolNodeGos;
        public float speed = 3.5f;
        public float reachRadius = 0.1f;
        [SerializeField]
        //protected NavMeshAgent m_navAgent;
        protected Motor m_motor;
        public bool startOnPawn = true;

        [HideInInspector]
        public Vector3[] patrolPath;
        protected int m_targetPatrolNode = -1;
        protected bool m_isStopped = false;

        private void OnGUI()
        {
            if (GUILayout.Button("start"))
                RestartPatrol();
            if (GUILayout.Button("stop"))
                StopPatrol();
        }

        public void BuildUpPath()
        {
            patrolPath = new Vector3[patrolNodeGos.Length];
            for (int i = 0; i < patrolPath.Length; ++i)
            {
                patrolPath[i] = patrolNodeGos[i].transform.position;
            }
            for (int i = 0; i < patrolNodeGos.Length; ++i)
            {
                Destroy(patrolNodeGos[i]);
            }
            System.Array.Clear(patrolNodeGos, 0, patrolNodeGos.Length);
        }

        public void RestartPatrol()
        {
            // find the path node which is as near to the patrolman and 
            // near to the forward direction of the patrolman as possible
            Vector3 pos = transform.position;
            Vector3 fwd = transform.forward;

            m_targetPatrolNode = 0;
            Vector3 nodePos = patrolPath[m_targetPatrolNode];
            float cos = Vector3.Dot(fwd, (nodePos - pos).normalized);
            float sqr = Vector3.SqrMagnitude(pos - nodePos);
            float maxScore = cos / sqr;

            for (int i = 1; i < patrolPath.Length; ++i)
            {
                nodePos = patrolPath[i];
                cos = Vector3.Dot(fwd, (nodePos - pos).normalized);
                sqr = Vector3.SqrMagnitude(pos - nodePos);
                float score = cos / sqr;

                if (score > maxScore)
                {
                    maxScore = score;
                    m_targetPatrolNode = i;
                }
            }

            // set agent params
            m_motor.Speed = speed;
            m_motor.ReachRadius = reachRadius;

            // move to the node
            m_isStopped = false;
            MoveTo(patrolPath[m_targetPatrolNode]);
        }

        public void UpdatePatrol()
        {
            if (m_isStopped)
                return;

            if (m_motor.IsAt(patrolPath[m_targetPatrolNode]))
            {
                m_targetPatrolNode = (m_targetPatrolNode + 1) % patrolPath.Length;
                MoveTo(patrolPath[m_targetPatrolNode]);
            }
        }

        public void StopPatrol()
        {
            m_isStopped = true;
            m_motor.Stop();
        }

        protected void MoveTo(Vector3 target)
        {
            m_motor.MoveTo(target);
        }

        protected void Start()
        {
            BuildUpPath();

            m_isStopped = !startOnPawn;
            if (startOnPawn)
            {
                RestartPatrol();
            }
        }

        protected void Update()
        {
            UpdatePatrol();
        }

        protected void OnDrawGizmos()
        {
            if (patrolPath != null)
            {
                Gizmos.color = Color.green;

                for (int i = 0; i < patrolPath.Length; ++i)
                {
                    Gizmos.DrawWireSphere(patrolPath[i], 0.3f);
                }
            }
        }
    }

}