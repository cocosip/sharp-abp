using System;

namespace SharpAbp.Abp.Data
{
    /// <summary>
    /// Marks a string or byte[] property as a large text column. The SharpAbp EntityFrameworkCore
    /// large text column convention maps the property to the large object type of the database (for
    /// example NCLOB on Oracle, longtext on MySQL, CLOB on DM and text on PostgreSQL), even when the
    /// property has no maximum length configured.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class LargeTextColumnAttribute : Attribute
    {

    }
}
