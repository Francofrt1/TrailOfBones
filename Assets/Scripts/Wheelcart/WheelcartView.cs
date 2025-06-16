using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WheelcartView : MonoBehaviour
{
    private WheelcartController wheelcart;
    public TextMeshProUGUI logText;
    private string logToRepair;
    public GameObject[] wheels;
    private AudioSource audio;

    private void Awake()
    {
        wheelcart = GetComponent<WheelcartController>();
        audio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        wheelcart.OnChangedLogStorage += ChangeLogText;
        wheelcart.OnSetMaxLogStorageUI += SetMaxLogStorageUI;
        wheelcart.OnShowLogStorageUI += ShowUnshowLogStorage;
        wheelcart.OnBlockWheelcartRequested += StopPlayWheelcartAnim;
    }

    private void OnDisable()
    {
        wheelcart.OnChangedLogStorage -= ChangeLogText;
        wheelcart.OnSetMaxLogStorageUI -= SetMaxLogStorageUI;
        wheelcart.OnShowLogStorageUI -= ShowUnshowLogStorage;
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
        if (isStopped)
        {
            audio.Pause();
            stopWheelcart();
        }
        else
        {
            audio.Play();
            startWheelcart();
        }
    }

    private void stopWheelcart()
    {
        foreach (var wheel in wheels)
        {
            wheel.GetComponent<Animator>().speed = 0;
        }

    }
    private void startWheelcart()
    {
        foreach (var wheel in wheels)
        {
            wheel.GetComponent<Animator>().speed = 1f;
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
