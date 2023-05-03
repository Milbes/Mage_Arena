using UnityEngine;

namespace Watermelon
{
    public class GameSaveController : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneLoader.OnSceneChanged += SaveGame;
        }

        private void OnDisable()
        {
            SceneLoader.OnSceneChanged -= SaveGame;
        }

        private void OnDestroy()
        {
            SaveGame();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if(!hasFocus)
            {
                SaveGame();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if(pauseStatus)
            {
                SaveGame();
            }
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private void SaveGame()
        {
            Debug.Log("Saving game..");

            Character.SaveIfRequired();
            Inventory.SaveIfRequired();
            Account.SaveIfRequired();
        }
    }
}
