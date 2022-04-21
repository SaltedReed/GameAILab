using UnityEngine;

public partial class DebugHUD : MonoBehaviour
{
    private void OnGUI()
    {
        if (GUILayout.Button("seek"))
        {
            DoSteering(m_Seek);
        }
        if (GUILayout.Button("flee"))
        {
            DoSteering(m_Flee);
        }
    }

    private void Start()
    {
        InitMovement();
    }

}
