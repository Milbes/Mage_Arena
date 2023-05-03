using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public abstract class TweenCase
    {
        public int activeId;

        public float delay;

        public bool isActive;
        public bool isPaused;
        public bool isUnscaled;
        public bool isCompleted;

        public TweenType tweenType;

        public bool isKilling;

        public float state;

        private Ease.EaseFunction easeFunction;

        protected GameObject parentObject;

        public Tween.TweenCallback onCompleteCallback;

        private AnimationCurve easingCurve = null;

        public TweenCase()
        {
            SetEasing(Ease.Type.Linear);
        }

        public virtual TweenCase StartTween()
        {
            Tween.AddTween(this, tweenType);

            return this;
        }

        public abstract bool Validate();

        /// <summary>
        /// Stop and remove tween
        /// </summary>
        public TweenCase Kill()
        {
            if (!isKilling)
            {
                isActive = false;

                Tween.MarkForKilling(this);

                isKilling = true;
            }

            return this;
        }

        /// <summary>
        /// Complete tween
        /// </summary>
        public TweenCase Complete()
        {
            if (isPaused)
                isPaused = false;

            state = 1;

            isCompleted = true;

            return this;
        }

        /// <summary>
        /// Pause current coroutine.
        /// </summary>
        public TweenCase Pause()
        {
            isPaused = true;

            return this;
        }

        /// <summary>
        /// Play tween if it was paused.
        /// </summary>
        public TweenCase Resume()
        {
            isPaused = false;

            return this;
        }

        public TweenCase EnableCustomEasing(AnimationCurve easingCurve)
        {
            this.easingCurve = easingCurve;

            float totalEasingTime = easingCurve.keys[easingCurve.length - 1].time;
            easeFunction = (float p) =>
            {
                return easingCurve.Evaluate(p * totalEasingTime);
            };

            return this;
        }

        /// <summary>
        /// Interpolate current easing function.
        /// </summary>
        /// <param name="p">Value between 0 and 1</param>
        /// <returns>Interpolated value</returns>
        public float Interpolate(float p)
        {
            return easeFunction.Invoke(p);
        }

        #region Set
        public TweenCase SetType(TweenType tweenType)
        {
            this.tweenType = tweenType;

            return this;
        }

        public TweenCase SetUnscaledMode(bool isUnscaled)
        {
            this.isUnscaled = isUnscaled;

            return this;
        }

        /// <summary>
        /// Set tween easing function.
        /// </summary>
        /// <param name="ease">Easing type</param>
        public TweenCase SetEasing(Ease.Type ease)
        {
            easeFunction = Ease.GetFunction(ease);

            return this;
        }

        /// <summary>
        /// Change tween delay.
        /// </summary>
        /// <param name="newDelay">New tween delay.</param>
        public TweenCase SetTime(float newDelay)
        {
            delay = newDelay;

            return this;
        }
        #endregion

        public void NextState(float deltaTime)
        {
            state += deltaTime / delay;
            state = Mathf.Clamp01(state);

            if (state >= 1)
                isCompleted = true;
        }

        /// <summary>
        /// Init function that called when it will completed.
        /// </summary>
        /// <param name="callback">Complete function.</param>
        public TweenCase OnComplete(Tween.TweenCallback callback)
        {
            onCompleteCallback += callback;

            return this;
        }

        public abstract void Invoke();
        public abstract void DefaultComplete();
    }

    public abstract class TweenCaseFunction<TBaseObject, TValue> : TweenCase
    {
        public TBaseObject tweenObject;

        public TValue startValue;
        public TValue resultValue;

        public TweenCaseFunction(TBaseObject tweenObject, TValue resultValue)
        {
            this.tweenObject = tweenObject;
            this.resultValue = resultValue;
        }
    }

    public class TweenCaseDefault : TweenCase
    {
        public override void DefaultComplete() { }
        public override void Invoke() { }

        public override bool Validate()
        {
            return true;
        }
    }

    public abstract class TweenCaseNextFrame : TweenCase
    {
        private Tween.TweenCallback callback;
        protected int resultFrame;
        protected int framesOffset;

        public TweenCaseNextFrame(Tween.TweenCallback callback, int framesOffset)
        {
            this.callback = callback;
            this.framesOffset = framesOffset;
        }

        public override void Invoke() 
        {
            if (CheckFrameState())
                Complete();
        }

        public override void DefaultComplete()
        {
            callback.Invoke();
        }

        public override bool Validate()
        {
            return true;
        }

        public abstract bool CheckFrameState();
    }

    public class TweenCaseUpdateNextFrame : TweenCaseNextFrame
    {
        public TweenCaseUpdateNextFrame(Tween.TweenCallback callback, int framesOffset) : base(callback, framesOffset)
        {

        }

        public override TweenCase StartTween()
        {
            resultFrame = Tween.UpdateFramesCount + framesOffset;

            return base.StartTween();
        }

        public override bool CheckFrameState()
        {
            return resultFrame <= Tween.UpdateFramesCount;
        }
    }

    public class TweenCaseFixedUpdateNextFrame : TweenCaseNextFrame
    {
        public TweenCaseFixedUpdateNextFrame(Tween.TweenCallback callback, int framesOffset) : base(callback, framesOffset)
        {

        }

        public override TweenCase StartTween()
        {
            resultFrame = Tween.FixedUpdateFramesCount + framesOffset;

            return base.StartTween();
        }

        public override bool CheckFrameState()
        {
            return resultFrame <= Tween.FixedUpdateFramesCount;
        }
    }

    public class TweenCaseLateUpdateNextFrame : TweenCaseNextFrame
    {
        public TweenCaseLateUpdateNextFrame(Tween.TweenCallback callback, int framesOffset) : base(callback, framesOffset)
        {

        }

        public override TweenCase StartTween()
        {
            resultFrame = Tween.LateUpdateFramesCount + framesOffset;

            return base.StartTween();
        }

        public override bool CheckFrameState()
        {
            return resultFrame <= Tween.LateUpdateFramesCount;
        }
    }

    public class TweenCaseFloat : TweenCase
    {
        public float startValue;
        public float resultValue;

        public TweenFloatCallback callback;

        public TweenCaseFloat(float startValue, float resultValue, TweenFloatCallback callback)
        {
            this.startValue = startValue;
            this.resultValue = resultValue;

            this.callback = callback;
        }

        public override bool Validate()
        {
            return true;
        }

        public override void DefaultComplete()
        {
            callback.Invoke(resultValue);
        }

        public override void Invoke()
        {
            callback.Invoke(Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }

        public delegate void TweenFloatCallback(float value);
    }

    public class TweenCaseColor : TweenCase
    {
        public Color startValue;
        public Color resultValue;

        public TweenColorCallback callback;

        public TweenCaseColor(Color startValue, Color resultValue, TweenColorCallback callback)
        {
            this.startValue = startValue;
            this.resultValue = resultValue;

            this.callback = callback;
        }

        public override bool Validate()
        {
            return true;
        }

        public override void DefaultComplete()
        {
            callback.Invoke(resultValue);
        }

        public override void Invoke()
        {
            callback.Invoke(Color.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }

        public delegate void TweenColorCallback(Color color);
    }

    public class TweenCaseTransfomRotateAngle : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomRotateAngle(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.eulerAngles;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.eulerAngles = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.eulerAngles = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomRotateQuaternion : TweenCaseFunction<Transform, Quaternion>
    {
        public TweenCaseTransfomRotateQuaternion(Transform tweenObject, Quaternion resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.rotation;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.rotation = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.rotation = Quaternion.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomLocalRotate : TweenCaseFunction<Transform, Quaternion>
    {
        public TweenCaseTransfomLocalRotate(Transform tweenObject, Quaternion resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.localRotation;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localRotation = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.localRotation = Quaternion.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomLocalRotateAngle : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomLocalRotateAngle(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.localEulerAngles;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localEulerAngles = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.localEulerAngles = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomPosition : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomPosition(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.position;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.position = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.position = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomPositionX : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomPositionX(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.position.x;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.position = new Vector3(resultValue, tweenObject.position.y, tweenObject.position.z);
        }

        public override void Invoke()
        {
            tweenObject.position = new Vector3(Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.position.y, tweenObject.position.z);
        }
    }

    public class TweenCaseTransfomPositionY : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomPositionY(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.position.y;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.position = new Vector3(tweenObject.position.x, resultValue, tweenObject.position.z);
        }

        public override void Invoke()
        {
            tweenObject.position = new Vector3(tweenObject.position.x, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.position.z);
        }
    }

    public class TweenCaseTransfomPositionZ : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomPositionZ(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.position.z;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.position = new Vector3(tweenObject.position.x, tweenObject.position.y, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.position = new Vector3(tweenObject.position.x, tweenObject.position.y, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseTransfomScale : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomScale(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.localScale;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localScale = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.localScale = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomDoPath : TweenCase
    {
        public Transform tweenObject;
        public Vector3[] path;
        public float[] maxStateValues;
        public Vector3 startValue;

        public int index;

        public TweenCaseTransfomDoPath(Transform tweenObject, Vector3[] path)
        {
            this.tweenObject = tweenObject;

            this.path = path;
            this.maxStateValues = new float[path.Length];

            this.startValue = tweenObject.position;

            parentObject = tweenObject.gameObject;  
            
            float[] distances = new float[path.Length];
            float totalDistance = 0;
            float minStateValue = 0;

            distances[0] = Vector3.Distance(startValue, path[0]);
            totalDistance += distances[0];

            for (int i = 1; i < path.Length; i++)
            {
                distances[i] = Vector3.Distance(path[i - 1], path[i]);
                totalDistance += distances[i];
            }

            for (int i = 0; i < path.Length; i++)
            {
                this.maxStateValues[i] = minStateValue + (distances[i] / totalDistance);
                minStateValue = maxStateValues[i];
            }
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.position = path[path.Length - 1];
        }

        private void UpdateIndex()
        {
            for (int i = 0; i < maxStateValues.Length; i++)
            {
                if(state < maxStateValues[i])
                {
                    index = i;
                    return;
                }
            }

            index = maxStateValues.Length - 1;
        }

        public override void Invoke()
        {
            UpdateIndex();

            if(index == 0)
            {
                tweenObject.position = Vector3.LerpUnclamped(startValue, path[0], Interpolate(Mathf.InverseLerp(0, maxStateValues[0], state)));
            }
            else
            {
                tweenObject.position = Vector3.LerpUnclamped(path[index - 1], path[index], Interpolate(Mathf.InverseLerp(maxStateValues[index - 1], maxStateValues[index], state)));
            }            
        }
    }

    public class TweenCaseTransfomPushScale : TweenCase
    {
        public Transform tweenObject;

        public Vector3 startValue;
        public Vector3 firstScaleValue;
        public Vector3 secondScaleValue;

        public float firstTime;
        public float secondTime;

        private Ease.Type firstScaleEasing;
        private Ease.Type secondScaleEasing;

        private float relativeState;

        public TweenCaseTransfomPushScale(Transform tweenObject, Vector3 firstScaleValue, Vector3 secondScaleValue, float firstTime, float secondTime, Ease.Type firstScaleEasing, Ease.Type secondScaleEasing)
        {
            this.tweenObject = tweenObject;

            this.startValue = tweenObject.localScale;

            this.firstScaleValue = firstScaleValue;
            this.secondScaleValue = secondScaleValue;

            this.firstTime = firstTime;
            this.secondTime = secondTime;

            this.firstScaleEasing = firstScaleEasing;
            this.secondScaleEasing = secondScaleEasing;

            parentObject = tweenObject.gameObject;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localScale = secondScaleValue;
        }

        public override void Invoke()
        {
            relativeState = delay * state;

            if(relativeState <= firstTime)
            {
                tweenObject.localScale = Vector3.LerpUnclamped(startValue, firstScaleValue, Ease.Interpolate(Mathf.InverseLerp(0, firstTime, relativeState), firstScaleEasing));
            }
            else
            {
                tweenObject.localScale = Vector3.LerpUnclamped(firstScaleValue, secondScaleValue, Ease.Interpolate(Mathf.InverseLerp(firstTime, delay, relativeState), secondScaleEasing));
            }
        }
    }

    public class TweenCaseTransfomScaleX : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomScaleX(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.localScale.x;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localScale = new Vector3(resultValue, tweenObject.localScale.y, tweenObject.localScale.z);
        }

        public override void Invoke()
        {
            tweenObject.localScale = new Vector3(Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.localScale.y, tweenObject.localScale.z);
        }
    }

    public class TweenCaseTransfomScaleY : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomScaleY(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.localScale.y;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localScale = new Vector3(tweenObject.localScale.x, resultValue, tweenObject.localScale.z);
        }

        public override void Invoke()
        {
            tweenObject.localScale = new Vector3(tweenObject.localScale.x, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.localScale.z);
        }
    }

    public class TweenCaseTransfomScaleZ : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomScaleZ(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.localScale.z;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localScale = new Vector3(tweenObject.localScale.x, tweenObject.localScale.y, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.localScale = new Vector3(tweenObject.localScale.x, tweenObject.localScale.y, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseTransfomLocalMove : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomLocalMove(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.localPosition;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localPosition = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.localPosition = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTransfomLocalPositionX : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomLocalPositionX(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.localPosition.x;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localPosition = new Vector3(resultValue, tweenObject.localPosition.y, tweenObject.localPosition.z);
        }

        public override void Invoke()
        {
            tweenObject.localPosition = new Vector3(Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.localPosition.y, tweenObject.localPosition.z);
        }
    }

    public class TweenCaseTransfomLocalPositionY : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomLocalPositionY(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.localPosition.y;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localPosition = new Vector3(tweenObject.localPosition.x, resultValue, tweenObject.localPosition.z);
        }

        public override void Invoke()
        {
            tweenObject.localPosition = new Vector3(tweenObject.localPosition.x, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)), tweenObject.localPosition.z);
        }
    }

    public class TweenCaseTransfomLocalPositionZ : TweenCaseFunction<Transform, float>
    {
        public TweenCaseTransfomLocalPositionZ(Transform tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.localPosition.z;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.localPosition = new Vector3(tweenObject.localPosition.x, tweenObject.localPosition.y, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.localPosition = new Vector3(tweenObject.localPosition.x, tweenObject.localPosition.y, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseTransfomLookAt : TweenCaseFunction<Transform, Vector3>
    {
        public TweenCaseTransfomLookAt(Transform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.position;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.LookAt(resultValue);
        }

        public override void Invoke()
        {
            tweenObject.LookAt(Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseTransfomLookAt2D : TweenCaseFunction<Transform, Vector3>
    {
        public LookAtType type;
        float rotationZ;

        public TweenCaseTransfomLookAt2D(Transform tweenObject, Vector3 resultValue, LookAtType type) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            this.type = type;

            startValue = tweenObject.eulerAngles;

            Vector3 different = (resultValue - tweenObject.position);
            different.Normalize();

            rotationZ = (Mathf.Atan2(different.y, different.x) * Mathf.Rad2Deg);

            if (type == LookAtType.Up)
                rotationZ -= 90;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.LookAt(resultValue);
        }

        public override void Invoke()
        {
            tweenObject.rotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(startValue.z, rotationZ, Interpolate(state)));
        }

        public enum LookAtType
        {
            Up,
            Right,
            Forward
        }
    }

    public class TweenCaseRectTransformAnchoredPosition : TweenCaseFunction<RectTransform, Vector3>
    {
        public TweenCaseRectTransformAnchoredPosition(RectTransform tweenObject, Vector3 resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.anchoredPosition;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.anchoredPosition = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.anchoredPosition = Vector3.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseRectTransformSizeScale : TweenCaseFunction<RectTransform, Vector2>
    {
        public TweenCaseRectTransformSizeScale(RectTransform tweenObject, Vector2 resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.sizeDelta;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.sizeDelta = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.sizeDelta = Vector2.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseSpriteRendererColor : TweenCaseFunction<SpriteRenderer, Color>
    {
        public TweenCaseSpriteRendererColor(SpriteRenderer tweenObject, Color resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.color;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.color = Color.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseSpriteRendererFade : TweenCaseFunction<SpriteRenderer, float>
    {
        public TweenCaseSpriteRendererFade(SpriteRenderer tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.color.a;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseImageColor : TweenCaseFunction<Image, Color>
    {
        public TweenCaseImageColor(Image tweenObject, Color resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.color;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.color = Color.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseImageFade : TweenCaseFunction<Image, float>
    {
        public TweenCaseImageFade(Image tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.color.a;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseImageFill : TweenCaseFunction<Image, float>
    {
        public TweenCaseImageFill(Image tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.fillAmount;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.fillAmount = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.fillAmount = startValue + (resultValue - startValue) * Interpolate(state);
        }
    }

    public class TweenCaseTextFontSize : TweenCaseFunction<Text, int>
    {
        public TweenCaseTextFontSize(Text tweenObject, int resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.fontSize;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.fontSize = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.fontSize = (int)Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseTextFade : TweenCaseFunction<Text, float>
    {
        public TweenCaseTextFade(Text tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.color.a;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.color = new Color(tweenObject.color.r, tweenObject.color.g, tweenObject.color.b, Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseTextColor : TweenCaseFunction<Text, Color>
    {
        public TweenCaseTextColor(Text tweenObject, Color resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.color;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.color = Color.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseCanvasGroupFade : TweenCaseFunction<CanvasGroup, float>
    {
        public TweenCaseCanvasGroupFade(CanvasGroup tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.alpha;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.alpha = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.alpha = Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseAudioSourceVolume : TweenCaseFunction<AudioSource, float>
    {
        public TweenCaseAudioSourceVolume(AudioSource tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            parentObject = tweenObject.gameObject;

            startValue = tweenObject.volume;
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.volume = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.volume = Mathf.LerpUnclamped(startValue, resultValue, Interpolate(state));
        }
    }

    public class TweenCaseMaterialColor : TweenCaseFunction<Material, Color>
    {
        private int colorID;

        public TweenCaseMaterialColor(int colorID, Material tweenObject, Color resultValue) : base(tweenObject, resultValue)
        {
            this.colorID = colorID;

            startValue = tweenObject.GetColor(colorID);
        }

        public override bool Validate()
        {
            return true;
        }

        public override void DefaultComplete()
        {
            tweenObject.color = resultValue;
        }

        public override void Invoke()
        {
            tweenObject.SetColor(colorID, Color.LerpUnclamped(startValue, resultValue, Interpolate(state)));
        }
    }

    public class TweenCaseMaterialFloat : TweenCaseFunction<Material, float>
    {
        private int floatID;

        public TweenCaseMaterialFloat(int floatID, Material tweenObject, float resultValue) : base(tweenObject, resultValue)
        {
            this.floatID = floatID;

            startValue = tweenObject.GetFloat(floatID);
        }

        public override bool Validate()
        {
            return true;
        }

        public override void DefaultComplete()
        {
            tweenObject.SetFloat(floatID, resultValue);
        }

        public override void Invoke()
        {
            tweenObject.SetFloat(floatID, startValue + (resultValue - startValue) * Interpolate(state));
        }
    }

    public class TweenCasePropertyBlockColor : TweenCaseFunction<Renderer, Color>
    {
        private MaterialPropertyBlock materialPropertyBlock;

        private int colorID;

        public TweenCasePropertyBlockColor(int colorID, MaterialPropertyBlock materialPropertyBlock, Renderer tweenObject, Color resultValue) : base(tweenObject, resultValue)
        {
            this.parentObject = tweenObject.gameObject;
            this.materialPropertyBlock = materialPropertyBlock;

            this.colorID = colorID;
            this.startValue = materialPropertyBlock.GetColor(colorID);
        }

        public override bool Validate()
        {
            return parentObject != null;
        }

        public override void DefaultComplete()
        {
            tweenObject.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor(colorID, resultValue);
            tweenObject.SetPropertyBlock(materialPropertyBlock);
        }

        public override void Invoke()
        {
            tweenObject.GetPropertyBlock(materialPropertyBlock);
            materialPropertyBlock.SetColor(colorID, Color.LerpUnclamped(startValue, resultValue, Interpolate(state)));
            tweenObject.SetPropertyBlock(materialPropertyBlock);
        }
    }

    public class TweenCaseCondition : TweenCase
    {
        public TweenConditionCallback callback;

        public TweenCaseCondition(TweenConditionCallback callback)
        {
            this.callback = callback;
        }

        public override void DefaultComplete()
        {

        }

        public override void Invoke()
        {
            callback.Invoke(this);
        }

        public override bool Validate()
        {
            return true;
        }
        
        public delegate void TweenConditionCallback(TweenCaseCondition tweenCase);
    }

    public class TweenCaseAction<T> : TweenCase
    {
        private System.Action<T, T, float> action;

        private T startValue;
        private T resultValue;

        public TweenCaseAction(T startValue, T resultValue, System.Action<T, T, float> action)
        {
            this.startValue = startValue;
            this.resultValue = resultValue;

            this.action = action;
        }

        public override bool Validate()
        {
            return true;
        }

        public override void DefaultComplete()
        {
            action.Invoke(startValue, resultValue, 1);
        }

        public override void Invoke()
        {
            action.Invoke(startValue, resultValue, Interpolate(state));
        }
    }
}


// -----------------
// Tween v 1.1f3
// -----------------