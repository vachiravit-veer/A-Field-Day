using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Setting : MonoBehaviour
{

    [SerializeField]
    private GameObject _settingsMenu;

    [SerializeField]
    private GameObject _chatMenu;

    public void OnClick_ShowSetting()
    {
        if(_settingsMenu.activeSelf)
        {
            _settingsMenu.SetActive(false);
        }
        else
        {
            _settingsMenu.SetActive(true);
        }
    }

    public void OnClick_ShowChat()
    {
        if (_chatMenu.activeSelf)
        {
            _chatMenu.SetActive(false);
        }
        else
        {
            _chatMenu.SetActive(true);
        }
    }





}
