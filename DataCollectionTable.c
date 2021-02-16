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
        
        public DataCollectionTable(string name,string dataPath,string[] index,string[] type,string[] empty,string[] alias,string primayKey)
        {
            Name = name;
            DataPath = dataPath;
            //createTable();
        }
        
        private bool createTable(string[] index,string[] type,string[] empty,string[] alias,string primayKey)
        {
            Error += $"テーブル作成エラー:DataCollectionTable.createTable()\r\n";
            
            //DataPathチェック
            if(!File.Exists(DataPath))
            {
                Error += $"{Name}テーブルのデータファイル:{DataPath}が見つかりませんでした。\r\n";
                return false;
            }
            //index他、引数のリストの要素数が同一かチェック
            if(!(index.Length == type.Length == empty.Length == alias.Length))
            {
                Error += $"{Name}テーブルの列・型・null許容・別名のリスト数が一致しません。\r\n";
                return false;
            }
            
            //primarykeyが空白もしくはindex配列に存在しない値でないかチェック
            if(primaryKey == null || primaryKey == "" || (Array.IndexOf(index,primaryKey) == -1))
            {
                Error += $"{Name}テーブルのprimarykey:{primaryKey}は不正な値です。\r\n";
                return false;
            }
            
            //Indexリストを作成
            Index = index.ToList();
            
            //PrimaryKeyを作成
            PrimaryKey = primaryKey;
            
            //indexをキーとしempty,type,aliasのディクショナリを作成            
            for(int i = 0 ; i < index.Length ; i++)
            {
                Type[index[i]] = type[i];
                Empty[index[i]] = empty[i];
                Alias[index[i]] = alias[i];
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
                    
                        if(index.Length != buff.Length)
                        {
                            Error += $"{Name}テーブルのデータ数:{buff.Length}は不正な値です。\r\n";
                            return false;
                        }
                    
                        for(int i = 0 ; i < index.Length ; i++)
                        {
                            lineDict[index[i]] = buff[i];
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
