using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Watermelon
{
    /// <summary>
    ///   <para>A component for making a child RectTransform scroll.</para>
    /// </summary>
    [SelectionBase]
    [AddComponentMenu("UI/Page Scroll Rect", 37)]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(RectTransform))]
    public class PageScrollRect : UIBehaviour, IEventSystemHandler, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IScrollHandler, ICanvasElement
    {
        [SerializeField]
        private float m_Elasticity = 0.1f;

        [SerializeField]
        private bool m_Inertia = true;
        [SerializeField]
        private float m_DecelerationRate = 0.135f;
        [SerializeField]
        private float m_ScrollSensitivity = 1f;

        private Vector2 m_PointerStartLocalCursor = Vector2.zero;
        private Vector2 m_ContentStartPosition = Vector2.zero;
        private Vector2 m_PrevPosition = Vector2.zero;
        private readonly Vector3[] m_Corners = new Vector3[4];

        [SerializeField]
        private RectTransform m_Content;
        [SerializeField]
        private RectTransform m_Viewport;

        private RectTransform m_ViewRect;
        private Bounds m_ContentBounds;
        private Bounds m_ViewBounds;
        private Vector2 m_Velocity;
        private bool m_Dragging;

        private Bounds m_PrevContentBounds;
        private Bounds m_PrevViewBounds;

        [NonSerialized] bool m_HasRebuiltLayout;
        [NonSerialized] RectTransform m_Rect;

        private DrivenRectTransformTracker m_Tracker;

        /// <summary>
        ///   <para>The content that can be scrolled. It should be a child of the GameObject with ScrollRect on it.</para>
        /// </summary>
        public RectTransform content
        {
            get
            {
                return m_Content;
            }
            set
            {
                m_Content = value;
            }
        }

        /// <summary>
        ///   <para>The amount of elasticity to use when the content moves beyond the scroll rect.</para>
        /// </summary>
        public float elasticity
        {
            get
            {
                return m_Elasticity;
            }
            set
            {
                m_Elasticity = value;
            }
        }

        /// <summary>
        ///   <para>Should movement inertia be enabled?</para>
        /// </summary>
        public bool inertia
        {
            get
            {
                return m_Inertia;
            }
            set
            {
                m_Inertia = value;
            }
        }

        /// <summary>
        ///   <para>The rate at which movement slows down.</para>
        /// </summary>
        public float decelerationRate
        {
            get
            {
                return m_DecelerationRate;
            }
            set
            {
                m_DecelerationRate = value;
            }
        }

        /// <summary>
        ///   <para>The sensitivity to scroll wheel and track pad scroll events.</para>
        /// </summary>
        public float scrollSensitivity
        {
            get
            {
                return m_ScrollSensitivity;
            }
            set
            {
                m_ScrollSensitivity = value;
            }
        }

        /// <summary>
        ///   <para>Reference to the viewport RectTransform that is the parent of the content RectTransform.</para>
        /// </summary>
        public RectTransform viewport
        {
            get
            {
                return m_Viewport;
            }
            set
            {
                m_Viewport = value;
            }
        }
        
        protected RectTransform viewRect
        {
            get
            {
                if (m_ViewRect == null)
                    m_ViewRect = m_Viewport;

                if (m_ViewRect == null)
                    m_ViewRect = (RectTransform)transform;

                return m_ViewRect;
            }
        }

        /// <summary>
        ///   <para>The current velocity of the content.</para>
        /// </summary>
        public Vector2 velocity
        {
            get
            {
                return m_Velocity;
            }
            set
            {
                m_Velocity = value;
            }
        }

        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();

                return m_Rect;
            }
        }

        /// <summary>
        ///   <para>The scroll position as a Vector2 between (0,0) and (1,1) with (0,0) being the lower left corner.</para>
        /// </summary>
        public Vector2 normalizedPosition
        {
            get
            {
                return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
            }
            set
            {
                SetNormalizedPosition(value.x, 0);
                SetNormalizedPosition(value.y, 1);
            }
        }

        /// <summary>
        ///   <para>The horizontal scroll position as a value between 0 and 1, with 0 being at the left.</para>
        /// </summary>
        public float horizontalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                if (m_ContentBounds.size.x <= m_ViewBounds.size.x)
                    return m_ViewBounds.min.x <= m_ContentBounds.min.x ? 0.0f : 1f;
                return (m_ViewBounds.min.x - m_ContentBounds.min.x) / (m_ContentBounds.size.x - m_ViewBounds.size.x);
            }
            set
            {
                SetNormalizedPosition(value, 0);
            }
        }

        /// <summary>
        ///   <para>The vertical scroll position as a value between 0 and 1, with 0 being at the bottom.</para>
        /// </summary>
        public float verticalNormalizedPosition
        {
            get
            {
                UpdateBounds();
                if ((double)m_ContentBounds.size.y <= (double)m_ViewBounds.size.y)
                    return (double)m_ViewBounds.min.y <= (double)m_ContentBounds.min.y ? 0.0f : 1f;
                return (float)(((double)m_ViewBounds.min.y - (double)m_ContentBounds.min.y) / ((double)m_ContentBounds.size.y - (double)m_ViewBounds.size.y));
            }
            set
            {
                SetNormalizedPosition(value, 1);
            }
        }
        
        protected PageScrollRect()
        {
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

            m_HasRebuiltLayout = true;
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
        
        protected override void OnEnable()
        {
            base.OnEnable();

            CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        }

        /// <summary>
        ///   <para>See MonoBehaviour.OnDisable.</para>
        /// </summary>
        protected override void OnDisable()
        {
            CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

            m_HasRebuiltLayout = false;
            m_Tracker.Clear();
            m_Velocity = Vector2.zero;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);

            base.OnDisable();
        }

        /// <summary>
        ///   <para>See member in base class.</para>
        /// </summary>
        public override bool IsActive()
        {
            if (base.IsActive())
                return m_Content != null;

            return false;
        }

        private void EnsureLayoutHasRebuilt()
        {
            if (m_HasRebuiltLayout || CanvasUpdateRegistry.IsRebuildingLayout())
                return;

            Canvas.ForceUpdateCanvases();
        }

        /// <summary>
        ///   <para>Sets the velocity to zero on both axes so the content stops moving.</para>
        /// </summary>
        public virtual void StopMovement()
        {
            m_Velocity = Vector2.zero;
        }

        /// <summary>
        ///   <para>See IScrollHandler.OnScroll.</para>
        /// </summary>
        /// <param name="data"></param>
        public virtual void OnScroll(PointerEventData data)
        {
            if (!IsActive())
                return;

            EnsureLayoutHasRebuilt();
            UpdateBounds();
            Vector2 scrollDelta = data.scrollDelta;
            scrollDelta.y *= -1f;

            if (Mathf.Abs(scrollDelta.x) > Mathf.Abs(scrollDelta.y))
                scrollDelta.y = scrollDelta.x;
            scrollDelta.x = 0.0f;

            Vector2 position = m_Content.anchoredPosition + scrollDelta * m_ScrollSensitivity;

            SetContentAnchoredPosition(position);
            UpdateBounds();
        }

        /// <summary>
        ///   <para>See: IInitializePotentialDragHandler.OnInitializePotentialDrag.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Velocity = Vector2.zero;
        }

        /// <summary>
        ///   <para>Handling for when the content is beging being dragged.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left || !IsActive())
                return;

            UpdateBounds();

            m_PointerStartLocalCursor = Vector2.zero;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);

            m_ContentStartPosition = m_Content.anchoredPosition;
            m_Dragging = true;
        }

        /// <summary>
        ///   <para>Handling for when the content has finished being dragged.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            m_Dragging = false;
        }

        /// <summary>
        ///   <para>Handling for when the content is dragged.</para>
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnDrag(PointerEventData eventData)
        {
            if (!GameMenuDragHandler.IsDefaultDragEnabled)
                return;

            Vector2 localPoint;
            if (eventData.button != PointerEventData.InputButton.Left || !IsActive() || !RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out localPoint))
                return;

            UpdateBounds();
            Vector2 vector2 = m_ContentStartPosition + localPoint - m_PointerStartLocalCursor;
            Vector2 offset = CalculateOffset(vector2 - m_Content.anchoredPosition);
            Vector2 position = vector2 + offset;

            if (offset.y != 0.0)
                position.y = position.y - PageScrollRect.RubberDelta(offset.y, m_ViewBounds.size.y);

            SetContentAnchoredPosition(position);
        }

        /// <summary>
        ///   <para>Sets the anchored position of the content.</para>
        /// </summary>
        /// <param name="position"></param>
        protected virtual void SetContentAnchoredPosition(Vector2 position)
        {
            position.x = m_Content.anchoredPosition.x;

            if (position == m_Content.anchoredPosition)
                return;

            m_Content.anchoredPosition = position;
            UpdateBounds();
        }

        protected virtual void LateUpdate()
        {
            if (!m_Content)
                return;

            if (!GameMenuDragHandler.IsDefaultDragEnabled)
                return;

            EnsureLayoutHasRebuilt();
            UpdateBounds();

            float unscaledDeltaTime = Time.unscaledDeltaTime;
            Vector2 offset = CalculateOffset(Vector2.zero);
            if (!m_Dragging && (offset != Vector2.zero || m_Velocity != Vector2.zero))
            {
                Vector2 anchoredPosition = m_Content.anchoredPosition;
                for (int index = 0; index < 2; ++index)
                {
                    if (offset[index] != 0.0)
                    {
                        float currentVelocity = m_Velocity[index];
                        anchoredPosition[index] = Mathf.SmoothDamp(m_Content.anchoredPosition[index], m_Content.anchoredPosition[index] + offset[index], ref currentVelocity, m_Elasticity, float.PositiveInfinity, unscaledDeltaTime);
                        m_Velocity[index] = currentVelocity;
                    }
                    else if (m_Inertia)
                    {
                        m_Velocity[index] *= Mathf.Pow(m_DecelerationRate, unscaledDeltaTime);
                        if (Mathf.Abs(m_Velocity[index]) < 1.0)
                            m_Velocity[index] = 0.0f;

                        anchoredPosition[index] += m_Velocity[index] * unscaledDeltaTime;
                    }
                    else
                    {
                        m_Velocity[index] = 0.0f;
                    }
                }

                if (m_Velocity != Vector2.zero)
                {
                    SetContentAnchoredPosition(anchoredPosition);
                }
            }

            if (m_Dragging && m_Inertia)
                m_Velocity = Vector3.Lerp(m_Velocity, ((m_Content.anchoredPosition - m_PrevPosition) / unscaledDeltaTime), unscaledDeltaTime * 10f);

            if (m_ViewBounds == m_PrevViewBounds && m_ContentBounds == m_PrevContentBounds && m_Content.anchoredPosition == m_PrevPosition)
                return;
            
            UpdatePrevData();
        }

        private void UpdatePrevData()
        {
            m_PrevPosition = m_Content != null ? m_Content.anchoredPosition : Vector2.zero;
            m_PrevViewBounds = m_ViewBounds;
            m_PrevContentBounds = m_ContentBounds;
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

            float num1 = m_ContentBounds.size[axis] - m_ViewBounds.size[axis];
            float num2 = m_ViewBounds.min[axis] - value * num1;
            float num3 = m_Content.localPosition[axis] + num2 - m_ContentBounds.min[axis];
            Vector3 localPosition = m_Content.localPosition;

            if (Mathf.Abs(localPosition[axis] - num3) <= 0.00999999977648258)
                return;

            localPosition[axis] = num3;
            m_Content.localPosition = localPosition;
            m_Velocity[axis] = 0.0f;

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
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();
            if (m_Content == null)
                return;

            Vector3 size = m_ContentBounds.size;
            Vector3 center = m_ContentBounds.center;
            Vector3 vector3 = m_ViewBounds.size - size;
            if (vector3.x > 0.0)
            {
                center.x -= vector3.x * (m_Content.pivot.x - 0.5f);
                size.x = m_ViewBounds.size.x;
            }

            if (vector3.y > 0.0)
            {
                center.y -= vector3.y * (m_Content.pivot.y - 0.5f);
                size.y = m_ViewBounds.size.y;
            }

            m_ContentBounds.size = size;
            m_ContentBounds.center = center;
        }

        private Bounds GetBounds()
        {
            if (m_Content == null)
                return new Bounds();

            Vector3 vector3_1 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 vector3_2 = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Matrix4x4 worldToLocalMatrix = viewRect.worldToLocalMatrix;

            m_Content.GetWorldCorners(m_Corners);
            for (int index = 0; index < 4; ++index)
            {
                Vector3 lhs = worldToLocalMatrix.MultiplyPoint3x4(m_Corners[index]);
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

            Vector2 min = m_ContentBounds.min;
            Vector2 max = m_ContentBounds.max;

            min.y += delta.y;
            max.y += delta.y;
            if (max.y < m_ViewBounds.max.y)
                zero.y = m_ViewBounds.max.y - max.y;
            else if (min.y > m_ViewBounds.min.y)
                zero.y = m_ViewBounds.min.y - min.y;

            return zero;
        }

        /// <summary>
        ///   <para>Override to alter or add to the code that keeps the appearance of the scroll rect synced with its data.</para>
        /// </summary>
        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }
    }
}