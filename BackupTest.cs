using System;

namespace XX
{
    public class Backup
    {
        public string Error = "";
        public string Msg = "";
        
        string name;
        string srcRootDir;
        string destRootDir;
        
        int oldBackupMaxCount = 10;
        
        
        public Backup(string name,string srcRootDir,string destRootDir)
        {
            this.name = name;
            this.srcRootDir = srcRootDir;
            this.destRootDir = destRootDir;
        }
        
        public bool BackupSync()
        {
            string error = $"バックアップエラー:Backup.BackupSync()\r\n";
            
            //バックアップ元・バックアップ先ルートフォルダパスチェック
            
            //バックアップ元のファイルインフォ一覧取得
            var srcFileList = new DirectoryInfo(srcRootDir).EnumerateFiles("*", SearchOption.AllDirectories).ToList();
            //バックアップ先に作るフォルダ名一覧
            var destDirList = Directory.EnumerateDirectories(srcRootDir, "*", SearchOption.AllDirectories).Select(x => x.Replace(srcRootDir,destRootDir)).ToList();
            
            //バックアップ先のファイルインフォ一覧
            var destFileList = new DirectoryInfo(destRootDir).EnumerateFiles("*", SearchOption.AllDirectories).ToList();
            
            //バックアップ元にあってバックアップ先に無いファイル（新規ファイル）のリスト
            List<FileInfo> newFileList = new List<FileInfo>();
            //
            foreach(FileInfo fI in srcFileList)
            {
                if(fI.FullName.Replace(srcRootDir,destRootDir))
                {
                }
            }
        }
        
        
        public bool BackupAll()
        {
            string error = $"バックアップエラー:Backup.BackupAll()\r\n";
            
            //バックアップ元フォルダのパスチェック
            if(!Directory.Exists(srcRootDir))
            {
                Error = error + $"バクアップ元フォルダ:{srcRootDir}が見つかりませんでした。\r\n";
                return false;
            }
            
            //バックアップ先のルートフォルダを作成
            destRootDir += "\\" + DateTime.Now.ToString("yyyyMMddhhmmss"); 
            if(!Directory.Exists(destRootDir))
            {
                Directory.CreateDirectory(destRootDir);
            }
            
            //ソース元フォルダのファイルインフォ一覧取得
            List<FileInfo> srcFileList = new DirectoryInfo(srcRootDir).EnumerateFiles("*", SearchOption.AllDirectories).ToList();
            //コピー先に作るフォルダ名一覧
            List<string> destDirList = srcFileList.Select(x => x.DirectoryName.Replace(srcRootDir,destRootDir)).Distinct().ToList();
            
            //ソース元ファイルの合計サイズ
            int srcFilesSize = srcFileList.Sum(x => x.Length);
            //バックアップ先ドライブの空き容量
            int destDriveFreeSize = new DriveInfo(destRootDir.Split('\')[0].Replace(":","")).TotalFreeSpace;
            //バックアップ先のドライブの空き容量とコピーするファイル容量を比較
            //不足していたらfalseを返す
            if(srcFileSize >= destDriveFreeSize)
            {
                Error = error + $"バクアップ先のドライブの空き容量が不足しています。\r\n";
                return false;
            }
            
            //バックアップ先ルートフォルダに子フォルダを作成
            foreach(string dir in destDirList)
            {
                try
                {
                    if(!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
                catch(Exception ex)
                {
                    Error = error + $"バックアップ先にフォルダ:{dir}を作成できませんでした。\r\n{ex.ToString()}\r\n";
                    return false;
                }
            }
            //ファイルをコピー
            foreach(string fl in srcFileList)
            {
                try
                {
                    File.Copy(fl,fl.Replace(srcRootDir,destRootDir));
                }
                catch(Exception ex)
                {
                    Error = error + $"ファイル:{fl}をコピーできませんでした。\r\n{ex.ToString()}\r\n";
                    return false;
                }
            }
                                                                    
                                                                    
            return true;
        }
    }
}
