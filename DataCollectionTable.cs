using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MovieDataBase
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

        public DataCollectionTable(string name, string dataPath)
        {
            Name = name;
            DataPath = dataPath;
            //createTable();
        }

        //データ取得
        public bool GetData()
        {
            Error += $"テーブル作成エラー:DataCollectionTable.GetData()\r\n";

            //DataPathチェック
            if (!File.Exists(DataPath))
            {
                Error += $"{Name}テーブルのデータファイル:{DataPath}が見つかりませんでした。\r\n";
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

                        if (Index.Count != buff.Length)
                        {
                            Error += $"{Name}テーブルのデータ数:{buff.Length}は不正な値です。\r\n";
                            return false;
                        }

                        for (int i = 0; i < Index.Count; i++)
                        {
                            lineDict[Index[i]] = buff[i];
                        }

                        Data.Add(lineDict);
                    }
                }
            }
            catch (Exception ex)
            {
                Error += $"{Name}テーブルのデータ取得に失敗しました。\r\n{ex.ToString()}\r\n";
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

        //検証（ヴァリデーション）（途中）
        private bool validate(Dictionary<string, string> newData, string mode = "regist")
        {
            string error = $"{Name}テーブル データ検証エラー:DataCollectionTable.validate()\r\n";
            
            //データ数(registモードなら同一か？、editモードなら超えていないか)
            if(mode == "regist" && newData.Count != Index.Count)
            {
                Error = error + $"新たなデータ数:{newData.Count}は、規定のデータ数:{Index.Count}と異なります。\r\n";
                return false;
            }
            else if(mode == "edit" && newData.Count > Index.Count){
                Error = error + $"新たなデータ数:{newData.Count}は、規定のデータ数:{Index.Count}を超えています。\r\n";
                return false;
            }
            
            //新データのディクショナリキーがIndexと一致しているか？
            //またキーの重複は無いか？
            string tempKey = "";
            foreach(string key in newData.Keys)
            {
                if(!Index.Contains(key))
                {
                    Error = error + $"新たなデータのキー:{key}はデータベースに存在しないキーです。\r\n";
                    return false;
                }
                else if(key == tempKey)
                {
                    Error = error + $"新たなデータにおいてキー:{key}が重複しています。\r\n";
                    return false;
                }                
                tempKey = key;
            }
            
            
            //管理番号(registモードのみ)
            

            //型、nullチェック

            return true;
        }

        //検索(途中)
        public List<Dictionary<string, string>> Search(List<string> idList, List<string> wordList, List<string> operandList, string andOr = "or")
        {
            string error = $"データ検索エラー:DataCollectionTable.Search()\r\n";

            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();

            //引数の値を検証
            if(idList.Count != wordList.Count || idList.Count != operandList.Cont)
            {
                Error += error + $"検索条件に指定された、idList・wordList・operandListの数が一致しません。\r\n";
                return null;
            }
            foreach (string id in idList)
            {
                if (!Index.Contains(id) && id !="" && id != "全て")
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

            
            //検索
            for(int i = 0 ; i < Data.Count ; i++)
            {
                //条件の数のboolリスト、1行のデータに対し条件ごとにfalseかtrueか判定
                List<bool> isMatchedList = new bool[idList.Count].ToList();
                
                for(int c = 0 ; c < idList.Count ; c++)
                {
                    //完全一致(=)かつ検索対象列が全ての場合
                    if(operandList[c] == "=" && (idList[c] == "" || idList[c] == "全て"))
                    {
                        if(Data[i].Values.Contains(wordList[c]))
                        {
                            isMatchedList[c] = true;
                        }
                    }
                    //完全一致(=)かつ検索対象列が指定されている場合
                    else if(operandList[c] == "=" && idList[c] != "" && idList[c] != "全て")
                    {
                        if(Data[i][idList[c]] == wordList[c])
                        {
                            isMatchedList[c] = true;
                        }
                    }
                    //部分一致(like)かつ検索対象列が全ての場合
                    else if(operandList[c] == "like" && (idList[c] == "" || idList[c] == "全て"))
                    {
                        foreach(string val in Data[i].Values)
                        {
                            if(val.Contains(wordList[c]))
                            {
                                isMatchedList[c] = true;
                                break;
                            }
                        }
                    }
                    //部分一致(like)かつ検索対象列が指定されている場合
                    else if(operandList[c] == "like=" && idList[c] != "" && idList[c] != "全て")
                    {
                        if(Data[i][idList[c]].Contains(wordList[c]))
                        {
                            isMatchedList[c] = true;
                        }
                    }
                }
            }
            
            Msg = $"【検索結果】";
            for(int i = 0 ; i < idList.Count ; i++)
            {
                Msg += $"検索条件{i} → 列'{idList[i]}' {operandList[i]} '{wordList[i]} '";
            }
            Msg += $"【{result.Count}件】";
            return result;
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

            Data.Add(newData);
            Msg += $"管理番号：{newData[PrimaryKey]}のデータを新規登録しました。\r\n";
            return true;
        }

        
    }
}
