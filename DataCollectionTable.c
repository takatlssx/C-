using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Test
{
    public class DataCollectionTable
    {
        public string Error = "";
        public string Msg = "";
        
        public string Name;
        public string DataPath;
        
        public List<string> Index = new List<string>();
        public Dictionary<string,string> Type = new Dictionay<string,string>();
        public Dictionary<string,string> Empty = new Dictionay<string,string>();
        public Dictionary<string,string> Alias = new Dictionay<string,string>();
        public string PrimaryKey;
        
        public List<Dictionay<string,string>> Data = new List<Dictionay<string,string>>();
        
        public DataCollectionTable(string name,string dataPath)
        {
            Name = name;
            DataPath = dataPath;
            //createTable();
        }
        
        private bool createTable()
        {
            Error += $"テーブル作成エラー:DataCollectionTable.createTable()\r\n";
            
            //DataPathチェック
            if(!File.Exists(DataPath))
            {
                Error += $"{Name}テーブルのデータファイル:{DataPath}が見つかりませんでした。\r\n";
                return false;
            }
                        
            //データ読み込み
            try
            {            
                using(StreamReader sr = new Streamreader(DataPath))
                {
                    string line;
                    while((line = sr.ReadLine())! = null)
                    {
                        Dictionary lineDict = new Dictionary<string,string>();
                        var buff = line.Split(',');
                    
                        if(Index.Count != buff.Length)
                        {
                            Error += $"{Name}テーブルのデータ数:{buff.Length}は不正な値です。\r\n";
                            return false;
                        }
                    
                        for(int i = 0 ; i < Index.Count ; i++)
                        {
                            lineDict[Index[i]] = buff[i];
                        }
                    
                        Data.Add(lineDict);                    
                    }
                }
            }
            catch(Exception ex)
            {
                Error += $"{Name}テーブルのデータ取得に失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }
            
            return true;
        }
    }
}
