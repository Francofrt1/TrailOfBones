using Assets.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelcartView : MonoBehaviour
{
    private WheelcartController wheelcart;
    private IStopWheelcart stopWheelcartInterface;
    public TextMeshProUGUI logText;
    private string logToRepair;
    public GameObject[] wheels;
    private AudioSource audio;

    private void Awake()
    {
        wheelcart = GetComponent<WheelcartController>();
        stopWheelcartInterface = GetComponent<IStopWheelcart>();
        audio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        wheelcart.OnChangedLogStorage += ChangeLogText;
        wheelcart.OnSetMaxLogStorageUI += SetMaxLogStorageUI;
        wheelcart.OnShowLogStorageUI += ShowUnshowLogStorage;
        stopWheelcartInterface.OnBlockWheelcartRequested += StopPlayWheelcartAnim;
    }

    private void OnDisable()
    {
        wheelcart.OnChangedLogStorage -= ChangeLogText;
        wheelcart.OnSetMaxLogStorageUI -= SetMaxLogStorageUI;
        wheelcart.OnShowLogStorageUI -= ShowUnshowLogStorage;
        stopWheelcartInterface.OnBlockWheelcartRequested -= StopPlayWheelcartAnim;
    }

    private void SetMaxLogStorageUI(int amount)
    {
        logToRepair = amount.ToString();
    }

    private void Start()
    {
        ChangeLogText(0);
        ShowUnshowLogStorage();
    }

    private void ChangeLogText(int amount)
    {
        logText.text = amount.ToString() + " / " + logToRepair;
    }

    private void StopPlayWheelcartAnim(bool isStopped)
    {
        StopPlayWheelcart(isStopped);
       
        if (isStopped)
        {
            audio.Pause();
        }
        else
        {
            audio.Play();
        }
    }

    private void StopPlayWheelcart(bool isStopped)
    {
        float speed;
        if (isStopped)
        {
            speed = 0f;
        }
        else
        {
            speed = 1f;
        }
        foreach (var wheel in wheels)
        {
            wheel.GetComponent<Animator>().speed = speed;
        }

    }

    private void ShowUnshowLogStorage()
    {
        GameObject canvas = logText.transform.parent.gameObject;
        if (canvas.activeInHierarchy)
        {
            canvas.SetActive(false);
        }
        else
        {
            canvas.SetActive(true);
        }
    }
}
