using System;
using System.Collections.Generic;
using System.Text;

namespace ORM
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {
    }
}
