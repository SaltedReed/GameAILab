using UnityEngine;

namespace GameAILab.Sandbox
{

    public class VfxManager
    {
        public static void SpawnParticles(GameObject prefab, Vector3 pos, Quaternion rotat, float destroyDelay)
        {
            if (prefab is null)
                return;

            GameObject go = GameObject.Instantiate(prefab, pos, rotat);
            GameObject.Destroy(go, destroyDelay<=0.0f?0.0f:destroyDelay);
        }
    }

}