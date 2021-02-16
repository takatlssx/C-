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
        
        public List<DataCollectionTable> Tables = new List<DataCollectionTable>(); 
        
        public DataCollection(string name,string rootDir)
        {
            Name = name;
            RootDir = rootDir;
            InfoPath = rootDir + "\\" + name + ".info";
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
                        else if(contents[0] == "@tableInfo")
                        {
                            processingTableName = "";
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
        
        public bool CreateTable()
        {
            foreach(string tblName in TableNames)
            {
                string dcPath = RootDir + "\\" + tblName + ".dc";
                
            }
        }
    }  
    
}
