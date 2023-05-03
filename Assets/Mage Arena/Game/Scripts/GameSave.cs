using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSave
{
    [SerializeField] int selectedSkinId;
    [SerializeField] List<SkinSave> skins;
    

    public SkinSave GetSkinSave(SkinData skinData)
    {
        if (skins == null) skins = new List<SkinSave>();

        for (int i = 0; i < skins.Count; i++)
        {
            if(skins[i].DataId == skinData.ID)
            {
                return skins[i];
            }
        }

        SkinSave skinSave = new SkinSave(skinData.ID);

        skins.Add(skinSave);

        return skinSave;
    }
}


