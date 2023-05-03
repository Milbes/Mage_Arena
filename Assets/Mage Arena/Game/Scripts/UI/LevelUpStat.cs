#pragma warning disable 649

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpStat : MonoBehaviour
{

    [SerializeField] LevelUps type;

    [Space]
    [SerializeField] Image fillImage;
    [SerializeField] Button button;
    
    public int Level { get; private set; }

    public void ResetStat()
    {
        Level = 0;

        fillImage.fillAmount = 0;

        button.enabled = true;
    }

    public void LevelUp()
    {
        Level++;
        Refresh();

        LevelUpUIController.LevelUpStat(type);
    }

    public void Refresh()
    {
        fillImage.fillAmount = Level / 5f;

        button.enabled = Level != 5;
    }

    public void DisableButton()
    {
        button.enabled = false;
    }
}
