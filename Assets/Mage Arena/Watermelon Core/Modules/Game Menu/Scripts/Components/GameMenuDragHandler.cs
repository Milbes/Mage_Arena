using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
	public class GameMenuDragHandler : MonoBehaviour
    {
        public const float MENU_HEIGHT = 160;
        private const float SWIPE_MULTIPLIER = 70;
        private const float SWIPE_LENGTH = 2f;
        private const float MIN_SWIPE_DETECTION = 4;
        
        public static IScrollBehavior ActiveScroll;
        public static bool IsDefaultDragEnabled = true;

        [SerializeField] float tempSwipeMultiplier = 1;
        [SerializeField] float tempSwipeLength = 0.4f;
        [SerializeField] float tempSwipeDetection = 0.5f;

        private RectTransform pagesRectTransform;
        private Camera canvasCamera;
        private RectTransform canvasRectTransform;

        private Vector2 touchStartPosition;
        private Vector2 touchLastPosition;
        private Vector2 touchSwipe;

        private Vector2 pointerOffset;

        private Vector2 currentPagePosition;

        private bool isDragAllowed = false;
        private bool allowSwipe = false;
        private bool isTouchInitialized = false;

        private float menuHeight;

        private float swipeLength => tempSwipeLength * Screen.dpi;
        private float swipeMultiplier => tempSwipeMultiplier * Screen.dpi;
        private float minSwipeDetection => tempSwipeDetection * Screen.dpi;

        private GameMenuController gameMenuController;

        [Space]
        [SerializeField] Text swipeLengthText;
        [SerializeField] Text swipeMultiplierText;
        [SerializeField] Text minSwipeDetectionText;

        public void SwipeLengthValue(float value)
        {
            tempSwipeLength += value;

            swipeLengthText.text = tempSwipeLength.ToString();
        }

        public void SwipeMultiplierValue(float value)
        {
            tempSwipeMultiplier += value;

            swipeMultiplierText.text = tempSwipeMultiplier.ToString();
        }

        public void MinSwipeValue(float value)
        {
            tempSwipeDetection += value;

            minSwipeDetectionText.text = tempSwipeDetection.ToString();
        }

        public void Init(GameMenuController gameMenuController)
        {
            this.gameMenuController = gameMenuController;

            pagesRectTransform = gameMenuController.PagesRectTransform;
            canvasCamera = gameMenuController.CanvasCamera;
            canvasRectTransform = gameMenuController.CanvasRectTransform;

            menuHeight = MENU_HEIGHT * gameMenuController.MainCanvas.scaleFactor;

            //swipeLength = SWIPE_LENGTH * Screen.dpi;
            //swipeMultiplier = SWIPE_MULTIPLIER * Screen.dpi;
            //minSwipeDetection = MIN_SWIPE_DETECTION * Screen.dpi;

            swipeLengthText.text = tempSwipeLength.ToString();
            swipeMultiplierText.text = tempSwipeMultiplier.ToString();
            minSwipeDetectionText.text = tempSwipeDetection.ToString();
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnTouchStart(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                OnTouchUpdate(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnTouchEnd(Input.mousePosition);
            }
        }

        private void OnTouchStart(Vector2 startPosition)
        {
            gameMenuController.OnTouchDown();

            if (GameMenuWindow.IsWindowOpened)
            {
                isDragAllowed = false;

                return;
            }

            touchStartPosition = startPosition;

            allowSwipe = false;
            isTouchInitialized = false;

            currentPagePosition = gameMenuController.CurrentPagePosition;

            if(startPosition.y < menuHeight)
            {
                isDragAllowed = false;
            }
            else
            {
                isDragAllowed = true;

                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, startPosition, canvasCamera, out pointerOffset);
            }

            touchLastPosition = startPosition;
        }

        private void OnTouchUpdate(Vector2 touchPosition)
        {
            if (!isDragAllowed)
                return;

            touchSwipe = new Vector2(Mathf.Abs(touchPosition.x - touchStartPosition.x), Mathf.Abs(touchPosition.y - touchStartPosition.y));

            if(!isTouchInitialized)
            {
                if (ActiveScroll != null)
                {
                    allowSwipe = false;

                    isTouchInitialized = true;
                }
                else
                {
                    if(Mathf.Abs(touchSwipe.x - touchSwipe.y) > minSwipeDetection)
                    {
                        if (touchSwipe.y > touchSwipe.x)
                        {
                            IsDefaultDragEnabled = true;
                            allowSwipe = false;

                            isTouchInitialized = true;
                        }
                        else if (touchSwipe.x > touchSwipe.y)
                        {
                            IsDefaultDragEnabled = false;
                            allowSwipe = true;

                            isTouchInitialized = true;
                        }
                    }
                }
            }
            else
            {
                if(allowSwipe)
                {
                    Vector2 localPointerPosition;
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, touchPosition, canvasCamera, out localPointerPosition))
                    {
                        gameMenuController.ButtonsPanel.ReportOffset(localPointerPosition.x - pointerOffset.x);

                        pagesRectTransform.anchoredPosition = new Vector2(((localPointerPosition.x - pointerOffset.x) * swipeMultiplier) - currentPagePosition.x, pagesRectTransform.anchoredPosition.y);
                    }
                }
            }

            touchLastPosition = touchPosition;
        }

        private void OnTouchEnd(Vector2 endPosition)
        {
            if (!isDragAllowed)
                return;

            if (allowSwipe)
                pagesRectTransform.DOAnchoredPosition(-gameMenuController.CurrentPagePosition, 0.3f).SetEasing(Ease.Type.ExpoOut);

            bool resetOffset = true;
            if (ActiveScroll == null)
            {
                Vector2 localPointerPosition;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, endPosition, canvasCamera, out localPointerPosition))
                {
                    float offset = localPointerPosition.x - pointerOffset.x;
                    float swipeVelocity = touchLastPosition.x - endPosition.x;

                    if (offset > swipeLength || (swipeVelocity < -3 && offset > 1))
                    {
                        if (GameMenuController.SelectedPage - 1 >= 0)
                        {
                            gameMenuController.PrevPage();

                            resetOffset = false;
                        }
                    }
                    else if (offset < -swipeLength || (swipeVelocity > 3 && offset < -1))
                    {
                        if (GameMenuController.SelectedPage + 1 < GameMenuController.PagesCount)
                        {
                            gameMenuController.NextPage();

                            resetOffset = false;
                        }
                    }
                }
            }

            if(resetOffset)
                gameMenuController.ButtonsPanel.ResetSmoothPosition();

            isTouchInitialized = false;

            GameMenuDragHandler.IsDefaultDragEnabled = true;
            GameMenuDragHandler.ActiveScroll = null;
        }
    }
}
