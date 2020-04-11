using System;
using System.Collections.Generic;
using System.Text;

namespace CreateTableHelper
{
    public class Table
    {
        public string DbName { get; set; }

        public string TableName { get; set; }

        public string TableDescription { get; set; }

        public List<Field> Fields { get; set; }
    }
}
