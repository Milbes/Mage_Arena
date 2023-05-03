using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{

    [System.Serializable]
    public class Zone
    {
        [SerializeField] string title;
        [SerializeField] Level[] levels;

        public string Title => title;
        public Level[] Levels => levels;

        public Zone(string title, Level[] levels)
        {
            this.title = title;
            this.levels = levels;
        }
    }

}