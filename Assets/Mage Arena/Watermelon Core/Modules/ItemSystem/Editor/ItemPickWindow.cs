using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    public class ItemPickWindow : EditorWindow
    {
        private static int m_SelectedItemId = -1;
        private static ItemPickWindow m_Window;

        public ItemDatabase m_ItemDatabase;
        private SerializedObject m_ItemDatabaseSerializedObject;

        private SerializedProperty m_ItemsList;

        private static Pagination m_Pagination = new Pagination(10, 5);

        private static SerializedProperty m_TargetProperty;

        private void OnEnable()
        {
            EditorApplication.update += Validate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= Validate;
        }

        private void Validate()
        {
            if (m_ItemsList == null)
                Close();
        }

        public static int GetItemId(SerializedProperty serializedProperty)
        {
            if (m_Window == null)
            {
                m_Window = GetWindow(typeof(ItemPickWindow), true, "Item picker", true) as ItemPickWindow;

                m_Window.m_ItemDatabaseSerializedObject = new SerializedObject(m_Window.m_ItemDatabase);
                m_Window.m_ItemsList = m_Window.m_ItemDatabaseSerializedObject.FindProperty("items");
                m_Window.minSize = new Vector2(320, 220);
                m_Window.maxSize = new Vector2(320, 220);

                m_Pagination.Init(m_Window.m_ItemsList);

                m_SelectedItemId = serializedProperty.intValue;
                m_TargetProperty = serializedProperty;
            }

            return m_SelectedItemId;
        }

        private void OnGUI()
        {
            int arrayList = m_ItemsList.arraySize;
            if (arrayList == 0)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                EditorGUILayout.HelpBox("Items Database is empty. Add item first!", MessageType.Warning, true);
                EditorGUILayout.EndHorizontal();

                return;
            }

            EditorGUILayout.BeginVertical(GUI.skin.box, GUILayout.Height(190));
            int paginationEnd = m_Pagination.GetMaxElementNumber();
            for (int i = m_Pagination.GetMinElementNumber(); i < paginationEnd; i++)
            {
                SerializedProperty itemProperty = m_ItemsList.GetArrayElementAtIndex(i);

                if (itemProperty.objectReferenceValue == null)
                {
                    continue;
                }

                Item item = itemProperty.objectReferenceValue as Item;

                bool isLevelSelected = m_SelectedItemId == item.ID;

                if (isLevelSelected)
                    GUI.color = Color.green;

                if (GUILayout.Button(item.ID.ToString("000") + ": " + (string.IsNullOrEmpty(item.ItemName) ? "Unknown Item" : item.ItemName), GUI.skin.textArea))
                {
                    if (m_SelectedItemId == item.ID)
                    {
                        Close();
                    }
                    else
                    {
                        m_SelectedItemId = item.ID;

                        m_TargetProperty.serializedObject.Update();
                        m_TargetProperty.intValue = m_SelectedItemId;
                        m_TargetProperty.serializedObject.ApplyModifiedProperties();
                    }

                    return;
                }

                GUI.color = Color.white;
            }
            EditorGUILayout.EndVertical();

            m_Pagination.DrawPagination();
        }

        private void OnDestroy()
        {
            if (m_TargetProperty != null)
            {
                m_TargetProperty.serializedObject.Update();
                m_TargetProperty.intValue = m_SelectedItemId;
                m_TargetProperty.serializedObject.ApplyModifiedProperties();
            }

            m_Window = null;
        }
    }
}