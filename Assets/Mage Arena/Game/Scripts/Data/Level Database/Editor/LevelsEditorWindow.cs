#pragma warning disable 649, 168

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using System;
using UnityEngine.UIElements;
using Watermelon;
using UnityEditorInternal;

[InitializeOnLoadAttribute]
public class LevelsEditorWindow : EditorWindow
{
    private const string MENU_ITEM_PATH = "Tools/Levels Editor";
    private const string EDITOR_WINDOW_TITLE = "Levels Editor Window";
    private const string OBSTACLE_FOLDER_PATH = "Assets/" + ApplicationConsts.PROJECT_FOLDER + "/Content/Level Database/Obstacles/";
    private const string ENEMIES_FOLDER_PATH = "Assets/" + ApplicationConsts.PROJECT_FOLDER + "/Content/Level Database/Enemies/";
    private const string LEVELS_FOLDER_PATH = "Assets/" + ApplicationConsts.PROJECT_FOLDER + "/Content/Level Database/Levels/";

    private readonly Vector2Int LEVEL_MIN_SIZE = new Vector2Int(10, 10);
    private readonly Vector2Int LEVEL_MAX_SIZE = new Vector2Int(50, 50);

    private const float START_OFFSET = 10;
    private const float LINE_WIDTH = 2f;
    private const float SPRITE_ELEMENT_SIZE = 32;

    private static LevelsEditorWindow window;
    private LevelDatabase levelDatabase;
    private StringBuilder stringBuilder;
    private string[] tabsNames = { "Obstacles", "Levels", "Editor textures" };
    private Vector2 globalScrollVector;
    private int selectedTabIndex;
    private bool elementInfosUpdated;
    private int selectedObstacleIndex;
    private int selectedEnemyIndex;

    private SerializedObject levelDatabaseSerializedObject;
    private SerializedProperty levelsSerializedProperty;
    private SerializedProperty obstaclesSerializedProperty;
    private SerializedProperty enemySerializedProperty;
    private SerializedProperty elementSerializedProperty;
    private SerializedProperty textureSerializedProperty;
    private SerializedProperty selectedObstacleSerializedProperty;
    private SerializedProperty roomSizeSerializedProperty;
    private SerializedProperty roomObstaclesSerializedProperty;
    private SerializedProperty roomEnemySerializedProperty;
    private SerializedProperty roomIsBossRoomSerializedProperty;
    private SerializedObject selectedObstacleSerializedObject;
    private SerializedObject selectedLevelSerializedObject;
    private SerializedProperty levelEnvironmentSkinSerializedProperty;
    private SerializedProperty levelRoomsSerializedProperty;
    private bool objectIsNull;
    private SerializedObject selectedEnemySerializedObject;
    private bool levelsDatabaseInitialized;


    private Vector2Int spriteSize;

    private Rect clickRect;
    private Texture2D placeholderTexture;
    private Texture2D texture;
    private Rect textureFieldRect;

    private Vector2 rectPosition;
    private Vector2 draggedItemGridOffset;
    private Vector2Int itemUIGridPosition;
    private Vector2Int itemLevelGridPosition;
    private int itemAngle;
    private ElementInfo itemInfo;
    private Vector2 itemPosition;
    private Rect textureRect;
    private Rect fullRect;
    private Rect texturePointRect;
    private Matrix4x4 matrixBackup;
    private bool needToFixTexture;
    private Event currentEvent;
    private Type texture2DType;
    private float startPosX;
    private float startPosY;
    private float maxHeight;
    private float margin;
    private string newObstacleName;
    private string newEnemyName;
    ScaleMode scaleMode;
    private int textureRotationAngle;
    private SerializedProperty horizontalOffsetSerializedProperty;
    private SerializedProperty verticalOffsetSerializedProperty;
    private Rect groupRect;
    private int levelCellSize;
    private int elementsCellSize;
    private ElementInfo[] obstaleInfos;
    private ElementInfo[] enemyInfos;
    private ElementInfo draggedElementInfo;
    private SerializedProperty draggedLevelItem;
    private int draggedLevelItemIndex;
    private int draggetItemAngle;
    private ItemType draggetItemType;
    private Vector2Int draggetItemUIGridPosition;
    private Texture2D greenTexture;
    private Texture2D redTexture;
    private Texture2D itemBackgroundTexture;
    private bool draggetItemSnapActive;
    private bool draggetItemDropAcceptable;
    private List<Vector2Int> filledGridCells;
    private List<Vector2Int> draggetItemGridCells;
    private ReorderableList levelsReordableList;
    private bool levelItemIsDragged;
    private Obstacle draggetElementObstacleReference;
    private Enemy draggetElementEnemyReference;
    private Obstacle itemObstacleReference;
    private Enemy itemEnemyReference;
    private float levelGridPositionY;
    private float levelGridPositionX;
    private Vector2Int gridSize;
    private Color bakupBackgroundColorOfGUI;
    private Rect windowRect;
    private Vector2 secondRotatingPoint;
    private Vector2 firstRotatingPoint;
    private Rect tempTextureRect;
    private int selectedLevelIndex;
    private int selectedRoomIndex;
    private Rect levelsListClickRect;
    private Rect roomClickRect;

    [MenuItem(MENU_ITEM_PATH)]
    public static void Init()
    {
        window = EditorWindow.GetWindow<LevelsEditorWindow>(EDITOR_WINDOW_TITLE);
        window.minSize = new Vector2(600, 400);
        window.Show();
    }

    private void OnEnable()
    {
        levelDatabase = EditorUtils.GetAsset<LevelDatabase>();
        //storeDatabase = EditorUtils.GetAsset<StoreDatabase>(); // Used to setup prefabs for skins in Enemies
        levelsDatabaseInitialized = false;

        if (levelDatabase == null)
        {
            Debug.LogError("LevelDatabase is missing. Please create level database in content folder");
            return;
        }

        //if (storeDatabase == null)
        //{
        //    Debug.LogError("StoreDatabase is missing. Please create store database in content folder");
        //    return;
        //}

        LoadSerializedProperties();
        LoadEditorTextures();
        InitVariables();
        
    }

    private void LoadSerializedProperties()
    {
        levelDatabaseSerializedObject = new SerializedObject(levelDatabase);
        levelsSerializedProperty = levelDatabaseSerializedObject.FindProperty("levels");
        obstaclesSerializedProperty = levelDatabaseSerializedObject.FindProperty("obstacles");
        enemySerializedProperty = levelDatabaseSerializedObject.FindProperty("enemies");
        levelsDatabaseInitialized = true;
    }

    private void LoadEditorTextures()
    {
        placeholderTexture = (Texture2D)levelDatabaseSerializedObject.FindProperty("placeholderTexture").objectReferenceValue;
        greenTexture = (Texture2D)levelDatabaseSerializedObject.FindProperty("greenTexture").objectReferenceValue;
        redTexture = (Texture2D)levelDatabaseSerializedObject.FindProperty("redTexture").objectReferenceValue;
        itemBackgroundTexture = (Texture2D)levelDatabaseSerializedObject.FindProperty("itemBackgroundTexture").objectReferenceValue;
        
    }

