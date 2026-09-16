using Mono.Cecil.Cil;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ConfirmCode
{
    private string codeTry;

    public bool CodeHandler(List<int> nums, string code)
    {
        codeTry = "";

        foreach (var item in nums)
        {
            codeTry += item.ToString();
        }

        return codeTry == code;
    }
}
