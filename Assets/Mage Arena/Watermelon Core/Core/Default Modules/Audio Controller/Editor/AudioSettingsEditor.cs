using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Watermelon
{
    [CustomEditor(typeof(AudioSettings))]
    public class AudioSettingsEditor : WatermelonEditor
    {
        private const string audioEnabledPropertyName = "isAudioEnabled";
        private const string musicEnabledPropertyName = "isMusicEnabled";
        private const string vibrationEnabledPropertyName = "isVibrationEnabled";

        private const string soundsPropertyName = "sounds";
        private const string vibrationsPropertyName = "vibrations";

        private const string musicAudioClipsPropertyName = "musicAudioClips";
        
        private SerializedProperty audioEnabledSerializedProperty;
        private SerializedProperty musicEnabledSerializedProperty;
        private SerializedProperty vibrationEnabledSerializedProperty;

        private SerializedProperty musicAudioClipsProperty;
        
        private GUIContent addMusicButton;

        private IEnumerable<SerializedProperty> soundsProperties;
        private IEnumerable<SerializedProperty> vibrationsProperties;

        protected override void OnEnable()
        {
            base.OnEnable();

            audioEnabledSerializedProperty = serializedObject.FindProperty(audioEnabledPropertyName);
            vibrationEnabledSerializedProperty = serializedObject.FindProperty(vibrationEnabledPropertyName);
            musicEnabledSerializedProperty = serializedObject.FindProperty(musicEnabledPropertyName);

            musicAudioClipsProperty = serializedObject.FindProperty(musicAudioClipsPropertyName);
            
            soundsProperties = serializedObject.FindProperty(soundsPropertyName).GetChildren();
            vibrationsProperties = serializedObject.FindProperty(vibrationsPropertyName).GetChildren();
        }

        protected override void Styles()
        {
            addMusicButton = new GUIContent(EditorStylesExtended.ICON_SPACE + "Add Music Clip", EditorStylesExtended.GetTexture("icon_add", EditorStylesExtended.IconColor));
        }

        public override void OnInspectorGUI()
        {
            InitStyles();

            Rect windowRect = EditorGUILayout.BeginVertical();

            serializedObject.Update();

            EditorStyles.textArea.wordWrap = true;

            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);

            audioEnabledSerializedProperty.boolValue = EditorGUILayoutCustom.HeaderToggle("AUDIO", audioEnabledSerializedProperty.boolValue);

            foreach(SerializedProperty soundProperty in soundsProperties)
            {
                EditorGUILayout.PropertyField(soundProperty);
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(8);
            
            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);

            musicEnabledSerializedProperty.boolValue = EditorGUILayoutCustom.HeaderToggle("MUSIC", musicEnabledSerializedProperty.boolValue);

            if(!musicEnabledSerializedProperty.boolValue)
            {
                EditorGUILayout.HelpBox("Music is disabled!", MessageType.Warning);
            }

            int musicArraySize = musicAudioClipsProperty.arraySize;
            if(musicArraySize > 0)
            {
                for (int i = 0; i < musicArraySize; i++)
                {
                    SerializedProperty arrayElementProperty = musicAudioClipsProperty.GetArrayElementAtIndex(i);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Game Music");
                    EditorGUILayout.ObjectField(arrayElementProperty, GUIContent.none, GUILayout.MinWidth(20));
                    
                    if(GUILayout.Button("X", EditorStylesExtended.button_04_mini, GUILayout.Height(18), GUILayout.Width(18)))
                    {
                        if(EditorUtility.DisplayDialog("Remove music clip", "Are you sure you want to remove music clip?", "Remove", "Cancel"))
                        {
                            musicAudioClipsProperty.RemoveFromObjectArrayAt(i);
                            
                            break;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button(addMusicButton, EditorStylesExtended.button_01, GUILayout.Width(120)))
            {
                musicAudioClipsProperty.arraySize++;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            GUILayout.Space(8);
            
            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);

            EditorGUI.BeginChangeCheck();
            vibrationEnabledSerializedProperty.boolValue = EditorGUILayoutCustom.HeaderToggle("VIBRATION", vibrationEnabledSerializedProperty.boolValue);

            if (!vibrationEnabledSerializedProperty.boolValue)
            {
                EditorGUILayout.HelpBox("Vibration is disabled!", MessageType.Warning);
            }
            
            foreach (SerializedProperty vibrationProperty in vibrationsProperties)
            {
                EditorGUILayout.PropertyField(vibrationProperty, new GUIContent(vibrationProperty.displayName + " (ms)"));
            }
            
            EditorGUILayout.EndVertical();
            
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.EndVertical();

            EditorGUILayoutCustom.DrawCompileWindow(windowRect);
        }
    }
}

// -----------------
// Audio Controller v 0.3.1
// -----------------