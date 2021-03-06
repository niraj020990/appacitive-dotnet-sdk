﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appacitive.Sdk
{
    internal struct DateFieldValue : IFieldValue
    {
        public DateFieldValue(object o)
            : this()
        {
            this.Value = "date('" + ((DateTime)o).ToString(Formats.BirthDate) + "')";
        }

        public string Value { get; private set; }

        public string GetStringValue()
        {
            return this.Value;
        }
    }
}
