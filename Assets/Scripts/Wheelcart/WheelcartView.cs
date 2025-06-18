using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WheelcartView : MonoBehaviour
{
    private WheelcartController wheelcart;
    public TextMeshProUGUI logText;
    private string logToRepair;
    public GameObject[] wheels;
    private AudioSource audio;

    public Slider progressBar = null;
    private bool isProgressing = true;

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
        wheelcart.OnWheelcartDuration += SetProgressBarMaxLimit;
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
        StartCoroutine("UpdateProgressBar");
    }

    private void ChangeLogText(int amount)
    {
        logText.text = amount.ToString() + " / " + logToRepair;
    }

    private void StopPlayWheelcartAnim(bool isStopped)
    {
        ChangeStateProgress(isStopped);
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

    IEnumerator UpdateProgressBar()
    {
        while (isProgressing && progressBar.value < progressBar.maxValue)
        {
            yield return new WaitForSeconds(1f);
            progressBar.value += 1f;
        }
    }

    private void SetProgressBarMaxLimit(float limit)
    {
        progressBar.maxValue = limit;
    }

    public void ChangeStateProgress(bool isStopped)
    {
        isProgressing = isStopped;
    }
}
