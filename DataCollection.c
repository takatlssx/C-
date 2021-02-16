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
