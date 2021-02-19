using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MovieDataBase
{
    public class DataCollection
    {
        public string Error;
        public string Msg;

        public string Name;
        public string RootDir;
        public string InfoPath;

        public List<string> TableNames;
        public List<string> RelationalTableNames;
        public List<string> SubTableNames;
        public string MainTableName;

        public Dictionary<string, DataCollectionTable> Tables = new Dictionary<string, DataCollectionTable>();
        public DataCollectionTable MainTable;

        public DataCollection(string name, string rootDir)
        {
            Name = name;
            RootDir = rootDir;
            InfoPath = rootDir + "\\" + name + ".info";

            if (GetInfo())
            {
                GetTableData();
            }
        }
        
        private int mySort(string colName){
            Dictironary<string,int> dict = new Dictironary<string,int>();
            foreach(Dictionary<string,string> data in MainTable.Data)
            {
                string [] buff = data[colName].Split('/');
                foreach(string bf in buff){
                    if(!dict.Keys.Contains(bf)){
                       dict[bf] = 1;
                    }
                    else{
                        dict[bf] += 1;
                    }
                }
            }
            var sortedDict = dict.OrderByDescending((x) => x.Value);
            List<string> lst = sortedDict.Values.ToList();
        }
        public bool RebuildDataCollection()
        {
            string error = "データコレクション再構築エラー:DataCollection.RebuildDataCollection()\r\n";
            
            
            
            //メインテーブルの管理番号等更新
            for(int i = 0 ; i < MainTable.Data.Count ; i++)
            {
                try
                {
                    Data[i][MainTable.PrimaryKey] = (i+!).ToString("000000");
                }
                catch(Exception ex)
                {
                    Error = error + $"{MainTableName}テーブルの{i}行目のデータ構築に失敗しました。\r\n{ex.ToString()}\r\n";
                    return false;
                }                
            }
            
            //リレーショナルテーブル更新
            //メインテーブルに無い項目は削除し管理番号を再割り振り
            foreach(string rltbl in RelationalTableNames)
            {
            }
        }
        
        private bool updateRelationalTables(Dictionary<string,string> newData)
        {
            string error = "リレーショナルテーブル更新エラー:DataCollection.updateRelationalTable()\r\n";
            
            //リレーショナルテーブル処理
            //１：リレーショナルテーブル名リストをループ
            //２：新規データの、テーブル名と同名列データを、'/'で分割（複数の場合は/で区切るルール）
            //３：２で分割したデータリスト毎に、そのデータが対象テーブルに含まれているかを、GetDataNumberメソッドでチェック
            //４：含まれていなければ新規データとして登録
            foreach(string rltbl in RelationalTableNames)
            {
                string[] words = newData[rltbl].Split('/');
                foreach(string word in words)
                {
                    if(Tables[rltbl].GetDataNumber(rltbl,word) == -1)
                    {
                        Dictionary<string,string> nD = new Dictionary<string,string>();
                        nD[Tables[rltbl].PrimaryKey] = "";
                        nD[rltbl] = word;
                        if(!Tables[rltbl].Regist(nD))
                        {
                            Error = error + Tables[rltbl].Error;
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        
        //新規登録
        public bool RegistData(Dictionary<string,string> newData)
        {
            string error = "メインテーブルデータ登録エラー:DataCollection.RegistData()\r\n";
            
            //メインテーブル登録
            if(!MainTable.Regist(newData)){
                Error = error + MainTable.Error;
                return false;
            }
            
            //リレーショナルテーブル処理
            if(!updateRelationalTables(newData)){
                Error = error + MainTable.Error;
                return false;
            }
            return true;
        }

        public bool GetInfo()
        {
            Error = $"設定取得エラー:DataCollection.GetInfo()\r\n";

            if (!File.Exists(InfoPath))
            {
                Error += $"infoファイル:{InfoPath}が見つかりませんでした。\r\n";
                return false;
            }

            try
            {
                using (StreamReader sr = new StreamReader(InfoPath))
                {
                    string line;
                    string processingTableName = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        var contents = line.Split(':');

                        if (contents[0] == "tableNames")
                        {
                            TableNames = contents[1].Split(',').ToList();
                        }
                        else if (contents[0] == "relationalTableNames")
                        {
                            RelationalTableNames = contents[1].Split(',').ToList();
                        }
                        else if (contents[0] == "subTableNames")
                        {
                            SubTableNames = contents[1].Split(',').ToList();
                        }
                        else if (contents[0] == "mainTableName")
                        {
                            MainTableName = contents[1];
                        }
                        else if (contents[0] == "@tableInfo")
                        {
                            processingTableName = "";
                        }
                        else if (contents[0] == "tableName")
                        {
                            processingTableName = contents[1];
                            Tables[processingTableName] = new DataCollectionTable(processingTableName, RootDir + "\\" + processingTableName + ".dc");
                            if (processingTableName == MainTableName)
                            {
                                MainTable = Tables[processingTableName];
                            }
                        }
                        else if (contents[0] == "index")
                        {
                            Tables[processingTableName].Index = contents[1].Split(',').ToList();
                        }
                        else if (contents[0] == "type")
                        {
                            var buff = contents[1].Split(',');
                            if (Tables[processingTableName].Index.Count != buff.Length)
                            {
                                Error += $"{processingTableName}テーブルのtypeリストの要素数:{buff.Length}は不正な値です。\r\n";
                                return false;
                            }

                            for (int i = 0; i < Tables[processingTableName].Index.Count; i++)
                            {
                                Tables[processingTableName].Type[Tables[processingTableName].Index[i]] = buff[i];
                            }
                        }
                        else if (contents[0] == "empty")
                        {
                            var buff = contents[1].Split(',');
                            if (Tables[processingTableName].Index.Count != buff.Length)
                            {
                                Error += $"{processingTableName}テーブルのemptyリストの要素数:{buff.Length}は不正な値です。\r\n";
                                return false;
                            }

                            for (int i = 0; i < Tables[processingTableName].Index.Count; i++)
                            {
                                Tables[processingTableName].Empty[Tables[processingTableName].Index[i]] = buff[i];
                            }
                        }
                        else if (contents[0] == "alias")
                        {
                            var buff = contents[1].Split(',');
                            if (Tables[processingTableName].Index.Count != buff.Length)
                            {
                                Error += $"{processingTableName}テーブルのaliasリストの要素数:{buff.Length}は不正な値です。\r\n";
                                return false;
                            }

                            for (int i = 0; i < Tables[processingTableName].Index.Count; i++)
                            {
                                Tables[processingTableName].Alias[Tables[processingTableName].Index[i]] = buff[i];
                            }
                        }
                        else if (contents[0] == "primaryKey")
                        {
                            if (contents[1] == null || contents[1] == "" || !Tables[processingTableName].Index.Contains(contents[1]))
                            {
                                Error += $"{processingTableName}テーブルのprimaryKey:{contents[1]}は不正な値です。\r\n";
                                return false;
                            }
                            Tables[processingTableName].PrimaryKey = contents[1];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Error += $"infoファイル読み込みに失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }
            return true;
        }

        public bool GetTableData()
        {
            Error = $"データ取得エラー:DataCollection.GetData()\r\n";
            foreach (string tblName in TableNames)
            {
                if (!Tables[tblName].GetData())
                {
                    Error += Tables[tblName].Error;
                    return false;
                }
            }
            return true;
        }
    }
}
