using System;

namespace XX
{
    public class Backup
    {
        public string Error = "";
        public string Msg = "";
        
        private string srcRootDir;
        private string destRootDir;
        
        public Backup(string srcRootDir,string destRootDir)
        {
        }
        
        public bool BackupAll()
        {
            string error = $"バックアップエラー:Backup.BackupAll()\r\n";
            //パスチェック
            if(!Directory.Exists(srcRootDir))
            {
                return false;
            }
            
            destRootDir += "\\" + DateTime.Now.ToString("yyyyMMddhhmmss"); 
            
            //ソース元フォルダのファイル一覧取得（FileInfo）
            List<FileInfo> srcFileList = new DirectoryInfo(srcRootDir).EnumerateFiles("*", SearchOption.AllDirectories).ToList();
            //コピー先の新ファイル名リスト（srcFileListをlinqでリネーム）
            List<string> destFileList = srcFileList.ConvertAll( x => x.FullName.Replace(srcRootDir,destRootDir)).ToList();
            //コピー先に作るフォルダ名一覧
            List<string> destDirList = destFileList.Select(x => Path.GetDirectoryName(x)).Distinct().ToList();
            
            //ソース元ファイルの合計サイズ
            int srcFilesSize = secFileList.Sum(x => x.Length);
            //コピー先ドライブの空き容量
            int destDriveFreeSize = 0;
            
            //コピー先へディレクトリ作成
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
                    return false;
                }
            }
            return true;
        }
    }
}
