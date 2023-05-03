using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public class HapticToggle : MonoBehaviour
{
    [SerializeField] Image imageRef;

    [Space]
    [SerializeField] Sprite activeSprite;
    [SerializeField] Sprite disableSprite;

    private bool isActive = true;

    private void Start()
    {
        //gameObject.SetActive(AudioController.IsVibrationEnabled());

        isActive = GameSettingsPrefs.Get<bool>("vibration");

        if (isActive)
            imageRef.sprite = activeSprite;
        else
            imageRef.sprite = disableSprite;
    }

    public void SwitchState()
    {
        isActive = !isActive;

        if (isActive)
        {
            imageRef.sprite = activeSprite;
            
            GameSettingsPrefs.Set("vibration", true);
        }
        else
        {
            imageRef.sprite = disableSprite;
            GameSettingsPrefs.Set("vibration", false);
        }
    }
}
