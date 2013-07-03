using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using QuickBloxSDK_Silverlight.Core;

namespace QuickBloxSDK_Silverlight.owners
{
    public class OwnerServiceEventArgs
    {
        /// <summary>
        /// Returned object
        /// </summary>
        public object result
        { get; set; }

        /// <summary>
        /// Returned object type
        /// </summary>
        public Type t
        { get; set; }

        /// <summary>
        /// Operation execution status
        /// </summary>
        public Status status
        { get; set; }

        /// <summary>
        /// Command that has been done at present time
        /// </summary>
        public OwnerServiceCommand currentCommand
        { get; set; }

        /// <summary>
        /// Error message if any exists.
        /// Must be guided by status.
        /// 
        /// </summary>
        public string errorMessage
        { get; set; }
    }
}
