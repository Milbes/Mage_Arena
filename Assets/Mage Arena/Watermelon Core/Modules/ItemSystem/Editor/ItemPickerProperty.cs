using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    [CustomPropertyDrawer(typeof(ItemPickerAttribute))]
    public class ItemPickerProperty : UnityEditor.PropertyDrawer
    {
        private ItemDatabase m_ItemDatabase;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_ItemDatabase == null)
            {
                m_ItemDatabase = ItemDatabase.GetDatabase();
                m_ItemDatabase.Init();
            }

            if (property.propertyType != SerializedPropertyType.Integer)
            {
                EditorGUI.HelpBox(position, "Wrong property type!", MessageType.Warning);
            }
            else
            {
                EditorGUI.BeginProperty(position, label, property);
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;

                string title = "";
                if (m_ItemDatabase.HasItemById(property.intValue))
                {
                    Item selectedItem = m_ItemDatabase.GetItemById(property.intValue);
                    title = selectedItem.ID.ToString("000") + ": " + selectedItem.name;
                }
                else
                {
                    title = "Wrong item id!";
                }

                label = EditorGUI.BeginProperty(position, label, property);
                Rect contentPosition = EditorGUI.PrefixLabel(position, label);
                contentPosition.width -= 20;
                EditorGUI.LabelField(contentPosition, title);
                if (GUI.Button(new Rect(position.width, contentPosition.y, 16, position.height), "o"))
                {
                    ItemPickWindow.GetItemId(property);
                }
                EditorGUI.indentLevel = indent;
                EditorGUI.EndProperty();
            }
        }
    }
}