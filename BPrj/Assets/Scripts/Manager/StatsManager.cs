using System;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    private float runStartTime;

    public string GetRunTime()
    {
        float time = Time.time - runStartTime;
        TimeSpan timeSpan = TimeSpan.FromSeconds(time);
        string timeText = string.Format("{0:D2}:{1:D2}:{2:D2}.{3}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds, timeSpan.Milliseconds);
        return timeText;
    }

    private void Start()
    {
        runStartTime = Time.time;
    }
}
