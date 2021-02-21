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
