
namespace GameAILab.Perception
{


    public interface ISightStimuliListener : IStimuliListener
    {
        float HalfAngleDegrees { get; set; }
        float Radius { get; set; }
        float Height { get; set; }
    }


}