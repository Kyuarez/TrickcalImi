using System.Collections.Generic;
using UnityEngine;

//@tk 일단 로컬로 저장하자.
public class JsonLocalUserData
{
    public int Coin;
    public Dictionary<int, List<bool>> ChapterData;
    public List<int> HeroList;
    public List<int> CardList;
}