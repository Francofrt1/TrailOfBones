using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WheelcartView : MonoBehaviour
{
    private WheelcartController wheelcart;
    public TextMeshProUGUI logText;
    private string logToRepair;

    private void Awake()
    {
        wheelcart = GetComponent<WheelcartController>();
    }

    private void OnEnable()
    {
        wheelcart.onChangedLogStorage += ChangeLogText;
        wheelcart.onSetMaxLogStorageUI += SetMaxLogStorageUI;
        wheelcart.onShowLogStorageUI += ShowUnshowLogStorage;
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
        logText.text = " 0 / " + logToRepair;
        ShowUnshowLogStorage();
    }

    private void ChangeLogText(int amount)
    {
        if (amount != 0)
        {
            logText.text = amount.ToString() + " / " + logToRepair;
        }
        else
        {
            logText.text = amount.ToString() + " / " + logToRepair;
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
