using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Linq;

namespace Watermelon
{
    [CustomPropertyDrawer(typeof(Loot))]
    public class LootProperty : UnityEditor.PropertyDrawer
    {
        private int m_ItemsCount;

        private const int PROPERTY_HEIGHT = 138;
        private readonly Color CONTAINER_COLOR = new Color(0.9f, 0.9f, 0.9f);

        private ItemDatabase m_ItemDatabase;

        public static Loot copiedObject = null;

        private void SettingsMenu(SerializedProperty property, SerializedProperty lootSerializedContainer)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add"), false, delegate
            {
                property.serializedObject.Update();

                Undo.RecordObject(lootSerializedContainer.serializedObject.targetObject, "loot adding");

                SerializedProperty lootProperty = property.FindPropertyRelative("loot");
                lootProperty.arraySize += 1;

            //Default values
            SerializedProperty newObject = lootProperty.GetArrayElementAtIndex(lootProperty.arraySize - 1);
                newObject.FindPropertyRelative("isUnique").boolValue = true;
                newObject.FindPropertyRelative("isEnabled").boolValue = true;

                newObject.FindPropertyRelative("minAmount").intValue = 1;
                newObject.FindPropertyRelative("maxAmount").intValue = 1;

                newObject.FindPropertyRelative("itemID").intValue = -1;

                EditorUtility.SetDirty(lootSerializedContainer.serializedObject.targetObject);

                property.serializedObject.ApplyModifiedProperties();

                return;
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Sort by id"), false, delegate
            {
                lootSerializedContainer.serializedObject.Update();

                Undo.RecordObject(lootSerializedContainer.serializedObject.targetObject, "loot sorting");

                Loot lootContainer = (Loot)EditorExtensions.GetPropertyObject(property);

                lootContainer.LootObject = lootContainer.LootObject.OrderBy(x => x.ItemID).ToArray();

                EditorUtility.SetDirty(lootSerializedContainer.serializedObject.targetObject);

                lootSerializedContainer.serializedObject.ApplyModifiedProperties();
            });

