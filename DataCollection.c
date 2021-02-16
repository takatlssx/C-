using System;

namespace Test
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
        
        public Dictionary<string,DataCollectionTable> Tables = new Dictionary<string,DataCollectionTable>(); 
        public DataCollectionTable MainTable;
        
        public DataCollection(string name,string rootDir)
        {
            Name = name;
            RootDir = rootDir;
            InfoPath = rootDir + "\\" + name + ".info";
            
            if(GetInfo())
            {
                GetData();
            }
        }
        
        public bool GetInfo()
        {
            Error = $"設定取得エラー:DataCollection.GetInfo()\r\n";
            
            if(!File.Exists(InfoPath))
            {
                Error += $"infoファイル:{InfoPath}が見つかりませんでした。\r\n";
                retrun false;
            }
            
            try
            {
                using(StreamReader sr = new StreamReader(InfoPath))
                {
                    string line;
                    string processingTableName = "";
                    while((line = sr.ReadLine()) != null)
                    {
                        var contents = line.Split(':');
                        
                        if(contents[0] == "tableNames")
                        {
                            TableNames = contents[1].Split(',').ToList();
                        }
                        else if(contents[0] == "relationalTableNames")
                        {
                            RelationalTableNames = contents[1].Split(',').ToList();
                        }
                        else if(contents[0] == "subTableNames")
                        {
                            SubTableNames = contents[1].Split(',').ToList();
                        }
                        else if(contents[0] == "mainTableName")
                        {
                            MainTableName = contents[1];
                        }
                        else if(contents[0] == "@tableInfo")
                        {
                            processingTableName = "";
                        }
                        else if(contents[0] == "tableName")
                        {
                            processingTableName = contents[1];
                            Tables[processingTableName] = new DataCollectionTable(processingTableName,RootDir+"\\"+processingTableName+".dc");
                            if(processingTableName == MainTableNames)
                            {
                                MainTable = Tables[processingTableName];
                            }
                        }
                        else if(contents[0] == "index")
                        {
                            Tables[processingTableName].Index = contents[1].Split(',').ToList();
                        }
                        else if(contents[0] == "type")
                        {
                            var buff = contents[1].Split(',');
                            if(Tables[processingTableName].Index.Count != buff.Length)
                            {
                                Error += $"{processingTableName}テーブルのtypeリストの要素数:{buff.Length}は不正な値です。\r\n";
                                return false;
                            }
                            
                            for(int i = 0; i < Tables[processingTableName].Index.Count ; i++)
                            {
                                Tables[processingTableName].Type[Tables[processingTableName].Index[i]] = buff[i];
                            }
                        }
                        else if(contents[0] == "empty")
                        {
                            var buff = contents[1].Split(',');
                            if(Tables[processingTableName].Index.Count != buff.Length)
                            {
                                Error += $"{processingTableName}テーブルのemptyリストの要素数:{buff.Length}は不正な値です。\r\n";
                                return false;
                            }
                            
                            for(int i = 0; i < Tables[processingTableName].Index.Count ; i++)
                            {
                                Tables[processingTableName].Empty[Tables[processingTableName].Index[i]] = buff[i];
                            }
                        }
                        else if(contents[0] == "alias")
                        {
                            var buff = contents[1].Split(',');
                            if(Tables[processingTableName].Index.Count != buff.Length)
                            {
                                Error += $"{processingTableName}テーブルのaliasリストの要素数:{buff.Length}は不正な値です。\r\n";
                                return false;
                            }
                            
                            for(int i = 0; i < Tables[processingTableName].Index.Count ; i++)
                            {
                                Tables[processingTableName].Alias[Tables[processingTableName].Index[i]] = buff[i];
                            }
                        }
                        else if(contents[0] == "primaryKey")
                        {
                            if(contents[1] == null || contents[1] == "" || !Tables[processingTableName].Index.Contains(contents[1]))
                            {
                                Error += $"{processingTableName}テーブルのprimaryKey:{contents[1]}は不正な値です。\r\n";
                                return false;
                            }
                            Tables[processingTableName].Primarykey = contents[1]; 
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                Error += $"infoファイル読み込みに失敗しました。\r\n{ex.ToString()}\r\n}";
                retrun false;
            }
        }
        
        public bool GetTableData()
        {
            Error = $"データ取得エラー:DataCollection.GetData()\r\n";
            foreach(string tblName in TableNames)
            {
                if(!Tables[tblName].GetData){
                    Error += Tables[tblName].Error;
                    return false;
                }
            }
        }
    }    
}