    private void InitVariables()
    {
        stringBuilder = new StringBuilder();
        selectedObstacleIndex = -1;
        selectedEnemyIndex = -1;
        texture2DType = placeholderTexture.GetType();
        selectedTabIndex = 0;
        elementInfosUpdated = false;
        filledGridCells = new List<Vector2Int>();
        draggetItemGridCells = new List<Vector2Int>();
        levelCellSize = 16;
        elementsCellSize = 16;
        newObstacleName = "Obstacle_1x1";
        newEnemyName = "Enemy_1x1";
        selectedLevelIndex = -1;
        selectedRoomIndex = -1;
    }
    private void OnGUI()
    {
        if (!levelsDatabaseInitialized)
        {
            return;
        }

        EditorGUILayout.BeginVertical();
        globalScrollVector = EditorGUILayout.BeginScrollView(globalScrollVector);

        using (new EditorGUI.DisabledScope(levelItemIsDragged))
        {
            selectedTabIndex = GUILayout.Toolbar(selectedTabIndex, tabsNames);
        }

        HandleElementInfos();

        switch (selectedTabIndex)
        {
            case 0:
                DisplayObstaclesTab();
                break;
            case 1:
                DisplayLevelsTab();
                break;
            case 2:
                DisplayEditorConfigurationTab();
                break;
            default:
                break;
        }

        HandleDragOfLevelItem();

        EditorGUILayout.EndScrollView();
        EditorGUILayout.EndVertical();

        levelDatabaseSerializedObject.ApplyModifiedProperties();
    }



    private void Update()
    {

        if (levelItemIsDragged)
        {
            Repaint();
        }

    }

    private void HandleElementInfos()
    {
        if (elementInfosUpdated)
        {
            if (selectedTabIndex == 0)
            {
                elementInfosUpdated = false;
            }
        }
        else
        {
            if (selectedTabIndex != 0)
            {
                UpdateElementInfos();
                elementInfosUpdated = true;
            }
        }
    }



    #region ObstacleTab

