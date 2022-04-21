using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameAILab.Sandbox
{

    public abstract class Weapon : MonoBehaviour
    {
        public virtual GameObject Owner { get; set; }

        public virtual void OnAttach() { }
        public virtual void OnDetach() { }
    }

}