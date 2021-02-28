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
        public List<Dictionari<string,string>> ViewingData;
        public string ViewingStatusText = "";
        public string SystemStatusText = "";
        
        public List<string> searchIdList = new List<string>();
        public List<string> searchWordList = new List<string>();
        public List<string> searchOperandList = new List<string>();
        public string searchCombination = "or";
        
        public void SetData(string dataFlg = "all",string sortOrder = "asc")
        {
            SystemStatusText = "データベース表示";
            if(dataFlg == "all")
            {
                ViewingData = DC.MainTable.Data;
                ViewingStatusText = $"全件表示【{ViewingData.Count}件】";
            }
            else //"search"
            {
                var res = DC.MainTable.Search(serchIdList,searchWordList,searchOperandList,searchCombination);
                
                ViewingStatusText = $"【検索結果】";
                for(int i = 0 ; i < searchIdList.Count ; i++)
                {
                    ViewingStatusText += $"条件{i} : 列'{DC.Alias[searchIdList[i]]}' {searchOperandList[i]} '{searchWordList[i]}' "
                }
                if(res == null || res.Count == 0)
                {
                    ViewingData = null;
                    ViewingStatusText += $"【0件】";
                }
                else
                {
                    ViewingData = res;
                    ViewingStatusText += $"【{res.Count}件】";
                }
            }
            
            //dGV描画
            dGV.Columns.Clear();
            dGV.Rows.Clear();
            
            
            //listBox描画
            listCategory.Items.Clear();
            listCategory.Items.Add("全て");
            foreach(var dt in DC.Tables["category"].Data)
            {
                listCategory.Items.Add(dt["category"]);
            }
            listTag.Items.Clear();
            listTag.Items.Add("全て");
            listSeries.Items.Clear();
            listSeries.Items.Add("全て");
            
            if(ViewingData == null || dataFlg == "all")
            {
                foreach(var dt in DC.Tables["tag"].Data)
                {
                    listTag.Items.Add(dt["tag"]);
                }
                foreach(var dt in DC.Tables["series"].Data)
                {
                    listSeries.Items.Add(dt["series"]);
                }
                
            }
            else
            {
                foreach(var dt in ViewingData)
                {
                    string [] buff = dt["tag"].Split('/');
                    int listTagSelectedIndex = 0;
                    int addCnt = 0;
                    foreach(string bf in buff)
                    {
                        if(bf != "" && !listTag.Items.Contains(bf))
                        {
                            listTag.Items.Add(bf);
                            addCnt++;
                            
                            if(searchIdList.Contains("tag") && wordList[searchIdList.IndexOf("tag")] == bf)
                            {
                                listTagSelectedIndex = addCnt;
                            }
                        }
                    }
                    
                    buff = dt["series"].Split('/');
                    int listSeriesSelectedIndex = 0;
                    int addCntSeries = 0;
                    foreach(string bf in buff)
                    {
                        if(bf != "" && !listSeries.Items.Contains(bf))
                        {
                            listSeries.Items.Add(bf);
                            addCntSeries++;
                            
                            if(searchIdList.Contains("series") && wordList[searchIdList.IndexOf("series")] == bf)
                            {
                                listSeriesSelectedIndex = addCnt;
                            }
                        }
                    }                    
                }
                listTag.SetSelect(listTagSelectedIndex,true);
                listSeries.SetSelect(listSeriesSelectedIndex,true);
            }
            
        }
        
        public void SetSelectedData(){
        }
        
        
        
        
        
        
        
        
        
        
        
        
        
        public List<string> SearchUnRegisteredData()
        {
            //データベースに登録されていないファイルパスを格納するリスト
            List<string> notExistsFileList = new List<string>();
            
            //G:\Movie内のファイルを取得
            List<string> fList = Directory.GetFiles("G:\\Movie", "*", SearchOption.AllDirectories).ToList();
            
            //メインテーブルの列'file'に登録されていないファイルをnotExistsFileListに加える
            try
            {
                foreach(string fl in fList)
                {
                    if(MainTable.GetDataNumber("file",fl) == -1)
                    {
                        notExistsFileList.Add(fl);
                    }
                }
            }
            catch(Exception ex)
            {
                Msg = error + $"未登録ファイルの検索に失敗しました。\r\n{ex.ToString()}\r\n";
                return new List<string>();
            }
            
            //未登録ファイルが無ければMsgに出す。
            if(notExistsFileList.Count == 0)
            {
                Msg = $"データベースに登録されていないファイルはありませんでした。\r\n";
            }           
            
            return notExistsFileList;
        }
        public bool RegistUnregisteredData(List<string> notExistsFileList)
        {
            string error = $"データベース未登録ファイル自動登録エラー:DataCollection.RegistUnregisteredData()\r\n";
            
            //未登録ファイルをループしデータベースのRegistメソッドで登録
            foreach(string fl in notExistsFileList)
            {
                Dictionary<string,string> newData = new Dictionary<string,string>();                
                try
                {
                    newData["id"] = "";
                    newData["title"] = Path.GetFileNameWithoutExtension(fl);
                    newData["subtitle"] = Path.GetFileNameWithoutExtension(fl);
                    newData["number"] = "0000";
                    newData["category"] = fl.Split('\\')[2]:
                    newData["tag"] = "";
                    newData["series"] = "";
                    newData["actor"] = "";
                    newData["source"] = "";
                    newData["date"] = new FileInfo(fl).LastWriteTime.ToString("yyyy/MM/dd");
                    newData["file"] = fl;
                    newData["rate"] = "0";
                    newData["detail"] = "";
                    
                    if(!RegistData(newData))
                    {
                        Error = error + Error;
                        return false;
                    }
                    
                }
                catch(Exception ex)
                {
                    Error = error + $"新規データ登録に失敗しました。\r\n{ex.ToString()}\r\n";
                    return false;
                }
                
            }
            return true;
        }
        public bool BackUp()
        {
            //コピー元のファイル一覧取得 
            
            //コピー先のファイル一覧取得
            
            //コピー先一覧とコピー元一覧のリストを比較
            //１：コピー先に無いファイル（新規かリネームされたか）
            //２：ファイル名は同じでも更新日時・作成日時が新しい場合
            //３：ファイル名が同じでもファイルデータサイズが違う場合
            //以上をcopyFileListとしてリスト化
        }
        
        
        private bool rollBack()
        {
            string error = "ロールバックエラー:DataCollection.rollBack()\r\n";
            
            string backupDir = RootDir + "\\backup";
            
            //バックアップフォルダの存在チェック
            if(!Directory.Exists(backupDir))
            {
                Error = error + $"バックアップフォルダ:{backupDir}が見つかりませんでした。\r\n";
                return false;
            }
            
            //バックアップフォルダ内の.dc.backupファイルの一覧を取得
            
            //.dc.backupファイルをルートフォルダに.backupを削除しコピー。
            
            
            return true;
        }
        
        private bool backup()
        {
            string error = "バックアップエラー:DataCollection.backup()\r\n";
            
            string backupDir = RootDir + "\\backup";
            
            //バックアップフォルダの存在チェック
            if(!Directory.Exists(backupDir))
            {
                Error = error + $"バックアップフォルダ:{backupDir}が見つかりませんでした。\r\n";
                return false;
            }
            
            //ルートフォルダ内の.dcファイル一覧を取得
            
            //.dcファイルをバックアップフォルダに「〇〇.dc.backup」という名前で保存
            
            return true;
        }
    }
}
