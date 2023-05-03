using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Watermelon
{
    [System.Serializable]
    public class Loot : IWeightTable
    {
        [SerializeField] LootObject[] loot;

#if UNITY_EDITOR
        public LootObject[] LootObject
        {
            get { return loot; }
            set { loot = value; }
        }
#endif

        /// <summary>
        /// The maximum number of entries expected in the Result. The final count of items in the result may be lower
        /// if some of the entries may return a null result (no drop)
        /// </summary>
        [SerializeField] int resultCount;
        public int ResultCount
        {
            get { return resultCount; }
            set { resultCount = value; }
        }

        /// <summary>
        /// Gets or sets the contents of this table
        /// </summary>
        [SerializeField] List<IWeightObject> contents = null;
        public IEnumerable<IWeightObject> Contents
        {
            get { return contents; }
        }

        private List<IWeightObject> uniquedrops = new List<IWeightObject>();

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightTable"/> class.
        /// </summary>
        public Loot() : this(null, 1)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightTable"/> class.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <param name="count">The count.</param>
        /// <param name="probability">The probability.</param>
        /// <param name="unique">if set to <c>true</c> any item of this table (or contained sub tables) can be in the result only once.</param>
        /// <param name="always">if set to <c>true</c> the probability is disabled and the result will always contain (count) entries of this table.</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public Loot(IEnumerable<IWeightObject> contents, int count)
        {
            if (contents != null)
                this.contents = contents.ToList();
            else
                ClearContents();

            ResultCount = count;
        }

        public Loot(Loot lootContainer)
        {
            LootObject[] loot = new LootObject[lootContainer.loot.Length];
            for (int i = 0; i < loot.Length; i++)
            {
                loot[i] = (LootObject)lootContainer.loot[i].Clone();
            }

            this.loot = loot;
            resultCount = lootContainer.ResultCount;
        }

        /// <summary>
        /// Clears the contents.
        /// </summary>
        public virtual void ClearContents()
        {
            contents = new List<IWeightObject>();
        }

        /// <summary>
        /// Adds the given entry to contents collection.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public virtual void AddEntry(IWeightObject entry)
        {
            contents.Add(entry);
            entry.ObjectTable = this;
        }

        /// <summary>
        /// Adds a new entry to the contents collection and allows directly assigning of a probability for it.
        /// Use this signature if (for whatever reason) the base classes constructor does not support all
        /// constructors of RDSObject or if you implemented IRDSObject directly in your class and you need
        /// to (re)apply a new probability at the moment you add it to a RDSTable.
        /// NOTE: The probability given is written back to the given instance "entry".
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="probability">The probability.</param>
        public virtual void AddEntry(IWeightObject entry, float probability)
        {
            contents.Add(entry);
            entry.Probability = probability;
            entry.ObjectTable = this;
        }

        /// <summary>
        /// Adds a new entry to the contents collection and allows directly assigning of a probability and drop flags for it.
        /// Use this signature if (for whatever reason) the base classes constructor does not support all
        /// constructors of RDSObject or if you implemented IRDSObject directly in your class and you need
        /// to (re)apply a new probability and flags at the moment you add it to a RDSTable.
        /// NOTE: The probability, unique, always and enabled flags given are written back to the given instance "entry".
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="probability">The probability.</param>
        /// <param name="unique">if set to <c>true</c> this object can only occur once per result.</param>
        /// <param name="always">if set to <c>true</c> [always] this object will appear always in the result.</param>
        /// <param name="enabled">if set to <c>false</c> [enabled] this object will never be part of the result (even if it is set to always=true!).</param>
        public virtual void AddEntry(IWeightObject entry, float probability, bool unique, bool always, bool enabled)
        {
            contents.Add(entry);
            entry.Probability = probability;
            entry.IsUnique = unique;
            entry.DropAlways = always;
            entry.IsEnabled = enabled;
            entry.ObjectTable = this;
        }

        /// <summary>
        /// Removes the given entry from the contents. If it is not part of the contents, an exception occurs.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public virtual void RemoveEntry(IWeightObject entry)
        {
            contents.Remove(entry);
            entry.ObjectTable = null;
        }

        /// <summary>
        /// Removes the entry at the given index position.
        /// If the index is out-of-range of the current contents collection, an exception occurs.
        /// </summary>
        /// <param name="index">The index.</param>
        public virtual void RemoveEntry(int index)
        {
            IWeightObject entry = contents[index];
            entry.ObjectTable = null;
            contents.RemoveAt(index);
        }

        private void AddToResult(List<IWeightObject> rv, IWeightObject o)
        {
            if (!o.IsUnique || !uniquedrops.Contains(o))
            {
                if (o.IsUnique)
                    uniquedrops.Add(o);

                if (!(o is WeightNullValue))
                {
                    if (o is IWeightTable)
                    {
                        rv.AddRange(((IWeightTable)o).result);
                    }
                    else
                    {
                        // INSTANCECHECK
                        // Check if the object to add implements IRDSObjectCreator.
                        // If it does, call the CreateInstance() method and add its return value
                        // to the result set. If it does not, add the object o directly.
                        IWeightObject adder = o;
                        if (o is IWeightObjectCreator)
                            adder = ((IWeightObjectCreator)o).CreateInstance();

                        rv.Add(adder);
                        o.OnHit(EventArgs.Empty);
                    }
                }
                else
                    o.OnHit(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the result. Calling this method will start the random pick process and generate the result.
        /// This result remains constant for the lifetime of this table object.
        /// Use the ResetResult method to clear the result and create a new one.
        /// </summary>
        public virtual IEnumerable<IWeightObject> result
        {
            get
            {
                // The return value, a list of hit objects
                List<IWeightObject> rv = new List<IWeightObject>();
                uniquedrops = new List<IWeightObject>();

                // Do the PreEvaluation on all objects contained in the current table
                // This is the moment where those objects might disable themselves.
                foreach (IWeightObject o in loot)
                    o.OnPreResultEvaluation(EventArgs.Empty);

                // Add all the objects that are hit "Always" to the result
                // Those objects are really added always, no matter what "Count"
                // is set in the table! If there are 5 objects "always", those 5 will
                // drop, even if the count says only 3.
                foreach (IWeightObject o in loot.Where(e => e.DropAlways && e.IsEnabled))
                    AddToResult(rv, o);

                // Now calculate the real dropcount, this is the table's count minus the
                // number of Always-drops.
                // It is possible, that the remaining drops go below zero, in which case
                // no other objects will be added to the result here.
                int alwayscnt = loot.Count(e => e.DropAlways && e.IsEnabled);
                int realdropcnt = ResultCount - alwayscnt;

                // Continue only, if there is a Count left to be processed
                if (realdropcnt > 0)
                {
                    for (int dropcount = 0; dropcount < realdropcnt; dropcount++)
                    {
                        // Find the objects, that can be hit now
                        // This is all objects, that are Enabled and that have not already been added through the Always flag
                        IEnumerable<LootObject> dropables = loot.Where(e => e.IsEnabled && !e.DropAlways);

                        // This is the magic random number that will decide, which object is hit now
                        double hitvalue = WeightRandom.GetValue(dropables.Sum(e => e.Probability));

                        // Find out in a loop which object's probability hits the random value...
                        double runningvalue = 0;
                        foreach (IWeightObject o in dropables)
                        {
                            // Count up until we find the first item that exceeds the hitvalue...
                            runningvalue += o.Probability;
                            if (hitvalue < runningvalue)
                            {
                                // ...and the oscar goes too...
                                AddToResult(rv, o);
                                break;
                            }
                        }
                    }
                }

                // Now give all objects in the result set the chance to interact with
                // the other objects in the result set.
                ResultEventArgs rea = new ResultEventArgs(rv);
                foreach (IWeightObject o in rv)
                    o.OnPostResultEvaluation(rea);

                // Return the set now
                return rv;
            }
        }
    }
}
