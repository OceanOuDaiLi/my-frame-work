using Excel;
using System;
using System.IO;
using UnityEngine;
using System.Data;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;

public class ExcelUtility
{
    public static Dictionary<string, object> targetClass = new Dictionary<string, object>();

    // cs脚本生成路径
    private static string CSharpSctiptPath = Application.dataPath + @"\Scripts\Common\Model\Base\GenTableData.cs";

    // 表格数据集合
    private DataSet mResultSet;

    public ExcelUtility(string excelFile)
    {
        FileStream mStream = File.Open(excelFile, FileMode.Open, FileAccess.Read);
        IExcelDataReader mExcelReader = ExcelReaderFactory.CreateOpenXmlReader(mStream);
        mResultSet = mExcelReader.AsDataSet();
    }


    public static void InitCSharpScript()
    {
        System.IO.File.WriteAllText(CSharpSctiptPath, string.Empty);
    }

    public void RecordCSharpFile(string className, string paramType, string paramName, string paramComment)
    {
        if (paramComment.Length > 0)
        {
            ExcelTools.cSharpTemplet += "    /// <summary>\n";
            //注释，每一行都增加注释
            ExcelTools.cSharpTemplet += "    /// " + paramComment.Replace("\n", "\n///") + "\n";
            ExcelTools.cSharpTemplet += "    /// <summary>\n";
        }
        ExcelTools.cSharpTemplet += "    public " + paramType + " " + paramName + ";" + "\n";
    }

    public void CreateCSharpBaseDateFile(string excelName, bool isEnd)
    {
        // 判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;

        // 默认读取第一个sheet数据表
        DataTable mSheet = mResultSet.Tables[0];

        // 判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return;

        ExcelTools.cSharpTemplet += "\n";
        ExcelTools.cSharpTemplet += "//" + excelName + "\n";
        ExcelTools.cSharpTemplet += "public class " + excelName + " : ModelData" + "{" + "\n";

        // 读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;
        // 存储整个表的数据
        List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

        // 读取数据
        for (int i = 3; i < rowCount; i++)
        {
            // 准备一个字典存储每一行的数据
            Dictionary<string, object> row = new Dictionary<string, object>();
            for (int j = 1; j < colCount; j++)
            {
                // 读取第1行数据作为表头字段
                string field = mSheet.Rows[0][j].ToString();

                // Key-Value对应
                if (!string.IsNullOrEmpty(field))
                    row[field] = mSheet.Rows[i][j];
            }

            //添加到表数据中
            table.Add(row);
        }

        //写入cs
        for (int j = 1; j < colCount; j++)
        {
            string paramName = mSheet.Rows[0][j].ToString();
            string paramType = mSheet.Rows[1][j].ToString();
            string paramComment = mSheet.Rows[2][j].ToString();

            // ModelData 默认带id.这里过滤下.
            if (string.IsNullOrEmpty(paramType) || string.IsNullOrEmpty(paramName) || paramName.ToLower().Equals("id"))
            {
                continue;
            }

            RecordCSharpFile(excelName, paramType, paramName, paramComment);
        }

        ExcelTools.cSharpTemplet += "\n" + "}";

        if (isEnd)
        {
            ExcelTools.cSharpTemplet += "\n" + "}";
            System.IO.File.WriteAllText(CSharpSctiptPath, ExcelTools.cSharpTemplet);
        }
    }

    #region Convert Methods

    /// <summary>
    /// 转换为Json.
    /// 分表: 一张配置表, 一个Json.
    /// </summary>
    public void ConvertToJson(string jsonPath, Encoding encoding)
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;

        //默认读取第一个数据表
        DataTable mSheet = mResultSet.Tables[0];

        //判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return;

        //读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;

        //准备一个列表存储整个表的数据
        List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

        //读取数据
        for (int i = 3; i < rowCount; i++)
        {
            //准备一个字典存储每一行的数据
            Dictionary<string, object> row = new Dictionary<string, object>();
            for (int j = 1; j < colCount; j++)
            {
                //读取第1行数据作为表头字段
                string field = mSheet.Rows[0][j].ToString();
                //Key-Value对应
                if (!string.IsNullOrEmpty(field))
                    row[field] = mSheet.Rows[i][j];
            }

            //添加到表数据中
            table.Add(row);
        }

