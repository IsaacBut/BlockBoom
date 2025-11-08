using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    public static CSVReader instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }


    int ColumnNameToIndex(string column)
    {
        int columnIndex = 0;
        int length = column.Length;

        for (int i = 0; i < length; i++)
        {
            char c = column[i];
            columnIndex *= 26;
            columnIndex += (c - 'A' + 1);  // 计算当前字符的列索引
        }

        //Debug.Log($"{columnIndex-1}");  // 输出指定单元格的数据

        return columnIndex - 1;  // 减 1 因为列是从 0 开始的
    }
    public string IndexToColumnName(int index)
    {
        if (index < 0)
        {
            throw new ArgumentException("Index must be a non-negative integer.");
        }

        string columnName = "";
        index++;  // Because your original function subtracts 1 at the end

        while (index > 0)
        {
            int remainder = (index - 1) % 26;
            columnName = (char)('A' + remainder) + columnName;
            index = (index - 1) / 26;
        }

        return columnName;
    }

    public float ReadTargetCellIndex(string csvPath, string rowLabel, int col)
    {
        return TargetCellIndex(csvPath, rowLabel, col);
    }
    float TargetCellIndex(string csvPath, string rowLabel, int col)
    {
        // 1. 检查文件是否存在
        TextAsset csvFile = Resources.Load<TextAsset>(csvPath);
        if (csvFile == null)
        {
            Debug.LogError("CSV文件未找到：" + csvPath);
            return 0;
        }

        // 读取CSV所有行（忽略空行）
        string[] lines = csvFile.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // 3. 将行标签（如"AB"）转换为行索引
        int rowIndex = ColumnNameToIndex(rowLabel);
        int colIndex = col - 1;

        string[] values = lines[colIndex].Split(',');
        //Debug.Log($"values({values.Length}");
        //Debug.Log($"Lines ({lines.Length}");

        if (rowIndex > values.Length)
        {
            Debug.LogError("行索引超出范围！");
            return 0;
        }
        if (colIndex > lines.Length)
        {
            Debug.LogError("列索引超出范围！");
            return 0;
        }


        // 5. 输出目标单元格数据
        string cellData = values[rowIndex].Trim();
        //Debug.Log($"单元格 ({rowLabel}{col}): {cellData}");

        float result;
        if (float.TryParse(cellData, out result))
        {
            //Debug.Log("转换成功：" + result);
            return result;
        }
        else
        {
            Debug.LogError("转换失败！原始数据：" + cellData);
            return 0;
        }
    }

    public string ReadTargetCellString(string csvPath, string rowLabel, int col)
    {
        return TargetCellString(csvPath, rowLabel, col);
    }
    string TargetCellString(string csvPath, string rowLabel, int col)
    {
        // 1. 检查文件是否存在
        TextAsset csvFile = Resources.Load<TextAsset>(csvPath);
        if (csvFile == null)
        {
            Debug.LogError("CSV文件未找到：" + csvPath);
            return null;
        }

        // 读取CSV所有行（忽略空行）
        string[] lines = csvFile.text.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        // 3. 将行标签（如"AB"）转换为行索引
        int rowIndex = ColumnNameToIndex(rowLabel);
        int colIndex = col - 1;

        string[] values = lines[colIndex].Split(',');
        //Debug.Log($"values({values.Length}");
        //Debug.Log($"Lines ({lines.Length}");

        if (rowIndex > values.Length)
        {
            Debug.LogError("行索引超出范围！");
            return null;
        }
        if (colIndex > lines.Length)
        {
            Debug.LogError("列索引超出范围！");
            return null;
        }


        // 5. 输出目标单元格数据
        string cellData = values[rowIndex].Trim();

       // Debug.Log(cellData);
        return cellData;
        //Debug.Log($"单元格 ({rowLabel}{col}): {cellData}");

    }


}
