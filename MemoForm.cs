//現在表示するデータ
public List<Dictionary<string,string>> ViewingData = new List<Dictionary<string,string>>();

//データを検索する条件
public List<string> SrcIdList = new List<string>();


//データグリッドビュー描画
public void DrawDGV(string sortId = "id",string sortOrder = "asc")
{
    dGV.Columuns.Clear();
    dGV.Rows.Clear();
    
    
}

//dGVのステータスラベル
public void DrawLabelDGVStatus()
{
    if(SrcIdList.Count == 0)
    {
        labelDGVStatus.Text = $"全件表示【{ViewingData.Count}件】";
    }
    else
    {
    }
}

//リストボックス描画
public void DrawListBox(string startingPointListBoxName = "all")
{
    if(startingPointListBoxName == "all")
    {
    }
    else if(startingPointListBoxName == "category")
    {
    }
    else if(startingPointListBoxName == "tag")
    {
    }
    else if(startingPointListBoxName == "series")
    {
    }    
}