        //生成Json字符串
        string json = JsonConvert.SerializeObject(table, Formatting.Indented);
        //写入文件
        using (FileStream fileStream = new FileStream(jsonPath, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
            {
                textWriter.Write(json);
            }
        }
    }

    /// <summary>
    /// 转化为Json.
    /// 整表: 所有配置表, 一个Json文件.
    /// </summary>
    public void ConvertToOneFileJson(string jsonPath, Encoding encoding, string excelName, bool isEnd)
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;

        //默认读取第一个数据表
        DataTable mSheet = mResultSet.Tables[0];

        //判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return;

        //准备一个列表存储整个表的数据
        Dictionary<string, object> table = new Dictionary<string, object>();


        //读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;//行
        int colCount = mSheet.Columns.Count;//列

        table["name"] = excelName;
        table["comments"] = null;
        //获取变量名
        List<string> arry = new List<string>();
        for (int x = 1; x < colCount; x++)
        {
            var value = mSheet.Rows[0][x].ToString();
            if (!string.IsNullOrEmpty(value))
            {
                arry.Add(value);
            }
        }
        table["properties"] = arry;

        List<object> objArray = new List<object>();
        for (int i = 3; i < rowCount; i++)
        {
            List<object> _array = new List<object>();
            for (int j = 1; j < colCount; j++)
            {
                if (!DBNull.Value.Equals(mSheet.Rows[i][j]))
                {
                    _array.Add(mSheet.Rows[i][j]);
                    //补充 若是中文类型的字符串，使用UTF-8
                }
                else
                {
                    // 自定义的数据类型。
                    switch (mSheet.Rows[1][j] as string)
                    {
                        case "int":
                            _array.Add(0);
                            break;
                        case "float":
                            _array.Add(0.0f);
                            break;
                        case "string":
                            _array.Add("");
                            break;
                        default:
                            break;
                    }
                }
            }
            objArray.Add(_array);
        }

        table["values"] = objArray;

        targetClass.Add("lo." + excelName, table);

        if (isEnd)
        {
            //生成Json字符串
            string json = JsonConvert.SerializeObject(targetClass, Newtonsoft.Json.Formatting.None);

            jsonPath = jsonPath.Remove(jsonPath.LastIndexOf(@"\"));
            jsonPath = jsonPath + "/localdata.json";
            //写入文件
            using (FileStream fileStream = new FileStream(jsonPath, FileMode.Create, FileAccess.Write))
            {
                using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
                {
                    textWriter.Write(json);
                }
            }
        }
    }

    /// <summary>
	/// 转换为lua.
	/// </summary>
	/// <param name="luaPath">lua文件路径</param>
	public void ConvertToLua(string luaPath, Encoding encoding)
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;

        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append("local datas = {");
        stringBuilder.Append("\r\n");

        //读取数据表
        foreach (DataTable mSheet in mResultSet.Tables)
        {
            //判断数据表内是否存在数据
            if (mSheet.Rows.Count < 1)
                continue;

            //读取数据表行数和列数
            int rowCount = mSheet.Rows.Count;
            int colCount = mSheet.Columns.Count;

            //准备一个列表存储整个表的数据
            List<Dictionary<string, object>> table = new List<Dictionary<string, object>>();

            //读取数据
            for (int i = 1; i < rowCount; i++)
            {
                //准备一个字典存储每一行的数据
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 0; j < colCount; j++)
                {
                    //读取第1行数据作为表头字段
                    string field = mSheet.Rows[0][j].ToString();
                    //Key-Value对应
                    row[field] = mSheet.Rows[i][j];
                }
                //添加到表数据中
                table.Add(row);
            }
            stringBuilder.Append(string.Format("\t\"{0}\" = ", mSheet.TableName));
            stringBuilder.Append("{\r\n");
            foreach (Dictionary<string, object> dic in table)
            {
                stringBuilder.Append("\t\t{\r\n");
                foreach (string key in dic.Keys)
                {
                    if (dic[key].GetType().Name == "String")
                        stringBuilder.Append(string.Format("\t\t\t\"{0}\" = \"{1}\",\r\n", key, dic[key]));
                    else
                        stringBuilder.Append(string.Format("\t\t\t\"{0}\" = {1},\r\n", key, dic[key]));
                }
                stringBuilder.Append("\t\t},\r\n");
            }
            stringBuilder.Append("\t}\r\n");
        }

        stringBuilder.Append("}\r\n");
        stringBuilder.Append("return datas");

