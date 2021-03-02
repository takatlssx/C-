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
        
        public void SetTextInvoker(string ctlName,string text)
        {
            ownerForm.Invoke(new Action(() => ownerForm.Controls[ctlName].Text = text));
        }
        
        public void SetProgressInvoker(string ctlName,int val)
        {
            ownerForm.Invoke(new Action(() => ownerForm.Controls[ctlName].value = val));
        }
        
        public bool BackupSync()
        {
            string error = $"バックアップエラー:Backup.BackupSync()\r\n";
            //バックアップするファイルのリスト
            List<FileInfo> newFileList = new List<FileInfo>();            
            //バックアップ先にあってバックアップ元に無い・
            //バックアップ元と更新日時が異なるファイル名（旧ファイル）リスト
            List<string> oldFileList = new List<string>();
            //バックアップ元のファイルインフォ一覧
            List<FileInfo> srcFileList;
            //バックアップ先に作るフォルダ名一覧
            List<string> destDirList;            
            //バックアップ先のファイルインフォ一覧
            List<FileInfo> destFileList;
            
            //バックアップ元・バックアップ先ルートフォルダパスチェック
            
            
            SetTextInvoker("SystemStatusLabel","バックアップ情報を取得しています。。。。");
            try
            {
                //バックアップ元のファイルインフォ一覧
                srcFileList = new DirectoryInfo(srcRootDir).EnumerateFiles("*", SearchOption.AllDirectories).ToList();
                //バックアップ先に作るフォルダ名一覧
                destDirList = Directory.EnumerateDirectories(srcRootDir, "*", SearchOption.AllDirectories).Select(x => x.Replace(srcRootDir,destRootDir)).ToList();            
                //バックアップ先のファイルインフォ一覧
                destFileList = new DirectoryInfo(destRootDir).EnumerateFiles("*", SearchOption.AllDirectories).ToList();
            }
            catch(Exception ex)
            {
                Error = error + $"バックアップ情報の取得に失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }
            
            SetTextInvoker("SystemStatusLabel","ファイルの差分を検証しています。。。。");
            try
            {
                //バックアップ元にあってバックアップ先に無いファイル（新規ファイル)
                //バックアップ元・バックアップ先の同名ファイルで更新日時が違うファイル（更新ファイル）
                //以上をnewFileListに追加
                foreach(FileInfo fI in srcFileList)
                {
                    //新規ファイル
                    if(!File.Exists(fI.FullName.Replace(srcRootDir,destRootDir)))
                    {
                        newFileList.Add(fI);
                    }
                    else
                    {
                        if(fI.LastWriteTime != File.GetLastWriteTime(fI.FullName.Replace(srcRootDir,destRootDir)))
                        {
                            newFileList.Add(fI);
                            //更新日時が違うバックアップ先ファイルをoldFileListへ追加
                            oldFileList.Add(fI.FullName.Replace(srcRootDir,destRootDir));
                        }
                    }
                }
                
                //バックアップ先にあってバックアップ元に無いファイルをoldFileListに追加
                //但しバックアップ先のoldfileフォルダにあるファイルは除外
                oldFileList.AddRange(destFileList.Where(x => !File.Exists(x.FullName.replace(destRootDir,srcRootDir)) && !x.FullName.Contains(destRootDir+"\\oldfile")).ToList());
            }
            catch(Exception ex)
            {
                Error = error + $"ファイルの差分情報検証に失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }          
            
            SetTextInvoker("SystemStatusLabel","フォルダ構造をコピーしています。。。。");
            //フォルダのコピー
            foreach(string dir in destDirList)
            {
                if(!Directory.Exists(dir))
                {
                    try
                    {                       
                        Directory.CreateDirectory(dir);
                    }
                    catch(Exception ex)
                    {
                        Error = error + $"フォルダ:{dir}が作成できませんでした。\r\n{ex.ToString()}\r\n";
                        return false;
                    }
                }
            }
            
            //旧ファイル移動
            
            SetTextInvoker("SystemStatusLabel","ファイルをコピーしています。。。。");
            SetProgressInvoker("",0);
            //ファイルのコピー
            for(int i = 0 ; i < srcFileList.Count ; i++)
            {
                try
                {
                    File.Copy(srcFileList[i].FullName,srcFileList[i].FullName.Replace(srcRootDir,destRootDir));                    
                }
                catch(Exception ex)
                {
                    Error = error + $"ファイル:{srcFileList[i].FullName}をコピーできませんでした。\r\n{ex.ToString()}\r\n";
                    return false;
                }
                int pV = (i*100)/srcFileList.Count-1;
                SetProgressInvoker("",pV);
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
