using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    [System.Flags]
    public enum ItemRarity
    {
        Common = (1 << 0),
        Uncommon = (1 << 1),
        Rare = (1 << 2),
        Epic = (1 << 3),
        Legendary = (1 << 4),
    }
}
