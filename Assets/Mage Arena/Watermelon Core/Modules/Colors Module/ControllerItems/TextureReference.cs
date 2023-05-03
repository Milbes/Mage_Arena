using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class TextureReference : Reference
    {
        [SerializeField]
        private Material material;

        public Material Material { get => material; set => material = value; }

        public TextureReference()
        {
        }

        public TextureReference(string uuid) : base(uuid)
        {

        }
    }
}

