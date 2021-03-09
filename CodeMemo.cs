public void DrawDGV(List<Dictionary<string,string>> data,List<string> viewIndex,string sortIndex,string sortOrder)
{
}

//mode = "all" or "select"
public void DrawListBox(List<Dictionary<string,string>> data,Dictionary<string,List<string>> searchConditions = null)
{
      IsChangingListBoxFromMethod = true;
      listCategory.Items.Clear();
      listCategory.Items.Add("全て");
      listCategory.Items.AddRange(DC.Tables["category"].Select(x => x["category"]).ToArray());
      listTag.Items.Clear();
      listTag.Items.Add("全て");
      listTag.Items.AddRange(data.Where(x => x["tag"]!="").Select(x => x["tag"]).Distinct().ToArray());
      listSeries.Items.Clear();
      listSeries.Items.Add("全て");
      listSeries.Items.AddRange(data.Where(x => x["series"]!="").Select(x => x["series"]).Distinct().ToArray());
      listActor.Items.Clear();
      listActor.Items.Add("全て");
      listActor.Items.AddRange(data.Where(x => x["actor"]!="").Select(x => x["actor"]).Distinct().ToArray());
      
      if(searchConditions != null)
      {
          if(searchConditions["id"].Contains("category"))
          {
              int ic = searchConditions["id"].IndexOf();
          }
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

public List<Dictionary<string,string>> Search(List<string> idList,List<string> wordList,List<string> operandList,andOr = "or")
{
      //idListチェック
      if(idList.Any(x => !Index.Contains[x] && x!="全て" && x!=""))
      {
            return false;
      }
      //idList、wordList、operandListの個数チェック
      if(idList.Count != wordList.Count || idList.Count != operandList.Count)
      {
            return false;
      }
      //operandListに"="か"like"以外の文字が指定されていたら自動で"like"にする
      operandList.Where(x => x!="=" && )

}

public void moveRip(){
      
      //ripフォルダのファイル一覧取得
      var fileList = "";
      
      foreach(string fl in fileList)
      {
            //ファイル名を_で分割
            //ファイル名はcategory_tag_series_number_title.拡張子の規則
            //buuf[0]=category,[1]=tag,[2]=series,[3]=number,[4]=title.拡張子
            string[] buff = Path.GetFileNameWithoutExtention(fl).Split('-');
            
            //移動先のファイルパス形成
            string newFilePath = "G:\\Movie\\"+category+"\\";
            if(buff[2] != "")
            {
                  newFilePath += buff[2] + "\\"
            }
            //上記のフォルダパスが存在しなければ作成
            if(!Directory.Exists(newFilePath))
            {
                  Directory.CreateDirectory(newFilePath);
            }
            newFilePath += Path.GetFileName(fl);
            
            //ファイルコピー
            File.Copy(fl,newFilePath);
            
            //Indexの個数分の空白の文字の配列を作成
            string[] vals = Enumerable.Repeat<string>("", Index.Count).ToArray();
            //Indexと上記空白配列をZip→ToDictionaryで新規データ辞書作成
            var newData = Index.Zip(vals,(k,v) => new {k,v}).ToDictionary(a => a.k, a => a.v);
            
            newData["title"] = buff[4];
            newData["subtitle"] = buff[4];
            newData["number"] = int.Parse(buff[3]).ToString("0000");
            newData["category"] = buff[0];
            newData["tag"] = buff[1];
            newData["series"] = buff[2];
            newData["file"] = newFilePath;
            
      }
      
}
