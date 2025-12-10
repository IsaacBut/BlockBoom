using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


/// <summary>
/// CSV ファイルの読み込み・キャッシュ・セルアクセスを管理するクラス。
/// StreamingAssets（Editor/開発ビルド）または Resources（モバイル端末）から読み込み、
/// 2 次元のセル指定（列名と行番号）で値を取得できる。
/// </summary>
public class CSVReader : MonoBehaviour
{
    /// <summary>
    /// CSVReader のシングルトンインスタンス。
    /// </summary>
    public static CSVReader instance;

    // 最大列名（例："AA" まで対応）
    const string columnLimit = "AA";
    int columnLimitIndex;

    private void Awake()
    {
        if (instance == null)
        {

            instance = this;                                    // シングルトン初期化
            CsvInit();                                          // CSV キャッシュ辞書を初期化
            columnLimitIndex = ColumnNameToIndex(columnLimit);  // 列上限を数値化
            DontDestroyOnLoad(gameObject);                      // シーン切り替え時も保持

        }
        else
        {

            Destroy(gameObject);                                // 二重生成防止
            return;
        }

    }

    /// <summary>
    /// Excel の列名（A, B, Z, AA...）を 0 始まりのインデックスに変換する。
    /// 
    /// 例：
    ///   "A" → 0 ,
    ///   "B" → 1 ,
    ///   "Z" → 25 ,
    ///   "AA" → 26 ,
    ///   "AB" → 27 ,
    ///
    /// Excel の列ラベルをプログラム上の数値インデックスとして扱いたい場合に使用する。
    /// </summary>
    /// <param name="column">変換したい列名（例: "A", "C", "AA"）</param>
    /// <returns>0 始まりの列インデックス</returns>
    int ColumnNameToIndex(string column)
    {
        int columnIndex = 0;
        int length = column.Length;

        // 与えられた列名を 26 進数として解釈（A=1, B=2, ..., Z=26）
        // 例: "AB" = A * 26 + B = (1 * 26) + 2 = 28 → 0-based にすると 27
        for (int i = 0; i < length; i++)
        {
            char c = column[i];
            columnIndex *= 26;  // 1 桁左へシフト（26倍）
            columnIndex += (c - 'A' + 1);   // 現在の文字分を加算（A=1 に調整）

        }

        // Excel は 1 始まりだが、
        // プログラムで扱う配列は通常 0 始まりのため -1 して調整する
        return columnIndex - 1; 
    }

    /// <summary>
    /// 列インデックス（0 始まり）を Excel の列名形式（A, B, Z, AA...）へ変換する。
    /// 例：
    ///   0 → A ,
    ///   1 → B ,
    ///   25 → Z ,
    ///   26 → AA ,
    ///   27 → AB ,
    /// エクセル形式の列処理を行いたい場合に使用する。
    /// </summary>
    /// <param name="index">変換したい列インデックス（0 以上の整数）</param>
    /// <returns>Excel スタイルの列名（A, B, Z, AA など）</returns>
    public string IndexToColumnName(int index)
    {
        // 0 未満は不正 → 例外を投げる
        if (index < 0)
        {
            throw new ArgumentException("Index must be a non-negative integer.");
        }

        string columnName = "";
        index++;    // Excel 列名は 1 始まりで計算するため、あらかじめ +1 する

        // 26 進数として文字列を構築（A=1 ～ Z=26）
        while (index > 0)
        {
            int remainder = (index - 1) % 26;   // 26 進の余りを求める（0〜25 → A〜Z に対応）
            columnName = (char)('A' + remainder) + columnName;  // 余りを Excel の列文字（A〜Z）に変換して先頭に追加
            index = (index - 1) / 26;   // 一桁分を除いて次のループへ（26 進数シフト）

        }

        return columnName;  // 完成した列名を返す
    }

