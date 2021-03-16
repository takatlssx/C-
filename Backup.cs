//１：バックアップ元のファイル一覧リスト
List<string> srcFileList = Directory.GetFiles(srcRootDir,"*",SearchOption.AllDirectories).ToList();
//２：新しいファイル・更新日時が違うファイルのリスト
List<string> newFileList = new List<string>();
//３：newFileListの中で、バックアップ先に無いフォルダリスト
List<string> newDirList = new List<string>();
//４：バックアップ先の中で、バックアップ元に無いファイル・更新日時が違うファイルのリスト
List<string> oldFileList = new List<string>();

//ファイルを比較し上記各リストに追加
foreach(string file in srcFileList)
{
    //もしバックアップ先にファイルが無ければ、newFileListに追加    
    if(!File.Exists(file.Replace(srcRootDir,destRootDir)))
    {
        newFileList.Add(file);
        //そのファイルのフォルダがバックアップ先に無ければnewDirListに追加
        if(!Directory.Exists(Path.GetDirectoryName(file.Replace(srcRootDir,destRootDir))))
        {
            newDirList.Add(Path.GetDirectoryName(file.Replace(srcRootDir,destRootDir)));
        }
    }
    else if(File.GetLastWriteTime(file) != File.GetLastWriteTime(file.Replace(srcRootDir,destRootDir)))
    {
        //もし更新日時が違っていればnewFileListに追加
        newFileList.Add(file);
        //バックアップ先のファイルはoldFileListに追加
        oldFileList.Add(file.Replace(srcRootDir,destRootDir));
    }
}

//バックアップ先にあって、バックアップ元に無いファイルをoldFileListに追加
var destFileList = Directory.GetFiles(destRootDir,"*",SearchOption.AllDirectories).Where(x => !x.Contains("Z:\\Movie\\oldfile")).ToList();
oldFileList.AddRange(destFileList.Where(x => !File.Exists(x.Replace(destRootDir,srcRootDir))).ToArray());

//バックアップするファイルの容量とバックアップ先のドライブ空き容量を比較し
//足りなければ処理を終了
long fileSize = srcFileList...;
long destFreeSize = dest.....;
if(fileSize >= destFreeSize)
{
    return false;
}
