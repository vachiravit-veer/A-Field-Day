using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Chat : MonoBehaviour
{
    [SerializeField]
    private Transform _ChatPanel;
    [SerializeField]
    private Message _textPrefab;
    [SerializeField]
    private Button _sendButton;
    [SerializeField]
    private InputField _input;

    List<Message> msgList = new List<Message>();
    PhotonView photonView;
    private void Start()
    {
        photonView = PhotonView.Get(this);
        _sendButton.GetComponent<Button>().onClick.AddListener(OnClick_SendMessage);
    }

    [PunRPC]
    public void SendMessageToChat(string text, PhotonMessageInfo info)
    {
        if(msgList.Count > 8 )
        {
            Destroy(msgList[0].gameObject);
            msgList.RemoveAt(0);
        }

        Message newmsg = Instantiate(_textPrefab, _ChatPanel);

        newmsg.SetText(info.Sender.NickName + ": " + text);

        msgList.Add(newmsg);

    }


    private void OnClick_SendMessage()
    {
        if(_input.text==string.Empty)
        {
            Debug.Log("Input Field is empty");
            return;
        }

        photonView.RPC("SendMessageToChat", RpcTarget.All, _input.text);
    
        _input.text = string.Empty;


    }



}