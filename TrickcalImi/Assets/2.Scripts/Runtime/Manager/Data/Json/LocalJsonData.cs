using System.Collections.Generic;
using UnityEngine;

//@tk 일단 로컬로 저장하자.
public class JsonLocalUserData
{
    public int Ticket;
    public int Coin;
    public int Cash;
    public int CurrentChapter;
    public Dictionary<int, List<bool>> ChapterOpenData; //bool은 open 여부
    public Dictionary<int, List<bool>> ChapterClearData; //bool은 open 여부
    public List<int> HeroList;
    public List<int> CardList;
    public Dictionary<int, int> HeroDeckData; //key : slotNumber, value : heroID
}