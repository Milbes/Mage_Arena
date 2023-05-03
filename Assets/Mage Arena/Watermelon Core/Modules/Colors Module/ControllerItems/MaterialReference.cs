using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class MaterialReference : Reference
    {
        [SerializeField]
        private MeshRenderer meshRenderer;

        public MeshRenderer MeshRenderer { get => meshRenderer; set => meshRenderer = value; }

        public MaterialReference()
        {
        }

        public MaterialReference(string uuid) : base(uuid)
        {

        }
    }
}
