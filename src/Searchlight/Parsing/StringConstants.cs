﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Searchlight.Parsing
{
    /// <summary>
    /// This class contains all whitelisted SQL tokens that can be placed into a legitimate SQL string
    /// </summary>
    public class StringConstants
    {
        /// <summary>
        /// Represents the list of query expressions we would recognize if the user passed them in a filter.
        /// The "KEY" represents the value we allow the user to provide.
        /// The "VALUE" represents the actual string we will place in the SQL.
        /// </summary>
        public static readonly Dictionary<string, OperationType> RECOGNIZED_QUERY_EXPRESSIONS = new Dictionary<string, OperationType> 
        {
            // Basic SQL query expressions
            { "=",  OperationType.Equals  },
            { ">",  OperationType.GreaterThan  },
            { ">=", OperationType.GreaterThanOrEqual },
            { "<>", OperationType.NotEqual },
            { "!=", OperationType.NotEqual },
            { "<",  OperationType.LessThan },
            { "<=", OperationType.LessThanOrEqual },

            // Microsoft's published REST standard alternatives for query expressions
            { "EQ", OperationType.Equals },
            { "GT", OperationType.GreaterThan  },
            { "GE", OperationType.GreaterThanOrEqual },
            { "NE", OperationType.NotEqual },
            { "LT", OperationType.LessThan },
            { "LE", OperationType.LessThanOrEqual },

            // Slightly less common query expressions
            { "BETWEEN",        OperationType.Between     },
            { "IN",             OperationType.In          },
            { "LIKE",           OperationType.Like        },
            { "STARTSWITH",     OperationType.StartsWith  },
            { "CONTAINS",       OperationType.Contains    },
            { "ENDSWITH",       OperationType.EndsWith    },
            { "IS",             OperationType.IsNull      },
        };

        /// <summary>
        /// Represents the list of query expressions we would recognize if the user passed them in a filter
        /// </summary>

        /// <summary>
        /// Represents the list of tokens that can close an "IN" clause
        /// </summary>
        public static readonly string[] SAFE_LIST_TOKENS = new string[] { ",", ")" };

        /// <summary>
        /// Represents the list of conjunctions that can occur between tests, and the insertion values that we should apply
        /// </summary>
        public static readonly Dictionary<string, string> SAFE_CONJUNCTIONS = new Dictionary<string, string>
        {
            { "(", "(" },
            { ")", ")" },
            { "AND", " AND " },
            { "OR", " OR " },
            { "NOT", " NOT " },
        };

        /// <summary>
        /// Represents the list of conjunctions that can safely terminate a statement
        /// </summary>
        public static readonly string[] SAFE_ENDING_CONJUNCTIONS = new string[] { ")" };

        /// <summary>
        /// Represents the list of single-character operators for tokenization
        /// </summary>
        public static readonly char[] SINGLE_CHARACTER_OPERATORS = new char[] { '=', '>', '<', '(', ')', ',', '!' };

        /// <summary>
        /// Represents the list of allowed sort-by orders
        /// </summary>
        public static readonly string[] SAFE_SORT_BY = new string[] { "ASC", "DESC" };

        /// <summary>
        /// Represents a single quote character for tokenization of strings
        /// </summary>
        public static readonly char SINGLE_QUOTE = '\'';

        /// <summary>
        /// Represents an open parenthesis character for tokenization of strings
        /// </summary>
        public static readonly string OPEN_PARENTHESIS = "(";

        /// <summary>
        /// Represents a close parenthesis character for tokenization of strings
        /// </summary>
        public static readonly string CLOSE_PARENTHESIS = ")";
    }
}
