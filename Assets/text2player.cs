using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class text2player : MonoBehaviour
{
    //public TextMesh sign;
    public GameObject sign;
    public string namelabel;
    void Start()
    {
     
        sign = new GameObject("player_label");
        sign.transform.rotation = Camera.main.transform.rotation;

   
        // Take care about camera rotation
        // Causes the text faces camera.
         TextMesh tm = sign.AddComponent<TextMesh>();
        tm.text = namelabel;
         tm.color = new Color(0.8f, 0.8f, 0.8f);
         tm.fontStyle = FontStyle.Bold;
         tm.alignment = TextAlignment.Center;
         tm.anchor = TextAnchor.MiddleCenter;
         tm.characterSize = 0.065f;
         tm.fontSize = 60;

    }

    private void OnDestroy()
    {
        Destroy(sign);
    }
    // Update is called once per frame
    void Update()
    {
        sign.transform.position = transform.position + Vector3.up * 2f;
        //nameLabel.transform.rotation = Quaternion.LookRotation(transform.position - target.position);
        //nameLabel.transform.position = namepos;
    }

  

}
