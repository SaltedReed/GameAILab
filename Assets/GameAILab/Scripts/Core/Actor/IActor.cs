using System;
using UnityEngine;

namespace GameAILab.Core
{

    [Flags]
    public enum AffiliationType
    {
        Neutral  = 1,
        Friendly = 1<<1,
        Hostile  = 1<<2,
    }


    public interface IActor
    {
        int Id { get; set; }
        GameObject Go { get; set; }
        AffiliationType Affiliation { get; set; }
    }

}