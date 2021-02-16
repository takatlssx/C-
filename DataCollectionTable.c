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
        
        //データのインデックス番号取得
        public int GetDataNumber(string indexName,string data)
        {
            int resInt = -1;
            if(!Index.Contains(indexName))
            {
                Error +=$"DataCollectionTable.GetDataNumber()\r\n{Name}テーブルに列'{}'は存在しません。\r\n";
                return resInt;
            }
            for(int i = 0 ; i < Data.Count ; i++)
            {
                if(Data[i][indexName] == data)
                {
                    resUnt = i;
                    break;
                }
            }
            return resInt;
        }
        
        //検証（ヴァリデーション）
        private bool validate(Dictionary<string,string> newData,string mode="regist"){
            error = $"{Name}テーブルデータ検証エラー:DataCollectionTable.validate()\r\n";
            //データ数(registモードなら同一か？、editモードなら超えていないか)
            
            //管理番号(registモードのみ)
            
            //型、nullチェック
        }
        
        //検索
        public List<Dictionary<string,string>> Search(List<string> idList,List<string> wordList,List<string,string> operandList,string andOr = "or")
        {
            error = $"データ検索エラー:DataCollectionTable.Search()\r\n";
            
            List<Dictionary<string,string>> result = new List<Dictionary<string,string>>();
            
            //引数の値を検証
            foreach(string id in idList){
                if(!Index.Contains(id)){
                    Error += error + $"{Name}テーブルに列'{id}'は存在しません。\r\n";
                    return null;
                }
            }
            
            List<string> ops = new List<string>{"=","like"}
            foreach(string ope in operandList){
                if(!ops.Contains(ope)){
                    Error += error + $"比較記号'{ope}'は不正な値です。'='か'like'のみ有効です。\r\n";
                    return null;
                }
            }
            
            if(andOr != "and" && andOr != "or" ){
                Error += error + $"組み合わせ記号'{andOr}'は不正な値です。'and'か'or'のみ有効です。\r\n";
                return null;
            }
            
            //検索
        }
        
        //編集
        public bool Edit(string id, Dictionary<string,string> newData)
        {
            error = $"{Name}テーブルデータ編集エラー:DataCollectionTable.Edit()\r\n";
            Msg += $"{Name}テーブルの管理番号'{id}'のデータを変更しました。\r\n";
            int indexNum = GetDataNumber(PrimaryKey,id);
            if(indexNum == -1)
            {
                Error = error + Error;
                return false;
            }
            if(!validate(newData,"edit")){
                Error = error + Error;
                return false;
            }
            //編集前のデータ
            var oldData = new Dictionary<string,string>(Data[indexNum]);
            
            try{
                foreach(string key in newData.Keys)
                {
                    Data[indexNum][key] = newData[key];
                    Msg += $"    列'{key}'  {oldData[key]} → {newData[key]}\r\n";
                }
            }
            catch(Exception ex){
                Error += error + $"データの変更に失敗しました。\r\n{ex.ToString()}\r\n";
            }            
            return true;
        }
        
        //新規登録
        public bool Regist(Dictionary<string,string> newData)
        {
            error = $"{Name}テーブルデータ登録エラー:DataCollectionTable.Regist()\r\n";
            if(!validate(newData)){
                Error = error + Error;
                return false;
            }
            
            Data.Add(newData);
            Msg += $"管理番号：{newData[PrimaryKey]}のデータを新規登録しました。\r\n";
            return true;
        }
        
        public bool GetData()
        {
            Error += $"テーブル作成エラー:DataCollectionTable.GetData()\r\n";
            
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
