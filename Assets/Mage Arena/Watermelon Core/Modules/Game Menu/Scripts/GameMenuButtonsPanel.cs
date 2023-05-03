using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
	public class GameMenuButtonsPanel : MonoBehaviour
	{
        private static GameMenuButtonsPanel instance;

        [SerializeField] RectTransform selectionRectTransform;
        [SerializeField] RectTransform buttonsContainerRectTransform;

        [Space]
        [SerializeField] MenuButton[] menuButtons;
        
        private GameMenuController gameMenuController;

        private int selectedButtonIndex;

        private float selectedButtonWidth;
        private float defaultButtonWidth;

        public static float SelectedButtonWidth => instance.selectedButtonWidth;
        public static float DefaultButtonWidth => instance.defaultButtonWidth;

        public float SelectionPosition
        {
            get { return (selectionRectTransform.anchoredPosition.x + (selectionRectTransform.sizeDelta.x / 2)) / GameMenuController.CanvasWidth; }
        }

        private void OnEnable()
        {
            GameMenuController.OnSelectionChanged += OnPageSelectionChanged;
        }

        private void OnDisable()
        {
            GameMenuController.OnSelectionChanged -= OnPageSelectionChanged;
        }

        public void Init(GameMenuController gameMenuController)
        {
            instance = this;

            this.gameMenuController = gameMenuController;

            selectedButtonWidth = (GameMenuController.CanvasWidth / menuButtons.Length) * 1.2f;
            defaultButtonWidth = (GameMenuController.CanvasWidth - selectedButtonWidth) / (menuButtons.Length - 1);

            // Init menu buttons
            for (int i = 0; i < menuButtons.Length; i++)
            {
                menuButtons[i].Init();
            }
        }
        
        private void OnPageSelectionChanged(int prevSelectedPageIndex, int selectedPageIndex)
        {
            selectedButtonIndex = selectedPageIndex;

            if (prevSelectedPageIndex != -1)
            {
                // Unselect previous page
                menuButtons[prevSelectedPageIndex].Unselect();

                Debug.Log("Page #" + selectedPageIndex + " selected");

                menuButtons[selectedPageIndex].Select();

                MoveSelection(selectedPageIndex);

                StartCoroutine(CalculateButtonsSize());
            }
            else
            {
                Debug.Log("Page #" + selectedPageIndex + " selected");

                menuButtons[selectedPageIndex].Select(false);

                // Init buttons positions and sizes
                for (int i = 0; i < menuButtons.Length; i++)
                {
                    menuButtons[i].BackgroundContainer.RectTransform.sizeDelta = new Vector2(i == selectedPageIndex ? selectedButtonWidth : defaultButtonWidth, 0);
                    menuButtons[i].BackgroundContainer.RectTransform.anchoredPosition = GetButtonPosition(i);

                    menuButtons[i].ButtonContentBehaviour.RectTransform.anchoredPosition = menuButtons[i].BackgroundContainer.RectTransform.anchoredPosition;
                    menuButtons[i].ButtonContentBehaviour.RectTransform.sizeDelta = menuButtons[i].BackgroundContainer.RectTransform.sizeDelta;
                }

                SetSelectionPosition(selectedPageIndex);
            }
        }

        private IEnumerator CalculateButtonsSize()
        {
            bool allButtonsAreCalculated = false;

            Vector2[] targetAnchoredPositions = new Vector2[menuButtons.Length];
            Vector2[] targetSizes = new Vector2[menuButtons.Length];

            for (int i = 0; i < menuButtons.Length; i++)
            {
                if (i == selectedButtonIndex)
                {
                    targetAnchoredPositions[i] = new Vector2(i * defaultButtonWidth, 0);
                    targetSizes[i] = new Vector2(selectedButtonWidth, 0);
                }
                else
                {
                    targetAnchoredPositions[i] = new Vector2(i > selectedButtonIndex ? (i - 1) * defaultButtonWidth + selectedButtonWidth : i * defaultButtonWidth, 0);
                    targetSizes[i] = new Vector2(defaultButtonWidth, 0);
                }
            }

            Vector2 resultSizeDelta;
            Vector2 resultAnchoredPosition;

            while (!allButtonsAreCalculated)
            {
                allButtonsAreCalculated = true;

                for (int i = 0; i < menuButtons.Length; i++)
                {
                    resultSizeDelta = Vector2.MoveTowards(menuButtons[i].BackgroundContainer.RectTransform.sizeDelta, targetSizes[i], Time.deltaTime * 800);
                    resultAnchoredPosition = Vector2.MoveTowards(menuButtons[i].BackgroundContainer.RectTransform.anchoredPosition, targetAnchoredPositions[i], Time.deltaTime * 800);

                    menuButtons[i].BackgroundContainer.RectTransform.sizeDelta = resultSizeDelta;
                    menuButtons[i].BackgroundContainer.RectTransform.anchoredPosition = resultAnchoredPosition;

                    menuButtons[i].ButtonContentBehaviour.RectTransform.sizeDelta = resultSizeDelta;
                    menuButtons[i].ButtonContentBehaviour.RectTransform.anchoredPosition = resultAnchoredPosition;

                    if (Vector2.Distance(menuButtons[i].BackgroundContainer.RectTransform.anchoredPosition, targetAnchoredPositions[i]) > 0.001f)
                        allButtonsAreCalculated = false;
                }

                yield return null;
            }
        }

        private Vector2 GetButtonPosition(int elementIndex)
        {
            if (elementIndex == selectedButtonIndex)
            {
                return new Vector2(elementIndex * defaultButtonWidth, 0);
            }
            else
            {
                return new Vector2(elementIndex > selectedButtonIndex ? (elementIndex - 1) * defaultButtonWidth + selectedButtonWidth : elementIndex * defaultButtonWidth, 0);
            }
        }

        public void ReportOffset(float offset)
        {
            selectionRectTransform.anchoredPosition = new Vector2((selectedButtonIndex * defaultButtonWidth) - (offset * 15), 0);
        }

        public void ResetSmoothPosition()
        {
            selectionRectTransform.DOAnchoredPosition(new Vector2(selectedButtonIndex * defaultButtonWidth, 0), 0.1f).SetEasing(Ease.Type.QuadIn);
        }

        private void SetSelectionPosition(int selectionIndex)
        {
            selectionRectTransform.sizeDelta = new Vector2(menuButtons[selectionIndex].BackgroundContainer.RectTransform.sizeDelta.x, 0);
            selectionRectTransform.anchoredPosition = new Vector2(menuButtons[selectionIndex].BackgroundContainer.RectTransform.anchoredPosition.x, 0);
        }

        private void MoveSelection(int selectionIndex)
        {
            selectionRectTransform.DOAnchoredPosition(new Vector3(selectionIndex * defaultButtonWidth, 0), 0.3f).SetEasing(Ease.Type.QuadIn);
        }

        #region Buttons
        public void SelectPageButton(int pageIndex)
        {
            gameMenuController.SelectPage(pageIndex);
        }
        #endregion

        [System.Serializable]
        public class MenuButton
        {
            [SerializeField] GameObject buttonBackground;
            [SerializeField] GameMenuButtonContentBehaviour buttonContentBehaviour;

            private Background backgroundContainer;
            public Background BackgroundContainer => backgroundContainer;

            public GameMenuButtonContentBehaviour ButtonContentBehaviour => buttonContentBehaviour;

            public void Init()
            {
                backgroundContainer = new Background(buttonBackground);
            }

            public void Select(bool animation = true)
            {
                buttonContentBehaviour.Select(animation);
            }

            public void Unselect(bool animation = true)
            {
                buttonContentBehaviour.Unselect(animation);
            }

            public class Background
            {
                private RectTransform rectTransform;
                public RectTransform RectTransform => rectTransform;
                
                private Image image;
                public Image Image => image;

                public Background(GameObject buttonBackground)
                {
                    rectTransform = (RectTransform)buttonBackground.transform;
                    image = buttonBackground.GetComponent<Image>();
                }
            }
        }
    }
}
