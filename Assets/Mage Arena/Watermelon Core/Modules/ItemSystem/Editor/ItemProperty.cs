using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    [CustomPropertyDrawer(typeof(ItemPropertyAttribute))]
    public class ItemProperty : UnityEditor.PropertyDrawer
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

                string title = "";
                if (m_ItemDatabase.HasItemById(property.intValue))
                {
                    Item selectedItem = m_ItemDatabase.GetItemById(property.intValue);
                    title = selectedItem.ID.ToString("000") + ": " + (!string.IsNullOrEmpty(selectedItem.ItemName) ? selectedItem.ItemName : selectedItem.name);
                }
                else
                {
                    title = "Wrong item id! (" + property.intValue + ")";
                }

                EditorGUI.LabelField(new Rect(position.x, position.y, position.width - 20, position.height), title);
                if (GUI.Button(new Rect(position.width - 5, position.y, 15, position.height), "o"))
                {
                    ItemPickWindow.GetItemId(property);
                }
                EditorGUI.EndProperty();
            }
        }
    }
}