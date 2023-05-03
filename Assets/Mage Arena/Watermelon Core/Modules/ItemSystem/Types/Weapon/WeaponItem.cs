using System.Text;
using UnityEngine;

namespace Watermelon
{
    [CreateAssetMenu(fileName = "Weapon Item", menuName = "Items/Weapon Item")]
    public class WeaponItem : Item, IItemSpecialEffect
    {
        [Header("Reffs")]
        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AudioClip hitAudio;

        [Header("Stats")]
        [SerializeField] Character.Stats stats;

        [Space]
        [SerializeField] float hitReloadTime;

        [Header("Stats Scaling")]
        [SerializeField] int minItemLevel = 1;
        [SerializeField] int maxItemLevel = 1;

        [Space]
        [SerializeField] ItemSpecialEffect[] itemSpecialEffects;

        public GameObject WeaponPrefab => weaponPrefab;
        public AudioClip HitAudio => hitAudio;

        public Character.Stats Stats => stats;

        public float HitReloadTime => hitReloadTime;

        public int MinItemLevel => minItemLevel;
        public int MaxItemLevel => maxItemLevel;

        public ItemSpecialEffect[] ItemSpecialEffects => itemSpecialEffects;

        public WeaponItem()
        {
            type = ItemType.Weapon;
            equipableItemType = EquipableItem.Weapon;
        }

        public override ItemHolder GetHolder()
        {
            return new WeaponItemHolder(this, minItemLevel, maxItemLevel);
        }

        public override ItemHolder GetDefaultHolder()
        {
            return new WeaponItemHolder(this, 1, ItemRarity.Common);
        }
    }
}
