#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public class LevelUpUIController : MonoBehaviour
{

    private static LevelUpUIController instance;

    [SerializeField] Canvas canvas;

    [Space]
    [SerializeField] List<LevelUpStat> stats;

    private void Awake()
    {
        instance = this;
    }

    public void Show()
    {

        canvas.enabled = true;

        for(int i = 0; i < stats.Count; i++)
        {
            stats[i].Refresh();
        }
    }

    public void Hide()
    {
        canvas.enabled = false;
    }

    public static void LevelUpStat(LevelUps type)
    {
        //PlayerController.LevelUpStat(type);

        for (int i = 0; i < instance.stats.Count; i++)
        {
            instance.stats[i].DisableButton();

        }

        Tween.DelayedCall(0.5f, () => {
            GamePanelBehavior.HideLevelUpPanel();

            LevelController.SetFinishState();
        });

        
    }
}

public enum LevelUps
{
    Health, Movement, Reload, Damage, Crit, Regen
}