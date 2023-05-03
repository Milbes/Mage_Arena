using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Watermelon
{
    [RequireComponent(typeof(Canvas))]
	public class GameMenuController : MonoBehaviour
    {
        private const float PAGES_OFFSET = 200;

        private static GameMenuController gameMenuController;

        public static float CanvasWidth;
        public static float CanvasHeight;

        [Header("Refferences")]
        [SerializeField] GameMenuButtonsPanel buttonsPanel;
        [SerializeField] GameMenuTopPanel topPanel;

        [SerializeField] Camera canvasCamera;
        [SerializeField] RectTransform backgroundRectTransform;

        [Space]
        [SerializeField] RectTransform pagesContainer;

        [Space]
        [SerializeField] Page[] activePages;

        [Space]
        [SerializeField] GameMenuPage.Type startPage;

        private Dictionary<GameMenuPage.Type, int> activePagesLink = new Dictionary<GameMenuPage.Type, int>();

        private int selectedPage = -1;

        public static int SelectedPage => gameMenuController.selectedPage;
        public static int PagesCount => gameMenuController.activePages.Length;

        private Canvas mainCanvas;
        private RectTransform mainCanvasRectTransform;

        private TweenCase scrollTweenCase;

        private float backgroundParallaxOffset;

        private GameMenuDragHandler dragHandler;

        public RectTransform PagesRectTransform => pagesContainer;
        public RectTransform CanvasRectTransform => mainCanvasRectTransform;

        public Canvas MainCanvas => mainCanvas;
        public Camera CanvasCamera => canvasCamera;

        public GameMenuButtonsPanel ButtonsPanel => buttonsPanel;

        public Vector2 CurrentPagePosition
        {
            get { return new Vector2((CanvasWidth + PAGES_OFFSET) * selectedPage, 0); }
        }

        // Callbacks
        public static MenuSelectionChangeCallback OnSelectionChanged;

        private AudioClip pageSwipeAudioClip;

        private void Awake()
        {
            gameMenuController = this;

            mainCanvas = GetComponent<Canvas>();
            mainCanvasRectTransform = (RectTransform)transform;

            //dragHandler = gameObject.AddComponent<GameMenuDragHandler>();
            dragHandler = gameObject.GetComponent<GameMenuDragHandler>();

            pageSwipeAudioClip = AudioController.Settings.sounds.pageSwipeSound;

            AudioController.SetVolume(AudioController.AudioType.Music, 1f);
        }

        private void Start()
        {
            CanvasWidth = mainCanvasRectTransform.sizeDelta.x;
            CanvasHeight = mainCanvasRectTransform.sizeDelta.y;

            buttonsPanel.Init(this);
            topPanel.Init(this);
            dragHandler.Init(this);

            // Init background
            backgroundRectTransform.sizeDelta = new Vector2(CanvasHeight, CanvasHeight);
            backgroundParallaxOffset = (CanvasHeight - CanvasWidth) / 2;

            // Init pages container size
            float totalContainerWidth = (CanvasWidth + PAGES_OFFSET) * activePages.Length;
            pagesContainer.sizeDelta = new Vector2(totalContainerWidth, 0);
            pagesContainer.anchoredPosition = new Vector2(0, 0);

            // Create a link between page types and their indexes in the array
            for (int i = 0; i < activePages.Length; i++)
            {
                if (!activePagesLink.ContainsKey(activePages[i].PageType))
                {
                    activePages[i].Init();

                    activePagesLink.Add(activePages[i].PageType, i);

                    RectTransform pageRectTransform = (RectTransform)activePages[i].PageBehaviour.transform;
                    pageRectTransform.sizeDelta = new Vector2(CanvasWidth, 0);
                    pageRectTransform.anchoredPosition = new Vector2((CanvasWidth + PAGES_OFFSET) * i, 0);
                }
                else
                {
                    Debug.LogWarning("[Game Menu]: Page with a " + activePages[i].PageType + " type already exists!");
                }
            }

            SelectPage(startPage, -1);
        }

        public void MoveBackground()
        {
            backgroundRectTransform.anchoredPosition = Vector2.Lerp(backgroundRectTransform.anchoredPosition, Vector2.Lerp(new Vector2(backgroundParallaxOffset, 0), new Vector2(-backgroundParallaxOffset, 0), buttonsPanel.SelectionPosition), Time.deltaTime * 50);
        }

        public void SelectPage(GameMenuPage.Type pageType, float scrollPosition = -1)
        {
            if(activePagesLink.ContainsKey(pageType))
            {
                SelectPage(activePagesLink[pageType], scrollPosition);
            }
            else
            {
                Debug.LogWarning("[Game Menu]: Page with a " + pageType + " type is missing!");
            }
        }

        public void SelectPage(int pageIndex, float scrollPosition = -1)
        {
            if (selectedPage == pageIndex)
            {
                ScrollToPosition(pageIndex, scrollPosition);
            }
            else
            {
                // Store previous selected page index
                int prevSelectedPage = selectedPage;

                selectedPage = pageIndex;

                // First show
                if (prevSelectedPage != -1)
                {
                    if (activePages[pageIndex].ShowTopPanel)
                        topPanel.Show();
                    else
                        topPanel.Hide();

                    pagesContainer.DOAnchoredPosition(new Vector2(-(CanvasWidth + PAGES_OFFSET) * pageIndex, 0), 0.3f).SetEasing(Ease.Type.CubicOut);
                }
                else
                {
                    if (activePages[pageIndex].ShowTopPanel)
                        topPanel.Show(false);
                    else
                        topPanel.Hide(false);

                    pagesContainer.anchoredPosition = new Vector2(-(CanvasWidth + PAGES_OFFSET) * pageIndex, 0);
                }

                ScrollToPosition(pageIndex, scrollPosition);

                if (OnSelectionChanged != null)
                    OnSelectionChanged.Invoke(prevSelectedPage, selectedPage);
            }
        }

        public void OnTouchDown()
        {
            if (scrollTweenCase != null && !scrollTweenCase.isCompleted)
                scrollTweenCase.Kill();
        }

        private void ScrollToPosition(int pageIndex, float scrollPosition)
        {
            if (scrollTweenCase != null && !scrollTweenCase.isCompleted)
                scrollTweenCase.Kill();

            if (scrollPosition != -1 && activePages[pageIndex].HasScrollView)
            {
                if (scrollPosition != activePages[pageIndex].PageScrollRect.verticalNormalizedPosition)
                {
                    scrollTweenCase = Tween.DoFloat(activePages[pageIndex].PageScrollRect.verticalNormalizedPosition, scrollPosition, 1.0f, (float value) =>
                    {
                        activePages[pageIndex].PageScrollRect.verticalNormalizedPosition = value;
                    }).SetEasing(Ease.Type.ExpoOut);
                }
            }
        }
        
        [Button("Next Page")]
        public void NextPage()
        {
            if(selectedPage + 1 < activePages.Length)
            {
                SelectPage(selectedPage + 1, -1);
            }
        }

        [Button("Prev Page")]
        public void PrevPage()
        {
            if (selectedPage - 1 >= 0)
            {
                SelectPage(selectedPage - 1, -1);
            }
        }

        public delegate void MenuSelectionChangeCallback(int prevSelectedPageIndex, int selectedPageIndex);
        
        [System.Serializable]
        private class Page
        {
            [SerializeField] GameMenuPage.Type pageType;
            public GameMenuPage.Type PageType => pageType;

            [SerializeField] GameMenuPage pageBehaviour;
            public GameMenuPage PageBehaviour => pageBehaviour;

            [SerializeField] bool showTopPanel = true;
            public bool ShowTopPanel => showTopPanel;

            private PageScrollRect pageScrollRect;
            public PageScrollRect PageScrollRect => pageScrollRect;

            private bool hasScrollView;
            public bool HasScrollView => hasScrollView;

            public void Init()
            {
                pageScrollRect = pageBehaviour.GetComponent<PageScrollRect>();
                hasScrollView = pageScrollRect != null;
            }
        }
	}
}
