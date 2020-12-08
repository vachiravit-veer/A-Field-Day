using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class SceneSwitch : MonoBehaviour
{
    public void GotoPlayScene()
    {
        SceneManager.LoadScene("0Board");
    }

    public void GotoMenuScene()
    {
        SceneManager.LoadScene("3Scenemenu");
    }


    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }

    public void RetryOnline()
    {
  
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LoadLevel(0);
        // SceneManager.LoadScene("Rooms");

    }



}
