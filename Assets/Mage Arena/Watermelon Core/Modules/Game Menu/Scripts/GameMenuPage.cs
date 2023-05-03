using UnityEngine;

namespace Watermelon
{
	public abstract class GameMenuPage : MonoBehaviour
	{
        [System.Serializable]
        public enum Type
        {
            Store = 0,
            Map = 1,
            Inventory = 2
        }
	}
}
