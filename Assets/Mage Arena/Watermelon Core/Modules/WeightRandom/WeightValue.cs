using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Watermelon
{
    /// <summary>
    /// This class holds a single RDS value.
    /// It's a generic class to allow the developer to add any type to a RDSTable.
    /// T can of course be either a value type or a reference type, so it's possible,
    /// to add RDSValue objects that contain a reference type, too.
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    public class WeightValue<T> : IWeightValue<T>
    {

        #region CONSTRUCTOR
        /// <summary>
        /// Initializes a new instance of the <see cref="WeightValue&lt;T&gt;"/> class.
        /// The Unique and Always flags are set to (default) false with this constructor, and Enabled is set to true.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="probability">The probability.</param>
        public WeightValue(T value, float probability)
            : this(value, probability, false, false, true)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeightValue&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="probability">The probability.</param>
        /// <param name="unique">if set to <c>true</c> [unique].</param>
        /// <param name="always">if set to <c>true</c> [always].</param>
        /// <param name="enabled">if set to <c>true</c> [enabled].</param>
        public WeightValue(T value, float probability, bool unique, bool always, bool enabled)
        {
            mvalue = value;
            this.Probability = probability;
            IsUnique = unique;
            DropAlways = always;
            IsEnabled = enabled;
            ObjectTable = null;
        }
        #endregion

        #region EVENTS
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
        #endregion

        #region IRDSValue<T> Members
        /// <summary>
        /// The value of this object
        /// </summary>
        public virtual T value
        {
            get { return mvalue; }
            set { mvalue = value; }
        }
        private T mvalue;
        #endregion

        #region IRDSObject Members
        /// <summary>
        /// Gets or sets the probability for this object to be (part of) the result
        /// </summary>
        public float Probability { get; set; }
        /// <summary>
        /// Gets or sets whether this object may be contained only once in the result set
        /// </summary>
        public bool IsUnique { get; set; }
        /// <summary>
        /// Gets or sets whether this object will always be part of the result set
        /// (Probability is ignored when this flag is set to true)
        /// </summary>
        public bool DropAlways { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IWeightObject"/> is enabled.
        /// Only enabled entries can be part of the result of a RDSTable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsEnabled { get; set; }
        /// <summary>
        /// Gets or sets the table this Object belongs to.
        /// Note to inheritors: This property has to be auto-set when an item is added to a table via the AddEntry method.
        /// </summary>
        public IWeightTable ObjectTable { get; set; }
        #endregion
    }
}