using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceTool
{
    /// <summary>
    /// Holds names and values for a command line argument.
    /// </summary>
    public class CommandLineArgument
    {
        /// <summary>
        /// Abbreviated argument, usually one letter.
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Long name of argument, usually more than one letter.
        /// </summary>
        public string LongName { get; set; }

        /// <summary>
        /// String value of the arugment
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// Get the value of the argument as a nullable int, null if not an integer.
        /// </summary>
        public int? AsInt
        {
            get
            {
                int intValue = 0;
                if (Value != null && Int32.TryParse(Value, out intValue))
                {
                    return intValue;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the value of the argument as a nullable boolean, null if not a boolean.
        /// </summary>
        public bool? AsBool
        {
            get
            {
                bool boolValue = false;
                if (Value != null && Boolean.TryParse(Value, out boolValue))
                {
                    return boolValue;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Get the value of the argument as a nullable decimal, null if not an decimal.
        /// </summary>
        public decimal? AsDecimal
        {
            get
            {
                decimal decimalValue = 0;
                if (Value != null && Decimal.TryParse(Value, out decimalValue))
                {
                    return decimalValue;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Holds names and values for a command line argument.
        /// </summary>
        /// <param name="shortname">Abbreviated argument, usually one letter.</param>
        /// <param name="longName">Optional long name of argument, usually more than one letter.</param>
        public CommandLineArgument(String shortname, String longName = null)
        {
            ShortName = "-" + shortname;
            LongName = "-" + longName;
        }
    }
}
