using System;
using System.Collections.Generic;
using System.Text;
namespace CreateTableHelper
{
    class Program
    {
        private static Table table = new Table
        {
            Fields = new List<Field>()
        };
        static string fieldTemplate =
         @"Use {0}
             Go
             CREATE TABLE {1}
             (
                    Id int primary key identity(1, 1) NOT NULL,
                    {2}
                    CreationTime  datetime default(getdate())  NOT NULL,
                    LastModifycationTime datetime default(getdate())  NOT NULL,
                    RowVersion rowversion not null,
                    IsDeleted tinyint not null default(0)
             )";

        static string descriptionTemplate = @" EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'{0}' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'{1}', @level2type=N'COLUMN',@level2name=N'{2}' 
GO";
        static void Main(string[] args)
        {
            WriteLine("为了方便生成建表sql且不忘记公司规定的默认字段所以有了此工具!", ConsoleColor.Green);
            WriteLine("生成的sql,默认带有 IsDeleted | RowVersion | LastModifycationTime | CreationTime | Id 字段", ConsoleColor.Green);
            WriteLine("命令说明", ConsoleColor.Red);
            WriteLine("--sql 创建sql语句", ConsoleColor.Red);
            WriteLine("--entity 创建实体类", ConsoleColor.Red);
            WriteLine("--field 查看当前所有字段", ConsoleColor.Red);
            WriteLine("--field-delete 索引  删除字段", ConsoleColor.Red);

            WriteLine("请输入库名:", ConsoleColor.Yellow);
            table.DbName = Console.ReadLine();
            WriteLine("请输入表名:", ConsoleColor.Yellow);
            table.TableName = Console.ReadLine();
            WriteLine("请输入表描述:", ConsoleColor.Yellow);
            table.TableDescription = Console.ReadLine();

            WriteLine("开输入表字段:", ConsoleColor.Green);
            WriteLine("格式::", ConsoleColor.Green);
            WriteLine("字段名 类型 是否可空(false true 简写t f) 描述 ", ConsoleColor.Green);
            WriteLine("中间以一个空格分割", ConsoleColor.Green);

            Console.WriteLine("举例:");
            Console.WriteLine(" OrderStatus int false  订单状态");



            while (true)
            {
                string inputString = Console.ReadLine();
                // 创建sql语句
                if (inputString.Equals("--sql"))
                {
                    Console.Clear();

                    string sql = string.Format(fieldTemplate, table.DbName, table.TableName, table.Fields.ParseFields());
                    string description = table.CreateDescripton(descriptionTemplate);
                    Console.WriteLine(sql + "\r\n \r\n" + description);
                }
                //  展示目前所有字段
                else if (inputString.Equals("--field"))
                {
                    Console.Clear();
                    foreach (var item in table.Fields)
                    {
                        Console.WriteLine($"{item.Name} | { item.Type} | {item.IsNull.ParseFieldIsNull()} | {item.Description}");
                    }
                }
                else if (inputString.StartsWith("--field-delete"))
                {
                    try
                    {
                        Console.Clear();
                        string[] temp = inputString.Split(' ');
                        int index = Convert.ToInt32(temp[1]);
                        table.Fields.RemoveAt(index);
                    }
                    catch
                    {
                    }
                }
                else if (inputString.Equals("--entity"))
                {
                    string enetity = table.CreateEntity();

                    Console.WriteLine(enetity);
                }
                else
                {
                    try
                    {
                        // 继续添加字段
                        string[] temp = inputString.Split(' ');
                        Field field = new Field
                        {
                            Name = temp[0],
                            Type = temp[1].ToUpper(),
                            IsNull = temp[2].ConvertFieldIsNull(),
                            Description = temp[3]
                        };
                        table.Fields.Add(field);
                    }
                    catch
                    {

                    }
                }

            }
        }


        private static void WriteLine(string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

    }
}
