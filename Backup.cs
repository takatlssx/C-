private void getInfo()
{
    using(StreamReader sr = new StreamReader("P:\\D#\\BackupManager\\app.info"))
    {
    }
}



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
    //もし同名のファイルがあったら更新日時を比較
    else if(File.GetLastWriteTime(file) != File.GetLastWriteTime(file.Replace(srcRootDir,destRootDir)))
    {
        //もし更新日時が違っていればnewFileListに追加
        newFileList.Add(file);
        //更新日時の違うバックアップ先のファイルはoldFileListに追加
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

//oldFileListのファイルをバックアップ先の「oldfile」フォルダに移動
if(oldFileList.Count != 0)
{
    //バックアップ先のルートフォルダ直下に「oldfile」フォルダが有るか確認
    //なければ作成
    if(!Directory.Exists(destRootDir+"\\oldfile"))
    {
        try
        {
            Directory.CreateDirectory(destRootDir+"\\oldfile");
        }
        catch(Exception ex)
        {
            Msg += $"旧ファイルを保存するフォルダ:{destRootDir+"\\oldfile"}\r\nの作成に失敗しました。\r\n{ex.ToString()}\r\n";
        }
    }
    foreach(string file in oldFileList)
    {
        try
        {
            File.Move(file,destRootDir+"\\oldfile\\"+Path.GetFileName(file));
        }
        catch(Exception ex)
        {
            Msg += $"旧ファイル:{file}\r\nをoldfileフォルダに移動できませんでした。\r\nこのファイルはバックアップ時上書きされる可能性が有ります。\r\n{ex.ToString()}\r\n";
        }
    }
}

//newDirListを作成(あれば)
if(newDirList.Count != 0)
{
    foreach(string dir in newDirList)
    {
        try
        {
            Directory.CreateDirectory(dir.Replace(srcRootDir,destRootDir));
        }
        catch(Exception ex)
        {
            Error = error + $"ディレクトリ:{dir}\r\nの作成に失敗しました。\r\n{ex.ToString()}\r\n";
            return false;
        }
    }
}

string copyStatusStr = "";
int failCount = 0;
//ファイルのコピー
for(int i = 0 ; i < newFileList ; i++)
{
    try
    {
        File.Copy(newFileList[i],newFileList[i].Replace(srcRootDir,destRootDir));
    }
    catch(Exception ex)
    {
        Msg += $"ファイル:{newFileList[i]}\r\nをバックアップできませんでした。\r\n{ex.ToString()}\r\n";
        failCount++;
    }
}

int successCount = newFileList.Count - failCount;
copyStatusStr = $"{successCount}個のファイルのバックアップに成功しました。\r\n";
if(failCount > 0){copyStatusStr += $"{failCount}個のファイルのバックアップに失敗しました。\r\n";}
