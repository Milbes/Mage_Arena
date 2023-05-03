using System;
using System.Collections.Generic;

namespace Watermelon
{
    /// <summary>
    /// This interface contains the properties an object must have to be a valid rds result object.
    /// </summary>
    public interface IWeightObject
    {
        /// <summary>
        /// Gets or sets the probability for this object to be (part of) the result
        /// </summary>
        float Probability { get; set; }

        /// <summary>
        /// Gets or sets whether this object may be contained only once in the result set
        /// </summary>
        bool IsUnique { get; set; }

        /// <summary>
        /// Gets or sets whether this object will always be part of the result set
        /// (Probability is ignored when this flag is set to true)
        /// </summary>
        bool DropAlways { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="IWeightObject"/> is enabled.
        /// Only enabled entries can be part of the result of a RDSTable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if enabled; otherwise, <c>false</c>.
        /// </value>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the table this Object belongs to.
        /// Note to inheritors: This property has to be auto-set when an item is added to a table via the AddEntry method.
        /// </summary>
        IWeightTable ObjectTable { get; set; }

        /// <summary>
        /// Occurs before all the probabilities of all items of the current RDSTable are summed up together.
        /// This is the moment to modify any settings immediately before a result is calculated.
        /// </summary>
        event EventHandler preResultEvaluation;
        /// <summary>
        /// Occurs when this RDSObject has been hit by the Result procedure.
        /// (This means, this object will be part of the result set).
        /// </summary>
        event EventHandler resultHit;
        /// <summary>
        /// Occurs after the result has been calculated and the result set is complete, but before
        /// the RDSTable's Result method exits.
        /// </summary>
        event ResultEventHandler postResultEvaluation;

        /// <summary>
        /// Raises the <see cref="E:PreResultEvaluation"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnPreResultEvaluation(EventArgs e);
        /// <summary>
        /// Raises the <see cref="E:Hit"/> event.
        /// </summary>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        void OnHit(EventArgs e);
        /// <summary>
        /// Raises the <see cref="E:PostResultEvaluation"/> event.
        /// </summary>
        /// <param name="e">The <see cref="rds.ResultEventArgs"/> instance containing the event data.</param>
        void OnPostResultEvaluation(ResultEventArgs e);
    }

    /// <summary>
    /// This interface holds a method that creates an instance of an object where it is implemented.
    /// If an object gets hit by RDS, it checks whether it is an ORDSObjectCreator. If yes, the result
    /// of .CreateInstance() is added to the result; if not, the object itself is returned.
    /// </summary>
    public interface IWeightObjectCreator : IWeightObject
    {
        /// <summary>
        /// Creates an instance of the object where this method is implemented in.
        /// Only paramaterless constructors are supported in the base implementation.
        /// Override (without calling base.CreateInstance()) to instanciate more complex constructors.
        /// </summary>
        /// <returns>A new instance of an object of the type where this method is implemented</returns>
        IWeightObject CreateInstance();
    }

    /// <summary>
    /// This interface describes a table of IRDSObjects. One (or more) of them is/are picked as the result set.
    /// </summary>
    public interface IWeightTable
    {
        /// <summary>
        /// The maximum number of entries expected in the Result. The final count of items in the result may be lower
        /// if some of the entries may return a null result (no drop)
        /// </summary>
        int ResultCount { get; set; }

        /// <summary>
        /// Gets or sets the contents of this table
        /// </summary>
        IEnumerable<IWeightObject> Contents { get; }

        /// <summary>
        /// Gets the result. Calling this method will start the random pick process and generate the result.
        /// This result remains constant for the lifetime of this table object.
        /// Use the ResetResult method to clear the result and create a new one.
        /// </summary>
        IEnumerable<IWeightObject> result { get; }
    }

    /// <summary>
    /// Generic template for classes that return only one value, which will be good enough in most use cases.
    /// </summary>
    /// <typeparam name="T">The type of the value of this object</typeparam>
    public interface IWeightValue<T> : IWeightObject
    {
        /// <summary>
        /// The value of this object
        /// </summary>
        T value { get; set; }
    }
}