using System;
using System.Collections.Generic;

namespace Watermelon
{

    #region RESULTEVENTARGS & DELEGATE
    /// <summary>
    /// EventArgs for the PostResultEvaluation event
    /// </summary>
    public class ResultEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResultEventArgs"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public ResultEventArgs(IEnumerable<IWeightObject> result)
        {
            Result = result;
        }

        /// <summary>
        /// Gets the result.
        /// </summary>
        public IEnumerable<IWeightObject> Result { get; private set; }
    }

    /// <summary>
    /// EventHandler for the PostResultEvaluation event
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="rds.ResultEventArgs"/> instance containing the event data.</param>
    public delegate void ResultEventHandler(object sender, ResultEventArgs e);
    #endregion
}