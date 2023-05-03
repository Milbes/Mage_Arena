using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public abstract class GameMenuWindow : MonoBehaviour
    {
        private static List<GameMenuWindow> gameMenuWindows = new List<GameMenuWindow>();

        public static bool IsWindowOpened;

        protected abstract void OpenAnimation();
        protected abstract void CloseAnimation();

        public static void ShowWindow<T>(T window) where T : GameMenuWindow
        {
            window.OpenAnimation();

            gameMenuWindows.Add(window);

            IsWindowOpened = true;
        }

        public static void HideWindow(GameMenuWindow gameMenuWindow)
        {
            gameMenuWindow.CloseAnimation();

            gameMenuWindows.Remove(gameMenuWindow);

            if (gameMenuWindows.Count == 0)
                IsWindowOpened = false;
        }
    }
}
