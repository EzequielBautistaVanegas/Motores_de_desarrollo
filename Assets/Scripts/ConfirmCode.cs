using Mono.Cecil.Cil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConfirmCode : MonoBehaviour, IInteractable
{
    private string code, codeTry;
    [SerializeField] private List<SliderController> codeNums;
    private List<int> nums;

    public void Awake()
    {
        code = Random.Range(1, 7).ToString()
            + Random.Range(1, 7).ToString()
            + Random.Range(1, 7).ToString();

        Debug.Log(code);
    }

    public void Interact()
    {
        codeTry = "";

        foreach (var item in codeNums)
        {
            codeTry += item.GetNumCode().ToString();
        }

        Debug.Log(codeTry);

        Debug.Log(
            codeTry == code ? "Correct" : "Wrong"
        );
    }
}
