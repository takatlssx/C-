//１：バックアップ元のルートフォルダのファイル一覧を取得する
var srcFileList = Directory.GetFiles(srcRootDir,"*",SearchOption.AllDirectories);

long secFileSize = srcFileList.Select(x => new FileInfo(x).Length).ToList.Sum();
//２：バックアップ先のルートフォルダのファイル一覧を取得する
var destFileLIst = Directory.GetFiles(destRootDir,"*",SearchOption.AllDirectories);
//３：バックアップ元にあってバックアップ先に無いファイル、あっても更新日時が違うファイルを取得する
//   そこから新しいフォルダ一覧も取得する
var newFileList;
var oldFileList;
var newDirList;
foreach(string file in srcFileList)
{
    if(!File.Exists(file.Replace(srcRootDir,destRootDir)))
    {
        newFileList.Add(file);
        if(!Directory.Exists(Path.GetDirectoryName(file.Replace(srcRootDir,destRootDir))))
        {
            newDirList.Add(Path.GetDirectoryName(file.Replace(srcRootDir,destRootDir)));
        }
    }
    else if(File.GetLastWriteTime(file) != File.GetLastWriteTime(file.Replace(srcRootDir,destRootDir)))
    {
        newFileList.Add(file);
        oldFileList.Add(file.Replace(srcRootDir,destRootDir));
    }
}


//５：バックアップ先にあってバックアップ元に無いファイル（古いファイル）一覧を取得する。
//６：バックアップするファイルの容量とバックアップ先のドライブ空き容量を比較し
//    足りなければ処理を中段
