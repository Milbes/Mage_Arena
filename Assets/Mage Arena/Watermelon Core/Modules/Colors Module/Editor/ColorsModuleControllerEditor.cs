using UnityEngine;
using UnityEditor;

namespace Watermelon
{
    //[CustomEditor(typeof(ColorsModuleController))]
    public class ColorsModuleControllerEditor : Editor
    {
        private SerializedProperty colorDatabaseProperty;
        private SerializedProperty materialReferencesProperty;
        private SerializedProperty uIImageReferencesProperty;
        private SerializedProperty textureReferencesProperty;
        private SerializedProperty referenceNameProperty;
        private SerializedProperty referenceValueProperty;
        private SerializedProperty selectedPresetValueProperty;

        private SerializedObject colorDatabaseSerializedObject;        
        private SerializedProperty presetsSerializedProperty;
        private SerializedProperty materialsSerializedProperty;
        private SerializedProperty uiImagesSerializedProperty;
        private SerializedProperty texturesSerializedProperty;


        private const string colorDatabasePropertyName = "colorPresetsDatabase";
        private const string materialReferencesPropertyName = "materialReferences";
        private const string uIImageReferencesPropertyName = "uIImageReferences";
        private const string textureReferencesPropertyName = "textureReferences";
        private const string selectedPresetPropertyName = "selectedPreset";
        private ColorsModuleController colorModuleController;

        private bool updated;
        private string[] presetNames;
        private int selectedPreset;
       

        private void OnEnable()
        {
            updated = false;
            colorDatabaseProperty = serializedObject.FindProperty(colorDatabasePropertyName);
            selectedPresetValueProperty = serializedObject.FindProperty(selectedPresetPropertyName);
            materialReferencesProperty = serializedObject.FindProperty(materialReferencesPropertyName);
            uIImageReferencesProperty = serializedObject.FindProperty(uIImageReferencesPropertyName);
            textureReferencesProperty = serializedObject.FindProperty(textureReferencesPropertyName);            
            colorModuleController = (ColorsModuleController)serializedObject.targetObject;
            selectedPreset = selectedPresetValueProperty.intValue;
        }

        private void UpdateReferences()
        {
            if(colorDatabaseProperty.objectReferenceValue == null)
            {
                colorDatabaseProperty.objectReferenceValue = EditorUtils.GetAsset<ColorPresetsDatabase>();

                if(colorDatabaseProperty.objectReferenceValue == null)
                {
                    return;
                }
                
            }

            colorDatabaseSerializedObject = new SerializedObject(colorDatabaseProperty.objectReferenceValue);
            presetsSerializedProperty = colorDatabaseSerializedObject.FindProperty("presets");
            materialsSerializedProperty = colorDatabaseSerializedObject.FindProperty("materialGroups");
            uiImagesSerializedProperty = colorDatabaseSerializedObject.FindProperty("uiImageGroups");
            texturesSerializedProperty = colorDatabaseSerializedObject.FindProperty("textureGroups");

            presetNames = new string[presetsSerializedProperty.arraySize];

            for (int i = 0; i < presetsSerializedProperty.arraySize; i++)
            {
                presetNames[i] = "#" + i + " | " + presetsSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue;
            }

            UpdatePresetProperty(materialReferencesProperty, materialsSerializedProperty);
            UpdatePresetProperty(uIImageReferencesProperty, uiImagesSerializedProperty);
            UpdatePresetProperty(textureReferencesProperty, texturesSerializedProperty);

            updated = true;
        }

        private void UpdatePresetProperty(SerializedProperty propertyArray, SerializedProperty configurationElementArray)
        {
            bool found;
            SerializedProperty uuidSerializedProperty;
            SerializedProperty groupUUIDSerializedProperty;

            for (int i = 0; i < configurationElementArray.arraySize; i++)
            {
                found = false;
                uuidSerializedProperty = configurationElementArray.GetArrayElementAtIndex(i).FindPropertyRelative("uuid");

                for (int j = i; j < propertyArray.arraySize; j++)
                {
                    groupUUIDSerializedProperty = propertyArray.GetArrayElementAtIndex(j).FindPropertyRelative("groupUUID");

                    if (uuidSerializedProperty.stringValue.Equals(groupUUIDSerializedProperty.stringValue))
                    {
                        found = true;

                        if (j != i)
                        {
                            for (int k = j - 1; k >= i; k--)
                            {
                                propertyArray.RemoveFromVariableArrayAt(k);
                            }
                        }
                    }
                }

                if (!found)
                {
                    propertyArray.InsertArrayElementAtIndex(i);
                    groupUUIDSerializedProperty = propertyArray.GetArrayElementAtIndex(i).FindPropertyRelative("groupUUID");
                    groupUUIDSerializedProperty.stringValue = uuidSerializedProperty.stringValue;

                }

            }

            if (configurationElementArray.arraySize < propertyArray.arraySize)
            {
                for (int i = propertyArray.arraySize - 1; i >= configurationElementArray.arraySize; i--)
                {
                    propertyArray.RemoveFromVariableArrayAt(i);
                }
            }
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            EditorGUILayout.PropertyField(colorDatabaseProperty);
            

            if (updated)
            {
                selectedPreset = EditorGUILayout.Popup(selectedPresetValueProperty.intValue, presetNames);

                if(selectedPreset != selectedPresetValueProperty.intValue)
                {
                    colorModuleController.SelectPreset(selectedPreset);
                }

                EditorGUILayout.Space();

                for (int i = 0; i < materialReferencesProperty.arraySize; i++)
                {
                    referenceNameProperty = materialsSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name");
                    referenceValueProperty = materialReferencesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("meshRenderer");
                    EditorGUILayout.PropertyField(referenceValueProperty, new GUIContent(referenceNameProperty.stringValue));
                }

                for (int i = 0; i < textureReferencesProperty.arraySize; i++)
                {
                    referenceNameProperty = texturesSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("name");
                    referenceValueProperty = textureReferencesProperty.GetArrayElementAtIndex(i).FindPropertyRelative("material");
                    EditorGUILayout.PropertyField(referenceValueProperty, new GUIContent(referenceNameProperty.stringValue));
                }
            }
            else
            {
                UpdateReferences();

                if (updated)
                {
                    Repaint();
                }
            }

            if (GUILayout.Button("UpdateReferences"))
            {
                UpdateReferences();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
