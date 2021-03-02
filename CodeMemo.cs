//or検索
List<T>1.Union(List<T>2);

//and検索
List<T>1.Intersect(List<T>2)

//すべての列を対象とし、キーワード(word)が含まれていればtrue
var bl = DC.MainTable.Data[1].Any( v => v.Value.Contains(word));

//すべての列を対象とし、キーワード(word)が一致すればばtrue
var bl = DC.MainTable.Data[1].Any( v => v.Value == word);
