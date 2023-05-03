using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Watermelon
{
    public class GameMenuPageMap : GameMenuPage
    {
        [SerializeField] LevelDatabase levelDatabase;

        [Space]
        [SerializeField] Text zoneTitleText;
        [SerializeField] Text stageText;

        private bool gameIsLoading = false;

        private void Start()
        {
            // Dev
            Zone currentZone = levelDatabase.GetZone(0);

            zoneTitleText.text = currentZone.Title;
            stageText.text = string.Format("Stage {0}/{1}", GameController.LastOpenLevelId + 1, 10);
        }

        private void LoadGame()
        {
            if (gameIsLoading)
                return;

            gameIsLoading = true;

            SceneLoader.LoadScene("Game");
            //SceneLoader.OnSceneOpened("Game", delegate
            //{
            //    Tween.NextFrame(GameController.StartGame);
            //}, true);
        }

        public void PlayButton()
        {
            LoadGame();
        }
    }
}
