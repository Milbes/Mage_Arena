using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public class ButtonSound : MonoBehaviour
{
    [SerializeField] AudioClip audioClip;

    private void Awake()
    {
        Button button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(PlaySound);
        }
    }

    public void PlaySound()
    {
        AudioController.PlaySound(audioClip);
    }
}