        //写入文件
        using (FileStream fileStream = new FileStream(luaPath, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
            {
                textWriter.Write(stringBuilder.ToString());
            }
        }
    }

    /// <summary>
    /// 转换为CSV.
    /// </summary>
    public void ConvertToCSV(string CSVPath, Encoding encoding)
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;

        //默认读取第一个数据表
        DataTable mSheet = mResultSet.Tables[0];

        //判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return;

        //读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;

        //创建一个StringBuilder存储数据
        StringBuilder stringBuilder = new StringBuilder();

        //读取数据
        for (int i = 0; i < rowCount; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                //使用","分割每一个数值
                stringBuilder.Append(mSheet.Rows[i][j] + ",");
            }
            //使用换行符分割每一行
            stringBuilder.Append("\r\n");
        }

        //写入文件
        using (FileStream fileStream = new FileStream(CSVPath, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, encoding))
            {
                textWriter.Write(stringBuilder.ToString());
            }
        }
    }

    /// <summary>
    /// 导出为XML.
    /// </summary>
    public void ConvertToXml(string XmlFile)
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return;

        //默认读取第一个数据表
        DataTable mSheet = mResultSet.Tables[0];

        //判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return;

        //读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;

        //创建一个StringBuilder存储数据
        StringBuilder stringBuilder = new StringBuilder();
        //创建Xml文件头
        stringBuilder.Append("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        stringBuilder.Append("\r\n");
        //创建根节点
        stringBuilder.Append("<Table>");
        stringBuilder.Append("\r\n");
        //读取数据
        for (int i = 1; i < rowCount; i++)
        {
            //创建子节点
            stringBuilder.Append("  <Row>");
            stringBuilder.Append("\r\n");
            for (int j = 0; j < colCount; j++)
            {
                stringBuilder.Append("   <" + mSheet.Rows[0][j].ToString() + ">");
                stringBuilder.Append(mSheet.Rows[i][j].ToString());
                stringBuilder.Append("</" + mSheet.Rows[0][j].ToString() + ">");
                stringBuilder.Append("\r\n");
            }
            //使用换行符分割每一行
            stringBuilder.Append("  </Row>");
            stringBuilder.Append("\r\n");
        }
        //闭合标签
        stringBuilder.Append("</Table>");
        //写入文件
        using (FileStream fileStream = new FileStream(XmlFile, FileMode.Create, FileAccess.Write))
        {
            using (TextWriter textWriter = new StreamWriter(fileStream, Encoding.GetEncoding("utf-8")))
            {
                textWriter.Write(stringBuilder.ToString());
            }
        }
    }

    #endregion

    #region ExcelToList<T>

    /// <summary>
    /// 转换为实体类列表
    /// </summary>
    private List<T> ConvertToList<T>()
    {
        //判断Excel文件中是否存在数据表
        if (mResultSet.Tables.Count < 1)
            return null;
        //默认读取第一个数据表
        DataTable mSheet = mResultSet.Tables[0];

        //判断数据表内是否存在数据
        if (mSheet.Rows.Count < 1)
            return null;

        //读取数据表行数和列数
        int rowCount = mSheet.Rows.Count;
        int colCount = mSheet.Columns.Count;

        //准备一个列表以保存全部数据
        List<T> list = new List<T>();

        //读取数据
        for (int i = 1; i < rowCount; i++)
        {
            //创建实例
            Type t = typeof(T);
            ConstructorInfo ct = t.GetConstructor(System.Type.EmptyTypes);
            T target = (T)ct.Invoke(null);
            for (int j = 0; j < colCount; j++)
            {
                //读取第1行数据作为表头字段
                string field = mSheet.Rows[0][j].ToString();
                object value = mSheet.Rows[i][j];
                //设置属性值
                SetTargetProperty(target, field, value);
            }

            //添加至列表
            list.Add(target);
        }

        return list;
    }

    /// <summary>
    /// 设置目标实例的属性
    /// </summary>
    private void SetTargetProperty(object target, string propertyName, object propertyValue)
    {
        //获取类型
        Type mType = target.GetType();
        //获取属性集合
        PropertyInfo[] mPropertys = mType.GetProperties();
        foreach (PropertyInfo property in mPropertys)
        {
            if (property.Name == propertyName)
            {
                property.SetValue(target, Convert.ChangeType(propertyValue, property.PropertyType), null);
            }
        }
    }

    #endregion
}