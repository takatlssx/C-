using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Movie
{
    public class Backup
    {
        #region 変数メンバ
        public string Error;
        public string Msg;

        //コピー元のルートフォルダ
        public string SrcRootDir;

        //コピー先のルートフォルダ
        public string DestRootDir;

        //コピー先の古いファイルを格納するフォルダ
        //コピー先にあって、コピー元にない名前のファイル
        //もしくは同名でも更新日時が違うファイルを入れる
        public string DestOldFileDir;

        //コピー先のバックアップログファイル
        public string BackupLog;

        List<string> srcFileList;
        List<string> destFileList;

        public List<string> CopyFileList = new List<string>();
        public List<string> OldFileList = new List<string>();
        public List<string> DirList = new List<string>();

        public FormMain owneForm;
        #endregion

        //コンストラクタ
        public Backup(string srcRootDir, string destRootDir, FormMain fm)
        {
            SrcRootDir = srcRootDir;
            DestRootDir = destRootDir;
            DestOldFileDir = destRootDir + "\\oldfile";
            BackupLog = destRootDir + "\\backup.log";
            owneForm = fm;

        }

        #region 処理単位毎のメソッド
        //①バックアップ情報を取得
        //コピー元、コピー先のファイル一覧取得
        public bool GetBackupInfo()
        {
            string error = $"ファイルバックアップ情報取得エラー:Backup.GetBackupInfo()\r\n";
            SetStatusTextInvoker("バックアップ情報を取得しています。。。");

            //ディレクトリ存在チェック
            if (!Directory.Exists(SrcRootDir))
            {
                Error = error + $"バックアップ元ルートフォルダ:{SrcRootDir}は存在しません。\r\n";
                return false;
            }
            if (!Directory.Exists(DestRootDir))
            {
                Error = error + $"バックアップ先ルートフォルダ:{DestRootDir}は存在しません。\r\n";
                return false;
            }
            if (!Directory.Exists(DestOldFileDir))
            {

                Error = error + $"バックアップ先・旧ファイルフォルダ:{DestOldFileDir}は存在しません。\r\n";
                return false;
            }

            //バックアップ元のファイル一覧取得
            try
            {
                srcFileList = Directory.GetFiles(SrcRootDir, "*", SearchOption.AllDirectories).ToList();
            }
            catch (Exception ex)
            {
                Error = error + $"バックアップ元ルートフォルダ:{SrcRootDir}内のファイル取得に失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }
            //バックアップ先のファイル一覧取得
            try
            {
                destFileList = Directory.GetFiles(DestRootDir, "*", SearchOption.AllDirectories).ToList();
            }
            catch (Exception ex)
            {
                Error = error + $"バックアップ先ルートフォルダ:{DestRootDir}内のファイル取得に失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }

            //ファイルを検証しコピーするファイルをリスト化
            foreach (string fl in srcFileList)
            {
                try
                {
                    //一時的にコピー元のファイルパスをコピー先のルートディレクトリにリプレース
                    string temp = fl.Replace(SrcRootDir, DestRootDir);

                    //一時ファイル名がコピー先に無い新しいパスのものならCopyFileListに追加
                    if (!destFileList.Contains(temp))
                    {
                        CopyFileList.Add(fl);
                    }
                    //ファイル名が同じ場合更新日時を比較し異なればコピーリストに追加
                    //コピー先ファイル名はOldFileListに追加
                    else
                    {
                        DateTime srcDt = new FileInfo(fl).LastWriteTime;
                        DateTime destDt = new FileInfo(temp).LastWriteTime;
                        if (srcDt != destDt)
                        {
                            CopyFileList.Add(fl);
                            OldFileList.Add(temp);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Error = error + $"ファイル:{fl}の比較検証に失敗しました。\r\n{ex.ToString()}\r\n";
                    return false;
                }

            }

            //バックアップ先のファイルでバックアップ元に存在しないファイルを取得（古いファイル）しリスト化
            foreach (string fl in destFileList)
            {
                try
                {
                    string temp = fl.Replace(DestRootDir, SrcRootDir);
                    if (!srcFileList.Contains(temp) && !fl.Contains(DestOldFileDir) && fl != DestRootDir+"\\backup.log")
                    {
                        OldFileList.Add(fl);
                    }
                }
                catch (Exception ex)
                {
                    Error = error + $"ファイル:{fl}が古いファイルかの比較検証に失敗しました。\r\n{ex.ToString()}\r\n";
                    return false;
                }

            }
            return true;
        }

        //②コピー元のフォルダ構造をコピー先にコピー
        public bool CopyDirectories()
        {
            string error = $"フォルダ構造コピーエラー:Backup.CopyDirectories()\r\n";
            SetStatusTextInvoker("フォルダ構造をコピーしています。。。");
            SetProgressBarValueInvoker(0);
            //コピー元フォルダ一覧取得
            try
            {
                DirList = Directory.EnumerateDirectories(SrcRootDir, "*", SearchOption.AllDirectories).ToList();
            }
            catch (Exception ex)
            {
                Error = error + $"コピー元のディレクトリ一覧の取得に失敗しました。\r\n{ex.ToString()}\r\n";
                return false;
            }
            int cnt = 0;
            foreach (string dir in DirList)
            {
                if (!Directory.Exists(dir.Replace(SrcRootDir, DestRootDir)))
                {
                    try
                    {
                        Directory.CreateDirectory(dir.Replace(SrcRootDir, DestRootDir));
                        int valInt = (cnt * 100) / DirList.Count;
                        SetProgressBarValueInvoker(valInt);
                        cnt++;
                    }
                    catch (Exception ex)
                    {
                        Error = error + $"フォルダ:{dir}をコピーできませんでした。\r\n{ex.ToString()}\r\n";
                        return false;
                    }
                }

            }
            SetStatusTextInvoker($"{DirList.Count}個のフォルダをコピーしました。");
            return true;
        }

        //③コピー先の（コピー元にないファイル名のファイル・コピー元と更新日時(LastWriteTime)が異なるファイルを古いファイルとして移動）
        //OldFileListをDestOldFileDirにMove
        public bool MoveDestOldFile()
        {
            string error = $"OLDファイルMoveエラー:Backup.MoveDestOldFile()\r\n";
            SetProgressBarValueInvoker(0);
            SetStatusTextInvoker("コピー先の古いファイルをoldfileフォルダに移動しています。。。");

            if (OldFileList.Count == 0 && OldFileList == null)
            {
                return true;
            }

            //OldFileListをDestOldFileDirにMove
            int cnt = 1;
            foreach (string oldfile in OldFileList)
            {
                try
                {
                    File.Move(oldfile, DestOldFileDir + "\\" + Path.GetFileName(oldfile));
                    int valInt = (cnt * 100) / OldFileList.Count;
                    SetProgressBarValueInvoker(valInt);
                    cnt++;
                }
                catch (Exception ex)
                {
                    Error = error + $"ファイル:{oldfile}を旧ファイルフォルダ:{DestOldFileDir}に移動できませんでした。\r\n{ex.ToString()}\r\n";
                    return false;
                }
            }
            return true;
        }

        //④コピーファイルリストのをコピー
        public bool CopyFiles()
        {
            string error = $"ファイルコピーエラー:Backup.CopyFiles()\r\n";
            if (CopyFileList.Count == 0 || CopyFileList == null)
            {
                Error = error + $"コピーすべきファイル（新規ファイル・更新ファイル）はありませんでした。\r\n";
                return false;
            }           

            int cnt = 1;
            SetProgressBarValueInvoker(0);
            SetStatusTextInvoker($"{CopyFileList.Count}個のファイルをバックアップしています。。。");
            foreach (string srcfl in CopyFileList)
            {
                string destfl = srcfl.Replace(SrcRootDir, DestRootDir);

                try
                {
                    File.Copy(srcfl, destfl, true);
                    int valInt = (cnt * 100) / CopyFileList.Count;
                    SetProgressBarValueInvoker(valInt);
                    cnt++;
                }
                catch (Exception ex)
                {
                    Error = error + $"ファイル:{srcfl}をコピーできませんでした。\r\n{ex.ToString()}\r\n";
                    return false;
                }
            }
            return true;
        }
        #endregion

        //バックアップ一連処理メソッド
        //各処理単位①GetBackupInfo()→②CopyDirectories()→③MoveDestOldFile()→④CopyFiles()を一連処理
        public bool BackupData()
        {
            string error = "バックアップ処理エラー:Backup.BackupData()\r\n";

            //①
            if (!GetBackupInfo())
            {
                Error = error + Error;
                return false;
            }
            //②
            if (!CopyDirectories())
            {
                Error = error + Error;
                return false;
            }
            //③
            if (!MoveDestOldFile())
            {
                Error = error + Error;
                return false;
            }
            //④
            if (!CopyFiles())
            {
                Error = error + Error;
                return false;
            }

            return true;
        }

        #region オーナーフォームのコントロールに対するInvokeメソッド
        //親フォームのステータスラベルの文字列変更
        public void SetStatusTextInvoker(string txt)
        {
            owneForm.Invoke(new Action(() => owneForm.SystemStatusLable.Text = txt));
        }

        //親フォームのプログレスバー変更
        public void SetProgressBarValueInvoker(int val)
        {
            owneForm.Invoke(new Action(() => owneForm.toolStripProgressBar.Visible = true));
            if (val >= 0 && val <= 100)
            {
                owneForm.Invoke(new Action(() => owneForm.toolStripProgressBar.Value = val));
            }
            
        }
        #endregion

    }
}
