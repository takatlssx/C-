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
        }
        
        private bool createTable(string[] index,string[] type,string[] empty,string[] alias,string primayKey)
        {
            //DataPathチェック
            if(!File.Exists(DataPath))
            {
                Error += $"{Name}テーブルのデータファイル:{DataPath}が見つかりませんでした。\r\n";
                return false;
            }
            
            if(!(index.Length == type.Length == empty.Length == alias.Length))
            {
            }
            return true;
        }
    }
}
