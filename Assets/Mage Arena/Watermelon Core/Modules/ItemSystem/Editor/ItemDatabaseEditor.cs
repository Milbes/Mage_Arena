using UnityEngine;
using UnityEditor;
using System.IO;

namespace Watermelon
{
    [CustomEditor(typeof(ItemDatabase))]
    public class ItemDatabaseEditor : Editor
    {
        /// <summary>
        /// Items (scriptable objects) folder name without slashes
        /// </summary>
        private const string ITEMS_FOLDER_NAME = "Items";

        /// <summary>
        /// Items (scriptable objects) folder path
        /// <para>Example: "Assets/InventorySystem/Items/"</para>
        /// </summary>
        private string m_FolderPath;

        /// <summary>
        /// Serialized items list 
        /// </summary>
        private SerializedProperty m_ItemsList;

        #region Creating
        /// <summary>
        /// Creating field (item type)
        /// </summary>
        private ItemType m_ItemType;

        /// <summary>
        /// Creating field (item name)
        /// </summary>
        private string m_ItemName;
        #endregion

        #region Selection
        /// <summary>
        /// Selected item instance id value
        /// </summary>
        private static int m_SelectedItemId = -1;

        /// <summary>
        /// Selected item serialized value
        /// </summary>
        private SerializedProperty m_SelectedItem;

        /// <summary>
        /// Selected item cached editor
        /// </summary>
        private Editor m_SelectedItemEditor;
        #endregion

        private static Pagination m_Pagination = new Pagination(10, 5);

        private void OnEnable()
        {
            //Get list property
            m_ItemsList = serializedObject.FindProperty("items");

            m_Pagination.Init(m_ItemsList);

            //Init selected object (if it exist)
            if (m_SelectedItemId != -1)
            {
                for (int i = 0; i < m_ItemsList.arraySize; i++)
                {
                    SerializedProperty property = m_ItemsList.GetArrayElementAtIndex(i);

                    //Compare selected item instance id with list properties ids
                    if (m_SelectedItemId == property.objectReferenceInstanceIDValue)
                    {
                        SelectedItem(property);

                        break;
                    }
                }
            }

            //Extract path from database and add folder name
            m_FolderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(target)) + "/" + ITEMS_FOLDER_NAME + "/";

            //Check if path folder exist and if not - create it
            if (!Directory.Exists(m_FolderPath))
                Directory.CreateDirectory(m_FolderPath);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            m_ItemType = (ItemType)EditorGUILayout.EnumPopup("Item Type:", m_ItemType);
            EditorGUILayout.BeginHorizontal();
            m_ItemName = EditorGUILayout.TextField("Item Name:", m_ItemName);

            GUI.color = Color.green;
            if (GUILayout.Button("Add", GUILayout.Height(14)))
            {
                if (!string.IsNullOrEmpty(m_ItemName))
                {
                    Item newItem = CreateItem(ItemUtils.GetType(m_ItemType), m_ItemName);

                    if (newItem != null)
                    {
                        int index = m_ItemsList.arraySize;
                        m_ItemsList.arraySize++;

                        m_ItemsList.GetArrayElementAtIndex(index).objectReferenceValue = newItem;

                        m_Pagination.Init(m_ItemsList);
                    }
                }
            }
            GUI.color = Color.white;

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            int arrayList = m_ItemsList.arraySize;
            if (arrayList == 0)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.HelpBox("Items Database is empty. Add item first!", MessageType.Warning, true);
                EditorGUILayout.EndHorizontal();

