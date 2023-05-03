using UnityEngine;
using Watermelon;

public class ChestAnimationEvent : MonoBehaviour
{
    public static OnChestOpenedCallback OnChestOpened;

    public void OnChestOpenedEvent()
    {
        if (OnChestOpened != null)
            OnChestOpened.Invoke();
    }

    public void OnChestPlaced()
    {
        AudioController.PlaySound(AudioController.Settings.sounds.chestPlaceSound);
    }

    public void OnChestCracked()
    {
        AudioController.PlaySound(AudioController.Settings.sounds.chestOpenSound);
    }

    public void OnChestShining()
    {
        AudioController.PlaySound(AudioController.Settings.sounds.chestShiningSound);
    }

    public delegate void OnChestOpenedCallback();
}
