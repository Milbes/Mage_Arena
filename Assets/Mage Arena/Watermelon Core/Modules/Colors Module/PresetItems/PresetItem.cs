#pragma warning disable 649

using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class PresetItem
    {
        [SerializeField]
        private string groupUUID;

        public string GroupUUID { get => groupUUID; }
    }
}
