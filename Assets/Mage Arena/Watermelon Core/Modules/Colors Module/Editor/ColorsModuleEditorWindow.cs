using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    [InitializeOnLoadAttribute]
    public class ColorsModuleEditorWindow : EditorWindow
    {
        private const string MENU_ITEM_PATH = "Tools/Colors Module";
        public static string EDITOR_WINDOW_TITLE = "Colors Module";
        private static ColorsModuleEditorWindow window;
        private ColorPresetsDatabase colorPresetsDatabase;
        private bool databaseLoaded;


        SerializedObject colorDatabaseSerializedObject;
        SerializedProperty presetsSerializedProperty;
        SerializedProperty materialsSerializedProperty;
        SerializedProperty uiImagesSerializedProperty;
        SerializedProperty texturesSerializedProperty;
        SerializedProperty materialPresetItemsSerializedProperty;
        SerializedProperty uIImagePresetItemsSerializedProperty;
        SerializedProperty texturePresetItemsSerializedProperty;

        SerializedProperty tempProperty;
        SerializedProperty nameProperty;
        SerializedProperty presetItemSerializedProperty;

        private string[] tabs = {"Elements","Presets"};
        private Vector2 tab1ScrollVector;
        private Vector2 tab2ScrollVector;
        private int selectedTab;
        private int tab1SelectedIndex;
        private GroupType tab1SelectedGroupType;
        private int tab2SelectedIndex;

        private Rect clickRect;


        [MenuItem(MENU_ITEM_PATH)]
        public static void Init()
        {
            window = EditorWindow.GetWindow<ColorsModuleEditorWindow>(EDITOR_WINDOW_TITLE);
            window.minSize = new Vector2(300,500);
            window.Show();
            
        }


        private void OnEnable()
        {
            colorPresetsDatabase = EditorUtils.GetAsset<ColorPresetsDatabase>();

            if(colorPresetsDatabase == null)
            {
                Debug.LogError("Color Database not created. Create color database in content folder");
            }
            else
            {
                databaseLoaded = true;
                colorDatabaseSerializedObject = new SerializedObject(colorPresetsDatabase);
                presetsSerializedProperty = colorDatabaseSerializedObject.FindProperty("presets");
                materialsSerializedProperty = colorDatabaseSerializedObject.FindProperty("materialGroups");
                uiImagesSerializedProperty = colorDatabaseSerializedObject.FindProperty("uiImageGroups");
                texturesSerializedProperty = colorDatabaseSerializedObject.FindProperty("textureGroups");

                UpdatePresets();
                tab1SelectedIndex = -1;
                tab2SelectedIndex = -1;
                selectedTab = 0;
            }
        }

        private void OnGUI()
        {
            if (!databaseLoaded)
            {
                return;
            }

            EditorGUILayout.BeginVertical();
            selectedTab = GUILayout.Toolbar(selectedTab, tabs);

            if(selectedTab == 0)
            {
                DisplayElementsTab();
            }
            else
            {
                DisplayPresetsTab();
            }


            EditorGUILayout.EndVertical();

            colorDatabaseSerializedObject.ApplyModifiedProperties();
        }

        private void DisplayElementsTab()
        {
            tab1ScrollVector = GUILayout.BeginScrollView(tab1ScrollVector);
            DisplayGroup(materialsSerializedProperty,GroupType.Material, "Materials", "Add Material");

            EditorGUILayout.Space();
            DisplayGroup(uiImagesSerializedProperty, GroupType.UIImage, "UI Images", "Add UI Image");

            EditorGUILayout.Space();
            DisplayGroup(texturesSerializedProperty, GroupType.Textures, "Textures", "Add Texture");

            EditorGUILayout.Space();
            GUILayout.EndScrollView();
        }

        private void DisplayGroup(SerializedProperty groupProperty,GroupType groupType, string groupName,string addText)
        {
            GUILayout.Label(groupName);

            if (GUILayout.Button(addText))
            {
                groupProperty.arraySize++;
                tempProperty = groupProperty.GetArrayElementAtIndex(groupProperty.arraySize - 1);
                tempProperty.FindPropertyRelative("name").stringValue = "New element";
                tempProperty.FindPropertyRelative("uuid").stringValue = System.Guid.NewGuid().ToString();
                UpdatePresets();
            }

            for (int i = 0; i < groupProperty.arraySize; i++)
            {
                nameProperty = groupProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name");

                clickRect = EditorGUILayout.BeginVertical(GUI.skin.box);

                if ((tab1SelectedGroupType == groupType) && (tab1SelectedIndex == i))
                {
                    nameProperty.stringValue = EditorGUILayout.TextField("Name:", nameProperty.stringValue);

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Delete"))
                    {
                        if (EditorUtility.DisplayDialog("Delete dialog", "Are yous sure you want to delete \"" + nameProperty.stringValue + "\"", "ok", "cancel"))
                        {
                            groupProperty.RemoveFromVariableArrayAt(i);
                            UpdatePresets();
                        }
                    }


                }
                else
                {
                    EditorGUILayout.LabelField(nameProperty.stringValue);
                }


                EditorGUILayout.EndVertical();

                // select and unselect element
                if (GUI.Button(clickRect, GUIContent.none, GUIStyle.none))
                {
                    if ((tab1SelectedGroupType == groupType) && (tab1SelectedIndex == i))
                    {
                        tab1SelectedIndex = -1;
                    }
                    else
                    {
                        tab1SelectedIndex = i;
                        tab1SelectedGroupType = groupType;
                    }
                }
            }
        }

        private void DisplayPresetsTab()
        {
            tab2ScrollVector = GUILayout.BeginScrollView(tab2ScrollVector);
            
            if(GUILayout.Button("Add Preset"))
            {
                AddPreset();
            }

            for (int i = 0; i < presetsSerializedProperty.arraySize; i++)
            {
                tempProperty = presetsSerializedProperty.GetArrayElementAtIndex(i);
                nameProperty = tempProperty.FindPropertyRelative("name");

                clickRect = EditorGUILayout.BeginVertical(GUI.skin.box);

                if (tab2SelectedIndex != i)
                {
                    EditorGUILayout.LabelField(nameProperty.stringValue);
                }
                else
                {
                    materialPresetItemsSerializedProperty = tempProperty.FindPropertyRelative("materialPresetItems");
                    uIImagePresetItemsSerializedProperty = tempProperty.FindPropertyRelative("uIImagePresetItems");
                    texturePresetItemsSerializedProperty = tempProperty.FindPropertyRelative("texturePresetItems");


                    nameProperty.stringValue = EditorGUILayout.TextField("Preset name:", nameProperty.stringValue);
                    EditorGUILayout.Space();
                    //materials

                    for (int j = 0; j < materialPresetItemsSerializedProperty.arraySize; j++)
                    {
                        EditorGUILayout.LabelField(materialsSerializedProperty.GetArrayElementAtIndex(j).FindPropertyRelative("name").stringValue);
                        presetItemSerializedProperty = materialPresetItemsSerializedProperty.GetArrayElementAtIndex(j);
                        EditorGUILayout.PropertyField(presetItemSerializedProperty.FindPropertyRelative("albedoColor"));
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PropertyField(presetItemSerializedProperty.FindPropertyRelative("isEmisionEnabled"),new GUIContent("emision"));

                        if (presetItemSerializedProperty.FindPropertyRelative("isEmisionEnabled").boolValue)
                        {
                            EditorGUILayout.PropertyField(presetItemSerializedProperty.FindPropertyRelative("emissionColor"),GUIContent.none);
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.Separator();
                    }

                    EditorGUILayout.Space();
                    //UI images

                    for (int j = 0; j < uIImagePresetItemsSerializedProperty.arraySize; j++)
                    {
                        presetItemSerializedProperty = uIImagePresetItemsSerializedProperty.GetArrayElementAtIndex(j);
                        EditorGUILayout.PropertyField(presetItemSerializedProperty.FindPropertyRelative("color"),new GUIContent(uiImagesSerializedProperty.GetArrayElementAtIndex(j).FindPropertyRelative("name").stringValue));
                        EditorGUILayout.Separator();
                    }

                    EditorGUILayout.Space();
                    //Textures

                    for (int j = 0; j < texturePresetItemsSerializedProperty.arraySize; j++)
                    {
                        
                        presetItemSerializedProperty = texturePresetItemsSerializedProperty.GetArrayElementAtIndex(j);
                        EditorGUILayout.PropertyField(presetItemSerializedProperty.FindPropertyRelative("texture"),new GUIContent(texturesSerializedProperty.GetArrayElementAtIndex(j).FindPropertyRelative("name").stringValue));
                        EditorGUILayout.Separator();
                    }

                    EditorGUILayout.Space();
                    if (GUILayout.Button("Delete"))
                    {
                        if (EditorUtility.DisplayDialog("Delete dialog", "Are yous sure you want to delete \"" + nameProperty.stringValue + "\"", "ok", "cancel"))
                        {
                            presetsSerializedProperty.RemoveFromVariableArrayAt(i);
                        }
                    }
                }

                EditorGUILayout.EndVertical();

                //select and unselect
                if (GUI.Button(clickRect, GUIContent.none, GUIStyle.none))
                {
                    if ((tab2SelectedIndex == i))
                    {
                        tab2SelectedIndex = -1;
                    }
                    else
                    {
                        tab2SelectedIndex = i;
                    }
                }
            }           

            GUILayout.EndScrollView();
        }

        private void AddPreset()
        {
            presetsSerializedProperty.arraySize++;
            tempProperty = presetsSerializedProperty.GetArrayElementAtIndex(presetsSerializedProperty.arraySize - 1);
            tempProperty.FindPropertyRelative("name").stringValue = "New element";

            ConfigureProperty(tempProperty.FindPropertyRelative("materialPresetItems"), materialsSerializedProperty);
            ConfigureProperty(tempProperty.FindPropertyRelative("uIImagePresetItems"), uiImagesSerializedProperty);
            ConfigureProperty(tempProperty.FindPropertyRelative("texturePresetItems"), texturesSerializedProperty);
        }

        private void ConfigureProperty(SerializedProperty presetPropertyArray,SerializedProperty configurationElementArray)
        {
            presetPropertyArray.arraySize = configurationElementArray.arraySize;

            for (int i = 0; i < configurationElementArray.arraySize; i++)
            {
                presetPropertyArray.GetArrayElementAtIndex(i).FindPropertyRelative("groupUUID").stringValue = configurationElementArray.GetArrayElementAtIndex(i).FindPropertyRelative("uuid").stringValue;
            }
        }

        private void UpdatePresets()
        {
            for (int i = 0; i < presetsSerializedProperty.arraySize; i++)
            {
                tempProperty = presetsSerializedProperty.GetArrayElementAtIndex(i);
                UpdatePresetProperty(tempProperty.FindPropertyRelative("materialPresetItems"), materialsSerializedProperty);
                UpdatePresetProperty(tempProperty.FindPropertyRelative("uIImagePresetItems"), uiImagesSerializedProperty);
                UpdatePresetProperty(tempProperty.FindPropertyRelative("texturePresetItems"), texturesSerializedProperty);
            }
        }

        private void UpdatePresetProperty(SerializedProperty presetPropertyArray, SerializedProperty configurationElementArray)
        {
            bool found;
            SerializedProperty uuidSerializedProperty;
            SerializedProperty groupUUIDSerializedProperty;

            for (int i = 0; i < configurationElementArray.arraySize; i++)
            {
                found = false;
                uuidSerializedProperty = configurationElementArray.GetArrayElementAtIndex(i).FindPropertyRelative("uuid");

                for (int j = i; j < presetPropertyArray.arraySize; j++)
                {
                    groupUUIDSerializedProperty = presetPropertyArray.GetArrayElementAtIndex(j).FindPropertyRelative("groupUUID");

                    if (uuidSerializedProperty.stringValue.Equals(groupUUIDSerializedProperty.stringValue))
                    {
                        found = true;

                        if (j != i)
                        {
                            for (int k = j - 1; k >= i; k--)
                            {
                                presetPropertyArray.RemoveFromVariableArrayAt(k);
                            }
                        }
                    }
                }

                if (!found)
                {
                    presetPropertyArray.InsertArrayElementAtIndex(i);
                    groupUUIDSerializedProperty = presetPropertyArray.GetArrayElementAtIndex(i).FindPropertyRelative("groupUUID");
                    groupUUIDSerializedProperty.stringValue = uuidSerializedProperty.stringValue;

                }

            }

            if(configurationElementArray.arraySize < presetPropertyArray.arraySize)
            {
                for (int i = presetPropertyArray.arraySize - 1; i >= configurationElementArray.arraySize; i--)
                {
                    presetPropertyArray.RemoveFromVariableArrayAt(i);
                }
            }
        }
    }    
}
