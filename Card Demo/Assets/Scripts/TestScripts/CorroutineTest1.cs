using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorroutineTest1 : MonoBehaviour
{


    int x;

    private void Start()
    {
        StartCoroutine(Cor());
    }

    public IEnumerator Cor()
    {
        x++;
        int i = 0;
        while (i<10)
        {
            yield return new WaitForSeconds(1);
            Debug.Log("Coroutine    X = "+x +"  ;   round = "+i);
            i++;
        }


        StartCoroutine(Cor());
    }

    public void Stop()
    {
        StopAllCoroutines();
    }
}
