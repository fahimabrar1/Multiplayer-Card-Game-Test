using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
[CreateAssetMenu(fileName = "Player Table", menuName = "Table Data/New Table Data")]

public class TableData : ScriptableObject
{
    public List<PlayerTurnData> _PlayerTurnData;

   
}

