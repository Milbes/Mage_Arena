using UnityEngine;

namespace Watermelon
{
    public class InventoryInitModule : InitModule
    {
        [SerializeField] ItemDatabase itemDatabase;
        [SerializeField] ItemSettings itemSettings;

        [Space]
        [SerializeField] bool showLogs = true;

        public override void CreateComponent(Initialiser initialiser)
        {
            itemSettings.Init();
            itemDatabase.Init();

            if(showLogs)
            {
                Debug.Log("[Inventory]: Items database is initialised!");
                Debug.Log("[Inventory]: Loaded " + itemDatabase.Items.Length + " items.");
            }

            Inventory.Init();

            if (showLogs)
                Debug.Log("[Inventory]: Inventory is initialised!");
        }

        public InventoryInitModule()
        {
            moduleName = "Inventory";
        }
    }
}
