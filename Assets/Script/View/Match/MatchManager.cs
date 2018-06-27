using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchManager : ViewBase
{
    private Button OneVOneBtn;
    private Button TwoVTwoBtn;
    private Button ThreeVThreeBtn;

    public override void Awake()
    {
        base.Awake();
        Transform canvasTrans = transform.Find("Canvas");
        OneVOneBtn = canvasTrans.Find("OneVOneBtn").GetComponent<Button>();
        TwoVTwoBtn = canvasTrans.Find("TwoVTwoBtn").GetComponent<Button>();
        ThreeVThreeBtn = canvasTrans.Find("ThreeVThreeBtn").GetComponent<Button>();
        OneVOneBtn.onClick.AddListener(() => { OneVOneBtnFunc(OneVOneBtn); });
        TwoVTwoBtn.onClick.AddListener(() => { OneVOneBtnFunc(TwoVTwoBtn); });
        ThreeVThreeBtn.onClick.AddListener(() => { OneVOneBtnFunc(ThreeVThreeBtn); });
    }
    
    void OneVOneBtnFunc(Button sender)
    {
        if (sender == OneVOneBtn)
        {
            _TcpSendManager.SendMatch(1);
        }
        else if (sender == TwoVTwoBtn)
        {
            _TcpSendManager.SendMatch(2);
        }
        else if (sender == ThreeVThreeBtn)
        {
            _TcpSendManager.SendMatch(3);
        }
    }
}
