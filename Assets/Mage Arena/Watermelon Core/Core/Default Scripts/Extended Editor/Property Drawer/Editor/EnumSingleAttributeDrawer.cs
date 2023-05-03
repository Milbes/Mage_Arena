using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    [CustomPropertyDrawer(typeof(EnumSingleAttribute))]
    public class EnumSingleAttributeDrawer : UnityEditor.PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

            property.intValue = (int)(ItemRarity)EditorGUI.EnumPopup(position, label, (ItemRarity)property.intValue);
            EditorGUI.EndProperty();
        }
    }
}