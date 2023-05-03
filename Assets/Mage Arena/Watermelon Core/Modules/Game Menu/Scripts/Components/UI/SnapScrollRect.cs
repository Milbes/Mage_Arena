using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    [SelectionBase]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    [AddComponentMenu("UI/Snap Scroll Rect", 37)]
    public class SnapScrollRect : UIBehaviour, IScrollBehavior, IEventSystemHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, ICanvasElement, IPointerDownHandler
    {
        [SerializeField] float scrollSensitivity = 1f;

        [SerializeField] RectTransform content;

        [SerializeField] PageScrollRect parentScroll;

        private Vector2 pointerStartLocalCursor = Vector2.zero;
        private Vector2 contentStartPosition = Vector2.zero;
        private readonly Vector3[] corners = new Vector3[4];

        private Vector2 touchSwipe;
        private Vector2 touchStartPosition;

        private Bounds contentBounds;
        private Bounds viewBounds;

        private Vector2 velocity;

        private bool dragging;
        private bool snapping;

        private bool routeToParent = false;

        private Bounds prevContentBounds;
        private Bounds prevViewBounds;

        private int currentPage = -1;
        private int pagesCount;

        private float pageWidth;

        [NonSerialized] bool hasRebuiltLayout;
        [NonSerialized] RectTransform rectTransform;
        
        public RectTransform RectTransform
        {
            get
            {
                if (rectTransform != null)
                    return rectTransform;

                return (RectTransform)transform;
            }
        }

        public bool IsHorizontalScroll
        {
            get { return true; }
        }

        public GameObject ScrollGameObject => gameObject;

        protected SnapScrollRect()
        {
        }

        protected override void Awake()
        {
            base.Awake();
        }

        /// <summary>
        ///   <para>Rebuilds the scroll rect data after initialization.</para>
        /// </summary>
        /// <param name="executing">The current step of the rendering CanvasUpdate cycle.</param>
        public virtual void Rebuild(CanvasUpdate executing)
        {
            if (executing != CanvasUpdate.PostLayout)
                return;

            UpdateBounds();
            UpdatePrevData();

            hasRebuiltLayout = true;
        }

        /// <summary>
        ///   <para>See ICanvasElement.LayoutComplete.</para>
        /// </summary>
        public virtual void LayoutComplete()
        {
        }

        /// <summary>
        ///   <para>See ICanvasElement.GraphicUpdateComplete.</para>
        /// </summary>
        public virtual void GraphicUpdateComplete()
        {
        }

        private void NextPage()
        {
            velocity = Vector2.zero;

            if (currentPage + 1 < pagesCount)
            {
                currentPage++;

                snapping = true;

                content.DOAnchoredPosition(new Vector3(currentPage * -pageWidth, content.anchoredPosition.y), 0.3f).SetEasing(Ease.Type.ExpoOut).OnComplete(delegate
                {
                    snapping = false;
                });
            }
        }

        private void PrevPage()
        {
            velocity = Vector2.zero;

            if (currentPage - 1 >= 0)
            {
                currentPage--;

                snapping = true;

                content.DOAnchoredPosition(new Vector3(currentPage * -pageWidth, content.anchoredPosition.y), 0.3f).SetEasing(Ease.Type.ExpoOut).OnComplete(delegate
                {
                    snapping = false;
                });
            }
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            rectTransform = (RectTransform)transform;

            pageWidth = rectTransform.rect.width;

            pagesCount = content.childCount;

            currentPage = 0;

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

        /// <summary>
        ///   <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            hasRebuiltLayout = false;

            velocity = Vector2.zero;

            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);

            base.OnDisable();
        }

        /// <summary>
        ///   <para>See member in base class.</para>
        /// </summary>
        public override bool IsActive()
        {
            if (base.IsActive())
                return content != null;

            return false;
        }

        private void EnsureLayoutHasRebuilt()
        {
            if (hasRebuiltLayout || CanvasUpdateRegistry.IsRebuildingLayout())
                return;

            Canvas.ForceUpdateCanvases();
        }

        /// <summary>
        ///   <para>Sets the velocity to zero on both axes so the content stops moving.</para>
        /// </summary>
        public virtual void StopMovement()
        {
            velocity = Vector2.zero;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GameMenuDragHandler.ActiveScroll = this;
        }
        
        /// <summary>
        ///   <para>See: IInitializePotentialDragHandler.OnInitializePotentialDrag.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            velocity = Vector2.zero;

            parentScroll.OnInitializePotentialDrag(eventData);
        }

        /// <summary>
        ///   <para>Handling for when the content is beging being dragged.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
                routeToParent = true;
            else
                routeToParent = false;

            if (routeToParent)
            {
                parentScroll.OnBeginDrag(eventData);
            }
            else
            {
                if (eventData.button != PointerEventData.InputButton.Left || !this.IsActive())
                    return;

                UpdateBounds();

                touchStartPosition = eventData.position;

                pointerStartLocalCursor = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out pointerStartLocalCursor);
                contentStartPosition = content.anchoredPosition;
                dragging = true;
            }
        }

        private const int SWIPE_MAGNITUDE = 120;
        /// <summary>
        ///   <para>Handling for when the content has finished being dragged.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (routeToParent)
            {
                parentScroll.OnEndDrag(eventData);
            }
            else
            {
                if (eventData.button != PointerEventData.InputButton.Left)
                    return;

                UpdateBounds();

                float direction = Mathf.Sign(eventData.position.x - touchStartPosition.x);
                float scrollMagnitude = new Vector2(eventData.position.x - touchStartPosition.x, eventData.position.y - touchStartPosition.y).magnitude;

                if (scrollMagnitude > SWIPE_MAGNITUDE)
                {
                    if (Mathf.Sign(eventData.position.x - touchStartPosition.x) > 0)
                    {
                        if (currentPage - 1 >= 0)
                        {
                            PrevPage();
                        }
                        else
                        {
                            SnapToCurrent();
                        }
                    }
                    else
                    {
                        if (currentPage + 1 < pagesCount)
                        {
                            NextPage();
                        }
                        else
                        {
                            SnapToCurrent();
                        }
                    }
                }
                else
                {
                    SnapToCurrent();
                }

                dragging = false;
            }
        }

        private void SnapToCurrent()
        {
            if (snapping)
                return;

            snapping = true;

            content.DOAnchoredPosition(new Vector3(currentPage * -pageWidth, content.anchoredPosition.y), 0.3f).SetEasing(Ease.Type.ExpoOut).OnComplete(delegate
            {
                snapping = false;
            });
        }

        /// <summary>
        ///   <para>Handling for when the content is dragged.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (routeToParent)
            {
                parentScroll.OnDrag(eventData);
            }
            else
            {
                Vector2 localPoint = Vector2.zero;
                if (eventData.button != PointerEventData.InputButton.Left || !IsActive() || !RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
                    return;

                UpdateBounds();

                Vector2 vector2 = contentStartPosition + localPoint - pointerStartLocalCursor;
                Vector2 offset = CalculateOffset(vector2 - content.anchoredPosition);
                Vector2 position = vector2 + offset;

                if (offset.x != 0.0)
                    position.x = position.x - SnapScrollRect.RubberDelta(offset.x, viewBounds.size.x);
                if (offset.y != 0.0)
                    position.y = position.y - SnapScrollRect.RubberDelta(offset.y, viewBounds.size.y);

                SetContentAnchoredPosition(position);
            }
        }

        /// <summary>
        ///   <para>Sets the anchored position of the content.</para>
        /// </summary>
        /// <param name="position"></param>
        protected virtual void SetContentAnchoredPosition(Vector2 position)
        {
            position.y = content.anchoredPosition.y;

            if (position == content.anchoredPosition)
                return;

            content.anchoredPosition = position;
            UpdateBounds();
        }

        private void UpdatePrevData()
        {
            prevViewBounds = viewBounds;
            prevContentBounds = contentBounds;
        }

        private void SetHorizontalNormalizedPosition(float value)
        {
            SetNormalizedPosition(value, 0);
        }

        private void SetVerticalNormalizedPosition(float value)
        {
            SetNormalizedPosition(value, 1);
        }

        private void SetNormalizedPosition(float value, int axis)
        {
            EnsureLayoutHasRebuilt();
            UpdateBounds();

            float num1 = contentBounds.size[axis] - viewBounds.size[axis];
            float num2 = viewBounds.min[axis] - value * num1;
            float num3 = content.localPosition[axis] + num2 - contentBounds.min[axis];

            Vector3 localPosition = content.localPosition;
            if (Mathf.Abs(localPosition[axis] - num3) <= 0.00999999977648258)
                return;

            localPosition[axis] = num3;
            content.localPosition = localPosition;
            velocity[axis] = 0.0f;

            UpdateBounds();
        }

        private static float RubberDelta(float overStretching, float viewSize)
        {
            return (float)(1.0 - 1.0 / (Mathf.Abs(overStretching) * 0.550000011920929 / viewSize + 1.0)) * viewSize * Mathf.Sign(overStretching);
        }

        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }
                
        private void UpdateBounds()
        {
            viewBounds = new Bounds(rectTransform.rect.center, rectTransform.rect.size);
            contentBounds = this.GetBounds();

            if (content == null)
                return;

            Vector3 size = contentBounds.size;
            Vector3 center = contentBounds.center;
            Vector3 vector3 = viewBounds.size - size;
            if (vector3.x > 0.0)
            {
                center.x -= vector3.x * (content.pivot.x - 0.5f);
                size.x = viewBounds.size.x;
            }

            if (vector3.y > 0.0)
            {
                center.y -= vector3.y * (content.pivot.y - 0.5f);
                size.y = viewBounds.size.y;
            }

            contentBounds.size = size;
            contentBounds.center = center;
        }

        private Bounds GetBounds()
        {
            if (content == null)
                return new Bounds();

            Vector3 vector3_1 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector3_2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            Matrix4x4 worldToLocalMatrix = rectTransform.worldToLocalMatrix;
            content.GetWorldCorners(corners);
            for (int index = 0; index < 4; ++index)
            {
                Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(corners[index]);
                vector3_1 = Vector3.Min(lhs, vector3_1);
                vector3_2 = Vector3.Max(lhs, vector3_2);
            }
            Bounds bounds = new Bounds(vector3_1, Vector3.zero);
            bounds.Encapsulate(vector3_2);
            return bounds;
        }

        private Vector2 CalculateOffset(Vector2 delta)
        {
            Vector2 zero = Vector2.zero;

            Vector2 min = contentBounds.min;
            Vector2 max = contentBounds.max;

            min.x += delta.x;
            max.x += delta.x;

            if (min.x > viewBounds.min.x)
                zero.x = viewBounds.min.x - min.x;
            else if (max.x < viewBounds.max.x)
                zero.x = viewBounds.max.x - max.x;

            return zero;
        }

        /// <summary>
        ///   <para>Override to alter or add to the code that keeps the appearance of the scroll rect synced with its data.</para>
        /// </summary>
        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        }
    }
}