            menu.AddItem(new GUIContent("Sort by percent"), false, delegate
            {
                lootSerializedContainer.serializedObject.Update();

                Undo.RecordObject(lootSerializedContainer.serializedObject.targetObject, "loot sorting");

                Loot lootContainer = (Loot)EditorExtensions.GetPropertyObject(property);

                lootContainer.LootObject = lootContainer.LootObject.OrderByDescending(x => x.Probability).ToArray();

                EditorUtility.SetDirty(lootSerializedContainer.serializedObject.targetObject);

                lootSerializedContainer.serializedObject.ApplyModifiedProperties();
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Convert to percents"), false, delegate
            {
                lootSerializedContainer.serializedObject.Update();

                Undo.RecordObject(lootSerializedContainer.serializedObject.targetObject, "loot convering");

                Loot lootContainer = (Loot)EditorExtensions.GetPropertyObject(property);
                float sum = lootContainer.LootObject.Sum(x => x.Probability);

                for (int i = 0; i < lootContainer.LootObject.Length; i++)
                {
                    lootContainer.LootObject[i].Probability = lootContainer.LootObject[i].Probability / sum;
                }

                EditorUtility.SetDirty(lootSerializedContainer.serializedObject.targetObject);

                lootSerializedContainer.serializedObject.ApplyModifiedProperties();
            });

            menu.AddSeparator("");

            menu.AddItem(new GUIContent("Copy"), false, delegate
            {
                copiedObject = new Loot((Loot)EditorExtensions.GetPropertyObject(property));
            });

            if (copiedObject != null)
            {
                menu.AddItem(new GUIContent("Paste"), false, delegate
                {
                    lootSerializedContainer.serializedObject.Update();

                    Undo.RecordObject(lootSerializedContainer.serializedObject.targetObject, "loot copying");

                    Loot lootContainer = (Loot)EditorExtensions.GetPropertyObject(property);
                    int resultCount = copiedObject.ResultCount;

                    lootContainer.LootObject = copiedObject.LootObject;
                    lootContainer.ResultCount = resultCount;

                    EditorUtility.SetDirty(lootSerializedContainer.serializedObject.targetObject);

                    lootSerializedContainer.serializedObject.ApplyModifiedProperties();
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste"));
            }

            menu.ShowAsContext();
        }

        private void ObjectMenu(int index, SerializedProperty lootObject, SerializedProperty lootContainer)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Remove"), false, delegate
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "This item will be removed!", "Remove", "Cancel"))
                {
                    Undo.RecordObject(lootObject.serializedObject.targetObject, "loot removing");

                    lootContainer.RemoveFromVariableArrayAt(index);

                    EditorUtility.SetDirty(lootObject.serializedObject.targetObject);
                }
            });

            menu.ShowAsContext();
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (m_ItemDatabase == null)
            {
                m_ItemDatabase = ItemDatabase.GetDatabase();
                m_ItemDatabase.Init();
            }

            SerializedProperty resultCount = property.FindPropertyRelative("resultCount");
            SerializedProperty lootContainer = property.FindPropertyRelative("loot");

            float sum = 0;
            m_ItemsCount = lootContainer.arraySize;
            for (int i = 0; i < m_ItemsCount; i++)
            {
                sum += lootContainer.GetArrayElementAtIndex(i).FindPropertyRelative("probability").floatValue;
            }

            EditorGUI.BeginProperty(position, label, property);
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y + GetHeight(0), position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, "Loot", true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUI.PropertyField(new Rect(position.x, position.y + GetHeight(1), position.width - 30, EditorGUIUtility.singleLineHeight), resultCount);

                if (GUI.Button(new Rect(position.width - 10, position.y + GetHeight(1), 20, EditorGUIUtility.singleLineHeight), "="))
                {
                    SettingsMenu(property, lootContainer);
                }

                if (m_ItemsCount > 0)
                {
                    GUI.Box(new Rect(position.x, position.y + GetHeight(2), position.width, GetObjectHeight(lootContainer, m_ItemsCount, 2 + m_ItemsCount) - GetHeight(2)), GUIContent.none);
                    for (int i = 0; i < m_ItemsCount; i++)
                    {
                        SerializedProperty lootObject = lootContainer.GetArrayElementAtIndex(i);

                        string percent = "Percent: " + (lootObject.FindPropertyRelative("probability").floatValue / sum).ToString("0.00") + "%";
                        string content = "";
                        string title = "Object index: " + i + "\n";

                        int itemID = lootObject.FindPropertyRelative("itemID").intValue;
                        if (m_ItemDatabase.HasItemById(itemID))
                        {
                            Item item = m_ItemDatabase.GetItemById(itemID);

                            content = (!string.IsNullOrEmpty(item.ItemName) ? item.ItemName : "Unknown item");
                            title += "Item id: " + item.ID.ToString("000");
                        }
                        else
                        {
                            content = "Unknown item";
                            title += "Item id: ???";
                        }
                        title += "\n" + percent;

                        if (GUI.Button(new Rect(position.width - 5, position.y + GetObjectHeight(lootContainer, i, 2 + i), 15, EditorGUIUtility.singleLineHeight), "=", EditorStyles.miniButton))
                        {
                            ObjectMenu(i, lootObject, lootContainer);
                        }

                        lootObject.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y + GetObjectHeight(lootContainer, i, 2 + i), position.width, EditorGUIUtility.singleLineHeight), lootObject.isExpanded, new GUIContent(content, title), true);
                        if (lootObject.isExpanded)
                        {
                            GUI.backgroundColor = CONTAINER_COLOR;

                            GUI.Box(new Rect(position.x, position.y + GetObjectHeight(lootContainer, i, 3 + i), position.width, PROPERTY_HEIGHT), GUIContent.none);

                            EditorGUI.PropertyField(new Rect(position.x, position.y + GetObjectHeight(lootContainer, i, 3 + i), position.width, EditorGUIUtility.singleLineHeight), lootObject.FindPropertyRelative("itemID"));
                            EditorGUI.PropertyField(new Rect(position.x, position.y + GetObjectHeight(lootContainer, i, 4 + i), position.width - 10, EditorGUIUtility.singleLineHeight), lootObject.FindPropertyRelative("probability"));
                            EditorGUI.PropertyField(new Rect(position.x, position.y + GetObjectHeight(lootContainer, i, 5 + i), position.width, EditorGUIUtility.singleLineHeight), lootObject.FindPropertyRelative("isUnique"));
                            EditorGUI.PropertyField(new Rect(position.x, position.y + GetObjectHeight(lootContainer, i, 6 + i), position.width, EditorGUIUtility.singleLineHeight), lootObject.FindPropertyRelative("dropAlways"));
                            EditorGUI.PropertyField(new Rect(position.x, position.y + GetObjectHeight(lootContainer, i, 7 + i), position.width, EditorGUIUtility.singleLineHeight), lootObject.FindPropertyRelative("isEnabled"));

                            EditorGUI.PrefixLabel(new Rect(position.x, position.y + GetObjectHeight(lootContainer, i, 8 + i), 60, EditorGUIUtility.singleLineHeight), new GUIContent("Amount: "));
                            EditorGUI.PropertyField(new Rect(position.x + 60, position.y + GetObjectHeight(lootContainer, i, 8 + i), (position.width - 80) / 2, EditorGUIUtility.singleLineHeight), lootObject.FindPropertyRelative("minAmount"), GUIContent.none);
                            EditorGUI.PropertyField(new Rect(position.x + 60 + (position.width - 80) / 2, position.y + GetObjectHeight(lootContainer, i, 8 + i), (position.width - 80) / 2, EditorGUIUtility.singleLineHeight), lootObject.FindPropertyRelative("maxAmount"), GUIContent.none);

                            GUI.Label(new Rect(position.x, position.y + GetObjectHeight(lootContainer, i, 9 + i), position.width, EditorGUIUtility.singleLineHeight), percent);

                            GUI.backgroundColor = Color.white;
                        }
                    }
                }
            }

            EditorGUI.indentLevel = indent;
            EditorGUI.EndProperty();
        }

        public float GetHeight(int sorting)
        {
            return (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * sorting;
        }

        public float GetObjectHeight(SerializedProperty property, int objectID, int sorting)
        {
            float height = (EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight) * sorting;

            for (int i = 0; i < objectID; i++)
            {
                if (property.GetArrayElementAtIndex(i).isExpanded)
                    height += PROPERTY_HEIGHT + EditorGUIUtility.standardVerticalSpacing;
            }

            return height;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            SerializedProperty lootSerialized = property.FindPropertyRelative("loot");
            int lootCount = lootSerialized.arraySize;
            for (int i = 0; i < lootCount; i++)
            {
                height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                if (lootSerialized.GetArrayElementAtIndex(i).isExpanded)
                    height += PROPERTY_HEIGHT + EditorGUIUtility.standardVerticalSpacing;
            }

            return base.GetPropertyHeight(property, label) + (property.isExpanded ? height : 0);
        }
    }
}