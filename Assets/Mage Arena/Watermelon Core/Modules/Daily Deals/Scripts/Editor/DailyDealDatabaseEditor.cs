using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Linq;
using System.Text.RegularExpressions;
using Type = System.Type;
using System.Collections.Generic;

namespace Watermelon
{
    [CustomEditor(typeof(DailyDealDatabase))]
    public class DailyDealDatabaseEditor : WatermelonEditor
    {
        private readonly string PRODUCTS_PROP_NAME = "products";
        private readonly string SETTINGS_PROP_NAME = "settings";

        private SerializedProperty productsProperty;
        private SerializedProperty setingsProperty;

        private IEnumerable<SerializedProperty> setingsContainerProperties;
        private bool displaySettings;

        private Type[] allowedTypes;
        private string[] typeNames;

        private int selectedType;

        private string selectedObjectName;

        private SerializedProperty selectedObject;
        private Editor selectedProductEditor;
        private static int selectedObjectInstanceID = -1;

        private DailyDealDatabase dailyDealsDatabase;

        protected override void OnEnable()
        {
            // Get properties
            productsProperty = serializedObject.FindProperty(PRODUCTS_PROP_NAME);
            setingsProperty = serializedObject.FindProperty(SETTINGS_PROP_NAME);

            setingsContainerProperties = setingsProperty.GetChildren();
            displaySettings = setingsContainerProperties.Count() > 0;

            // Get store product types
            allowedTypes = Assembly.GetAssembly(typeof(DailyDealProduct)).GetTypes().Where(type => type.IsClass && !type.IsAbstract && (type.IsSubclassOf(typeof(DailyDealProduct)) || type.Equals(typeof(DailyDealProduct)))).ToArray();
            typeNames = new string[allowedTypes.Length];

            for (int i = 0; i < allowedTypes.Length; i++)
            {
                typeNames[i] = Regex.Replace(allowedTypes[i].ToString(), "([a-z]) ?([A-Z])", "$1 $2");
            }

            // Cache store database
            dailyDealsDatabase = (DailyDealDatabase)target;
        }

        protected override void Styles()
        {

        }

