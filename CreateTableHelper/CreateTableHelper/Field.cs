using System;
using System.Collections.Generic;
using System.Text;

namespace CreateTableHelper
{
    public class Field
    {

        public string Name { get; set; }

        public string Type { get; set; }

        public bool IsNull { get; set; }

        public string Description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string  Default {get;set;}
    }
}
