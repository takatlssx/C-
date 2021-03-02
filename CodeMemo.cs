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
