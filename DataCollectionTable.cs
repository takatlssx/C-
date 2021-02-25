using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Movie
{
    public class DataCollectionTable
    {
        public string Error = "";
        public string Msg = "";

        public string Name;
        public string DataPath;

        public List<string> Index = new List<string>();
        public Dictionary<string, string> Type = new Dictionary<string, string>();
        public Dictionary<string, string> Empty = new Dictionary<string, string>();
        public Dictionary<string, string> Alias = new Dictionary<string, string>();
        public string PrimaryKey;

        public List<Dictionary<string, string>> Data = new List<Dictionary<string, string>>();

        //コンストラクタ
        //テーブル名設定→データファイルパス設定
        public DataCollectionTable(string name, string dataPath)
        {
            Name = name;
            DataPath = dataPath;
            //createTable();
        }

        //テーブルデータ取得
        public bool GetData()
        {
            string error = $"{Name}テーブル作成エラー:DataCollectionTable.GetData()\r\n";

            //DataPathチェック(.dcファイルのパス確認)
            if (!File.Exists(DataPath))
            {
                Error = error + $"データファイル:{DataPath}が見つかりませんでした。\r\n";
                return false;
            }

            //データ読み込み
            try
            {
                using (StreamReader sr = new StreamReader(DataPath))
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        Dictionary<string, string> lineDict = new Dictionary<string, string>();
                        var buff = line.Split(',');

                        //もしテーブルの列の数と、読みこんだ1行(line)ををカンマで分割したデータ(buff)数が一致しなかったらエラー
                        if (Index.Count != buff.Length)
                        {
                            Error = error + $"データ数:{buff.Length}は不正な値です。\r\n";
                            return false;
                        }
                        //Indexとbuffから１行のデータディクショナリ(lineDict)を作成
                        //linedict = Index.Zip(buff,(ky,vl) => new {ky,vl}).ToDictionary(a => a.ky,a => a.vl);
                        //Data.Add(Index.Zip(buff,(ky,vl) => new {ky,vl}).ToDictionary(a => a.ky,a => a.vl));
                        for (int i = 0; i < Index.Count; i++)
                        {
                            lineDict[Index[i]] = buff[i];
                        }

                        //データ追加
                        Data.Add(lineDict);
                    }
                }
            }
            catch (Exception ex)
            {
                Error = error + $"データ取得に失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }

            return true;
        }

        //データのインデックス番号取得
        public int GetDataNumber(string indexName, string data)
        {
            int resInt = -1;
            if (!Index.Contains(indexName))
            {
                Error += $"DataCollectionTable.GetDataNumber()\r\n{Name}テーブルに列'{indexName}'は存在しません。\r\n";
                return resInt;
            }
            for (int i = 0; i < Data.Count; i++)
            {
                if (Data[i][indexName] == data)
                {
                    resInt = i;
                    break;
                }
            }
            return resInt;
        }

        //登録するデータを検証（ヴァリデーション）
        private bool validate(Dictionary<string, string> newData, string mode = "regist")
        {
            string error = $"{Name}テーブル登録データ検証エラー:DataCollectionTable.validate()\r\n";

            //データ数(registモードなら同一か？、editモードなら超えていないか)
            if (mode == "regist" && newData.Count != Index.Count)
            {
                Error = error + $"新たなデータ数:{newData.Count}は、規定のデータ数:{Index.Count}と異なります。\r\n";
                return false;
            }
            else if (mode == "edit" && newData.Count > Index.Count)
            {
                Error = error + $"新たなデータ数:{newData.Count}は、規定のデータ数:{Index.Count}を超えています。\r\n";
                return false;
            }

            //新データディクショナリのキーがIndexと一致しているか？
            //またキーの重複は無いか？
            string tempKey = "";
            foreach (string key in newData.Keys)
            {
                if (!Index.Contains(key))
                {
                    Error = error + $"新たなデータのキー:{key}はデータベースに存在しないキーです。\r\n";
                    return false;
                }
                else if (key == tempKey)
                {
                    Error = error + $"新たなデータにおいてキー:{key}が重複しています。\r\n";
                    return false;
                }
                tempKey = key;
            }


            //管理番号(registモードのみ)
            if (mode == "regist")
            {
                newData[PrimaryKey] = (Data.Count + 1).ToString("000000");
            }

            //型、nullチェック
            foreach (string idx in newData.Keys)
            {
                //型
                int tryInt = 0;
                if (Type[idx] == "int" && !int.TryParse(newData[idx], out tryInt))
                {
                    error += $"型エラー：{idx}列のデータは整数値を入力してください。\r\n";
                    return false;
                }
                DateTime tryDate;
                if (Type[idx] == "date" && !DateTime.TryParse(newData[idx], out tryDate))
                {
                    error += $"型エラー：{idx}列のデータは日付値(yyyy/MM/dd)を入力してください。\r\n";
                    return false;
                }

                //null
                if (Empty[idx] == "not_null" && (newData[idx] == "" || newData[idx] == null))
                {
                    error += $"NULLエラー：{idx}列のデータはNULL不許容です。何か値を入力してください。\r\n";
                    return false;
                }
            }
            if (error != $"{Name}テーブル登録データ検証エラー:DataCollectionTable.validate()\r\n")
            {
                Error += error;
                return false;
            }

            return true;
        }

        //編集
        public bool Edit(string id, Dictionary<string, string> newData)
        {
            string error = $"{Name}テーブルデータ編集エラー:DataCollectionTable.Edit()\r\n";

            Msg += $"{Name}テーブルの管理番号'{id}'のデータを変更しました。\r\n";

            int indexNum = GetDataNumber(PrimaryKey, id);
            if (indexNum == -1)
            {
                Error = error + Error;
                return false;
            }
            if (!validate(newData, "edit"))
            {
                Error = error + Error;
                return false;
            }
            //編集前のデータ
            var oldData = new Dictionary<string, string>(Data[indexNum]);

            try
            {
                foreach (string key in newData.Keys)
                {
                    Data[indexNum][key] = newData[key];
                    Msg += $"    列'{key}'  {oldData[key]} → {newData[key]}\r\n";
                }
            }
            catch (Exception ex)
            {
                Error += error + $"データの変更に失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }
            return true;
        }

        //新規登録
        public bool Regist(Dictionary<string, string> newData)
        {
            string error = $"{Name}テーブルデータ登録エラー:DataCollectionTable.Regist()\r\n";

            if (!validate(newData))
            {
                Error = error + Error;
                return false;
            }

            try
            {
                Data.Add(newData);
            }
            catch (Exception ex)
            {
                Error += error + $"データの追加に失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }

            Msg = $"{Name}テーブルに、管理番号：{newData[PrimaryKey]}のデータを新規登録しました。\r\n";
            return true;
        }

        //データ保存（.dcファイルに書き出し）
        public bool Update()
        {
            string error = $"{Name}テーブルデータ保存エラー：DataCollectionTable.Update()\r\n";

            //DataPathのパスチェック
            if (!File.Exists(DataPath))
            {
                Error = error + $"データファイル:{DataPath}は存在しません。\r\n";
                return false;
            }

            using (StreamWriter sw = new StreamWriter(DataPath))
            {
                string csvStr = "";
                for (int i = 0; i < Data.Count; i++)
                {
                    try
                    {
                        csvStr += String.Join(",", Data[i].Values) + "\r\n";
                    }
                    catch (Exception ex)
                    {
                        Error = error + $"{i}行目のデータをCSV文字列に変換できませんでした。\r\n";
                        return false;
                    }
                }

                try
                {
                    sw.Write(csvStr);
                }
                catch (Exception ex)
                {
                    Error = error + $"データをファイルに書き込みできませんでした。\r\n";
                    return false;
                }
            }
            return true;
        }

        //検索
        public List<Dictionary<string, string>> Search(List<string> idList, List<string> wordList, List<string> operandList, string andOr = "or")
        {
            string error = $"データ検索エラー:DataCollectionTable.Search()\r\n";

            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            if (idList.Count != wordList.Count || operandList.Count != idList.Count)
            {
                Error += error + $"列・検索語句・比較記号のリスト数が一致しません。\r\n";
                return null;
            }

            //引数の値を検証
            foreach (string id in idList)
            {
                if (!Index.Contains(id) && id != "全て" && id != "")
                {
                    Error += error + $"{Name}テーブルに列'{id}'は存在しません。\r\n";
                    return null;
                }
            }

            List<string> ops = new List<string> { "=", "like" };
            foreach (string ope in operandList)
            {
                if (!ops.Contains(ope))
                {
                    Error += error + $"比較記号'{ope}'は不正な値です。'='か'like'のみ有効です。\r\n";
                    return null;
                }
            }

            if (andOr != "and" && andOr != "or")
            {
                Error += error + $"組み合わせ記号'{andOr}'は不正な値です。'and'か'or'のみ有効です。\r\n";
                return null;
            }

            // 検索
            for (int i = 0; i < Data.Count; i++)
            {
                //条件の数のboolリスト、1行のデータに対し条件ごとにfalseかtrueか判定
                List<bool> isMatchedList = new bool[idList.Count].ToList();

                for (int c = 0; c < idList.Count; c++)
                {
                    //完全一致(=)かつ検索対象列が全ての場合
                    if (operandList[c] == "=" && (idList[c] == "" || idList[c] == "全て"))
                    {
                        if (Data[i].Values.Contains(wordList[c]))
                        {
                            isMatchedList[c] = true;
                        }
                    }
                    //完全一致(=)かつ検索対象列が指定されている場合
                    else if (operandList[c] == "=" && idList[c] != "" && idList[c] != "全て")
                    {
                        if (Data[i][idList[c]] == wordList[c])
                        {
                            isMatchedList[c] = true;
                        }
                    }
                    //部分一致(like)かつ検索対象列が全ての場合
                    else if (operandList[c] == "like" && (idList[c] == "" || idList[c] == "全て"))
                    {
                        foreach (string val in Data[i].Values)
                        {
                            if (val.Contains(wordList[c]))
                            {
                                isMatchedList[c] = true;
                                break;
                            }
                        }
                    }
                    //部分一致(like)かつ検索対象列が指定されている場合
                    else if (operandList[c] == "like" && idList[c] != "" && idList[c] != "全て")
                    {
                        if (Data[i][idList[c]].Contains(wordList[c]))
                        {
                            isMatchedList[c] = true;
                        }
                    }
                }

                if ((andOr == "and" && !isMatchedList.Contains(false)) || (andOr == "or" && isMatchedList.Contains(true)))
                {
                    result.Add(Data[i]);
                }
            }

            Msg = $"【検索結果】";
            for (int i = 0; i < idList.Count; i++)
            {
                Msg += $"検索条件{i + 1} → 列'{idList[i]}' = '{wordList[i]} '";
            }
            Msg += $"【{result.Count}件】";

            return result;
        }

        //ソート
        public List<Dictionary<string, string>> Sort(string index, string order = "asc")
        {
            List<Dictionary<string, string>> sortedData = new List<Dictionary<string, string>>(Data);

            Msg += $"基準列'{Alias[index]}',ソート方式'{order}'で並び替え\r\n";

            string error = "ソートエラー:DataCollectionTable.Sort()\r\n";

            //引数の列名チェック
            if (!Index.Contains(index))
            {
                Error = error + $"{Name}テーブルに列'{index}'は存在しません。\r\n";
                return null;
            }
            //引数のソート方式名チェック( asc or desc )
            if (order != "asc" && order != "desc")
            {
                Error = error + $"ソート方式文字列は'asc(昇順)'か'desc(降順)'のみ有効です。\r\n";
                return null;
            }

            //ソート
            if (order == "asc")
            {
                sortedData.Sort((a, b) => a[index].CompareTo(b[index]));
            }
            else
            {
                sortedData.Sort((a, b) => b[index].CompareTo(a[index]));
            }

            return sortedData;
        }

    }
}
