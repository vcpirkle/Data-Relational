using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataRelational.Search
{
    /// <summary>
    /// A single element of a complex search.
    /// </summary>
    public class ComplexSearchElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the field name.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets the value of the field.
        /// </summary>
        public object Value
        {
            get
            {
                if (this.Values != null && this.Values.Length > 0)
                    return this.Values[0];
                return null;
            }
            set
            {
                if (value != null && value.GetType().IsArray)
                {
                    Array array = (Array)value;
                    this.Values = new object[array.Length];
                    for (int i = 0; i < array.Length; i++)
                        this.Values[i] = array.GetValue(i);
                }
                else
                {
                    this.Values = new object[] { value };
                }
            }
        }

        /// <summary>
        /// Gets or sets the values of the field.
        /// </summary>
        public object[] Values { get; set; }

        /// <summary>
        /// Gets or sets the operator to perform on the search element.
        /// </summary>
        public ComplexSearchOperator SearchOperator { get; set; }

        /// <summary>
        /// Gets or sets the search type to perform on the search element.
        /// </summary>
        public ComplexSearchType SearchType { get; set; }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new instance of the DataRelational.Search.ComplexSearchElement class.
        /// </summary>
        /// <param name="fieldName">The field name.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="searchType">The search type to perform on the search element.</param>
        /// <param name="searchOperator">The operator to perform on the search element.</param>
        public ComplexSearchElement(string fieldName, object value,ComplexSearchType searchType, ComplexSearchOperator searchOperator)
        {
            this.FieldName = fieldName;
            this.Value = value;
            this.SearchType = searchType;
            this.SearchOperator = searchOperator;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the string representation of the complex search element.
        /// </summary>
        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// Gets the string representation of the complex search element.
        /// </summary>
        /// <param name="includeType">Whether to include the search type. (And / Or)</param>
        public string ToString(bool includeType)
        {
            return ToString(includeType, null);
        }

        /// <summary>
        /// Gets the string representation of the complex search element.
        /// </summary>
        /// <param name="includeType">Whether to include the search type. (And / Or)</param>
        /// <param name="objAlias">The alias of the object.</param>
        public string ToString(bool includeType, string objAlias)
        {
            StringBuilder sb = new StringBuilder();

            if (includeType)
                sb.Append(SearchType == ComplexSearchType.And ? " AND " : " OR ");

            sb.Append(GetFieldName(objAlias) + " ");
            switch (SearchOperator)
            {
                case ComplexSearchOperator.Equal:
                    sb.Append("= ");
                    sb.Append(GetToStrings(false)[0]);
                    break;
                case ComplexSearchOperator.NotEqual:
                    sb.Append("<> ");
                    sb.Append(GetToStrings(false)[0]);
                    break;
                case ComplexSearchOperator.Like:
                    sb.Append("LIKE ");
                    sb.Append(GetToStrings(true)[0]);
                    break;
                case ComplexSearchOperator.GreaterThan:
                    sb.Append("> ");
                    sb.Append(GetToStrings(false)[0]);
                    break;
                case ComplexSearchOperator.GreaterThanOrEqual:
                    sb.Append(">= ");
                    sb.Append(GetToStrings(false)[0]);
                    break;
                case ComplexSearchOperator.LessThan:
                    sb.Append("< ");
                    sb.Append(GetToStrings(false)[0]);
                    break;
                case ComplexSearchOperator.LessThanOrEqual:
                    sb.Append("<= ");
                    sb.Append(GetToStrings(false)[0]);
                    break;
                case ComplexSearchOperator.Between:
                    sb.Append("BETWEEN ");
                    string[] vals = GetToStrings(false);
                    sb.Append(vals[0] + " AND " + vals[1]);
                    break;
                case ComplexSearchOperator.In:
                    sb.Append("IN ");
                    sb.Append("(");
                    string[] vals2 = GetToStrings(false);
                    foreach (string val in vals2)
                        sb.Append(val + ", ");
                    if (vals2.Length > 0)
                        sb.Length -= 2;
                    sb.Append(")");
                    break;
            }
            return sb.ToString();
        }

        private string[] GetToStrings(bool isLike)
        {
            List<string> strings = new List<string>();
            foreach (object obj in this.Values)
            {
                if (obj == null)
                    strings.Add("NULL");
                else
                {
                    if (obj is DateTime || obj is Guid || obj is string)
                    {
                        strings.Add((isLike ? "'%" : "'") + obj.ToString().Replace("'", "''") + (isLike ? "%'" : "'"));
                    }
                    else if (obj is bool)
                    {
                        strings.Add(obj.ToString().Replace("True", "1").Replace("False", "0"));
                    }
                    else
                    {
                        strings.Add(obj.ToString().Replace("'", "''"));
                    }
                }
            }
            return strings.ToArray();
        }

        private string GetFieldName(string objAlias)
        {
            StringBuilder sb = new StringBuilder();
            int dotIndex = FieldName.LastIndexOf(".");
            if (dotIndex > 0)
            {
                if (objAlias == null)
                {
                    int prevDotIndex = FieldName.IndexOf(".");
                    if (prevDotIndex != dotIndex)
                    {
                        while (prevDotIndex != dotIndex)
                        {
                            int nextIndex = FieldName.IndexOf(".", prevDotIndex + 1);
                            if (nextIndex == dotIndex)
                                break;
                            prevDotIndex = nextIndex;
                        }
                        string str = FieldName.Substring(prevDotIndex + 1);
                        string var = str.Substring(0, str.IndexOf("."));
                        string fld = str.Substring(str.IndexOf(".") + 1);
                        sb.Append(var + "." + "[" + fld);
                    }
                    else
                    {
                        sb.Append("[");
                        sb.Append(FieldName);
                    }
                }
                else
                {
                    sb.Append(objAlias + ".");
                    string[] parts = FieldName.Split(".".ToCharArray());
                    sb.Append("[");
                    sb.Append(parts[parts.Length-1]);
                }
            }
            else
            {
                if(objAlias != null)
                    sb.Append(objAlias + ".");
                sb.Append("[");
                sb.Append(FieldName);
            }
            sb.Append("]");
            return sb.ToString();
        }

        #endregion Methods
    }

    /// <summary>
    /// A complex search element collection
    /// </summary>
    public class ComplexSearchElementCollection : List<ComplexSearchElement>
    {
        /// <summary>
        /// Finds the first element matching the field name.
        /// </summary>
        /// <param name="fieldName">The field name to find.</param>
        /// <returns>The first element matching the field name.</returns>
        public ComplexSearchElement Find(string fieldName)
        {
            foreach (ComplexSearchElement element in this)
            {
                if (element.FieldName.ToLower() == fieldName.ToLower())
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the string representation of the complex search element collection.
        /// </summary>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Count; i++)
            {
                sb.Append(i == 0 ? this[i].ToString() : this[i].ToString(true));
            }
            return sb.ToString();
        }
    }
}
