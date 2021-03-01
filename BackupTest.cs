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
            //パスチェック
            if(!Directory.Exists(srcRootDir))
            {
                return false;
            }
            
            //ソース元フォルダのファイル一覧取得（FileInfo）
            List<FileInfo> srcFileList = new DirectoryInfo(srcRootDir).EnumerateFiles("*", SearchOption.AllDirectories).ToList();
            List<string> destFileList = srcFileList.ConvertAll( x => x.FullName.Replace(srcRootDir,destRootDir)).ToList();
            int srcFilesSize = secFileList.Sum(x => x.Length);
            
            return true;
        }
    }
}
