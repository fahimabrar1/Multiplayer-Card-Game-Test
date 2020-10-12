using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Card-" , menuName = "Create Card/ New Card")]
public class CardInfo : ScriptableObject
{
    public enum CardType{
        
        Dimond = 1,
        Speads = 2,
        Hearts = 3,
        Clubs = 4
        
    }
    public enum Number{
       
        two = 2,
        three = 3,
        four = 4,
        five = 5,
        six = 6,
        seven = 7,
        eight = 8,
        nine = 9,
        ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13,
        A = 14
        
    }

    public bool Distributed;
    public Number number;
    public CardType cardType;
    public Sprite Sprite;

}
