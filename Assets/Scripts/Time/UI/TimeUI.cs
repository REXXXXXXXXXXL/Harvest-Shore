using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    public RectTransform dayNightImage;
    public RectTransform clockParent;
    public Image seasonImage;
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI timeText;

    public Sprite[] seasonSprites;//创建数组获得季节的图片

    private List<GameObject> clockBlocks = new List<GameObject>();
    private int lastSystemSecond = -1;

    private void Awake()
    {
        for (int i = 0; i < clockParent.childCount; i++)//循环子物体，获得gameobject
        {
            clockBlocks.Add(clockParent.GetChild(i).gameObject);
            clockParent.GetChild(i).gameObject.SetActive(false);
        }

        UpdateSystemTime(true);
    }

    private void OnEnable()//注册事件
    {
        EventHandler.GameDateEvent += OnGameDateEvent;
    }

    private void OnDisable()
    {
        EventHandler.GameDateEvent -= OnGameDateEvent;
    }

    private void Update()
    {
        UpdateSystemTime();
    }

    private void OnGameDateEvent(int hour, int day, int month, int year, Season season)
    {
        UpdateSystemTime(true);
        seasonImage.sprite = seasonSprites[(int)season];//月份对应季节并切换图片

        SwitchHourImage(hour);
        DayNightImageRotate(hour);
    }

    private void UpdateSystemTime(bool force = false)
    {
        var now = DateTime.Now;
        if (!force && now.Second == lastSystemSecond)
            return;

        lastSystemSecond = now.Second;

        if (dateText != null)
            dateText.text = now.ToString("yyyy年MM月dd日");

        if (timeText != null)
            timeText.text = now.ToString("HH:mm");
    }

    /// <summary>
    /// 根据小时切换时间块显示
    /// </summary>
    /// <param name="hour"></param>
    private void SwitchHourImage(int hour)
    {
        int index = hour / 4;

        if (index == 0)
        {
            foreach (var item in clockBlocks)
            {
                item.SetActive(false);
            }
        }
        else
        {
            for (int i = 0; i < clockBlocks.Count; i++)
            {
                if (i < index + 1)
                    clockBlocks[i].SetActive(true);
                else
                    clockBlocks[i].SetActive(false);
            }
        }
    }

    private void DayNightImageRotate(int hour)    
    {        
        var target = new Vector3(0, 0, hour * 15 - 90);        
        dayNightImage.DORotate(target, 1f, RotateMode.Fast);    
    }
}
