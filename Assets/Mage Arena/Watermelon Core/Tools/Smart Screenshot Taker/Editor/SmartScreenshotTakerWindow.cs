using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.IO;
using System.Linq;

public class EditorSmartScreenshotTakerWindow : EditorWindow
{
    private const string BASE_FOLDER_NAME = "Screenshots/";
    
    private static bool[] activeRatios;

    private static bool isInitialized;

    private IEnumerator screenShotCoroutine;

    private static object gameViewSizesInstance;
    private static MethodInfo getGroup;

    private int[] sizes = new int[] { 1, 2, 4, 8 };
    private string[] sizeNames;
    private int selectedSize = 0;

    private static GameViewSizeGroupType gameViewSizeGroupType;
    
    [MenuItem("Window/Smart Screenshot Taker/Screnshot Taker Window")]
    public static EditorSmartScreenshotTakerWindow ShowWindow()
    {
        EditorSmartScreenshotTakerWindow window = GetWindow<EditorSmartScreenshotTakerWindow>(false);
        window.titleContent = new GUIContent("Smart Screenshot Taker");
        
        return window;
    }

    [MenuItem("Window/Smart Screenshot Taker/Take Screenshot _F12")]
    public static void TakeScreenshotShortcut()
    {
        EditorSmartScreenshotTakerWindow window = ShowWindow();

        if (Application.isPlaying)
        {
            if (window != null)
            {
                window.TakeScreenshot();
            }
        }
    }

    private void OnEnable()
    {
        Initialize();

        sizeNames = sizes.Select(x => x.ToString()).ToArray();
    }

    public void TakeScreenshot()
    {
        if (screenShotCoroutine == null)
        {
            if (isInitialized && activeRatios.Where(x => x).Count() > 0)
            {
                EditorApplication.update += EditorUpdate;

                screenShotCoroutine = TakeScreenshotEnumerator();
            }
        }
        else
        {
            Debug.LogWarning("Screenshot Taker is already in work. Please wait.");
        }
    }

    private void EditorUpdate()
    {
        if(screenShotCoroutine != null)
        {
            if(!screenShotCoroutine.MoveNext())
            {
                EditorApplication.update -= EditorUpdate;

                screenShotCoroutine = null;
            }
        }
    }
        
    private static void Initialize()
    {
        Type sizesType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSizes");
        Type singleType = typeof(ScriptableSingleton<>).MakeGenericType(sizesType);
        PropertyInfo instanceProp = singleType.GetProperty("instance");
        getGroup = sizesType.GetMethod("GetGroup");
        gameViewSizesInstance = instanceProp.GetValue(null, null);

        gameViewSizeGroupType = GetCurrentGroupType();

        activeRatios = new bool[AspectRatios.aspectRatios.Length];
        for (int i = 0; i < AspectRatios.aspectRatios.Length; i++)
        {
            if (!SizeExists(gameViewSizeGroupType, AspectRatios.aspectRatios[i].xAspect, AspectRatios.aspectRatios[i].yAspect))
            {
                AddCustomSize(gameViewSizeGroupType, AspectRatios.aspectRatios[i].xAspect, AspectRatios.aspectRatios[i].yAspect, AspectRatios.aspectRatios[i].name);
            }
        }

        isInitialized = true;
    }

