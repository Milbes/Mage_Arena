#pragma warning disable 649

using System.Text;
using UnityEngine;
using Watermelon;

[CreateAssetMenu(menuName = "Skin Data", fileName = "Skin Data")]
[System.Serializable]
public class SkinData : ScriptableObject
{
    [SerializeField] int id;

    [Header("Initial Data")]
    [SerializeField] Character.Stats initialStats;

    [Header("Upgrades")]
    [SerializeField] Upgrade[] upgrades;

    [Space]
    [SerializeField] float stepDelay;
    [SerializeField] AudioClip stepSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip resurrectSound;

    public float StepDelay => stepDelay;
    public AudioClip StepSound => stepSound;
    public AudioClip DeathSound => deathSound;
    public AudioClip ResurrectSound => resurrectSound;
    
    public int ID => id;

    public Character.Stats InitialStats => initialStats;

    public Upgrade[] Upgrades => upgrades;
    
    [System.Serializable]
    public class Upgrade : Character.Stats
    {
        [Header("Cost")]
        [SerializeField] int cost;
        [SerializeField] int requiredItemsCount;

        public int Cost => cost;
        public int RequiredItemsCount => requiredItemsCount;
    }
}
