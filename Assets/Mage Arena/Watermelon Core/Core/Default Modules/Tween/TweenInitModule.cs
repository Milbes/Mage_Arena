﻿#pragma warning disable 0649

using UnityEngine;

namespace Watermelon
{
    [RegisterModule("Core/Tween")]
    public class TweenInitModule : InitModule
    {
        [SerializeField] int tweensUpdateCount = 300;
        [SerializeField] int tweensFixedUpdateCount = 30;
        [SerializeField] int tweensLateUpdateCount = 0;

        [Space]
        [SerializeField] bool enableSystemLogs;


        public override void CreateComponent(Initialiser Initialiser)
        {
            Tween tween = Initialiser.gameObject.AddComponent<Tween>();
            tween.Init(tweensUpdateCount, tweensFixedUpdateCount, tweensLateUpdateCount, enableSystemLogs);
        }

        public TweenInitModule()
        {
            moduleName = "Tween";
        }
    }
}


// -----------------
// Tween v 1.1f3
// -----------------