    /// <summary>
    /// 指定した CSV のセル値を float 型として取得するための公開メソッド。
    /// 列は Excel 形式（例: "A", "C", "AA"）、行番号は 1 始まりで指定する。
    /// 内部では TargetCellIndex() を呼び出して実際の取得処理を行う。
    /// </summary>
    /// <param name="csvPath">CSV ファイルのパスまたは Resources/StreamingAssets 上のキー名</param>
    /// <param name="row">列名（Excel 形式）例: "A", "B", "AA"</param>
    /// <param name="col">行番号（1 始まり）</param>
    /// <returns>指定されたセルの float 値。変換できない場合は 0 を返す。</returns>
    public float ReadTargetCellIndex(string csvPath, string row, int col)
    {
        return TargetCellIndex(csvPath, row, col);
    }
    
    /// <summary>
    /// 指定した CSV のセルを string として取得するための公開メソッド。
    /// 列は Excel 形式の列名（例: "A", "B", "AA"）、
    /// 行番号は 1 始まりで指定する。
    /// 実際の取得処理は TargetCellString() に委譲される。
    /// </summary>
    /// <param name="csvPath">CSV ファイルのパスまたはキー名（拡張子なし）</param>
    /// <param name="row">列名（Excel 形式）例: "A", "C", "AA"</param>
    /// <param name="col">行番号（1 始まり）</param>
    /// <returns>
    /// 指定セルの文字列。セルが存在しない場合は null を返す。
    /// </returns>
    public string ReadTargetCellString(string csvPath, string row, int col)
    {
        return TargetCellString(csvPath, row, col);
    }

    /// <summary>
    /// CSV データをキャッシュするための辞書。
    /// キー：CSV ファイル名（拡張子なし）
    /// 値：CSV の各行データ（string 配列）
    /// </summary>
    private static Dictionary<string, string[]> csvDict;

    /// <summary>
    /// CSV キャッシュ辞書を初期化する。
    /// アプリ起動時や CSVReader 初期化時に呼び出され、
    /// すべての CSV 読み込み状態をリセットする。
    /// </summary>
    public void CsvInit() => csvDict = new Dictionary<string, string[]>();

    /// <summary>
    /// キャッシュ済みの CSV データをすべてクリアする。
    /// メモリを解放したい場合や、別データを再読み込みしたい場合に使用する。
    /// </summary>
    public void CsvEnd() => csvDict.Clear();

    /// <summary>
    /// 指定した CSV ファイルを読み込み、行ごとの string 配列として返す。
    /// 
    /// ■ 読み込み仕様
    /// ・Editor / 開発ビルド：StreamingAssets から読み込む（.csv 拡張子が必要）
    /// ・Android / iOS / 正式リリース：Resources から読み込む（拡張子なし）
    /// 
    /// ■ キャッシュ機能
    /// 一度読み込んだ CSV はメモリ内にキャッシュされ、
    /// 同じファイルを再度読み込む際のディスクアクセスを防ぐ。
    /// 
    /// </summary>
    /// <param name="fileName">読み込む CSV のキー名（拡張子なし）</param>
    /// <returns>
    /// 行ごとの文字列配列。  
    /// ファイルが存在しない場合は null を返す。
    /// </returns>
    public static string[] LoadCSV(string fileName)
    {
        string key = fileName;

        // すでにキャッシュされている場合 → 即座に返す
        //if (csvDict.ContainsKey(key))
        //{
        //    return csvDict[key];
        //}
        //string path = Path.Combine(Application.streamingAssetsPath, key + ".csv");

        //// ファイルが存在しない場合
        //if (!File.Exists(path))
        //{
        //    Debug.LogWarning("ファイルが存在しない: " + path);
        //    return null;
        //}

        // UTF-8（BOM 対応）で CSV を読み込む
        //string[] lines = File.ReadAllLines(path, new UTF8Encoding(true));
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        // ============================================================
        // Editor / 開発ビルド：StreamingAssets から読み込む
        // ============================================================

        string path = Path.Combine(Application.streamingAssetsPath, key + ".csv");

        // ファイルが存在しない場合
        if (!File.Exists(path))
        {
            Debug.LogWarning("ファイルが存在しない: " + path);
            return null;
        }

        // UTF-8（BOM 対応）で CSV を読み込む
        string[] lines = File.ReadAllLines(path, new UTF8Encoding(true));

#else
        // ============================================================
        // Android / iOS（実機）：Resources から読み込む
        // ============================================================

        TextAsset csvFile = Resources.Load<TextAsset>(key);

        // Resources に存在しない場合
        if (csvFile == null)
        {
            Debug.LogWarning("Resources に存在しない: " + key);
            return null;
        }

        // 改行コード（LF/CRLF）を取り除いて行配列に変換
        string[] lines = csvFile.text.Split(
            new char[] { '\n', '\r' },
            StringSplitOptions.RemoveEmptyEntries
        );
#endif

        // 読み込んだ CSV をキャッシュに保存
        csvDict[key] = lines;

        // 行配列を返す
        return lines;
    }

