using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DataRelational
{
    /// <summary>
    /// Contains cross thread safe methods for invoking delegates.
    /// </summary>
    class SafeInvoke
    {
        /// <summary>
        /// Safely invokes a delegate ensuring cross thread safety and control handle state.
        /// </summary>
        /// <param name="del">The delegate to be invoked.</param>
        /// <param name="param">The paramaters of the target delegate method.</param>
        public static object Invoke(Delegate del, object[] param)
        {
            return Invoke(del, null, param);
        }

        /// <summary>
        /// Safely invokes a delegate ensuring cross thread safety and control handle state.
        /// </summary>
        /// <param name="del">The delegate to be invoked.</param>
        /// <param name="invk">The object that will invoke the delegate.</param>
        /// <param name="param">The paramaters of the target delegate method.</param>
        public static object Invoke(Delegate del, ISynchronizeInvoke invk, object[] param)
        {
            if (del != null)
            {
                if (invk != null && invk.InvokeRequired)
                {
                    return invk.Invoke(del, param);
                }
                else
                {
                    return del.DynamicInvoke(param);
                }
            }
            return null;
        }
    }
}
