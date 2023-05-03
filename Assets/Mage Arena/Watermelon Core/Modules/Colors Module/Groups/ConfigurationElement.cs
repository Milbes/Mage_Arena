using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class ConfigurationElement
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private string uuid;

        public string Name { get => name; set => name = value; }
        public string UUID { get => uuid; set => uuid = value; }
        
    }
}
