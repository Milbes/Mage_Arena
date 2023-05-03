using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class Reference
    {
        [SerializeField]
        private string groupUUID;

        public string GroupUUID { get => groupUUID; }

        public Reference()
        {
        }

        public Reference(string groupUUID)
        {
            this.groupUUID = groupUUID;
        }
    }
}
