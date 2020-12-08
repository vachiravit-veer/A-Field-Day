using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField]
    private GameObject _bg, _text;
 
    public void OnClick_Pause()
    {
        //show Text Canvas

        if (Time.timeScale == 1)
        {
            _bg.SetActive(true);
            _text.SetActive(true);
            Time.timeScale = 0;
        }
        else
        {
            _bg.SetActive(false);
            _text.SetActive(false);
            Time.timeScale = 1;
        }
    }

}
