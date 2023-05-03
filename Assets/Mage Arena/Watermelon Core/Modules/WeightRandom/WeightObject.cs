using System;
using UnityEngine;

namespace Watermelon
{
    /// <summary>
    /// Base implementation of the IRDSObject interface.
    /// This class only implements the interface and provides all events required.
    /// Most methods are virtual and ready to be overwritten. Unless there is a good reason,
    /// do not implement IRDSObject for yourself, instead derive your base classes that shall interact
    /// in *any* thinkable way as a result source with any RDSTable from this class.
    /// </summary>
    [System.Serializable]
    public class WeightObject : IWeightObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeightObject"/> class.
        /// </summary>
        public WeightObject() : this(0)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightObject"/> class.
        /// </summary>
        /// <param name="probability">The probability.</param>
        public WeightObject(float probability) : this(probability, false, false, true)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightObject"/> class.
        /// </summary>
        /// <param name="probability">The probability.</param>
        /// <param name="unique">if set to <c>true</c> this object can only occur once per result.</param>
        /// <param name="always">if set to <c>true</c> [always] this object will appear always in the result.</param>
        /// <param name="enabled">if set to <c>false</c> [enabled] this object will never be part of the result (even if it is set to always=true!).</param>
        public WeightObject(float probability, bool unique, bool always, bool enabled)
        {
            this.Probability = probability;

            IsUnique = unique;
            DropAlways = always;
            IsEnabled = enabled;

            ObjectTable = null;
        }

        /// <summary>
        /// Occurs before all the probabilities of all items of the current RDSTable are summed up together.
        /// This is the moment to modify any settings immediately before a result is calculated.
        /// </summary>
        public event EventHandler preResultEvaluation;
        /// <summary>
        /// Occurs when this RDSObject has been hit by the Result procedure.
        /// (This means, this object will be part of the result set).
        /// </summary>
        public event EventHandler resultHit;
        /// <summary>
        /// Occurs after the result has been calculated and the result set is complete, but before
        /// the RDSTable's Result method exits.
        /// </summary>
        public event ResultEventHandler postResultEvaluation;

        /// <summary>
        /// Raises the <see cref="E:PreResultEvaluation"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public virtual void OnPreResultEvaluation(EventArgs e)
        {
            if (preResultEvaluation != null) preResultEvaluation(this, e);
        }
        /// <summary>
        /// Raises the <see cref="E:Hit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public virtual void OnHit(EventArgs e)
        {
            if (resultHit != null) resultHit(this, e);
        }
        /// <summary>
        /// Raises the <see cref="E:PostResultEvaluation"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        public virtual void OnPostResultEvaluation(ResultEventArgs e)
        {
            if (postResultEvaluation != null) postResultEvaluation(this, e);
        }

        /// <summary>
        /// Gets or sets the probability for this object to be (part of) the result
        /// </summary>
        [SerializeField]
        private float probability = 0;
        public float Probability
        {
            get { return probability; }
            set { probability = value; }
        }

        /// <summary>
        /// Gets or sets whether this object may be contained only once in the result set
        /// </summary>
        [SerializeField] bool isUnique = true;
        public bool IsUnique
        {
            get { return isUnique; }
            set { isUnique = value; }
        }

        /// <summary>
        /// Gets or sets whether this object will always be part of the result set
        /// (Probability is ignored when this flag is set to true)
        /// </summary>
        [SerializeField] bool dropAlways = false;
        public bool DropAlways
        {
            get { return dropAlways; }
            set { dropAlways = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IWeightObject"/> is enabled.
        /// Only enabled entries can be part of the result of a RDSTable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        [SerializeField] bool isEnabled = true;
        public bool IsEnabled
        {
            get { return isEnabled; }
            set { isEnabled = value; }
        }

        /// <summary>
        /// Gets or sets the table this Object belongs to.
        /// Note to inheritors: This property has to be auto-set when an item is added to a table via the AddEntry method.
        /// </summary>
        [SerializeField] IWeightTable objectTable;
        public IWeightTable ObjectTable
        {
            get { return objectTable; }
            set { objectTable = value; }
        }
    }
}