using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : ViewBase
{
    private InputField mAccountInput;
    private InputField mPasswordInput;
    private Button mLoginBtn;

    public override void Awake()
    {
        base.Awake();
        Transform canvasTrans = transform.Find("Canvas");
        mAccountInput = canvasTrans.Find("AccountInput").GetComponent<InputField>();
        mPasswordInput = canvasTrans.Find("PasswordInput").GetComponent<InputField>();
        mLoginBtn = canvasTrans.Find("LoginBtn").GetComponent<Button>();
        mLoginBtn.onClick.AddListener(OnLogin);
    }
    
    void OnLogin()
    {
        if (!string.IsNullOrEmpty(mAccountInput.text) && !string.IsNullOrEmpty(mPasswordInput.text))
        {
            _TcpSendManager.SendLogin(mAccountInput.text, mPasswordInput.text);
        }
        
        //if (!string.IsNullOrEmpty(mAccountInput.text) && !string.IsNullOrEmpty(mPasswordInput.text))
        //{
        //    LoadingManager.LoadSceneAsync(SceneConfig.Fight);
        //}
    }
}
