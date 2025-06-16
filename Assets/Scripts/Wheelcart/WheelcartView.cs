using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Steamworks.InventoryItem;

public class WheelcartView : MonoBehaviour
{
    private WheelcartController wheelcart;
    public TextMeshProUGUI logText;
    private string logToRepair;
    public GameObject wheel1;
    public GameObject wheel2;
    private AudioSource audio;

    private void Awake()
    {
        wheelcart = GetComponent<WheelcartController>();
        audio = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        wheelcart.onChangedLogStorage += ChangeLogText;
        wheelcart.onSetMaxLogStorageUI += SetMaxLogStorageUI;
        wheelcart.onShowLogStorageUI += ShowUnshowLogStorage;
        wheelcart.OnBlockWheelcartRequested += StopPlayWheelcarAnim;
    }

    private void OnDisable()
    {
        wheelcart.onChangedLogStorage -= ChangeLogText;
        wheelcart.onSetMaxLogStorageUI -= SetMaxLogStorageUI;
        wheelcart.onShowLogStorageUI -= ShowUnshowLogStorage;
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

    private void StopPlayWheelcarAnim(bool isStopped)
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
        wheel1.GetComponent<Animator>().speed = 0f;
        wheel2.GetComponent<Animator>().speed = 0f;

    }
    private void startWheelcart()
    {
        wheel1.GetComponent<Animator>().speed = 1f;
        wheel2.GetComponent<Animator>().speed = 1f;
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
