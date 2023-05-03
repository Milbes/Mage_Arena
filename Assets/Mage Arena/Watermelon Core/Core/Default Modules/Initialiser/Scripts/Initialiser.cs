#pragma warning disable 0649

using UnityEngine;

namespace Watermelon
{
    [DefaultExecutionOrder(-999)]
    [HelpURL("https://docs.google.com/document/d/1ORNWkFMZ5_Cc-BUgu9Ds1DjMjR4ozMCyr6p_GGdyCZk")]
    public class Initialiser : MonoBehaviour
    {
        [SerializeField] ProjectInitSettings initSettings;
        [SerializeField] Canvas systemCanvas;

        public static Canvas SystemCanvas;
        public static GameObject InitialiserGameObject;

        private bool isInitialized = false;
        public bool IsInititalized
        {
            get { return isInitialized; }
        }

        private void Awake()
        {
            if(!isInitialized)
            {
                isInitialized = true;

                SystemCanvas = systemCanvas;
                InitialiserGameObject = gameObject;

                DontDestroyOnLoad(gameObject);

                initSettings.Init(this); 
            }
            else
            {
                Debug.Log("[Initialiser]: Game is already initialized!");

                Destroy(gameObject);
            }
        }

        private void Start()
        {
            initSettings.StartInit(this);
        }

        private void OnDestroy()
        {
            isInitialized = false;
        }

    }
}

// -----------------
// Initialiser v 0.4.1
// -----------------

// Changelog
// v 0.4.1
// • Fixed error on module remove
// v 0.3.1
// • Added link to the documentation
// • Initializer renamed to Initialiser
// • Fixed problem with recompilation
// v 0.2
// • Added sorting feature
// • Initialiser MonoBehaviour will destroy after initialization
// v 0.1
// • Added basic version
