public List<DIctionary<string,string>> search()
{
    List<DIctionary<string,string>> result = new List<DIctionary<string,string>>();
    
    for(int i = 0; i < idList.Count ; i++)
    {
        List<DIctionary<string,string>> perConditionResult = new List<DIctionary<string,string>>(); 
        if(idList[i] == "" && operandList[i] == "=")
        {
            perConditionResult = Data.Where(x => x.Values.Contains(wordList[i])).ToList();
        }
        else if(idList[i] == "" && operandList[i] == "like")
        {
            perConditionResult = Data.Where(x => x.Values.Where(y => y.Contains(wordList[i]))).ToList();
        }
        else if(idList[i] != "" && operandList[i] == "=")
        {
            perConditionResult = Data.Where(x => x[idList[i]] == wordList[i]).ToList();
        }
        else if(idList[i] != "" && operandList[i] == "like")
        {
            perConditionResult = Data.Where(x => x[idList[i]].Contains(wordList[i])).ToList();
        }
        
        if(andOr == "or")
        {
            result.Union(perConditionResult);
        }
        else
        {
            result.Intersect(perConditionResult);
        }
        
    }
}

List<Dictironary<string,string>> newDataList = new List<Dictironary<string,string>>();
public button()
{
    string msg = "";
    //ヴァりでーとする
    
    int counter = 1;
    foreach(string file in checkedList.Items)
    {
        var dt = Enumeratable.repeat<string>("",ownerForm.DC.MainTable.Index.Count).ToList();
        var dict = ownerForm.DC.MainTable.Index.Zip(dt,(ky,vl) => new {ky,vl}).ToDictionary(a => a.ky,b => b.vl);
        dict["file"] = file;
        //タイトル
        dict["title"] = (textTitle.Text == "") ? Path.GetFileNameWithoutExtension(dct["file"]) : textTitle.Text + $"#{counter.ToString("0000")}";
        //サブタイトル
        dict["subtitle"] = (textSubTitle.Text == "") ? Path.GetFileNameWithoutExtension(dct["file"]) : textSubTitle.Text + $"#{counter.ToString("0000")}";        
        //カテゴリ
        dict["category"] = comboCategory.text;
        //タグ
        //シリーズ
        //出演者
        //配信元
        //録画日
        //評価
        //備考
        
        //登録
        if(!ownerForm.DC.RegistData(dict))
        {
            MessageBox.Show(ownerForm.DC.Error,"登録エラー");
            return;
        }
        
        msg += ownerForm.DC.Msg;
        counter++;
    }
    
    MessageBox.Show(msg,"登録完了!");
    return;
}



//
public List<string> SearchIdList;
public List<string> SearchWordList;
public List<string> SearchOperandList;
public string SearchAndOr;

public void DrawStatusLabel(string mode = "all")
{
    if(mode == "all")
    {
        StatusLabel.Text = $"全件表示【{ViewingData.Count}件】";
    }
    else
    {
        StatusLabel.Text = $"【検索結果表示({SearchAndOr}検索)】";
        for(int i = 0 ; i < SearchIdList.Count ; i++)
        {
            StatusLabel.Text += $"[条件{i+1}] 項目：'{SearchIdList[i]}' {SearchOperandList[i]} '{SearchWordList[i]}' ";
        }
        StatusLabel.Text += $"【{ViewingData.Count}件】";
    }
}

//or検索
List<T>1.Union(List<T>2);

//and検索
List<T>1.Intersect(List<T>2)

//すべての列を対象とし、キーワード(word)が含まれていればtrue
var bl = DC.MainTable.Data[1].Any( v => v.Value.Contains(word));

//すべての列を対象とし、キーワード(word)が一致すればばtrue
var bl = DC.MainTable.Data[1].Any( v => v.Value == word);

//すべての列対象で、吉田類と一致する列がある場合
var ss = DC.MainTable.Search(new List<string> { ""},new List<string> {"吉田類" },new List<string> {"=" },"and");
var dd = DC.MainTable.Data.Where(x => x.Values.Any(y => y == "吉田類")).ToList();

//すべての列対象で、吉田類を含むする列がある場合
var ss = DC.MainTable.Search(new List<string> { ""},new List<string> {"吉田類" },new List<string> {"like" },"and");
var dd = DC.MainTable.Data.Where(x => x.Values.Any(y => y.Contains("吉田類"))).ToList();

//検索時のidListの値検証
//idListに 1:Indexに含まれていない文字 且つ 2:"全て"でない　且つ　3:""でない
//以上が「true」であれば不正なidが含まれている
if(idList.Any(x => !Index.Contains[x] && x!="全て" && x!=""))
{
    return false;
}

//フォームコントロールのチェックボックスを全て未チェックにする
foreach (var c in Cntrols.Cast<Control>().Where(c => c is CheckBox))
{
    c.Checked = false;
}
