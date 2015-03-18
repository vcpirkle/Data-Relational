using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Attributes
{
    /// <summary>
    /// Specifies that the type is a storeable data relational object.  Objects are always saved, soft deleted, not cached, and first level updated by default.
    /// </summary>
    [AttributeUsageAttribute(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class SavableObjectAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the DataRelational.Attributes.SavableObjectAttribute class.
        /// </summary>
        public SavableObjectAttribute()
        {
            this.SaveBehavior = SaveBehavior.Always;
            this.DeleteBehavior = DeleteBehavior.Soft;
            this.CacheBehavior = CacheBehavior.NoCahce;
            this.UpdateBehavior = UpdateBehavior.FirstLevel;
        }

        /// <summary>
        /// Gets or sets the save behavior
        /// </summary>
        public SaveBehavior SaveBehavior { get; set; }

        /// <summary>
        /// Gets or sets the delete behavior
        /// </summary>
        public DeleteBehavior DeleteBehavior { get; set; }

        /// <summary>
        /// Gets or sets the cache behavior
        /// </summary>
        public CacheBehavior CacheBehavior { get; set; }

        /// <summary>
        /// Gets or sets the update behavior
        /// </summary>
        public UpdateBehavior UpdateBehavior { get; set; }
    }
}