        public override void OnInspectorGUI()
        {
            InitStyles();

            serializedObject.Update();

            GUILayout.Space(10f);

            if(displaySettings)
            {
                EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);
                EditorGUILayoutCustom.Header("SETTINGS");

                foreach (SerializedProperty prop in setingsContainerProperties)
                {
                    EditorGUILayout.PropertyField(prop);
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);
            EditorGUILayout.BeginHorizontal(EditorStylesExtended.padding05, GUILayout.Height(21), GUILayout.ExpandWidth(true));
            EditorGUILayout.LabelField("PRODUCTS", EditorStylesExtended.boxHeader, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();

            selectedType = EditorGUILayout.Popup("Product Type", selectedType, typeNames);
            EditorGUILayout.BeginHorizontal();
            selectedObjectName = EditorGUILayout.TextField("Object Name", selectedObjectName);
            if (GUILayout.Button("Add", GUILayout.Height(18f)))
            {
                if (!string.IsNullOrEmpty(selectedObjectName))
                {
                    GUI.FocusControl(null);

                    CreateProduct(allowedTypes[selectedType], selectedObjectName);
                }
                else
                {
                    Debug.LogWarning("[Daily Deal]: Object name can't be empty!");
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            //Display objects array box with fixed size
            EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box, GUILayout.ExpandWidth(true));
            int productsCount = productsProperty.arraySize;
            for (int i = 0; i < productsCount; i++)
            {
                int index = i;
                SerializedProperty objectProperty = productsProperty.GetArrayElementAtIndex(i);

                if (objectProperty.objectReferenceValue != null)
                {
                    SerializedObject referenceObject = new SerializedObject(objectProperty.objectReferenceValue);
                    bool isLevelSelected = IsObjectSelected(objectProperty);

                    EditorGUILayout.BeginVertical(EditorStylesExtended.editorSkin.box);

                    Rect clickRect = EditorGUILayout.BeginHorizontal();

                    string title = referenceObject.FindProperty("productName").stringValue;
                    EditorGUILayout.LabelField(string.IsNullOrEmpty(title) ? objectProperty.objectReferenceValue.name.Replace(".asset", "") : title);

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("=", EditorStyles.miniButton, GUILayout.Width(16), GUILayout.Height(16)))
                    {
                        GenericMenu menu = new GenericMenu();

                        int productId = referenceObject.FindProperty("id").intValue;

                        menu.AddItem(new GUIContent("Remove"), false, delegate
                        {
                            if (EditorUtility.DisplayDialog("This product will be removed!", "Are you sure?", "Remove", "Cancel"))
                            {
                                UnselectObject();

                                Object removedObject = objectProperty.objectReferenceValue;

                                productsProperty.RemoveFromObjectArrayAt(index);

                                AssetDatabase.RemoveObjectFromAsset(removedObject);
                                Object.DestroyImmediate(removedObject, true);

                                return;
                            }
                        });

                        menu.AddSeparator("");

                        if (i > 0)
                        {
                            menu.AddItem(new GUIContent("Move up"), false, delegate
                            {
                                productsProperty.MoveArrayElement(index, index - 1);
                                serializedObject.ApplyModifiedProperties();

                                if (selectedObject != null)
                                    UnselectObject();
                            });
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("Move up"));
                        }


                        if (i + 1 < productsCount)
                        {
                            menu.AddItem(new GUIContent("Move down"), false, delegate
                            {
                                productsProperty.MoveArrayElement(index, index + 1);
                                serializedObject.ApplyModifiedProperties();

                                if (selectedObject != null)
                                    UnselectObject();
                            });
                        }
                        else
                        {
                            menu.AddDisabledItem(new GUIContent("Move down"));
                        }

                        menu.ShowAsContext();
                    }

                    GUILayout.Space(5);

                    if (GUI.Button(clickRect, GUIContent.none, GUIStyle.none))
                    {
                        SelectedObject(objectProperty, i);

                        return;
                    }

                    EditorGUILayout.EndHorizontal();

                    if (selectedObject != null && selectedObjectInstanceID != -1 && selectedObjectInstanceID == objectProperty.objectReferenceInstanceIDValue)
                    {
                        GUILayout.Space(3);
                        EditorGUILayout.LabelField(GUIContent.none, EditorStylesExtended.editorSkin.horizontalSlider);
                        GUILayout.Space(-10);

                        EditorGUILayout.BeginVertical();

                        if (selectedProductEditor == null)
                            Editor.CreateCachedEditor(selectedObject.objectReferenceValue, null, ref selectedProductEditor);

                        selectedProductEditor.OnInspectorGUI();

                        EditorGUILayout.EndVertical();
                    }

                    EditorGUILayout.EndVertical();
                }
            }

            if (productsCount == 0)
            {
                EditorGUILayout.HelpBox("Database is empty, add products first!", MessageType.Warning);
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void UnselectObject()
        {
            GUI.FocusControl(null);

            if (selectedProductEditor != null)
                DestroyImmediate(selectedProductEditor, true);
            selectedProductEditor = null;

            selectedObject = null;
            selectedObjectInstanceID = -1;
        }

        private void SelectedObject(SerializedProperty serializedProperty, int index)
        {
            GUI.FocusControl(null);

            if (selectedProductEditor != null)
                DestroyImmediate(selectedProductEditor, true);
            selectedProductEditor = null;

            //Check if current selected object is equals to new and unselect it
            if (selectedObject != null && selectedObject.objectReferenceInstanceIDValue == serializedProperty.objectReferenceInstanceIDValue)
            {
                selectedObject = null;
                selectedObjectInstanceID = -1;

                return;
            }

            if (serializedProperty != null)
            {
                selectedObjectInstanceID = serializedProperty.objectReferenceInstanceIDValue;
                selectedObject = serializedProperty;
            }
        }

        private bool IsObjectSelected(SerializedProperty serializedProperty)
        {
            return selectedObject != null && selectedObjectInstanceID == serializedProperty.objectReferenceInstanceIDValue;
        }

        private int GetUniqueProductId()
        {
            if (dailyDealsDatabase.Products != null && dailyDealsDatabase.Products.Length > 0)
            {
                return dailyDealsDatabase.Products.Max(x => x.ID) + 1;
            }
            else
            {
                return 1;
            }
        }

        private void CreateProduct(Type type, string name)
        {
            if (type != typeof(DailyDealProduct))
            {
                Debug.LogError("[Daily Deal]: Product type should be subclass of Daily Deal Product class!");

                return;
            }

            //if (!type.IsSubclassOf(typeof(DailyDealProduct)))
            //{
            //    Debug.LogError("[Daily Deal]: Product type should be subclass of Daily Deal Product class!");

            //    return;
            //}

            serializedObject.Update();

            productsProperty.arraySize++;

            int productUniqueID = GetUniqueProductId();

            DailyDealProduct tempProduct = (DailyDealProduct)ScriptableObject.CreateInstance(type);
            tempProduct.name = name.Replace(" ", "") + "Product" + productUniqueID.ToString("000");
            tempProduct.hideFlags = HideFlags.HideInHierarchy;

            AssetDatabase.AddObjectToAsset(tempProduct, target);
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(tempProduct));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            productsProperty.GetArrayElementAtIndex(productsProperty.arraySize - 1).objectReferenceValue = tempProduct;

            serializedObject.ApplyModifiedProperties();

            SerializedObject tempProductSerializedObject = new SerializedObject(tempProduct);
            tempProductSerializedObject.Update();
            tempProductSerializedObject.FindProperty("productName").stringValue = name;
            tempProductSerializedObject.FindProperty("id").intValue = productUniqueID;
            tempProductSerializedObject.ApplyModifiedProperties();

            selectedObjectName = "";
        }
    }
}
