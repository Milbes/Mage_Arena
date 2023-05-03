using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    [CustomPropertyDrawer(typeof(LineSpacerAttribute))]
    public class LineSpacerDrawer : DecoratorDrawer
    {
        private const int LINE_HEIGHT = 18;

        private LineSpacerAttribute lineSpacer
        {
            get { return (LineSpacerAttribute)attribute; }
        }

        public override void OnGUI(Rect position)
        {
            LineSpacerAttribute lineSpacer = this.lineSpacer;

            Color oldGuiColor = GUI.color;
            EditorGUI.LabelField(new Rect(position.x, position.y + LINE_HEIGHT - 12, position.width, LINE_HEIGHT), lineSpacer.title, EditorStyles.boldLabel);
            EditorGUI.LabelField(new Rect(position.x, position.y + LINE_HEIGHT, position.width, LINE_HEIGHT), "", GUI.skin.horizontalSlider);
            GUI.color = oldGuiColor;
        }

        public override float GetHeight()
        {
            return base.GetHeight() + LINE_HEIGHT;
        }
    }
}