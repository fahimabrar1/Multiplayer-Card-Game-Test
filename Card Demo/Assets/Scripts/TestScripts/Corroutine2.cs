using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corroutine2 : MonoBehaviour
{

    private CorroutineTest1 corroutine1;

    private void Start()
    {
        corroutine1 = FindObjectOfType<CorroutineTest1>().GetComponent<CorroutineTest1>();
    }

    public void OnClickCorroutine2()
    {
        corroutine1.Stop();
        StartCoroutine(corroutine1.Cor());
    }



}