    private void OnGUI()
    {
        if (Application.isPlaying)
        {
            if (!isInitialized)
                Initialize();
            
            EditorGUILayout.BeginVertical(GUI.skin.box);
            for (int i = 0; i < activeRatios.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.LabelField(new GUIContent(AspectRatios.aspectRatios[i].name));
                activeRatios[i] = EditorGUILayout.Toggle(activeRatios[i], GUILayout.Width(20));
                if (EditorGUI.EndChangeCheck())
                {
                    string folderPath = Application.dataPath.Replace("Assets", "") + BASE_FOLDER_NAME + AspectRatios.aspectRatios[i].name + "/";
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
            
            selectedSize = EditorGUILayout.Popup("Extra size: ", selectedSize, sizeNames);
            
            if (GUILayout.Button("Take Screenshot"))
            {
                TakeScreenshot();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Screenshot Taker works only in play mode.", MessageType.Info);
        }
    }

    private IEnumerator TakeScreenshotEnumerator()
    {
        List<Camera> cameras = new List<Camera>(Camera.allCameras);
        cameras.OrderBy(x => x.depth);

        float timeScale = Time.timeScale;
        Time.timeScale = 0;

        Assembly assembly = Assembly.GetAssembly(typeof(ScreenshotResizeAttribute));
        IEnumerable<MethodInfo> methods = assembly.GetTypes().SelectMany(m => m.GetMethods().Where(i => i.GetCustomAttributes(typeof(ScreenshotResizeAttribute), false).Count() > 0));
        Dictionary<MethodInfo, UnityEngine.Object[]> cachableObjects = new Dictionary<MethodInfo, UnityEngine.Object[]>();

        EditorWindow window = FullscreenManger.OpenFullscreen(EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView")));

        yield return null;

        foreach (var method in methods)
        {
            UnityEngine.Object[] objects = FindObjectsOfType(method.ReflectedType);
            if (objects != null)
            {
                cachableObjects.Add(method, objects);
            }
        }

        for (int i = 0; i < AspectRatios.aspectRatios.Length; i++)
        {
            if (activeRatios[i])
            {
                string path = BASE_FOLDER_NAME + AspectRatios.aspectRatios[i].name + "/Sccreenshot_" + DateTime.Now.Hour.ToString() + "-" + DateTime.Now.Minute.ToString() + "-" + DateTime.Now.Second.ToString();

                int sizeIndex = FindSize(gameViewSizeGroupType, AspectRatios.aspectRatios[i].xAspect, AspectRatios.aspectRatios[i].yAspect);
                if (sizeIndex != -1)
                    SetSize(sizeIndex);

                yield return null;
                yield return null;

                window.Repaint();

                foreach (var cachableObject in cachableObjects)
                {
                    for (int c = 0; c < cachableObject.Value.Length; c++)
                    {
                        cachableObject.Key.Invoke(cachableObject.Value[c], null);
                    }
                }

                yield return null;

                int width = AspectRatios.aspectRatios[i].xAspect * sizes[selectedSize];
                int height = AspectRatios.aspectRatios[i].yAspect * sizes[selectedSize];

                RenderTexture renderTexture = RenderTexture.GetTemporary(width, height, 24);
                RenderTexture.active = renderTexture;

                foreach (Camera camera in cameras)
                {
                    if (camera.enabled)
                    {
                        float fov = camera.fieldOfView;
                        camera.targetTexture = renderTexture;
                        camera.Render();
                        camera.targetTexture = null;
                        camera.fieldOfView = fov;
                    }
                }

                Texture2D result = new Texture2D(width, height, TextureFormat.RGB24, true);
                result.ReadPixels(new Rect(0.0f, 0.0f, width, height), 0, 0, true);
                result.Apply();

                RenderTexture.active = null;
                RenderTexture.ReleaseTemporary(renderTexture);

                byte[] bytes = result.EncodeToPNG();

                File.WriteAllBytes(path + ".png", bytes);

                yield return null;
            }
        }

        FullscreenManger.CloseWindow();

        Time.timeScale = timeScale;
    }

    #region Unity
    private static GameViewSizeGroupType GetCurrentGroupType()
    {
        var getCurrentGroupTypeProp = gameViewSizesInstance.GetType().GetProperty("currentGroupType");
        return (GameViewSizeGroupType)(int)getCurrentGroupTypeProp.GetValue(gameViewSizesInstance, null);
    }

    private static void SetSize(int index)
    {
        Type gvWndType = typeof(Editor).Assembly.GetType("UnityEditor.GameView");
        PropertyInfo selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var gvWnd = EditorWindow.GetWindow(gvWndType);
        selectedSizeIndexProp.SetValue(gvWnd, index, null);
    }

    private static void AddCustomSize(GameViewSizeGroupType sizeGroupType, int width, int height, string text)
    {
        System.Object group = GetGroup(sizeGroupType);
        MethodInfo addCustomSize = getGroup.ReturnType.GetMethod("AddCustomSize");
        Type gvsType = typeof(Editor).Assembly.GetType("UnityEditor.GameViewSize");

        //ConstructorInfo ctor = gvsType.GetConstructor(new Type[] { typeof(int), typeof(int), typeof(int), typeof(string) });
        ConstructorInfo ctor = gvsType.GetConstructors()[0];

        System.Object newSize = ctor.Invoke(new object[] { 1, width, height, text });
        addCustomSize.Invoke(group, new object[] { newSize });
    }

    private static bool SizeExists(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        return FindSize(sizeGroupType, width, height) != -1;
    }

    private static int FindSize(GameViewSizeGroupType sizeGroupType, int width, int height)
    {
        var group = GetGroup(sizeGroupType);
        var groupType = group.GetType();
        var getBuiltinCount = groupType.GetMethod("GetBuiltinCount");
        var getCustomCount = groupType.GetMethod("GetCustomCount");
        int sizesCount = (int)getBuiltinCount.Invoke(group, null) + (int)getCustomCount.Invoke(group, null);
        var getGameViewSize = groupType.GetMethod("GetGameViewSize");
        var gvsType = getGameViewSize.ReturnType;
        var widthProp = gvsType.GetProperty("width");
        var heightProp = gvsType.GetProperty("height");
        var indexValue = new object[1];
        for (int i = 0; i < sizesCount; i++)
        {
            indexValue[0] = i;
            var size = getGameViewSize.Invoke(group, indexValue);
            int sizeWidth = (int)widthProp.GetValue(size, null);
            int sizeHeight = (int)heightProp.GetValue(size, null);
            if (sizeWidth == width && sizeHeight == height)
                return i;
        }

        return -1;
    }

    private static object GetGroup(GameViewSizeGroupType type)
    {
        return getGroup.Invoke(gameViewSizesInstance, new object[] { (int)type });
    }
    #endregion

    [InitializeOnLoadMethod]
    public static void InitializeScreenshotTaker()
    {
        Initialize();

        string fullPath = Application.dataPath.Replace("Assets", "") + BASE_FOLDER_NAME;
        if (!Directory.Exists(fullPath))
            Directory.CreateDirectory(fullPath);
    }
}
