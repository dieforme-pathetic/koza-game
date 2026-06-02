using UnityEngine;
using TMPro;
using System;

public class TipsManager : MonoBehaviour
{
    public static Action<string> displayTipEvent;
    public static Action<string> disabeTipEvent;

    [SerializeField] private TMP_Text messageText;

    private Animator anim;

    private int activeTips;

    private void OnEnable()
    {
        displayTipEvent += displayTip;
        disabeTipEvent += disableTip;
    }
    private void OnDisable()
    {
        displayTipEvent -= displayTip;
        disabeTipEvent -= disableTip;
    }
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    private void displayTip(string message)
    {
        messageText.text = message;
        anim.SetInteger("state", ++activeTips);
    }
    private void disableTip(string message)
    {
        messageText.text = message;
        anim.SetInteger("state", --activeTips);
    }
}