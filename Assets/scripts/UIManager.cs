using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnitySocketIO;
using UnitySocketIO.Events;
using System.Runtime.InteropServices;
using SimpleJSON;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject LowBtn;
    public GameObject MediumBtn;
    public GameObject HighBtn;

    public TMP_Text walletAmount_Text;
    public TMP_Text info_Text;

    public TMP_Text firstCaseBenefit;
    public TMP_Text secondCaseBenefit;
    public TMP_Text thirdCaseBenefit;
    public TMP_Text fourthCaseBenefit;
    public TMP_Text fifthCaseBenefit;
    public TMP_Text sixthCaseBenefit;
    public TMP_Text seventhCaseBenefit;
    public TMP_Text eighthCaseBenefit;
    public TMP_Text nineteenthCaseBenefit;
    public TMP_Text tenthCaseBenefit;

    public Texture[] textures = new Texture[3];

    public Texture cardImage;
    public Texture cardBackImage;

    public GameObject[] cards = new GameObject[36];

    public GameObject[] cardBackImages = new GameObject[36];

    public int[] cardArray = new int[36];

    private int[] selectedCardArray = new int[3];

    public GameObject[] panels = new GameObject[10];

    public Sprite[] BtnImages = new Sprite[6];

    public SocketIOController io;

    private int currentSelectedCardNum = 0;

    BetPlayer _player;

    public Button BetBtn;

    private int riskFlag = 0;

    private bool connectedToServer = false;

    private bool canSelect = true;

    public TMP_InputField AmountField;

    private GameObject selectedCard;

    // GameReadyStatus Send
    [DllImport("__Internal")]
    private static extern void GameReady(string msg);

    // Start is called before the first frame update
    void Start()
    {
        AmountField.text = "10.0";

        for(int i = 0; i < 36; i++)
        {
            cardArray[i] = 0;
        }

        _player = new BetPlayer();
        currentSelectedCardNum = 0;

        riskFlag = 0;
        //LowBtn.GetComponent<Image>().color = Color.red;
        //MediumBtn.GetComponent<Image>().color = Color.white;
        //HighBtn.GetComponent<Image>().color = Color.white;

        firstCaseBenefit.text = "X0.50";
        secondCaseBenefit.text = "X0.80";
        thirdCaseBenefit.text = "X1.20";
        fourthCaseBenefit.text = "X1.50";
        fifthCaseBenefit.text = "X2.10";
        sixthCaseBenefit.text = "X3.50";
        seventhCaseBenefit.text = "X4.50";
        eighthCaseBenefit.text = "X7.00";
        nineteenthCaseBenefit.text = "X15.00";
        tenthCaseBenefit.text = "X40.00";


        io.Connect();

        io.On("connect", (e) =>
        {
            connectedToServer = true;
            Debug.Log("Game started");

            io.On("bet result", (res) =>
            {
                StartCoroutine(GameResult(res));
            });

            io.On("error message", (res) =>
            {
                ShowError(res);
            });
        });

        #if UNITY_WEBGL == true && UNITY_EDITOR == false
            GameReady("Ready");
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ShowError(SocketIOEvent socketIOEvent)
    {
        var res = ReceiveJsonObject.CreateFromJSON(socketIOEvent.data);
        info_Text.text = res.errMessage.ToString();
        canSelect = true;
        BetBtn.interactable = true;
    }

    IEnumerator GameResult(SocketIOEvent socketIOEvent)
    {
        var res = ReceiveJsonObject.CreateFromJSON(socketIOEvent.data);
        walletAmount_Text.text = res.amount.ToString("F2");
        SetResultPanel(res.panelIndex);
        SetResultCard(res.cardPosition, res.selectedCardArray);
        yield return new WaitForSeconds(0.5f);
        BetBtn.interactable = true;
    }

    void SetResultCard(int[] cardPosition, int[] selectedCardArray)
    {
        for(int i = 0; i < cardPosition.Length; i++)
        {
            cardBackImages[i].GetComponent<RawImage>().texture = null;
            cardBackImages[i].GetComponent<RawImage>().color = new Color32(255, 255, 255, 0);
            cards[i].GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);

            if (cardPosition[i] == 0)
                cards[i].GetComponent<RawImage>().texture = textures[0];
            else if (cardPosition[i] == 1)
                cards[i].GetComponent<RawImage>().texture = textures[1];
            else if (cardPosition[i] == 2)
                cards[i].GetComponent<RawImage>().texture = textures[2];
        }
        for(int i = 0; i < selectedCardArray.Length; i++)
        {
           cardBackImages[selectedCardArray[i]].GetComponent<RawImage>().texture = cardBackImage;
           cardBackImages[selectedCardArray[i]].GetComponent<RawImage>().color = new Color32(0, 255, 107, 255);
        }
    }

    void SetResultPanel(int panelIndex)
    {
        panels[panelIndex].GetComponent<Image>().color = new Color32(0,255,107,255);
    }

    void SetPanelInitState()
    {
        for(int i = 0; i < 10; i++)
        {
            panels[i].GetComponent<Image>().color = new Color32(255,255,255,100);
        }
    }

    public void RequestToken(string data)
    {
        JSONNode usersInfo = JSON.Parse(data);
        Debug.Log("token=--------" + usersInfo["token"]);
        Debug.Log("amount=------------" + usersInfo["amount"]);
        Debug.Log("userName=------------" + usersInfo["userName"]);
        _player.token = usersInfo["token"];
        _player.username = usersInfo["userName"];

        float i_balance = float.Parse(usersInfo["amount"]);
        walletAmount_Text.text = i_balance.ToString("F3");
    }

    public void MinBtn_Clicked()
    {
        AmountField.text = "10.0";
    }

    public void CrossBtn_Clicked()
    {
        float amount = float.Parse(AmountField.text);
        if (amount >= 100000f)
            AmountField.text = "100000.0";
        else
            AmountField.text = (amount * 2.0f).ToString("F2");
    }

    public void HalfBtn_Clicked()
    {
        float amount = float.Parse(AmountField.text);
        if (amount <= 10f)
            AmountField.text = "10.0";
        else
            AmountField.text = (amount / 2.0f).ToString("F2");
    }

    public void MaxBtn_Clicked()
    {
        float myTotalAmount = float.Parse(string.IsNullOrEmpty(walletAmount_Text.text) ? "0" : walletAmount_Text.text);
        if (myTotalAmount >= 100000f)
            AmountField.text = "100000.0";
        else if (myTotalAmount >= 10f && myTotalAmount < 100000f)
            AmountField.text = myTotalAmount.ToString("F2");
    }

    public void AmountField_Changed()
    {
        if (float.Parse(AmountField.text) < 10f)
            AmountField.text = "10.0";
        else if (float.Parse(AmountField.text) > 100000f)
        {
            AmountField.text = "100000.0";
        }
    }

    public void LowBtnClicked()
    {
        riskFlag = 0;
        LowBtn.GetComponent<Image>().sprite = BtnImages[0];
        MediumBtn.GetComponent<Image>().sprite = BtnImages[4];
        HighBtn.GetComponent<Image>().sprite = BtnImages[5];

        firstCaseBenefit.text = "X0.50";
        secondCaseBenefit.text = "X0.80";
        thirdCaseBenefit.text = "X1.20";
        fourthCaseBenefit.text = "X1.50";
        fifthCaseBenefit.text = "X2.10";
        sixthCaseBenefit.text = "X3.50";
        seventhCaseBenefit.text = "X4.50";
        eighthCaseBenefit.text = "X7.00";
        nineteenthCaseBenefit.text = "X15.00";
        tenthCaseBenefit.text = "X40.00";
    }

    public void MediumBtnClicked()
    {
        riskFlag = 1;
        LowBtn.GetComponent<Image>().sprite = BtnImages[3];
        MediumBtn.GetComponent<Image>().sprite = BtnImages[1];
        HighBtn.GetComponent<Image>().sprite = BtnImages[5];

        firstCaseBenefit.text = "X0.00";
        secondCaseBenefit.text = "X0.50";
        thirdCaseBenefit.text = "X1.50";
        fourthCaseBenefit.text = "X2.40";
        fifthCaseBenefit.text = "X3.00";
        sixthCaseBenefit.text = "X6.70";
        seventhCaseBenefit.text = "X10.00";
        eighthCaseBenefit.text = "X15.00";
        nineteenthCaseBenefit.text = "X30.00";
        tenthCaseBenefit.text = "X80.00";
    }

    public void HighBtnClicked()
    {
        riskFlag = 2;
        LowBtn.GetComponent<Image>().sprite = BtnImages[3];
        MediumBtn.GetComponent<Image>().sprite = BtnImages[4];
        HighBtn.GetComponent<Image>().sprite = BtnImages[2];

        firstCaseBenefit.text = "X0.00";
        secondCaseBenefit.text = "X0.00";
        thirdCaseBenefit.text = "X0.50";
        fourthCaseBenefit.text = "X3.00";
        fifthCaseBenefit.text = "X4.20";
        sixthCaseBenefit.text = "X9.00";
        seventhCaseBenefit.text = "X15.00";
        eighthCaseBenefit.text = "X30.00";
        nineteenthCaseBenefit.text = "X60.00";
        tenthCaseBenefit.text = "X200.00";
    }

    public void ClickCardEvent(GameObject obj)
    {
        selectedCard = obj;
    }

    public void IndexCardPosition(int posIndex)
    {
        if (canSelect)
        {
            if (currentSelectedCardNum < 3)
            {
                info_Text.text = "";
                selectedCardArray[currentSelectedCardNum] = posIndex;
                if (cardArray[posIndex] == 0)
                {
                    cardArray[posIndex] = 1;
                    selectedCard.GetComponent<RawImage>().color = new Color32(41, 127, 229, 255);
                    currentSelectedCardNum++;
                }
                else if (cardArray[posIndex] == 1)
                {
                    cardArray[posIndex] = 0;
                    currentSelectedCardNum--;
                    selectedCard.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);
                }
            }
            else if (currentSelectedCardNum >= 3)
            {
                if (cardArray[posIndex] == 1)
                {
                    info_Text.text = "";
                    cardArray[posIndex] = 0;
                    currentSelectedCardNum--;
                    selectedCard.GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);
                }
                else if (cardArray[posIndex] == 0)
                {
                    info_Text.text = "Please place 3 cards before betting!";
                }
            }
        }
        else
        {
            canSelect = true;
            ClearTableBtnClicked();
        }
    }

    public void ClearTableBtnClicked()
    {
        SetPanelInitState();
        info_Text.text = "";
        currentSelectedCardNum = 0;
        for (int i = 0; i < 36; i++)
        {
            cards[i].GetComponent<RawImage>().texture = cardImage;
            cards[i].GetComponent<RawImage>().color = new Color32(255,255,255,255);
            cardArray[i] = 0;
        }
    }

    public void BetBtnClicked()
    {
        if (connectedToServer)
        {
            info_Text.text = "";
            SetPanelInitState();
            JsonType JObject = new JsonType();
            float myTotalAmount = float.Parse(string.IsNullOrEmpty(walletAmount_Text.text) ? "0" : walletAmount_Text.text);
            float betamount = float.Parse(string.IsNullOrEmpty(AmountField.text) ? "0" : AmountField.text);
            if (betamount <= myTotalAmount && currentSelectedCardNum == 3)
            {
                BetBtn.interactable = false;
                JObject.userName = _player.username;
                JObject.betAmount = betamount;
                JObject.isBetting = true;
                JObject.token = _player.token;
                JObject.amount = myTotalAmount;
                JObject.riskFlag = riskFlag;
                JObject.selectedCardArray = selectedCardArray;
                canSelect = false;
                io.Emit("bet info", JsonUtility.ToJson(JObject));
            }
            else if(currentSelectedCardNum < 3)
                info_Text.text = "Please place 3 cards before betting!";
            else if (betamount > myTotalAmount)
                info_Text.text = "Insufficient Funds";
        }
        else
        {
            info_Text.text = "Can't connect to Game Server!";
        }
        
    }
}

public class BetPlayer
{
    public string username;
    public string token;
}