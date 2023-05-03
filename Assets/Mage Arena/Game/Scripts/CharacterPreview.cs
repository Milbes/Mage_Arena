#pragma warning disable 649

using UnityEngine;
using Watermelon;

public class CharacterPreview : MonoBehaviour
{
    [SerializeField] Camera previewCamera;

    [SerializeField] Transform playerHolder;
    [SerializeField] Transform weaponHolder;

    [SerializeField] ParticleSystem levelUpParticleSystem;

    private GameObject equipedWeaponPrefab;

    private void OnEnable()
    {
        Character.OnItemEquiped += OnItemEquiped;
        Character.OnLevelUp += OnLevelUp;
    }

    private void OnDisable()
    {
        Character.OnItemEquiped -= OnItemEquiped;
        Character.OnLevelUp -= OnLevelUp;
    }

    private void Start()
    {
        DisplayWeapon(Character.GetEquipedItem(EquipableItem.Weapon));
    }

    private void OnItemEquiped(EquipableItem equipableItemType, ItemHolder itemHolder, ItemHolder previousItemHolder)
    {
        if(equipableItemType == EquipableItem.Weapon)
        {
            DisplayWeapon(itemHolder);
        }
    }

    private void OnLevelUp(int newLevel, SkinData.Upgrade upgrade)
    {
        levelUpParticleSystem.gameObject.SetActive(true);
        levelUpParticleSystem.Play();
    }

    private void DisplayWeapon(ItemHolder itemHolder)
    {
        GameObject weaponPrefab;

        if (itemHolder != null)
        {
            WeaponItem weaponItem = (WeaponItem)itemHolder.Item;

            weaponPrefab = weaponItem.WeaponPrefab;
        }
        else
        {
            weaponPrefab = ItemSettings.GetDefaultWeapon().WeaponPrefab;
        }

        if (equipedWeaponPrefab != null)
            Destroy(equipedWeaponPrefab);

        equipedWeaponPrefab = Instantiate(weaponPrefab);
        equipedWeaponPrefab.layer = LayerMask.NameToLayer("Store Preview");

        equipedWeaponPrefab.transform.parent = weaponHolder;
        equipedWeaponPrefab.transform.localPosition = Vector3.zero;
        equipedWeaponPrefab.transform.localEulerAngles = Vector3.zero;
        equipedWeaponPrefab.transform.localScale = Vector3.one * 100;
    }
}
