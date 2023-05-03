using System;
using Watermelon;

public enum ItemType
{
    Misc = 0,
    Weapon = 1,
    Armor = 2,
    MagicBook = 3,
    Ring = 4,
    Necklaces = 5,
}

public static class ItemUtils
{
    public static Type GetType(ItemType type)
    {
        switch (type)
        {
            case ItemType.Misc:
                return typeof(MiscItem);
            case ItemType.Weapon:
                return typeof(WeaponItem);
            case ItemType.Armor:
                return typeof(ArmorItem);
            case ItemType.MagicBook:
                return typeof(MagicBookItem);
            case ItemType.Ring:
                return typeof(RingItem);
            case ItemType.Necklaces:
                return typeof(NecklacesItem);
        }

        return typeof(Item);
    }
}