    private void DisplayObstaclesTab()
    {
        EditorGUILayout.BeginVertical();

        //obstacles
        EditorGUILayout.LabelField("Obstacles");

        newObstacleName = EditorGUILayout.TextField("New object filename", newObstacleName);

        if (GUILayout.Button("Add obstacle"))
        {
            AddObstacle();
        }

        EditorGUILayout.Space();

        for (int i = 0; i < obstaclesSerializedProperty.arraySize; i++)
        {

            if (selectedObstacleIndex == i)
            {
                DisplaySelectedObstacle(i, selectedObstacleSerializedObject, obstaclesSerializedProperty, "prefab", ItemType.Obstacle);
            }
            else
            {
                DisplayUnselectedObstacle(i, obstaclesSerializedProperty);
            }

            if (GUI.Button(clickRect, GUIContent.none, GUIStyle.none))
            {
                HandleObstacleSelection(i);
            }
        }

        //Enemies

        EditorGUILayout.LabelField("Enemies");

        newEnemyName = EditorGUILayout.TextField("New object filename", newEnemyName);

        if (GUILayout.Button("Add enemy"))
        {
            AddEnemy();
        }

        EditorGUILayout.Space();

        for (int i = 0; i < enemySerializedProperty.arraySize; i++)
        {

            if (selectedEnemyIndex == i)
            {
                DisplaySelectedObstacle(i, selectedEnemySerializedObject, enemySerializedProperty, "prefab", ItemType.Enemies);
            }
            else
            {
                DisplayUnselectedObstacle(i, enemySerializedProperty);
            }

            if (GUI.Button(clickRect, GUIContent.none, GUIStyle.none))
            {
                HandleEnemySelection(i);
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("You can open elements for editing by clicking on them", MessageType.Info);
        EditorGUILayout.EndVertical();
    }

    private void AddObstacle()
    {
        Obstacle obstacle = Obstacle.CreateInstance<Obstacle>();
        obstacle.name = newObstacleName;

        AssetDatabase.CreateAsset(obstacle, OBSTACLE_FOLDER_PATH + newObstacleName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        obstaclesSerializedProperty.arraySize++;
        obstaclesSerializedProperty.GetArrayElementAtIndex(obstaclesSerializedProperty.arraySize - 1).objectReferenceValue = obstacle;
    }
    private void AddEnemy()
    {
        Enemy Enemy = Enemy.CreateInstance<Enemy>();
        Enemy.name = newEnemyName;

        AssetDatabase.CreateAsset(Enemy, ENEMIES_FOLDER_PATH + newEnemyName + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        enemySerializedProperty.arraySize++;
        enemySerializedProperty.GetArrayElementAtIndex(enemySerializedProperty.arraySize - 1).objectReferenceValue = Enemy;
    }

    private void DisplaySelectedObstacle(int index, SerializedObject selectedSerializedObject, SerializedProperty group, string prefabPropertyName, ItemType itemType)
    {
        clickRect = EditorGUILayout.BeginVertical(GUI.skin.box);

        if (objectIsNull)
        {
            EditorGUILayout.LabelField("Can`t edit null object.");
        }
        else
        {
            EditorGUILayout.PropertyField(selectedSerializedObject.FindProperty(prefabPropertyName));

            if(itemType == ItemType.Enemies)
            {
                EditorGUILayout.PropertyField(selectedSerializedObject.FindProperty("maxHP"));
                EditorGUILayout.PropertyField(selectedSerializedObject.FindProperty("isBoss"));
            }

            /*
            if (itemType == ItemType.Obstacle)
            {
                DisplayObstacleSkinArray(selectedSerializedObject.FindProperty(prefabPropertyName));
                //EditorGUILayout.PropertyField(selectedSerializedObject.FindProperty(prefabPropertyName));
            }
            else
            {
                DisplayMovableSkinArray(selectedSerializedObject.FindProperty(prefabPropertyName));
            }
            */

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(selectedSerializedObject.FindProperty("size"), new GUIContent("object size"));
            spriteSize = selectedSerializedObject.FindProperty("size").vector2IntValue;
            DisplayEditorTextureField(selectedSerializedObject.FindProperty("editorTexture"), spriteSize, SPRITE_ELEMENT_SIZE);
            GUILayout.Space(spriteSize.y * (SPRITE_ELEMENT_SIZE + margin) + 20);

            if (spriteSize.Equals(Vector2Int.zero))
            {
                EditorGUILayout.HelpBox("Please set size of object", MessageType.Info);
            }
            else if (selectedSerializedObject.FindProperty("editorTexture").FindPropertyRelative("texture").objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("Please drag texture to texture field", MessageType.Info);
            }

            selectedSerializedObject.ApplyModifiedProperties();


            if (GUILayout.Button("Delete"))
            {
                HandleRemove(group, index);
            }
        }


        EditorGUILayout.EndVertical();
    }
    /*
    private void DisplayMovableSkinArray(SerializedProperty array)
    {
        array.arraySize = movableProducts.Count;
        EditorGUILayout.LabelField("Skin prefabs:");

        for (int i = 0; i < movableProducts.Count; i++)
        {
            stringBuilder.Clear();
            stringBuilder.Append("Id: ");
            stringBuilder.Append(movableProducts[i].ID);
            stringBuilder.Append(" | ");
            stringBuilder.Append(movableProducts[i].ProductName);
            EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), new GUIContent(stringBuilder.ToString()));
        }
    }

    private void DisplayObstacleSkinArray(SerializedProperty array)
    {
        array.arraySize = environmentProducts.Count;
        EditorGUILayout.LabelField("Skin prefabs:");

        for (int i = 0; i < environmentProducts.Count; i++)
        {
            stringBuilder.Clear();
            stringBuilder.Append("Id: ");
            stringBuilder.Append(environmentProducts[i].ID);
            stringBuilder.Append(" | ");
            stringBuilder.Append(environmentProducts[i].ProductName);
            EditorGUILayout.PropertyField(array.GetArrayElementAtIndex(i), new GUIContent(stringBuilder.ToString()));
        }
    }
    */
    private void DisplayUnselectedObstacle(int index, SerializedProperty group)
    {
        clickRect = EditorGUILayout.BeginHorizontal(GUI.skin.box);
        EditorGUILayout.PropertyField(group.GetArrayElementAtIndex(index));

        if (GUILayout.Button("Delete", GUILayout.MaxWidth(60f)))
        {
            HandleRemove(group, index);
        }

        EditorGUILayout.EndHorizontal();
    }

    private void HandleObstacleSelection(int index)
    {
        if (selectedObstacleIndex == index)
        {
            selectedObstacleIndex = -1;
            selectedEnemyIndex = -1;
        }
        else
        {
            if (obstaclesSerializedProperty.GetArrayElementAtIndex(index).objectReferenceValue == null)
            {
                objectIsNull = true;
            }
            else
            {
                objectIsNull = false;
                selectedObstacleSerializedObject = new SerializedObject((Obstacle)obstaclesSerializedProperty.GetArrayElementAtIndex(index).objectReferenceValue);
            }

            //UpdateStoreProductsList();
            selectedObstacleIndex = index;
            selectedEnemyIndex = -1;
        }
    }

    private void HandleEnemySelection(int index)
    {
        if (selectedEnemyIndex == index)
        {
            selectedObstacleIndex = -1;
            selectedEnemyIndex = -1;
        }
        else
        {
            if (enemySerializedProperty.GetArrayElementAtIndex(index).objectReferenceValue == null)
            {
                objectIsNull = true;
            }
            else
            {
                objectIsNull = false;
                selectedEnemySerializedObject = new SerializedObject((Enemy)enemySerializedProperty.GetArrayElementAtIndex(index).objectReferenceValue);
            }

            //UpdateStoreProductsList();
            selectedEnemyIndex = index;
            selectedObstacleIndex = -1;
        }

    }

    /*private void UpdateStoreProductsList()
    {
        movableProducts.Clear();
        environmentProducts.Clear();

        foreach (StoreProduct product in storeDatabase.Products)
        {
            if (product.BehaviourType == BehaviourType.Default)
            {
                if (product.Type == StoreProductType.CharacterSkin)
                {
                    movableProducts.Add(product);
                }
                else
                {
                    environmentProducts.Add(product);
                }

            }
        }


    }*/

    private void DisplayEditorTextureField(SerializedProperty editorTexture, Vector2Int fieldSize, float elementSize)
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("TextureField:");
        textureSerializedProperty = editorTexture.FindPropertyRelative("texture");
        horizontalOffsetSerializedProperty = editorTexture.FindPropertyRelative("horizontalOffset");
        verticalOffsetSerializedProperty = editorTexture.FindPropertyRelative("verticalOffset");

        EditorGUILayout.PropertyField(horizontalOffsetSerializedProperty);
        EditorGUILayout.PropertyField(verticalOffsetSerializedProperty);


        textureFieldRect = EditorGUILayout.BeginVertical();

        textureRect = new Rect(START_OFFSET + textureFieldRect.xMin, START_OFFSET + textureFieldRect.yMin, elementSize * fieldSize.x, elementSize * fieldSize.y);
        //handle drag and drop of texture
        currentEvent = Event.current;

        if (textureRect.Contains(currentEvent.mousePosition))
        {
            if (currentEvent.type == EventType.DragUpdated)
            {
                if ((DragAndDrop.objectReferences.Length == 1) && (DragAndDrop.objectReferences[0].GetType() == texture2DType))
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
                else
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
                }

                currentEvent.Use();
            }
            else if (currentEvent.type == EventType.DragPerform)
            {
                if ((DragAndDrop.objectReferences.Length == 1) && (DragAndDrop.objectReferences[0].GetType() == texture2DType))
                {
                    textureSerializedProperty.objectReferenceValue = DragAndDrop.objectReferences[0];
                }

                currentEvent.Use();
            }
        }



        if (textureSerializedProperty.objectReferenceValue == null)
        {
            GUI.DrawTexture(textureRect, placeholderTexture);
        }
        else
        {
            textureRect = new Rect(START_OFFSET + textureFieldRect.xMin + horizontalOffsetSerializedProperty.floatValue,
                START_OFFSET + textureFieldRect.yMin + horizontalOffsetSerializedProperty.floatValue,
                elementSize * fieldSize.x - (horizontalOffsetSerializedProperty.floatValue * 2f),
                elementSize * fieldSize.y - (verticalOffsetSerializedProperty.floatValue * 2f));

            texture = (Texture2D)textureSerializedProperty.objectReferenceValue;

            GUI.DrawTexture(textureRect, texture, ScaleMode.ScaleToFit);

        }

        DrawGrid(textureFieldRect.xMin + START_OFFSET, textureFieldRect.yMin + START_OFFSET, spriteSize, SPRITE_ELEMENT_SIZE);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();
    }



    #endregion


    private void DrawGrid(float startX, float startY, Vector2Int size, float cellSize)
    {
        tempTextureRect = new Rect(startX, startY, LINE_WIDTH, cellSize * size.y);

        for (int i = 0; i <= size.x; i++)
        {
            GUI.DrawTexture(tempTextureRect, Texture2D.whiteTexture);
            tempTextureRect.x += cellSize;
        }

        tempTextureRect = new Rect(startX, startY, cellSize * size.x, LINE_WIDTH);

        for (int i = 0; i <= size.y; i++)
        {
            GUI.DrawTexture(tempTextureRect, Texture2D.whiteTexture);
            tempTextureRect.y += cellSize;
        }
    }

    private void HandleRemove(SerializedProperty arrayProperty,int index)
    {
        UnityEngine.Object removable = arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue;
        arrayProperty.RemoveFromObjectArrayAt(index);

        if (removable != null)
        {
            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(removable));
            AssetDatabase.Refresh();
        }
    }

    #region levels tab

    #region levels operations

    private string GetLevelName(int index)
    {
        stringBuilder.Clear();
        stringBuilder.Append("#");
        stringBuilder.Append(index + 1);
        stringBuilder.Append("| ");

        if (levelsSerializedProperty.GetArrayElementAtIndex(index).objectReferenceValue == null)
        {
            stringBuilder.Append("[Null file]");
        }
        else
        {
            stringBuilder.Append(levelsSerializedProperty.GetArrayElementAtIndex(index).objectReferenceValue.name);
        }

        return stringBuilder.ToString();
    }

    private void AddLevel()
    {
        selectedLevelIndex = -1;
        selectedRoomIndex = -1;

        int maxNumber = 0;
        int levelNumber;
        string name;

        for (int i = 0; i < levelsSerializedProperty.arraySize; i++)
        {
            if (levelsSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue != null)
            {
                name = levelsSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue.name;
                levelNumber = Int32.Parse(name.Substring(6));

                if (levelNumber > maxNumber)
                {
                    maxNumber = levelNumber;
                }
            }
        }

        maxNumber++;
        name = "Level " + maxNumber;


        Level level = Level.CreateInstance<Level>();
        level.name = name;

        AssetDatabase.CreateAsset(level, LEVELS_FOLDER_PATH + name + ".asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        levelsSerializedProperty.arraySize++;
        levelsSerializedProperty.GetArrayElementAtIndex(levelsSerializedProperty.arraySize - 1).objectReferenceValue = level;
    }
    #endregion

    private void DisplayLevelsList()
    {
        for (int i = 0; i < levelsSerializedProperty.arraySize; i++)
        {
            levelsListClickRect = EditorGUILayout.BeginVertical(GUI.skin.box);

            if(selectedLevelIndex == i)
            {
                DisplaySelectedLevel(i);
            }
            else
            {
                DisplayUnselectedLevel(i);
            }

            

            EditorGUILayout.EndVertical();
            GUILayout.Space(4);

            //handle selection
            if (GUI.Button(levelsListClickRect, GUIContent.none, GUIStyle.none))
            {
                if (selectedLevelIndex == i)
                {
                    selectedLevelIndex = -1;
                }
                else
                {
                    if(levelsSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue != null)
                    {
                        selectedLevelSerializedObject = new SerializedObject((Level)levelsSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                        levelEnvironmentSkinSerializedProperty = selectedLevelSerializedObject.FindProperty("environmentSkin");
                        levelRoomsSerializedProperty = selectedLevelSerializedObject.FindProperty("rooms");
                        selectedLevelIndex = i;

                    }
                    else
                    {
                        selectedLevelIndex = -1;
                    }

                                      
                }

                selectedRoomIndex = -1;
            }

        }
    }

    private void DisplaySelectedLevel(int index)
    {
        EditorGUILayout.LabelField(GetLevelName(index), EditorStyles.boldLabel, GUILayout.MaxWidth(86));
        EditorGUILayout.LabelField("Rooms:", GUILayout.MaxWidth(86));

        for (int roomIndex = 0; roomIndex < levelRoomsSerializedProperty.arraySize; roomIndex++)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider,GUILayout.MaxWidth(170));
            DisplayRoom(roomIndex);
        }

        if (levelRoomsSerializedProperty.arraySize > 0)
        {
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider, GUILayout.MaxWidth(170));
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Add room"))
        {
            levelRoomsSerializedProperty.arraySize++;
            selectedRoomIndex = -1;
            SerializedProperty room = levelRoomsSerializedProperty.GetArrayElementAtIndex(levelRoomsSerializedProperty.arraySize - 1);
            room.FindPropertyRelative("size").vector2IntValue = new Vector2Int(10, 10);
            room.FindPropertyRelative("obstacles").arraySize = 0;
            room.FindPropertyRelative("enemies").arraySize = 0;
            room.FindPropertyRelative("isBossRoom").boolValue = false;

            selectedLevelSerializedObject.ApplyModifiedProperties();
        }

        if(GUILayout.Button("Copy room"))
        {
            CreateCopyContextMenu();
        }

        if (GUILayout.Button("Delete level"))
        {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to remove level #" + (index + 1) + " ?", "ok", "cancel"))
            {
                selectedLevelIndex = -1;
                selectedRoomIndex = -1;
                HandleRemove(levelsSerializedProperty, index);
            }
        }
    }

    private void CreateCopyContextMenu()
    {
        GenericMenu copyMenu = new GenericMenu();
        SerializedProperty rooms;
        SerializedObject level;
        string menuPath;

        for (int levelIndex = 0; levelIndex < levelsSerializedProperty.arraySize; levelIndex++)
        {
            if (levelsSerializedProperty.GetArrayElementAtIndex(levelIndex).objectReferenceValue != null)
            {
                level = new SerializedObject(levelsSerializedProperty.GetArrayElementAtIndex(levelIndex).objectReferenceValue);
                rooms = level.FindProperty("rooms");

                if (rooms.arraySize > 0)
                {
                    for (int roomIndex = 0; roomIndex < rooms.arraySize; roomIndex++)
                    {
                        menuPath = String.Format("Level {0}/Copy room {1}", (levelIndex + 1), (roomIndex + 1));
                        copyMenu.AddItem(new GUIContent(menuPath), false, OnCopyMenuClick, rooms.GetArrayElementAtIndex(roomIndex));
                    }
                }
            }
        }

        copyMenu.ShowAsContext();
    }

    void OnCopyMenuClick(object roomObject)
    {
        SerializedProperty roomToCopy = (SerializedProperty)roomObject;
        levelRoomsSerializedProperty.arraySize++;
        selectedRoomIndex = -1;

        

        SerializedProperty room = levelRoomsSerializedProperty.GetArrayElementAtIndex(levelRoomsSerializedProperty.arraySize - 1);
        room.FindPropertyRelative("size").vector2IntValue = roomToCopy.FindPropertyRelative("size").vector2IntValue;
        room.FindPropertyRelative("isBossRoom").boolValue = roomToCopy.FindPropertyRelative("isBossRoom").boolValue;
        room.FindPropertyRelative("obstacles").arraySize = roomToCopy.FindPropertyRelative("obstacles").arraySize;
        room.FindPropertyRelative("enemies").arraySize = roomToCopy.FindPropertyRelative("enemies").arraySize;

        for (int i = 0; i < room.FindPropertyRelative("obstacles").arraySize; i++)
        {
            room.FindPropertyRelative("obstacles").GetArrayElementAtIndex(i).FindPropertyRelative("obstacle").objectReferenceValue = 
                roomToCopy.FindPropertyRelative("obstacles").GetArrayElementAtIndex(i).FindPropertyRelative("obstacle").objectReferenceValue;
            room.FindPropertyRelative("obstacles").GetArrayElementAtIndex(i).FindPropertyRelative("position").vector2IntValue =
                roomToCopy.FindPropertyRelative("obstacles").GetArrayElementAtIndex(i).FindPropertyRelative("position").vector2IntValue;
            room.FindPropertyRelative("obstacles").GetArrayElementAtIndex(i).FindPropertyRelative("angle").intValue =
                roomToCopy.FindPropertyRelative("obstacles").GetArrayElementAtIndex(i).FindPropertyRelative("angle").intValue;
        }

        for (int i = 0; i < room.FindPropertyRelative("enemies").arraySize; i++)
        {
            room.FindPropertyRelative("enemies").GetArrayElementAtIndex(i).FindPropertyRelative("enemy").objectReferenceValue =
                roomToCopy.FindPropertyRelative("enemies").GetArrayElementAtIndex(i).FindPropertyRelative("enemy").objectReferenceValue;
            room.FindPropertyRelative("enemies").GetArrayElementAtIndex(i).FindPropertyRelative("position").vector2IntValue =
                roomToCopy.FindPropertyRelative("enemies").GetArrayElementAtIndex(i).FindPropertyRelative("position").vector2IntValue;
            room.FindPropertyRelative("enemies").GetArrayElementAtIndex(i).FindPropertyRelative("angle").intValue =
                roomToCopy.FindPropertyRelative("enemies").GetArrayElementAtIndex(i).FindPropertyRelative("angle").intValue;
        }

        selectedLevelSerializedObject.ApplyModifiedProperties();
    }

    private void DisplayRoom(int roomIndex)
    {
        //GUILayout.Space(4);

        roomClickRect = EditorGUILayout.BeginHorizontal();

        GUILayout.Space(20);
        EditorGUILayout.LabelField("Room #"+(roomIndex+1), GUILayout.MaxWidth(86));

        GUILayout.FlexibleSpace();

        if (roomIndex > 0)
        {
            if (GUILayout.Button("↑", GUILayout.MaxWidth(20)))
            {
                levelRoomsSerializedProperty.MoveArrayElement(roomIndex, roomIndex - 1);
                selectedLevelSerializedObject.ApplyModifiedProperties();
            }
        }
        if (roomIndex < levelRoomsSerializedProperty.arraySize - 1)
        {
            if (GUILayout.Button("↓", GUILayout.MaxWidth(20)))
            {
                levelRoomsSerializedProperty.MoveArrayElement(roomIndex, roomIndex + 1);
                selectedLevelSerializedObject.ApplyModifiedProperties();
            }
        }

        if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
        {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to remove room #" + (roomIndex + 1) + " ?", "ok", "cancel"))
            {
                selectedRoomIndex = -1;
                levelRoomsSerializedProperty.RemoveFromVariableArrayAt(roomIndex);
                selectedLevelSerializedObject.ApplyModifiedProperties();
            }
        }

        EditorGUILayout.EndHorizontal();

        if (GUI.Button(roomClickRect, GUIContent.none, GUIStyle.none))
        {
            if(selectedRoomIndex != roomIndex)
            {
                roomSizeSerializedProperty = levelRoomsSerializedProperty.GetArrayElementAtIndex(roomIndex).FindPropertyRelative("size");
                roomObstaclesSerializedProperty = levelRoomsSerializedProperty.GetArrayElementAtIndex(roomIndex).FindPropertyRelative("obstacles");
                roomEnemySerializedProperty = levelRoomsSerializedProperty.GetArrayElementAtIndex(roomIndex).FindPropertyRelative("enemies");
                roomIsBossRoomSerializedProperty = levelRoomsSerializedProperty.GetArrayElementAtIndex(roomIndex).FindPropertyRelative("isBossRoom");
                selectedRoomIndex = roomIndex;
            }            
        }
    }

    private void DisplayUnselectedLevel(int index)
    {
        EditorGUILayout.BeginHorizontal(GUILayout.Width(180));
        EditorGUILayout.LabelField(GetLevelName(index), EditorStyles.boldLabel, GUILayout.MaxWidth(86));

        GUILayout.FlexibleSpace();

        if (index > 0)
        {
            if (GUILayout.Button("↑", GUILayout.MaxWidth(20)))
            {
                levelsSerializedProperty.MoveArrayElement(index, index - 1);
            }
        }
        if (index < levelsSerializedProperty.arraySize -1)
        {
            if (GUILayout.Button("↓", GUILayout.MaxWidth(20)))
            {
                levelsSerializedProperty.MoveArrayElement(index, index + 1);
            }
        }

        if (GUILayout.Button("X", GUILayout.MaxWidth(20)))
        {
            if (EditorUtility.DisplayDialog("Warning", "Are you sure you want to remove level #" + (index + 1) + " ?", "ok", "cancel"))
            {
                selectedLevelIndex = -1;
                selectedRoomIndex = -1;

                HandleRemove(levelsSerializedProperty, index);
            }
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DisplayLevelsTab()
    {

        EditorGUILayout.BeginHorizontal();

        using (new EditorGUI.DisabledScope(levelItemIsDragged))
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(180),GUILayout.MaxWidth(180));
            //levelListScrollVector = EditorGUILayout.BeginScrollView(levelListScrollVector, GUILayout.MaxWidth(180));

            //levelsReordableList.DoLayoutList();
            DisplayLevelsList();
            EditorGUILayout.Space();

            if (GUILayout.Button("Add level"))
            {
                AddLevel();
            }

            if (GUILayout.Button("Rename levels"))
            {
                RenameLevels();
            }

            //EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(START_OFFSET);

        //level edit area
        EditorGUILayout.BeginVertical();
        DisplayWarningAboutMissingTextures();
        
        if(selectedLevelIndex != -1)
        {
            EditorGUILayout.PropertyField(levelsSerializedProperty.GetArrayElementAtIndex(selectedLevelIndex), new GUIContent("Level #" + (selectedLevelIndex + 1)));
            EditorGUILayout.PropertyField(levelEnvironmentSkinSerializedProperty);

            if(selectedRoomIndex != -1)
            {
                levelCellSize = EditorGUILayout.IntField("levelCellSize", levelCellSize);
                elementsCellSize = EditorGUILayout.IntField("elementsCellSize", elementsCellSize);
                EditorGUILayout.Space();

                EditorGUILayout.BeginVertical();

                EditorGUILayout.EndVertical();
                DisplayGridSize(roomSizeSerializedProperty, LEVEL_MIN_SIZE, LEVEL_MAX_SIZE);
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(roomIsBossRoomSerializedProperty);
                EditorGUILayout.LabelField(levelsSerializedProperty.GetArrayElementAtIndex(selectedLevelIndex).objectReferenceValue.name + "-Room #"+(selectedRoomIndex +1));
                DrawLevelGrid();

                DrawElementInfoArray("Obstacles", obstaleInfos, elementsCellSize, ItemType.Obstacle);
                DrawElementInfoArray("Enemies", enemyInfos, elementsCellSize, ItemType.Enemies);
                DisplayLevelTutorialInfo();

                selectedLevelSerializedObject.ApplyModifiedProperties();
            }


        }
        
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();


    }

    

    private void DisplayLevelTutorialInfo()
    {
        EditorGUILayout.HelpBox("Use left click to select object", MessageType.Info);
        EditorGUILayout.HelpBox("Use left click to place selected object", MessageType.Info);
        EditorGUILayout.HelpBox("Use right click to rotate selected object", MessageType.Info);
        EditorGUILayout.HelpBox("Place object outside of grid to remove object from grid", MessageType.Info);
        EditorGUILayout.HelpBox("All changes save automatically", MessageType.Info);
        EditorGUILayout.HelpBox("Green cell show position of selected object", MessageType.Info);
        EditorGUILayout.HelpBox("Red cell means that you can`t place object in this cell", MessageType.Info);
        EditorGUILayout.HelpBox("Enemies can only be placed at least one cell away from grid border of grid", MessageType.Info);
    }

    private void RenameLevels()
    {
        string originalName;
        string newName;
        string originalPath;

        for (int i = 0; i < levelsSerializedProperty.arraySize; i++)
        {
            if (levelsSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue != null)
            {
                originalName = levelsSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue.name;
                originalPath = LEVELS_FOLDER_PATH + originalName + ".asset";
                newName = "Old " + originalName;

                AssetDatabase.RenameAsset(originalPath, newName);
            }
        }

        for (int i = 0; i < levelsSerializedProperty.arraySize; i++)
        {
            if (levelsSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue != null)
            {
                originalName = levelsSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue.name;
                originalPath = LEVELS_FOLDER_PATH + originalName + ".asset";
                newName = "Level " + (i + 1);

                AssetDatabase.RenameAsset(originalPath, newName);
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void DrawElementInfoArray(string label, ElementInfo[] elementInfos, float cellSize, ItemType itemType)
    {
        EditorGUILayout.LabelField(label);

        groupRect = EditorGUILayout.BeginVertical();
        startPosX = groupRect.xMin + START_OFFSET;
        startPosY = groupRect.yMin + START_OFFSET;
        maxHeight = 0f;


        for (int i = 0; i < elementInfos.Length; i++)
        {
            if (elementInfos[i].IsNull())
            {
                continue;
            }


            if (startPosX + elementInfos[i].GetWidth(cellSize) < Screen.width)
            {
                textureRect = elementInfos[i].GetRect(startPosX, startPosY, cellSize);
                GUI.DrawTexture(textureRect, elementInfos[i].texture, ScaleMode.ScaleToFit);
                DrawGrid(startPosX, startPosY, elementInfos[i].size, cellSize);

                startPosX += textureRect.width + START_OFFSET;

                if (maxHeight < textureRect.height)
                {
                    maxHeight = textureRect.height;
                }
            }
            else
            {
                startPosX = groupRect.xMin + START_OFFSET;
                startPosY += maxHeight + START_OFFSET;
                maxHeight = 0;
                textureRect = elementInfos[i].GetRect(startPosX, startPosY, cellSize);
                GUI.DrawTexture(textureRect, elementInfos[i].texture, ScaleMode.ScaleToFit);
                DrawGrid(startPosX, startPosY, elementInfos[i].size, cellSize);

                startPosX += textureRect.width + START_OFFSET;

                if (maxHeight < textureRect.height)
                {
                    maxHeight = textureRect.height;
                }

            }

            //handle drag start
            if (!levelItemIsDragged)
            {
                currentEvent = Event.current;

                if (textureRect.Contains(currentEvent.mousePosition))
                {
                    if (currentEvent.type == EventType.MouseDown)
                    {

                        draggedElementInfo = elementInfos[i];
                        draggedLevelItemIndex = -1;
                        draggetItemAngle = 0;
                        draggetItemType = itemType;

                        if (itemType == ItemType.Obstacle)
                        {
                            draggetElementObstacleReference = (Obstacle)obstaclesSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                        }
                        else
                        {
                            draggetElementEnemyReference = (Enemy)enemySerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue;
                        }

                        UpdateLevelFilledGridCell();
                        levelItemIsDragged = true;
                        currentEvent.Use();
                    }
                }
            }
        }

        startPosY += maxHeight + START_OFFSET;

        GUILayout.Space(startPosY - groupRect.yMin);
        EditorGUILayout.EndVertical();
    }

    private void DrawLevelItems(float startX, float startY, SerializedProperty levelGroupSerializedProperty, float cellSize, ItemType itemType)
    {
        for (int i = 0; i < levelGroupSerializedProperty.arraySize; i++)
        {
            if (levelItemIsDragged && (draggetItemType == itemType) && (draggedLevelItemIndex == i))
            {
                continue;
            }

            itemLevelGridPosition = levelGroupSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("position").vector2IntValue;
            itemUIGridPosition = new Vector2Int(itemLevelGridPosition.x,roomSizeSerializedProperty.vector2IntValue.y - 1 - itemLevelGridPosition.y);

            itemAngle = levelGroupSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("angle").intValue;

            if (itemType == ItemType.Obstacle)
            {
                itemObstacleReference = (Obstacle)levelGroupSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("obstacle").objectReferenceValue;
                itemInfo = GetElementInfo(itemObstacleReference, itemType);
            }
            else
            {
                itemEnemyReference = (Enemy)levelGroupSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("enemy").objectReferenceValue;
                itemInfo = GetElementInfo(itemEnemyReference, itemType);
            }

            itemPosition = Vector2.one * itemUIGridPosition * cellSize + ElementInfo.GetOffset(cellSize, itemAngle);
            itemPosition.x += startX;
            itemPosition.y += startY;

            DrawRotatedTexture(itemPosition, itemAngle, cellSize, itemInfo, true, true, delegate
            {
                if (!levelItemIsDragged)
                {
                    currentEvent = Event.current;
                    draggedElementInfo = itemInfo;
                    draggedLevelItem = levelGroupSerializedProperty.GetArrayElementAtIndex(i);
                    draggedLevelItemIndex = i;
                    draggetItemAngle = itemAngle;
                    draggetItemType = itemType;

                    UpdateLevelFilledGridCell();
                    levelItemIsDragged = true;

                }
            });
        }
    }

    private void DrawRotatedTexture(Vector2 itemPosition, int itemAngle, float cellSize, ElementInfo itemInfo, bool displayItemBackground, bool isButton, Action onClickCallBack)
    {
        matrixBackup = GUI.matrix;

        fullRect = itemInfo.GetFullRect(itemPosition.x, itemPosition.y, cellSize);
        textureRect = itemInfo.GetRect(itemPosition.x, itemPosition.y, cellSize);

        if ((itemAngle != 0) && ((Screen.width - fullRect.xMax < 30) || (Screen.height - fullRect.yMax + globalScrollVector.y < 30)))
        {
            secondRotatingPoint = itemPosition;
            firstRotatingPoint = secondRotatingPoint - fullRect.size;
            fullRect.position = firstRotatingPoint - fullRect.size;
            textureRect = itemInfo.GetRect(fullRect.position.x, fullRect.position.y, cellSize);

            //GUI.DrawTexture(fullRect, greenTexture, ScaleMode.StretchToFill); debug stuff
            GUIUtility.RotateAroundPivot(180, firstRotatingPoint);
            //GUI.DrawTexture(fullRect, redTexture, ScaleMode.StretchToFill);   debug stuff
            GUIUtility.RotateAroundPivot(180 + itemAngle, secondRotatingPoint);
        }
        else
        {
            GUIUtility.RotateAroundPivot(itemAngle, itemPosition);
        }

        if (displayItemBackground)
        {
            GUI.DrawTexture(fullRect, itemBackgroundTexture, ScaleMode.StretchToFill);
        }

        GUI.DrawTexture(textureRect, itemInfo.texture, ScaleMode.ScaleToFit);

        if (isButton)
        {
            if (GUI.Button(fullRect, GUIContent.none, GUIStyle.none))
            {
                onClickCallBack?.Invoke();
            }
        }

        GUI.matrix = matrixBackup;
    }

    private void DrawLevelGrid()
    {
        groupRect = EditorGUILayout.BeginVertical(GUILayout.MinHeight(roomSizeSerializedProperty.vector2IntValue.y * levelCellSize));

        if (groupRect.yMin != 0)
        {
            levelGridPositionY = groupRect.yMin;
            levelGridPositionX = groupRect.xMin;
        }

        DrawGrid(groupRect.xMin, groupRect.yMin, roomSizeSerializedProperty.vector2IntValue, levelCellSize);
        DrawLevelItems(groupRect.xMin, groupRect.yMin, roomObstaclesSerializedProperty, levelCellSize, ItemType.Obstacle);
        DrawLevelItems(groupRect.xMin, groupRect.yMin, roomEnemySerializedProperty, levelCellSize, ItemType.Enemies);
        GUILayout.Space(roomSizeSerializedProperty.vector2IntValue.y * levelCellSize);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space();
    }

    private ElementInfo GetElementInfo(UnityEngine.Object objectReference, ItemType itemType)
    {
        if (itemType == ItemType.Obstacle)
        {
            for (int i = 0; i < obstaclesSerializedProperty.arraySize; i++)
            {
                if (obstaclesSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue == objectReference)
                {
                    return obstaleInfos[i];
                }
            }
        }
        else
        {
            for (int i = 0; i < enemySerializedProperty.arraySize; i++)
            {
                if (enemySerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue == objectReference)
                {
                    return enemyInfos[i];
                }
            }
        }

        if (objectReference == null)
        {
            throw new Exception("GetElementInfo called with null objectReference ");
        }
        else
        {
            throw new Exception("objectReference not found for " + objectReference.name);
        }


    }

    private void UpdateLevelFilledGridCell()
    {
        filledGridCells.Clear();

        for (int i = 0; i < roomObstaclesSerializedProperty.arraySize; i++)
        {
            if ((draggetItemType == ItemType.Obstacle) && (draggedLevelItemIndex == i))
            {
                continue;
            }


            itemLevelGridPosition = roomObstaclesSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("position").vector2IntValue;
            itemUIGridPosition = new Vector2Int(itemLevelGridPosition.x, roomSizeSerializedProperty.vector2IntValue.y - 1 - itemLevelGridPosition.y);
            itemAngle = roomObstaclesSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("angle").intValue;
            itemInfo = GetElementInfo(roomObstaclesSerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("obstacle").objectReferenceValue, ItemType.Obstacle);

            filledGridCells.AddRange(itemInfo.GetGridCells(itemUIGridPosition, itemAngle));
        }

        for (int i = 0; i < roomEnemySerializedProperty.arraySize; i++)
        {
            if ((draggetItemType == ItemType.Enemies) && (draggedLevelItemIndex == i))
            {
                continue;
            }

            itemLevelGridPosition = roomEnemySerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("position").vector2IntValue;
            itemUIGridPosition = new Vector2Int(itemLevelGridPosition.x, roomSizeSerializedProperty.vector2IntValue.y - 1 - itemLevelGridPosition.y);
            itemAngle = roomEnemySerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("angle").intValue;
            itemInfo = GetElementInfo(roomEnemySerializedProperty.GetArrayElementAtIndex(i).FindPropertyRelative("enemy").objectReferenceValue, ItemType.Enemies);

            filledGridCells.AddRange(itemInfo.GetGridCells(itemUIGridPosition, itemAngle));
        }
    }

    private void HandleDragOfLevelItem()
    {
        if (!levelItemIsDragged)
        {
            return;
        }

        currentEvent = Event.current;
        //handle events
        switch (currentEvent.type)
        {
            case EventType.MouseLeaveWindow:
                levelItemIsDragged = false;
                currentEvent.Use();
                break;
            case EventType.MouseDown:

                if (currentEvent.button == 0)
                {
                    if (draggetItemDropAcceptable)
                    {
                        UpdateLevelItem();
                        levelItemIsDragged = false;
                    }
                    else
                    {
                        if (!draggetItemSnapActive)
                        {
                            RemoveLevelItem();
                            levelItemIsDragged = false;
                        }
                    }



                }

                currentEvent.Use();
                break;

            case EventType.ContextClick:
                draggetItemAngle = (draggetItemAngle + 90) % 360;
                currentEvent.Use();
                break;
            default:
                break;
        }

        //find element position
        draggetItemUIGridPosition.x = Mathf.FloorToInt((currentEvent.mousePosition.x - levelGridPositionX) / levelCellSize);
        draggetItemUIGridPosition.y = Mathf.FloorToInt((currentEvent.mousePosition.y - levelGridPositionY) / levelCellSize);


        //snap if inside grid
        if ((draggetItemUIGridPosition.x >= 0) && (draggetItemUIGridPosition.x < roomSizeSerializedProperty.vector2IntValue.x) &&
            (draggetItemUIGridPosition.y >= 0) && (draggetItemUIGridPosition.y < roomSizeSerializedProperty.vector2IntValue.y))
        {
            draggetItemSnapActive = true;
            rectPosition.x = (draggetItemUIGridPosition.x * levelCellSize + levelGridPositionX);
            rectPosition.y = (draggetItemUIGridPosition.y * levelCellSize + levelGridPositionY);
            texturePointRect = new Rect(rectPosition, new Vector2(levelCellSize, levelCellSize));
            rectPosition += ElementInfo.GetOffset(levelCellSize, draggetItemAngle);
            UpdateDragStatusForLevelItem();
        }
        else
        {
            draggetItemSnapActive = false;
            rectPosition = currentEvent.mousePosition;
            texturePointRect = new Rect(rectPosition, new Vector2(levelCellSize, levelCellSize));
            draggetItemDropAcceptable = false;
        }

        DrawRotatedTexture(rectPosition, draggetItemAngle, levelCellSize, draggedElementInfo, true, false, null);

        //element position cell in grid
        if (draggetItemSnapActive)
        {
            if (draggetItemDropAcceptable)
            {
                GUI.DrawTexture(texturePointRect, greenTexture, ScaleMode.ScaleAndCrop);
            }
            else
            {
                GUI.DrawTexture(texturePointRect, redTexture, ScaleMode.ScaleAndCrop);
            }
        }
    }



    private void UpdateLevelItem()
    {
        Vector2Int draggetItemLevelGridPosition = new Vector2Int(draggetItemUIGridPosition.x, roomSizeSerializedProperty.vector2IntValue.y - 1 - draggetItemUIGridPosition.y);

        if (draggetItemType == ItemType.Obstacle)
        {
            //Create new if we took element from palette
            if (draggedLevelItemIndex == -1)
            {
                roomObstaclesSerializedProperty.arraySize++;
                draggedLevelItem = roomObstaclesSerializedProperty.GetArrayElementAtIndex(roomObstaclesSerializedProperty.arraySize - 1);
                draggedLevelItem.FindPropertyRelative("obstacle").objectReferenceValue = draggetElementObstacleReference;
            }

            draggedLevelItem.FindPropertyRelative("position").vector2IntValue = draggetItemLevelGridPosition;
            draggedLevelItem.FindPropertyRelative("angle").intValue = draggetItemAngle;
        }
        else if (draggetItemType == ItemType.Enemies)
        {
            //Create new if we took element from palette
            if (draggedLevelItemIndex == -1)
            {
                roomEnemySerializedProperty.arraySize++;
                draggedLevelItem = roomEnemySerializedProperty.GetArrayElementAtIndex(roomEnemySerializedProperty.arraySize - 1);
                draggedLevelItem.FindPropertyRelative("enemy").objectReferenceValue = draggetElementEnemyReference;
            }

            draggedLevelItem.FindPropertyRelative("position").vector2IntValue = draggetItemLevelGridPosition;
            draggedLevelItem.FindPropertyRelative("angle").intValue = draggetItemAngle;
        }
        selectedLevelSerializedObject.ApplyModifiedProperties();
    }

    private void RemoveLevelItem()
    {
        if (draggetItemType == ItemType.Obstacle)
        {
            if (draggedLevelItemIndex == -1)
            {
                return;
            }

            roomObstaclesSerializedProperty.RemoveFromVariableArrayAt(draggedLevelItemIndex);
        }
        else if (draggetItemType == ItemType.Enemies)
        {
            if (draggedLevelItemIndex == -1)
            {
                return;
            }

            roomEnemySerializedProperty.RemoveFromVariableArrayAt(draggedLevelItemIndex);
        }

        selectedLevelSerializedObject.ApplyModifiedProperties();
    }

    private void UpdateDragStatusForLevelItem()
    {
        draggetItemGridCells.Clear();
        draggetItemGridCells.AddRange(draggedElementInfo.GetGridCells(draggetItemUIGridPosition, draggetItemAngle));
        draggetItemDropAcceptable = true;

        foreach (Vector2Int cell in draggetItemGridCells)
        {
            if ((cell.x < 0) || (cell.y < 0) || (cell.x >= roomSizeSerializedProperty.vector2IntValue.x) || (cell.y >= roomSizeSerializedProperty.vector2IntValue.y))
            {
                draggetItemDropAcceptable = false;
                break;
            }

            if (filledGridCells.Contains(cell))
            {
                draggetItemDropAcceptable = false;
                break;
            }
        }
    }





    #endregion

    private void DisplayWarningAboutMissingTextures()
    {
        for (int i = 0; i < obstaleInfos.Length; i++)
        {
            if (obstaleInfos[i].IsNull())
            {
                if (obstaclesSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("Null obstale with index:" + i, MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("Texture not set for " + obstaclesSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue.name, MessageType.Error);

                }
            }
        }

        for (int i = 0; i < enemyInfos.Length; i++)
        {
            if (enemyInfos[i].IsNull())
            {
                if (enemySerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    EditorGUILayout.HelpBox("Null obstale with index:" + i, MessageType.Error);
                }
                else
                {
                    EditorGUILayout.HelpBox("Texture not set for " + enemySerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue.name, MessageType.Error);

                }
            }
        }

        if (redTexture == null)
        {
            EditorGUILayout.HelpBox("Texture not set for redTexture. Please set texture in Editor textures tab.", MessageType.Error);
        }

        if (greenTexture == null)
        {
            EditorGUILayout.HelpBox("Texture not set for greenTexture. Please set texture in Editor textures tab.", MessageType.Error);
        }

        if (placeholderTexture == null)
        {
            EditorGUILayout.HelpBox("Texture not set for placeholderTexture. Please set texture in Editor textures tab.", MessageType.Error);
        }

        if (itemBackgroundTexture == null)
        {
            EditorGUILayout.HelpBox("Texture not set for itemBackgroundTexture. Please set texture in Editor textures tab.", MessageType.Error);
        }
    }

    private void DisplayEditorConfigurationTab()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(levelDatabaseSerializedObject.FindProperty("placeholderTexture"));
        EditorGUILayout.PropertyField(levelDatabaseSerializedObject.FindProperty("greenTexture"));
        EditorGUILayout.PropertyField(levelDatabaseSerializedObject.FindProperty("redTexture"));
        EditorGUILayout.PropertyField(levelDatabaseSerializedObject.FindProperty("itemBackgroundTexture"));
        EditorGUILayout.EndVertical();

        if (GUI.changed)
        {
            LoadEditorTextures();
        }
    }

    private void DisplayGridSize(SerializedProperty sizeSerializedProperty, Vector2Int minSize, Vector2Int maxSize)
    {
        gridSize = sizeSerializedProperty.vector2IntValue;
        gridSize.x = Mathf.Clamp(gridSize.x, minSize.x, maxSize.x);
        gridSize.y = Mathf.Clamp(gridSize.y, minSize.y, maxSize.y);
        gridSize = EditorGUILayout.Vector2IntField("size:", gridSize);
        gridSize.x = Mathf.Clamp(gridSize.x, minSize.x, maxSize.x);
        gridSize.y = Mathf.Clamp(gridSize.y, minSize.y, maxSize.y);

        sizeSerializedProperty.vector2IntValue = gridSize;
    }

    public void UpdateElementInfos()
    {
        obstaleInfos = new ElementInfo[obstaclesSerializedProperty.arraySize];
        enemyInfos = new ElementInfo[enemySerializedProperty.arraySize];
        SerializedObject tempSerializableObject;

        for (int i = 0; i < obstaleInfos.Length; i++)
        {

            if (obstaclesSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue == null)
            {
                obstaleInfos[i] = new ElementInfo();
            }
            else
            {
                tempSerializableObject = new SerializedObject((Obstacle)obstaclesSerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                obstaleInfos[i] = new ElementInfo(tempSerializableObject);
            }
        }

        for (int i = 0; i < enemySerializedProperty.arraySize; i++)
        {
            if (enemySerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue == null)
            {
                enemyInfos[i] = new ElementInfo();
            }
            else
            {
                tempSerializableObject = new SerializedObject((Enemy)enemySerializedProperty.GetArrayElementAtIndex(i).objectReferenceValue);
                enemyInfos[i] = new ElementInfo(tempSerializableObject);
            }
        }
    }





    //class needed for optimization
    private class ElementInfo
    {
        public Texture texture;
        public Vector2Int size;
        public float horizontalOffset;
        public float verticalOffset;

        public ElementInfo()
        {
            this.texture = Texture2D.redTexture;
            this.size = Vector2Int.zero;
        }

        public ElementInfo(Texture texture, Vector2Int size, float horizontalOffset, float verticalOffset)
        {
            this.texture = texture;
            this.size = size;
            this.horizontalOffset = horizontalOffset;
            this.verticalOffset = verticalOffset;
        }

        public ElementInfo(SerializedObject tempSerializableObject)
        {
            this.size = tempSerializableObject.FindProperty("size").vector2IntValue;
            SerializedProperty editorTextureSerializedProperty = tempSerializableObject.FindProperty("editorTexture");

            if (editorTextureSerializedProperty.FindPropertyRelative("texture").objectReferenceValue == null)
            {
                this.texture = Texture2D.redTexture;
            }
            else
            {
                this.texture = (Texture2D)editorTextureSerializedProperty.FindPropertyRelative("texture").objectReferenceValue;
            }

            this.horizontalOffset = editorTextureSerializedProperty.FindPropertyRelative("horizontalOffset").floatValue;
            this.verticalOffset = editorTextureSerializedProperty.FindPropertyRelative("verticalOffset").floatValue;
        }

        public bool IsNull()
        {
            return ((size == Vector2Int.zero) || (this.texture.Equals(Texture2D.redTexture)));
        }

        public float GetWidth(float cellSize)
        {
            return cellSize * size.x - horizontalOffset * 2;
        }

        public float GetHeight(float cellSize)
        {
            return cellSize * size.y - verticalOffset * 2;
        }

        public Rect GetFullRect(float startX, float startY, float cellSize)
        {
            return new Rect(startX, startY, cellSize * size.x, cellSize * size.y);
        }

        public Rect GetFullRect(float cellSize)
        {
            return GetFullRect(0, 0, cellSize);
        }

        public Rect GetRect(float startX, float startY, float cellSize)
        {
            return new Rect(startX + horizontalOffset, startY + verticalOffset, GetWidth(cellSize), GetHeight(cellSize));
        }

        public Rect GetRect(float cellSize)
        {
            return GetRect(0, 0, cellSize);
        }

        public Rect GetTexturePointRect(float startX, float startY, float cellSize)
        {
            return new Rect(startX, startY, cellSize, cellSize);
        }

        public static Vector2 GetOffset(float cellSize, int angle)
        {
            switch (angle)
            {
                case 0:
                    return Vector2.zero;
                case 90:
                    return new Vector2(cellSize, 0);
                case 180:
                    return new Vector2(cellSize, cellSize);
                case 270:
                    return new Vector2(0, cellSize);
                default:
                    Debug.LogError("Incorect angle in GetOffset");
                    return Vector2.one * (9999);
            }
        }

        public List<Vector2Int> GetGridCells(Vector2Int currentPosition, int angle)
        {
            List<Vector2Int> resultList = new List<Vector2Int>();
            Vector2Int tempVector;

            switch (angle)
            {
                case 0:
                    for (int x = 0; x < size.x; x++) // 3
                    {
                        for (int y = 0; y < size.y; y++)  //6
                        {
                            resultList.Add(new Vector2Int(currentPosition.x + x, currentPosition.y + y));
                        }
                    }

                    break;

                case 90:
                    for (int x = 0; x < size.x; x++)
                    {
                        for (int y = 0; y < size.y; y++)
                        {
                            resultList.Add(new Vector2Int(currentPosition.x - y, currentPosition.y + x));
                        }
                    }

                    break;
                case 180:
                    for (int x = 0; x < size.x; x++) // 3
                    {
                        for (int y = 0; y < size.y; y++)  //6
                        {
                            resultList.Add(new Vector2Int(currentPosition.x - x, currentPosition.y - y));
                        }
                    }

                    break;
                case 270:
                    for (int x = 0; x < size.x; x++)
                    {
                        for (int y = 0; y < size.y; y++)
                        {
                            resultList.Add(new Vector2Int(currentPosition.x + y, currentPosition.y - x));
                        }
                    }

                    break;
                default:
                    Debug.LogError("Incorect angle in GetGridCells");
                    break;
            }

            return resultList;
        }

    }

    private enum ItemType
    {
        Obstacle,
        Enemies
    }
}
