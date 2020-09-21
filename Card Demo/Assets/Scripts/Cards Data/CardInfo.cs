﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "Card-" , menuName = "Create Card/ New Card")]
public class CardInfo : ScriptableObject
{
    public enum CardType{
        Hearts,
        Dimond,
        Speads,
        Clubs
        
    }
    public enum Number{
        A = 1,
        two = 2,
        three = 3,
        four = 4,
        five = 5,
        six = 6,
        seven = 7,
        eight = 8,
        nine = 9,
        ten = 10,
        King,
        Queen,
        Jack

        
    }

    public bool Distributed;
    public Number number;
    public CardType cardType;
    public Sprite Sprite;

}