    /// <summary>
    /// 指定した CSV のセルを float 型として取得する内部処理メソッド。
    /// 列は Excel 形式（例: "A", "C", "AA"）、
    /// 行番号は 1 始まりで指定し、0 始まりへ変換してからアクセスする。
    /// <para>
    /// ・セルが存在しない場合  
    /// ・空文字または数値として解析できない場合  
    /// は 0 を返す。
    /// </para>
    /// </summary>
    /// <param name="csvPath">CSV ファイルのキー名（拡張子なし）</param>
    /// <param name="row">列名（Excel 形式）例: "A", "B", "AA"</param>
    /// <param name="col">行番号（1 始まり）</param>
    /// <returns>float 値。失敗時は 0。</returns>
    float TargetCellIndex(string csvPath, string row, int col)
    {
        
        string[] lines = LoadCSV(csvPath);              // CSV の全行データを取得
        int rowIndex = ColumnNameToIndex(row);          // Excel 形式の列名（例: "C" "AA"）を 0-based インデックスへ変換
        int colIndex = col - 1;                         // 行番号（1-based）を 0-based へ変換
        string[] values = lines[colIndex].Split(',');   // 対象行をカンマ区切りで分割し、セル配列にする

        // 列インデックスが範囲外なら null を返す
        if (rowIndex > values.Length)
        {
            Debug.LogError($"行インデックスが範囲外です: {rowIndex}");
            return 0;
        }
        // 行インデックスが CSV 行数の範囲外なら null
        if (colIndex > lines.Length)
        {
            Debug.LogError($"列インデックスが範囲外です: {colIndex}");
            return 0;
        }

        float result;   // セル文字列を float として解析
        if (float.TryParse(values[rowIndex].Trim(), out result))
            return result;

        return 0;       // 数値変換できない場合も 0
    }


    /// <summary>
    /// 指定した CSV のセルを string として取得する内部処理メソッド。
    /// 列は Excel 形式（例: "A", "B", "AA"）、
    /// 行番号は 1 始まりで指定し、内部で 0 始まりへ変換してアクセスする。
    /// <para>
    /// 行または列の範囲外の場合は null を返し、
    /// エラーログを出力する。
    /// </para>
    /// </summary>
    /// <param name="csvPath">CSV ファイルのキー名（拡張子なし）</param>
    /// <param name="row">列名（Excel 形式）例: "A", "C", "AA"</param>
    /// <param name="col">行番号（1 始まり）</param>
    /// <returns>
    /// 指定セルの文字列。  
    /// 範囲外の場合は null を返す。
    /// </returns>
    string TargetCellString(string csvPath, string row, int col)
    {
        string[] lines = LoadCSV(csvPath);                  // CSV 全行データを取得
        int rowIndex = ColumnNameToIndex(row);              // Excel 形式の列名（"A", "B", "AA"）を 0-based インデックスへ変換
        int colIndex = col - 1;                             // 行番号（1-based）を 0-based に調整
        string[] values = lines[colIndex].Split(',');       // 対象の「行」をカンマ区切りで分割し、セル配列を取得


        // 列インデックスが範囲外なら null を返す
        if (rowIndex > values.Length)
        {
            Debug.LogError($"行インデックスが範囲外です: {rowIndex}");
            return null;
        }
        // 行インデックスが CSV 行数の範囲外なら null
        if (colIndex > lines.Length)
        {
            Debug.LogError($"列インデックスが範囲外です: {colIndex}");
            return null;
        }

        // 目的のセル文字列を取得（前後の空白を除去）
        string cellData = values[rowIndex].Trim();

        return cellData;

    }
}
