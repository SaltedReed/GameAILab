using UnityEngine;

namespace GameAILab.SpaceRepre
{


    public abstract class GraphNode
    {
        public virtual int NodeIndex { get; set; }
        public virtual Vector3 WorldPos { get; set; }
        public virtual float Cost { get; set; } = 1;
        public int flag1;

        public virtual string DebugString()
        {
            string str = "";

            str += $"node index: {NodeIndex}";
            str += $"world position: {WorldPos}";
            str += $"cost: {Cost}";

            return str;
        }
    }


}