                return;
            }

            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(264));
            int paginationEnd = m_Pagination.GetMaxElementNumber();
            for (int i = m_Pagination.GetMinElementNumber(); i < paginationEnd; i++)
            {
                SerializedProperty itemProperty = m_ItemsList.GetArrayElementAtIndex(i);

                if (itemProperty.objectReferenceValue == null)
                {
                    continue;
                }

                bool isLevelSelected = m_SelectedItem != null && m_SelectedItemId == itemProperty.objectReferenceInstanceIDValue;

                if (isLevelSelected)
                    GUI.color = Color.green;

                Rect clickRect = (Rect)EditorGUILayout.BeginHorizontal(GUI.skin.box);

                Item item = itemProperty.objectReferenceValue as Item;
                EditorGUILayout.LabelField(item.ID.ToString("000") + ": " + (string.IsNullOrEmpty(item.ItemName) ? "Unknown Item" : item.ItemName));

                GUILayout.FlexibleSpace();

                GUI.color = Color.grey;
                if (GUILayout.Button("=", EditorStyles.miniButton, GUILayout.Width(16), GUILayout.Height(16)))
                {
                    int tempIndex = i;

                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent(isLevelSelected ? "Unselect" : "Select"), false, delegate
                    {
                        SelectedItem(itemProperty);
                    });

                    menu.AddItem(new GUIContent("Remove"), false, delegate
                    {
                        if (EditorUtility.DisplayDialog("Are you sure?", "This level will be removed!", "Remove", "Cancel"))
                        {
                            if (isLevelSelected)
                            {
                                m_SelectedItem = null;
                                m_SelectedItemId = -1;
                            }
 
                            Object removable = m_ItemsList.GetArrayElementAtIndex(tempIndex).objectReferenceValue;
                            m_ItemsList.RemoveFromObjectArrayAt(tempIndex);

                            if (removable != null)
                            {
                                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(removable));
                                AssetDatabase.Refresh();
                            }

                            m_Pagination.Init(m_ItemsList);

                            return;
                        }
                    });

                    menu.AddSeparator("");

                    if (i > 0)
                    {
                        menu.AddItem(new GUIContent("Move up"), false, delegate
                        {
                            m_ItemsList.MoveArrayElement(i, i - 1);
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Move up"));
                    }


                    if (i + 1 < arrayList)
                    {
                        menu.AddItem(new GUIContent("Move down"), false, delegate
                        {
                            m_ItemsList.MoveArrayElement(i, i + 1);
                        });
                    }
                    else
                    {
                        menu.AddDisabledItem(new GUIContent("Move down"));
                    }

                    menu.AddSeparator("");

                    menu.AddItem(new GUIContent("Select source"), false, delegate
                    {
                        EditorUtility.FocusProjectWindow();
                        Selection.activeObject = itemProperty.objectReferenceValue;
                    });

                    menu.ShowAsContext();
                }

                GUI.color = Color.white;

                GUILayout.Space(5);

                if (GUI.Button(clickRect, GUIContent.none, GUIStyle.none))
                {
                    SelectedItem(itemProperty);

                    return;
                }

                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();

            m_Pagination.DrawPagination();

            if (m_SelectedItemId != -1)
            {
                DrawLocationEditor();
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLocationEditor()
        {
            GUILayout.BeginVertical(GUI.skin.box);

            if (m_SelectedItem != null)
            {
                if (m_SelectedItemEditor == null)
                    Editor.CreateCachedEditor(m_SelectedItem.objectReferenceValue, null, ref m_SelectedItemEditor);

                m_SelectedItemEditor.OnInspectorGUI();
            }

            GUILayout.EndVertical();
        }

        private void SelectedItem(SerializedProperty serializedItem)
        {
            GUI.FocusControl(null);
            m_SelectedItemEditor = null;

            if (m_SelectedItem != null && m_SelectedItem.objectReferenceInstanceIDValue == serializedItem.objectReferenceInstanceIDValue)
            {
                m_SelectedItem = null;
                m_SelectedItemId = -1;

                return;
            }

            if (serializedItem != null)
            {
                m_SelectedItemId = serializedItem.objectReferenceInstanceIDValue;
                m_SelectedItem = serializedItem;
            }
        }

        private int GetLastId()
        {
            int arraySize = m_ItemsList.arraySize;

            if (arraySize == 0)
                return 0;

            Item item = (Item)m_ItemsList.GetArrayElementAtIndex(m_ItemsList.arraySize - 1).objectReferenceValue;

            return item.ID + 1;
        }

        private Item CreateItem(System.Type type, string name)
        {
            Item item = (Item)ScriptableObject.CreateInstance(type);

            item.SetName(name);
            item.SetId(GetLastId());

            string itemPath = m_FolderPath + name.Replace(" ", "") + "Item" + item.ID.ToString("000") + ".asset";

            AssetDatabase.CreateAsset(item, itemPath);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return item;
        }
    }
}