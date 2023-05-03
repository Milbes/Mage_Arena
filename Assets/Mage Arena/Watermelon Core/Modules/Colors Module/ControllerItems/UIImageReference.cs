using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    [System.Serializable]
    public class UIImageReference : Reference
    {
        [SerializeField]
        private Image image;

        public UIImageReference()
        {
        }

        public UIImageReference(string uuid) : base(uuid)
        {

        }
    }
}
