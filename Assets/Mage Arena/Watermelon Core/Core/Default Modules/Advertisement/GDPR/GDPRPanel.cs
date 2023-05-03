#pragma warning disable 0649 

using UnityEngine;

namespace Watermelon
{
    public class GDPRPanel : MonoBehaviour
    {
        private static GDPRPanel instance;

        [SerializeField] GameObject panelObject;

        public bool IsPanelOpened => panelObject.activeInHierarchy;

        private void Awake()
        {
            instance = this;

            if (AdsManager.Settings.gdprContainer.enableGDPR)
            {
                panelObject.SetActive(!AdsManager.IsGDPRStateExist());
            }
            else
            {
                panelObject.SetActive(false);
            }
        }

        public void OpenPrivacyLinkButton()
        {
            Application.OpenURL(AdsManager.Settings.gdprContainer.privacyLink);
        }

        public void OpenTermsOfUseLinkButton()
        {
            Application.OpenURL(AdsManager.Settings.gdprContainer.termsOfUseLink);
        }

        public void SetGDPRStateButton(bool state)
        {
            AdsManager.SetGDPR(state);

            CloseWindow();
        }

        public static void OpenWindow()
        {
            instance.panelObject.SetActive(true);
        }

        public static void CloseWindow()
        {
            instance.panelObject.SetActive(false);
        }
    }
}

// -----------------
// Advertisement v 1.1f3
// -----------------