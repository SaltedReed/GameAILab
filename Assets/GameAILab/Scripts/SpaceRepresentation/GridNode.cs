

namespace GameAILab.SpaceRepre
{

    public class GridNode : GraphNode
    {
        public GridGraph Graph { get; set; }
        public int XCoord { get; set; }
        public int ZCoord { get; set; }
        public bool Walkable { get; set; }

    }

}