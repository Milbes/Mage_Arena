using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Watermelon
{
    public class ItemEditorWindow : EditorWindow
    {
        private readonly string[] TABS = new string[]
        {
            "Items",
            "Types",
            "Settings"
        };

        private static int currentTab;

        #region Types
        private List<TypeObject> types = new List<TypeObject>();
        #endregion

        #region Settings
        private static ItemDatabase itemDatabase;
        private static string folderPath;

        private readonly string TYPES_FILE_NAME = "ItemType.cs";
        private readonly string TYPES_FOLDER_NAME = "Types";

        private string typeName;
        #endregion


        private Editor itemsDatabaseEditor;

        private Color defaultColor;

        [MenuItem("Tools/Item Editor")]
        public static void ShowWindow()
        {
            ItemEditorWindow window = (ItemEditorWindow)GetWindow(typeof(ItemEditorWindow), false, "Item Editor");
            window.minSize = new Vector2(280, 0);
            window.Show();
        }

        private void OnEnable()
        {
            GUI.FocusControl(null);

            Init();

            defaultColor = GUI.color;
        }

        private void Init()
        {
            types = new List<TypeObject>();

            int[] enumIds = (int[])System.Enum.GetValues(typeof(ItemType));
            string[] enumNames = System.Enum.GetNames(typeof(ItemType));

            for (int i = 0; i < enumIds.Length; i++)
            {
                types.Add(new TypeObject(enumNames[i], enumIds[i]));
            }

            string[] itemDatabase = AssetDatabase.FindAssets("t:ItemDatabase");
            if (itemDatabase.Length > 0)
            {
                Debug.Log(AssetDatabase.GUIDToAssetPath(itemDatabase[0]));
                ItemEditorWindow.itemDatabase = (ItemDatabase)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(itemDatabase[0]), typeof(ItemDatabase));

                folderPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(ItemEditorWindow.itemDatabase));
            }

            Editor.CreateCachedEditor(ItemEditorWindow.itemDatabase, typeof(ItemDatabaseEditor), ref itemsDatabaseEditor);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            currentTab = GUILayout.Toolbar(currentTab, TABS);
            EditorGUILayout.BeginVertical(GUI.skin.box);

            switch (currentTab)
            {
                case 0: //Items
                    ItemsGUI();
                    break;
                case 1: //Types
                    TypesGUI();
                    break;
                case 2: //Settings
                    SettingsGUI();
                    break;
            }
            EditorGUILayout.EndVertical();
        }

        private bool ErrorGUI()
        {
            string error = "";

            if (itemDatabase == null)
                error = "Please, set ItemDatabase file!";
            else if (string.IsNullOrEmpty(folderPath))
                error = "Please, set folder path!";

            if (!string.IsNullOrEmpty(error))
            {
                EditorGUILayout.HelpBox(error, MessageType.Error, true);

                return true;
            }

            return false;
        }

        private void ItemsGUI()
        {
            if (ErrorGUI())
                return;

            itemsDatabaseEditor.OnInspectorGUI();
        }

        private void TypesGUI()
        {
            if (ErrorGUI())
                return;

            EditorGUILayout.BeginHorizontal(GUI.skin.box);
            typeName = EditorGUILayout.TextField(typeName);
            if (GUILayout.Button("Add"))
            {
                if (string.IsNullOrEmpty(typeName))
                {
                    EditorUtility.DisplayDialog("Name can't be empty!", "Please, set type name.", "Ok");

                    return;
                }

                if (types.FindIndex(x => x.name == typeName) != -1)
                {
                    EditorUtility.DisplayDialog("Type already exist!", "Please, set different type name.", "Ok");

                    return;
                }

                CreateItemType(typeName);

                types.Add(new TypeObject(typeName, GetId()));

                typeName = "";

                GUI.FocusControl(null);

                GenerateType();

            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < types.Count; i++)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                EditorGUILayout.LabelField(types[i].name + ":" + types[i].number);

                GUI.color = Color.red;
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    if (EditorUtility.DisplayDialog("Are you sure?", "This type will be removed!", "Remove", "Cancel"))
                    {
                        RemoveItemType(types[i].name);

                        types.RemoveAt(i);

                        GenerateType();
                    }
                }
                GUI.color = defaultColor;
                EditorGUILayout.EndHorizontal();
            }
        }

        private void SettingsGUI()
        {
            itemDatabase = (ItemDatabase)EditorGUILayout.ObjectField(new GUIContent("Item Database", "Item database scriptable object"), itemDatabase, typeof(ItemDatabase), false);

            GUI.enabled = false;
            folderPath = EditorGUILayout.TextField(new GUIContent("Folder path", "Items folder path"), folderPath);
            GUI.enabled = true;

            if (GUILayout.Button("Reset settings"))
            {
                if (EditorUtility.DisplayDialog("Are you sure?", "This type will be removed!", "Reset", "Cancel"))
                {
                    GUI.FocusControl(null);

                    Init();
                }
            }
        }

        private void CreateItemType(string name)
        {
            string folderPath = Application.dataPath.Replace("Assets", "") + ItemEditorWindow.folderPath + "/" + TYPES_FOLDER_NAME + "/" + name;

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            else
            {
                if (!EditorUtility.DisplayDialog("Are you sure?", "Folder already exist, all files will be rewritten!", "Rewrite", "Cancel"))
                {
                    return;
                }
            }

            string itemHolderName = name + "ItemHolder";
            System.Text.StringBuilder holder = new System.Text.StringBuilder();
            holder.AppendLine("namespace Watermelon");
            holder.AppendLine("{");
            holder.AppendLine("    [System.Serializable]");
            holder.AppendLine("    public class " + itemHolderName + " : ItemHolder");
            holder.AppendLine("    {");
            holder.AppendLine("        public " + itemHolderName + "(Item item, int amount = 1) : base(item, amount)");
            holder.AppendLine("        {");
            holder.AppendLine("");
            holder.AppendLine("        }");
            holder.AppendLine("");
            holder.AppendLine("        public override bool Check()");
            holder.AppendLine("        {");
            holder.AppendLine("            return false;");
            holder.AppendLine("        }");
            holder.AppendLine("    }");
            holder.AppendLine("}");

            CreateFile(folderPath + "/" + itemHolderName + ".cs", holder.ToString());

            string itemName = name + "Item";
            System.Text.StringBuilder item = new System.Text.StringBuilder();
            item.AppendLine("using UnityEngine;");
            item.AppendLine("");
            item.AppendLine("namespace Watermelon");
            item.AppendLine("{");
            item.AppendLine("    [CreateAssetMenu(fileName = \"" + name + " Item\", menuName = \"Items/" + name + " Item\")]");
            item.AppendLine("    public class " + itemName + " : Item");
            item.AppendLine("    {");
            item.AppendLine("        public " + itemName + "()");
            item.AppendLine("        {");
            item.AppendLine("            type = ItemType." + name + ";");
            item.AppendLine("        }");
            item.AppendLine("");
            item.AppendLine("        public override ItemHolder GetHolder()");
            item.AppendLine("        {");
            item.AppendLine("            return new " + itemHolderName + "(this, 1);");
            item.AppendLine("        }");
            item.AppendLine("    }");
            item.AppendLine("}");

            CreateFile(folderPath + "/" + itemName + ".cs", item.ToString());

            AssetDatabase.Refresh();
        }

        private void RemoveItemType(string name)
        {
            string folderPath = Application.dataPath.Replace("Assets", "") + ItemEditorWindow.folderPath + "/" + TYPES_FOLDER_NAME + "/" + name + "/";

            if (Directory.Exists(folderPath))
            {
                if (EditorUtility.DisplayDialog("Remove item folder?", "Folder already exist, all files will be removed!", "Remove", "Skip"))
                {
                    string[] files = Directory.GetFiles(folderPath);

                    for (int i = 0; i < files.Length; i++)
                    {
                        File.SetAttributes(files[i], FileAttributes.Normal);
                        File.Delete(files[i]);
                    }

                    Directory.Delete(folderPath, false);
                }
            }

            AssetDatabase.Refresh();
        }

        private void GenerateType()
        {
            //Generate ItemType.cs
            //Class strings
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using Watermelon;");
            sb.AppendLine("");
            sb.AppendLine("public enum ItemType");
            sb.AppendLine("{");
            for (int i = 0; i < types.Count; i++)
            {
                sb.AppendLine("    " + types[i].name + " = " + types[i].number + ",");
            }
            sb.AppendLine("}");
            sb.AppendLine("");
            sb.AppendLine("public static class ItemUtils");
            sb.AppendLine("{");
            sb.AppendLine("    public static Type GetType(ItemType type)");
            sb.AppendLine("    {");
            sb.AppendLine("        switch (type)");
            sb.AppendLine("        {");
            for (int i = 0; i < types.Count; i++)
            {
                sb.AppendLine("            case ItemType." + types[i].name + ":");
                sb.AppendLine("                return typeof(" + types[i].name + "Item);");
            }
            sb.AppendLine("        }");
            sb.AppendLine("");
            sb.AppendLine("        return typeof(Item);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            string fullPath = Application.dataPath.Replace("Assets", "") + folderPath + "/" + TYPES_FILE_NAME;

            CreateFile(fullPath, sb.ToString());

            AssetDatabase.Refresh();
        }

        private void CreateFile(string path, string content)
        {
            if (File.Exists(path))
                File.Delete(path);

            File.WriteAllText(path, content, System.Text.Encoding.UTF8);
        }

        private int GetId()
        {
            int typesCount = types.Count;

            if (typesCount == 0)
                return 0;

            return types[typesCount - 1].number + 1;
        }

        private class TypeObject
        {
            private string m_Name;
            public string name
            {
                get { return m_Name; }
            }

            private int m_Number;
            public int number
            {
                get { return m_Number; }
            }

            public TypeObject(string name, int number)
            {
                m_Name = name;
                m_Number = number;
            }
        }
    }
}