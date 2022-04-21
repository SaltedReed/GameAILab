using UnityEngine;

namespace GameAILab.Sandbox
{

    public class ResourceManager
    {
        public static T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
    }

}