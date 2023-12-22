using System;
using System.Collections.Generic;

//기존 pzMap C# 으로 컨버팅

//Key 값을 다루는 클래스
public class RzPair<TKey, TItem>{
    public TKey Key { get; set; }
    public TItem Item { get; set; }

    public RzPair() { }

    public RzPair(TKey key, TItem item){
        Key = key;
        Item = item;
    }
}

//(_pair) 관리하는 클래스
public class PzMap<TKey, TItem>{
    private List<RzPair<TKey, TItem>> _pairs = new List<RzPair<TKey, TItem>>();

    public int Count => _pairs.Count;           //Count >> _pairs 리스트의 현재 항목수


    //인덱스에 해당하는 _pair 리스트의 항목 가져오거나 설정 하는 역할
    public RzPair<TKey, TItem> this[int index]{ 
        get{
            if (index < 0 || index >= Count)        //범위 초과시 예외처리
                throw new ArgumentOutOfRangeException(nameof(index));

            return _pairs[index];
        }
        set{
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            _pairs[index] = value;
        }
    }

    //_pair 의 Capacity설정, 혹은 빈 항목 추가하거나 초과하는 항목 제거
    public void SetCount(int count){
        _pairs.Capacity = count;

        while (_pairs.Count < count)
            _pairs.Add(new RzPair<TKey, TItem>());

        while (_pairs.Count > count)
            _pairs.RemoveAt(_pairs.Count - 1);
    }


    //_pair 리스트 초기화 (비워버림)
    public void Reset() => _pairs.Clear();


    //(RzPair<TKey, TItem> pair) 메서드의 새로운 RzPair 를 _pairs 리스트에 추가함
    public void Append(RzPair<TKey, TItem> pair) => _pairs.Add(pair);

    //주어진 키와 일치하는 모든 RzPair 항목을 _pairs 리스트에서 제거
    public void Remove(TKey key) => _pairs.RemoveAll(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key));


    //주어진 인덱스에 해당하는 RzPair를 _pair 리스트에서 제거하고 리턴
    public RzPair<TKey, TItem> RemoveIndex(int index){
        if (index < 0 || index >= Count)
            throw new ArgumentOutOfRangeException(nameof(index));

        RzPair<TKey, TItem> removedPair = _pairs[index];
        _pairs.RemoveAt(index);
        return removedPair;
    }


    //주어진 키와 일치하는 첫 번째 RzPair 항목을 _pairs 리스트에서 찾아 반환함
    public RzPair<TKey, TItem> Find(TKey key) => _pairs.Find(pair => EqualityComparer<TKey>.Default.Equals(pair.Key, key));
}
