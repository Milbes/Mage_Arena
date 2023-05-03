using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public static class TweenExtensions
    {
        #region Transform
        /// <summary>
        /// Changes rotation angle of object.
        /// </summary>
        public static TweenCase DORotate(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomRotateAngle(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes quaternion rotation of object.
        /// </summary>
        public static TweenCase DORotate(this Transform tweenObject, Quaternion resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomRotateQuaternion(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes local quaternion rotation of object.
        /// </summary>
        public static TweenCase DOLocalRotate(this Transform tweenObject, Quaternion resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomLocalRotate(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes local angle rotation of object.
        /// </summary>
        public static TweenCase DOLocalRotate(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomLocalRotateAngle(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes position of object.
        /// </summary>
        public static TweenCase DOMove(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomPosition(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        public static TweenCase DoPath(this Transform tweenObject,Vector3[] path, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomDoPath(tweenObject, path).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes x position of object.
        /// </summary>
        public static TweenCase DOMoveX(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomPositionX(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes y position of object.
        /// </summary>
        public static TweenCase DOMoveY(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomPositionY(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes z position of object.
        /// </summary>
        public static TweenCase DOMoveZ(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomPositionZ(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes local scale of object.
        /// </summary>
        public static TweenCase DOScale(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomScale(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes local scale of object.
        /// </summary>
        public static TweenCase DOScale(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomScale(tweenObject, new Vector3(resultValue, resultValue, resultValue)).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes local scale of object twice.
        /// </summary>
        public static TweenCase DOPushScale(this Transform tweenObject, Vector3 firstScale, Vector3 secondScale, float firstScaleTime, float secondScaleTime, Ease.Type firstScaleEasing = Ease.Type.Linear, Ease.Type secondScaleEasing = Ease.Type.Linear, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomPushScale(tweenObject, firstScale, secondScale, firstScaleTime, secondScaleTime, firstScaleEasing, secondScaleEasing).SetTime(firstScaleTime + secondScaleTime).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes local scale of object twice.
        /// </summary>
        public static TweenCase DOPushScale(this Transform tweenObject, float firstScale, float secondScale, float firstScaleTime, float secondScaleTime, Ease.Type firstScaleEasing = Ease.Type.Linear, Ease.Type secondScaleEasing = Ease.Type.Linear, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomPushScale(tweenObject, firstScale.ToVector3(), secondScale.ToVector3(), firstScaleTime, secondScaleTime, firstScaleEasing, secondScaleEasing).SetTime(firstScaleTime + secondScaleTime).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes x scale of object.
        /// </summary>
        public static TweenCase DOScaleX(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomScaleX(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes y scale of object.
        /// </summary>
        public static TweenCase DOScaleY(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomScaleY(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes z scale of object.
        /// </summary>
        public static TweenCase DOScaleZ(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomScaleZ(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes local position of object.
        /// </summary>
        public static TweenCase DOLocalMove(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomLocalMove(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes x local position of object.
        /// </summary>
        public static TweenCase DOLocalMoveX(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomLocalPositionX(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes y local position of object.
        /// </summary>
        public static TweenCase DOLocalMoveY(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomLocalPositionY(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Changes z local position of object.
        /// </summary>
        public static TweenCase DOLocalMoveZ(this Transform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomLocalPositionZ(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Rotates object face to position.
        /// </summary>
        public static TweenCase DOLookAt(this Transform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomLookAt(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Rotates 2D object face to position.
        /// </summary>
        public static TweenCase DOLookAt2D(this Transform tweenObject, Vector3 resultValue, TweenCaseTransfomLookAt2D.LookAtType type, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTransfomLookAt2D(tweenObject, resultValue, type).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }
        #endregion

        #region RectTransform
        /// <summary>
        /// Change anchored position of rectTransform
        /// </summary>
        public static TweenCase DOAnchoredPosition(this RectTransform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseRectTransformAnchoredPosition(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Change sizeDelta of rectTransform
        /// </summary>
        public static TweenCase DOSizeScale(this RectTransform tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseRectTransformSizeScale(tweenObject, tweenObject.sizeDelta * resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Change sizeDelta of rectTransform
        /// </summary>
        public static TweenCase DOSize(this RectTransform tweenObject, Vector3 resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseRectTransformSizeScale(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }
        #endregion

        #region SpriteRenderer
        /// <summary>
        /// Change color of sprite renderer
        /// </summary>
        public static TweenCase DOColor(this SpriteRenderer tweenObject, Color resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseSpriteRendererColor(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Change sprite renderer color alpha
        /// </summary>
        public static TweenCase DOFade(this SpriteRenderer tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseSpriteRendererFade(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }
        #endregion

        #region Image
        /// <summary>
        /// Change color of image
        /// </summary>
        public static TweenCase DOColor(this Image tweenObject, Color resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseImageColor(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Change image color alpha
        /// </summary>
        public static TweenCase DOFade(this Image tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseImageFade(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Change image fill
        /// </summary>
        public static TweenCase DOFillAmount(this Image tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseImageFill(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }
        #endregion

        #region Text
        /// <summary>
        /// Change text font size
        /// </summary>
        public static TweenCase DOFontSize(this Text tweenObject, int resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTextFontSize(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        public static TweenCase DOFontSize(this TMPro.TMP_Text tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseAction<float>(tweenObject.fontSize, resultValue, (start, end, t) => {
                tweenObject.fontSize = start + (end - start) * t;
            }).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Change text color alpha
        /// </summary>
        public static TweenCase DOFade(this Text tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTextFade(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        public static TweenCase DOFade(this TMPro.TMP_Text tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseAction<float>(tweenObject.color.a, resultValue, (start, end, t) => {

                tweenObject.color = tweenObject.color.SetAlpha(start + (end - start) * t);

            }).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Change color of text
        /// </summary>
        public static TweenCase DOColor(this Text tweenObject, Color resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseTextColor(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }
        #endregion

        #region CanvasGroup
        /// <summary>
        /// Change alpha value of canvas group
        /// </summary>
        public static TweenCase DOFade(this CanvasGroup tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseCanvasGroupFade(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }
        #endregion

        #region AudioSource
        /// <summary>
        /// Change audio source volume
        /// </summary>
        public static TweenCase DOVolume(this AudioSource tweenObject, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseAudioSourceVolume(tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }
        #endregion

        #region Material
        /// <summary>
        /// Change color of material
        /// </summary>
        public static TweenCase DOColor(this Material tweenObject, int colorID, Color resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseMaterialColor(colorID, tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        /// <summary>
        /// Change float of material
        /// </summary>
        public static TweenCase DoFloat(this Material material, int floatId, float resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseMaterialFloat(floatId, material, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }

        #endregion

        #region Renderer
        /// <summary>
        /// Change color of renderer
        /// </summary>
        public static TweenCase DOPropertyBlockColor(this Renderer tweenObject, int colorID, MaterialPropertyBlock materialPropertyBlock, Color resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCasePropertyBlockColor(colorID, materialPropertyBlock, tweenObject, resultValue).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }
        #endregion

        #region Other
        public static TweenCase DOAction<T>(this object tweenObject, System.Action<T, T, float> action, T startValue, T resultValue, float time, bool unscaledTime = false, TweenType tweenType = TweenType.Update)
        {
            return new TweenCaseAction<T>(startValue, resultValue, action).SetTime(time).SetUnscaledMode(unscaledTime).SetType(tweenType).StartTween();
        }
        #endregion
    }
}


// -----------------
// Tween v 1.1f3
// -----------------