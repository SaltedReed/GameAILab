using UnityEngine;

public partial class DebugHUD : MonoBehaviour
{
    private Seek m_Seek;
    private Flee m_Flee;

    private void DoSteering(MonoBehaviour m)
    {
        if (m != null)
            m.enabled = true;
    }

    private void InitMovement()
    {
        m_Seek = GetComponent<Seek>();
        DisableComponent(m_Seek);

        m_Flee = GetComponent<Flee>();
        DisableComponent(m_Flee);
    }

    private void DisableComponent(MonoBehaviour m)
    {
        if (m != null)
            m.enabled = false;
    }
}
