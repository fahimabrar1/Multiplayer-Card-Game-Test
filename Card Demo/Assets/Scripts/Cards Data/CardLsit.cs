using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "Card List", menuName = "Create Card/ New Card List")]

public class CardLsit : ScriptableObject
{
    public List<CardInfo> CardList;
}
