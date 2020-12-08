using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListings : MonoBehaviour
{
    [SerializeField]
    private Text _NameText;
    [SerializeField]
    private Text _IDText;
    [SerializeField]
    private Text _YouText;
    [SerializeField]
    private Text _MasterText;
    [SerializeField]
    private Text _readyText;

    private float AnimationSpeed = 5f;

    LayoutElement _layoutElement;
    CanvasGroup _canvasGroup;

    float preferredHeight;
    float animatedTargetHeight;
    float animatedTargetAlpha;

    public Player Player { get; private set; }
    public bool Ready = false;

    void awake()
    {
        if (_layoutElement == null)
        {
            _layoutElement = GetComponent<LayoutElement>();
            preferredHeight = _layoutElement.preferredHeight;

            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    public void SetPlayerInfo(Player player)
     {
         Player = player;
        _NameText.text = player.NickName;
        _IDText.text = player.ActorNumber.ToString();
   
        _YouText.enabled = player.IsLocal;
        _MasterText.enabled = player.IsMasterClient;
        _readyText.enabled = !player.IsMasterClient;
        
    }

    public void SetReadyText(bool _ready)
    {
        switch(_ready)
        {
            case true:
                _readyText.text = "Ready";
                break;
            case false:
                _readyText.text = "Not Ready";
                break;
        }
    }


    /// <summary>
    /// Animates the item to reveal it. 
    /// Note that is make sure the parent is active, else coroutine is nto allowed and will fire error
    /// This can happen if the UI List GameObject is disabled and PlayerListUIController is calling AnimateRevealItem() 
    /// </summary>
    public void AnimateRevealItem()
    {
        if (this.transform.parent.gameObject.activeInHierarchy)
        {
            StartCoroutine(AnimateRevealItem_cr());
        }

    }

    IEnumerator AnimateRevealItem_cr()
    {
        if (_layoutElement == null) awake();

        animatedTargetHeight = preferredHeight;

        _layoutElement.preferredHeight = 0;
        _canvasGroup.alpha = 0f;

        yield return new WaitForEndOfFrame();

        while (true)
        {
            _layoutElement.preferredHeight = Mathf.Lerp(_layoutElement.preferredHeight, animatedTargetHeight, AnimationSpeed * Time.deltaTime);

            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 1, AnimationSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (_layoutElement.preferredHeight >= animatedTargetHeight) break;
        }
    }


    /// <summary>
    /// Animates the remove item. 
    /// Note that is make sure the parent is active, else coroutine is nto allowed and will fire error
    /// This can happen if the UI List GameObject is disabled and PlayerListUIController is calling AnimateRemoveItem()
    /// The GameObject will be destroy at the end of the animation to prevent leakage. Prefer a Pooling solution if your number of players will be high.
    /// </summary>
    public void AnimateRemoveItem()
    {
        if (this.transform.parent.gameObject.activeInHierarchy)
        {
            StartCoroutine(AnimateRemoveItem_cr());
        }
    }

    IEnumerator AnimateRemoveItem_cr()
    {
        if (_layoutElement == null) awake();

        animatedTargetHeight = 0;

        yield return new WaitForEndOfFrame();

        while (true)
        {
            _layoutElement.preferredHeight = Mathf.Lerp(_layoutElement.preferredHeight, animatedTargetHeight, 5f * Time.deltaTime);
            _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0, AnimationSpeed * Time.deltaTime);

            yield return new WaitForEndOfFrame();

            if (_layoutElement.preferredHeight <= animatedTargetHeight) break;
        }

        Destroy(this.gameObject);
    }


}
