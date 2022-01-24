using UnityEngine;
using System.Collections;

public class JsonType
{
    public string userName;
    public float betAmount;
    public int riskFlag;
    public bool isBetting;
    public int[] selectedCardArray = new int[3];
    public int posIndex;
    public string token;
    public float amount;
}

public class ReceiveJsonObject
{
    public double amount;
    public float earnAmount;
    public int[] cardPosition;
    public int panelIndex;
    public int[] selectedCardArray;
    public string errMessage;
    public ReceiveJsonObject()
    {
    }
    public static ReceiveJsonObject CreateFromJSON(string data)
    {
        return JsonUtility.FromJson<ReceiveJsonObject>(data);
    }
}