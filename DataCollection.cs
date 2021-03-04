using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Movie
{
    public class DataCollection
    {
        #region メンバー変数

        //エラー文字列
        public string Error;
        //メッセージ文字列
        public string Msg;

        //データコレクションの名称
        public string Name;
        //データコレクションのルートフォルダ→ドライブ:\ルートフォルダ
        public string RootDir;
        //データコレクションの設定情報(.info)ファイルパス(RootDir\データコレクション名.info)
        public string InfoPath;


        //テーブル名のList<string>
        public List<string> TableNames;

        //リレーショナルテーブル名のList<string>
        public List<string> RelationalTableNames;

        //サブテーブル名のList<string>
        public List<string> SubTableNames;

        //メインテーブル名
        public string MainTableName;
        //メインテーブルDataCollectionTableオブジェクト
        public DataCollectionTable MainTable;

        //DataCollectionTableオブジェクトのディクショナリ
        //キー：テーブル名、値：DataCollectionTableオブジェクト
        public Dictionary<string, DataCollectionTable> Tables = new Dictionary<string, DataCollectionTable>();
        #endregion

        #region コンストラクタ
        //コンストラクタ
        //引数：データコレクション名,ルートディレクトリパス
        //GetInfo()で設定情報取得→GetTableData()でテーブルデータ取得
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
        #endregion

        #region 初期化時実行メソッド(GetInfo(),GetTableData())

        //設定取得
        public bool GetInfo()
        {
            string error = $"設定取得エラー:DataCollection.GetInfo()\r\n";
            //設定ファイルパスチェック
            if (!File.Exists(InfoPath))
            {
                Error = error + $"infoファイル:{InfoPath}が見つかりませんでした。\r\n";
                return false;
            }

            try
            {
                using (StreamReader sr = new StreamReader(InfoPath))
                {
                    //1行分の文字列
                    string line;
                    //現在処理中のテーブル名
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
                            //Tables[processingTableName].Type = Index.Zip(buff,(ky,val) => new { ky,val }).ToDictionary(d => d.ky,d => d.val);        
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
                            //Tables[processingTableName].Empty = Index.Zip(buff,(ky,val) => new { ky,val }).ToDictionary(d => d.ky,d => d.val);        
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
                            
                            //Tables[processingTableName].Alias = Index.Zip(buff,(ky,val) => new { ky,val }).ToDictionary(d => d.ky,d => d.val);
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
                Error = error + $"infoファイル読み込みに失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }
            return true;
        }

        //テーブルデータ取得
        //テーブル名リストをループし、テーブルオブジェクト毎にそのGetData()メソッドでデータ取得
        public bool GetTableData()
        {
            string error = $"データ取得エラー:DataCollection.GetData()\r\n";
            foreach (string tblName in TableNames)
            {
                if (!Tables[tblName].GetData())
                {
                    Error = error + Tables[tblName].Error;
                    return false;
                }
            }
            return true;
        }
        #endregion

        //リレーショナルテーブルをメインテーブルの新規データをもとに更新
        private bool updateRelationalTables(Dictionary<string, string> newData)
        {
            string error = "リレーショナルテーブル更新エラー:DataCollection.updateRelationalTable()\r\n";

            //もし新規データにリレーショナルデータベース名の列が含まれていなければ処理を抜ける
           // if(!RelationalTableNames.Any(x => newData.Keys.Contains(x)))
            //{
            //    return true;
            //}
            bool isContainRLTBL = false;
            foreach (string key in newData.Keys)
            {
                if (RelationalTableNames.Contains(key))
                {
                    isContainRLTBL = true;
                    break;
                }
            }
            if (!isContainRLTBL)
            {
                return true;
            }

            //リレーショナルテーブル処理
            //１：リレーショナルテーブル名リストをループ
            //２：新規データにリレーショナルテーブル名の列が含まれていれば、テーブル名と同名列データを、'/'で分割（複数の場合は/で区切るルール）
            //３：２で分割したデータリストをループし、word毎に、そのwordが対象テーブルに含まれているかを、GetDataNumberメソッドでチェック
            //４：含まれていなければ新規データとして登録

            //1
            foreach (string rltbl in RelationalTableNames)
            {
                //2
                if (newData.Keys.Contains(rltbl))
                {
                    //2
                    string[] words = newData[rltbl].Split('/');

                    //3
                    foreach (string word in words)
                    {
                        //3:wordが新規単語かつ空白でない
                        if (Tables[rltbl].GetDataNumber(rltbl, word) == -1 && word != "" && word != null)
                        {
                            Dictionary<string, string> nD = new Dictionary<string, string>();
                            nD[Tables[rltbl].PrimaryKey] = "";
                            nD[rltbl] = word;

                            //4
                            if (!Tables[rltbl].Regist(nD))
                            {
                                Error = error + Tables[rltbl].Error;
                                return false;
                            }
                            Msg += Tables[rltbl].Msg;
                        }
                    }
                }

            }
            return true;
        }

        //新規登録処理
        //メインテーブル登録→リレーショナルテーブル更新
        public bool RegistData(Dictionary<string, string> newData)
        {
            string error = "テーブルデータ登録処理エラー:DataCollection.RegistData()\r\n";

            //メインテーブル登録
            if (!MainTable.Regist(newData))
            {
                Error = error + MainTable.Error;
                return false;
            }
            Msg = MainTable.Msg;

            //リレーショナルテーブル処理
            if (!updateRelationalTables(newData))
            {
                Error = error + Error;
                return false;
            }

            //全てのテーブルを更新
            foreach (string tblName in TableNames)
            {
                if (!Tables[tblName].Update())
                {
                    Error = error + Tables[tblName].Error;
                    return false;
                }
            }
            return true;
        }

        //編集処理
        public bool EditData(List<string> idList, Dictionary<string, string> newData)
        {
            string error = "テーブルデータ編集処理エラー:DataCollection.EditData()\r\n";

            //編集
            foreach (string id in idList)
            {
                if (!MainTable.Edit(id, newData))
                {
                    Error = error + MainTable.Error;
                    return false;
                }
            }
            Msg = $"{MainTableName}テーブルで{idList.Count}件のデータを以下の通り変更しました。\r\n";
            foreach (string ky in newData.Keys)
            {
                Msg += $"{ky}列 = '{newData[ky]}'\r\n";
            }

            if (!updateRelationalTables(newData))
            {
                Error = error + Error;
                return false;
            }

            //全てのテーブルを更新
            foreach (string tblName in TableNames)
            {
                if (!Tables[tblName].Update())
                {
                    Error = error + Tables[tblName].Error;
                    return false;
                }
            }

            return true;
        }

        //メインテーブル検索
        public List<Dictionary<string, string>> SearchData(List<string> idList, List<string> wordList, List<string> operandList, string andOr = "or")
        {
            List<Dictionary<string, string>> result = MainTable.Search(idList, wordList, operandList, andOr);
            if (result == null)
            {
                Error = MainTable.Error;
            }
            return result;
        }
    }
}
