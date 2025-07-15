using System.Collections.Generic;
using UnityEngine;

//@tk �ϴ� ���÷� ��������.
public class JsonLocalUserData
{
    public int Ticket;
    public int Coin;
    public int Cash;
    public int CurrentChapter;
    public Dictionary<int, List<bool>> ChapterOpenData; //bool�� open ����
    public Dictionary<int, List<bool>> ChapterClearData; //bool�� open ����
    public List<int> HeroList;
    public List<int> CardList;
    public Dictionary<int, int> HeroDeckData; //key : slotNumber, value : heroID
}