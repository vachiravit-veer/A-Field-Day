using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{

    [SerializeField]
    private Sprite _on, _off;
    [SerializeField]
    private GameObject _sound,_music,_close;

    private int _stateM,_stateS;

    private void Start()
    {
        _sound.GetComponent<Button>().onClick.AddListener(OnClick_Sound);
        _music.GetComponent<Button>().onClick.AddListener(OnClick_Music);
        _close.GetComponent<Button>().onClick.AddListener(OnClick_CloseSetting);
    }

    private void OnClick_Music()
    {
        switch(_stateM)
        {
            case 0:
                _music.GetComponent<Image>().sprite = _off;
                //AudioListener.volume = 0f;
                AudioListener.pause = true;
                _stateM = 1;
                break;
            case 1:
                _music.GetComponent<Image>().sprite = _on;
                _stateM = 0;
                AudioListener.pause = false;
                break;

        }
        
    }

    private void OnClick_Sound()
    {
        switch (_stateS)
        {
            case 0:
                _sound.GetComponent<Image>().sprite = _off;
                _stateS = 1;
                break;
            case 1:
                _sound.GetComponent<Image>().sprite = _on;
                _stateS = 0;
                break;

        }

    }


    private void OnClick_CloseSetting()
    {
        gameObject.SetActive(false);
    }



}
