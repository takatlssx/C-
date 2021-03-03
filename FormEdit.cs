using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Movie
{
    public partial class FormEdit : Form
    {
        FormMain ownerForm;
        List<string> idList;

        Dictionary<string, string> defVals = new Dictionary<string, string>();

        string error = "";

        public FormEdit(FormMain owner,List<string> idList)
        {
            InitializeComponent();
            ownerForm = owner;
            this.idList = idList;
        }

        private void FormEdit_Load(object sender, EventArgs e)
        {
            //各列のデフォルト値を設定
            //データが1個ならそのデータ値
            //複数データなら、値が同じ場合その値、異なる値の場合は「複数の値」にする
            for (int i = 0; i < idList.Count ; i++)
            {
                var dt = ownerForm.DC.MainTable.Data[int.Parse(idList[i]) - 1];
                foreach (string ky in dt.Keys)
                {
                    if (i == 0)
                    {
                        defVals[ky] = dt[ky];
                    }
                    else
                    {
                        if (defVals[ky] != dt[ky])
                        {
                            defVals[ky] = "複数の値";
                        }
                    }
                }
            }

            //各コントロールのTextにdeValを設定
            foreach (string ky in defVals.Keys)
            {
                foreach (Control ctl in this.Controls.Cast<Control>().Where(x => x.Tag != null && x.Tag.ToString() == ky))
                {
                    ctl.Text = defVals[ky];
                }
            }

            //idListが複数個なら
            //checkFileはチェックできなくしfile項目は編集できなくする。
            //フォームのテキストを変える
            if (idList.Count > 1)
            {
                checkFile.Enabled = false;
                this.Text = $"動画データ編集（{idList.Count}件）";
            }

            //コンボボックスの値設定
            comboCategory.Items.AddRange(ownerForm.DC.Tables["category"].Data.Select(x => x["category"]).ToArray());

            foreach (var dict in ownerForm.DC.Tables["tag"].Data)
            {
                comboTag.Items.Add(dict["tag"]);
            }
            foreach (var dict in ownerForm.DC.Tables["series"].Data)
            {
                comboSeries.Items.Add(dict["series"]);
            }
            foreach (var dict in ownerForm.DC.Tables["actor"].Data)
            {
                comboActor.Items.Add(dict["actor"]);
            }
            foreach (var dict in ownerForm.DC.Tables["source"].Data)
            {
                comboSource.Items.Add(dict["source"]);
            }

        }

        #region チェックボックス処理
        //チェックボックス状態変化処理
        private void checkStateChanged(object sender,Control ctl)
        {
            ctl.Enabled = ((CheckBox)sender).Checked;
            ctl.Text = defVals[ctl.Tag.ToString()];
        }

        private void checkTitle_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender,textTitle);
        }

        private void checkSubTitle_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, textSubTitle);
        }

        private void checkNumber_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, textNumber);
        }

        private void checkCategory_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, comboCategory);
        }

        private void checkTag_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, comboTag);
        }

        private void checkSeries_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, comboSeries);
        }

        private void checkActor_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, comboActor);
        }

        private void checkSource_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, comboSource);
        }

        private void checkDate_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, textDate);
        }

        private void checkFile_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, textFile);
            linkOpenFile.Enabled = checkFile.Checked;
        }

        private void checkRate_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, comboRate);
        }

        private void checkDetail_CheckedChanged(object sender, EventArgs e)
        {
            checkStateChanged(sender, textDetail);
        }
        #endregion

        //入力コントロール値検証
        private bool validate()
        {
            error = "";

            //値が変更されているかチェック
            bool isOK = false;
            foreach (Control ctl in this.Controls)
            {
                if (ctl.Tag != null && !(ctl is CheckBox) && defVals[ctl.Tag.ToString()] != ctl.Text)
                {
                    isOK = true;
                }
            }
            if (!isOK)
            {
                error = "変更されたデータ値はありませんでした。";
                return isOK;
            }

            //値の型チェック
            int tryInt;
            if (checkNumber.Checked && !int.TryParse(textNumber.Text,out tryInt))
            {
                error += "話数の項目は整数値を入力してください。\r\n";
            }
            if (checkRate.Checked && !new List<string> { "0","1","2","3","4","5"}.Contains(comboRate.Text))
            {
                error += "評価の項目は整数値(0～5)を入力してください。\r\n";
            }
            DateTime tryDt;
            if (checkDate.Checked && !DateTime.TryParse(textNumber.Text, out tryDt))
            {
                error += "録画日の項目は日付値(yyyy/MM/dd)を入力してください。\r\n";
            }
            if (checkCategory.Checked && !comboCategory.Items.Contains(comboCategory.Text))
            {
                error += "カテゴリの項目は決められた値を入力してください。\r\n";
            }
            //nullチェック
            if (checkTitle.Checked && textTitle.Text == "")
            {
                error += "タイトルの項目は必ず入力してください。\r\n";
            }
            if (checkNumber.Checked && textNumber.Text == "")
            {
                error += "話数の項目は必ず入力してください。\r\n";
            }
            if (checkCategory.Checked && comboCategory.Text == "")
            {
                error += "カテゴリの項目は必ず入力してください。\r\n";
            }
            if (checkFile.Checked && textFile.Text == "")
            {
                error += "ファイルの項目は必ず入力してください。\r\n";
            }

            //ファイルチェック
            if (checkFile.Checked && !File.Exists(textFile.Text))
            {
                error += "ファイルの項目に指定されたパスは存在しません。\r\n";
            }

            if (error != "")
            {
                return false;
            }
            
            return true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> newData = new Dictionary<string, string>();
            string msg = "";
            if (!validate())
            {
                MessageBox.Show(error,"値エラー");
                return;
            }

            foreach (Control ctl in this.Controls)
            {
                if ((ctl is CheckBox)&&((CheckBox)ctl).Checked)
                {
                    newData[this.Controls[((CheckBox)ctl).Tag.ToString()].Tag.ToString()] = this.Controls[((CheckBox)ctl).Tag.ToString()].Text;
                    msg += $"{((CheckBox)ctl).Text}  '{defVals[this.Controls[((CheckBox)ctl).Tag.ToString()].Tag.ToString()]}' → '{this.Controls[((CheckBox)ctl).Tag.ToString()].Text}'\r\n";
                }
            }

            msg = $"下記の内容で{idList.Count}件のデータを編集します。よろしいでしょうか？\r\n" + msg;

            if (MessageBox.Show(msg,"確認",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (!ownerForm.DC.EditData(idList,newData))
                {
                    MessageBox.Show(ownerForm.DC.Error);
                    return;
                }
                else
                {
                    MessageBox.Show(ownerForm.DC.Msg);
                    ownerForm.SetDataDGV(ownerForm.DGVData, "desc");
                    ownerForm.SelectRow(ownerForm.DGVSelectedIndexList);
                    this.Close();
                }
            }
            else
            {
                return;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("編集フォームを終了しますか?","確認",MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("値を初期化しますか?", "確認", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                foreach (Control ctl in this.Controls)
                {
                    if (ctl is CheckBox)
                    {
                        ((CheckBox)ctl).Checked = false;
                    }
                }
            }
            
        }

        private void linkOpenFile_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (oFD.ShowDialog() == DialogResult.OK)
            {
                textFile.Text = oFD.FileName;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (CheckBox chkbx in this.Controls.Cast<Control>().Where(c => c is CheckBox))
            {
                if (chkbx.Checked)
                {
                    
                    //newData[this.Controls[chb.Tag.ToString()].Tag.ToString()] = this.Controls[chb.Tag.ToString()].Text;
                    //msg += $"{((CheckBox)ctl).Text}  '{defVals[this.Controls[((CheckBox)ctl).Tag.ToString()].Tag.ToString()]}' → '{this.Controls[((CheckBox)ctl).Tag.ToString()].Text}'\r\n";
                }
            }
        }
    }
}
