using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class MiscItemHolder : ItemHolder
    {
        [SerializeField] int amount;

        /// <summary>
        /// Current item amount
        /// </summary>
        public override int Amount
        {
            get { return amount; }
        }

        public MiscItemHolder(Item item, int amount, ItemRarity itemRarity) : base(item, itemRarity)
        {
            this.amount = amount;
        }

        public override bool Check()
        {
            return false;
        }

        public void ChangeAmount(int value)
        {
            amount += value;
        }

        public void SetAmount(int value)
        {
            amount = value;
        }

        public override string GetStatsString()
        {
            return string.Empty;
        }

        public override string GetCompareStatsString(ItemHolder itemHolder)
        {
            return string.Empty;
        }
    }
}
