using System;
using System.Collections.Generic;
using System.Text;

namespace CreateTableHelper
{
    public static class Extension
    {
        public static string ParseFields(this List<Field> fields)
        {
            string result = string.Empty;
            foreach (var item in fields)
            {
                result += $"{item.Name}  {item.Type}  {ParseFieldIsNull(item.IsNull)} , \r\n ";

            }
            return result;
        }
        public static string ParseFieldIsNull(this bool isNull)
        {
            return isNull ? "NULL" : "NOT NULL";
        }

        public static bool ConvertFieldIsNull(this string isNull)
        {
            isNull = isNull.ToLower() == "f" ? "false" : "true";
            return Convert.ToBoolean(isNull);
        }

        public static string CreateDescripton(this Table table, string descriptionTemplate)
        {
            string description = $@"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{table.TableDescription}', @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{table.TableName}' 
GO";
            description += "\r\n";
            description += $@" EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'序号' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{table.TableName}', @level2type=N'COLUMN',@level2name=N'Id'
GO";
            description += "\r\n";

            // 添加自定义字段
            foreach (var item in table.Fields)
            {
                description += string.Format(descriptionTemplate, item.Description, table.TableName, item.Name) + " \r\n ";
            }


            description += $@"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'创建时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{table.TableName}', @level2type=N'COLUMN',@level2name=N'CreationTime'
GO";
            description += "\r\n";
            description += $@"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最后修改时间' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{table.TableName}', @level2type=N'COLUMN',@level2name=N'LastModifycationTime'
GO";
            description += "\r\n";
            description += $@" EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'RowVersion' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{table.TableName}', @level2type=N'COLUMN',@level2name=N'RowVersion'
GO";
            description += "\r\n";
            description += $@"EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否删除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{table.TableName}', @level2type=N'COLUMN',@level2name=N'IsDeleted'
GO";

            return description;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string CreateEntity(this Table table)
        {
            string eneity = $"public class {table.TableName} " + "\r\n";
            eneity += "{";
            eneity += "\r\n";

            eneity = CreateProperty(eneity, "int", "Id", "主键Id");

            foreach (var item in table.Fields)
            {
                string type = ConvertEntityPropertyType(item.Type);
                eneity = CreateProperty(eneity, item.Type, item.Name, item.Description);
            }

            eneity = CreateProperty(eneity, "DateTime", "CreationTime", "创建时间");
            eneity = CreateProperty(eneity, "DateTime", "LastModifycationTime", "最后修改时间");
            eneity = CreateProperty(eneity, "bool", "IsDeleted", "是否删除");

            eneity += "}";

            return eneity;
        }

        private static string CreateProperty(string entity , string type ,string name,string description)
        {
            type = ConvertEntityPropertyType(type);
            entity += "///<summary> \r\n";
            entity += "///" + "  " + description + "\r\n";
            entity += "///</summary> \r\n";
            entity += "public " + type + " " + name + " {get; set;}";
            entity += "\r\n";
            return entity;
        }


        private static string ConvertEntityPropertyType(string tempType)
        {
            string type = "string";
            string temp = tempType.ToLower();
            if (temp.StartsWith("varchar") || temp.StartsWith("nvarchar") || temp.StartsWith("char") || temp.StartsWith("ncahr"))
            {
                type = "string";
            }
            else if (temp.StartsWith("int") || temp.StartsWith("tinyint") || temp.StartsWith("smallint"))
            {
                type = "int";
            }
            else if (temp.StartsWith("datetime"))
            {
                type = "DateTime";
            }
            else if (temp.StartsWith("bit"))
            {
                type = "bool";
            }
            return type;
        }
    }
}
