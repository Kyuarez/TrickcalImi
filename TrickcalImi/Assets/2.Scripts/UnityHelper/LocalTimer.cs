using UnityEngine;
using System.Collections;
using System;

public class LocalTimer 
{
    private float limitTime;
    private float elapsedTime;
    private bool isOnTimer;

    public Action OnTick;
    public Action OnTimeOver;

    public LocalTimer()
    {
        this.limitTime = 0f;
        elapsedTime = 0f;
        isOnTimer = false;
    }

    public void OnTimer(float limitTime)
    {
        this.limitTime = limitTime; 
        elapsedTime = 0f;
        isOnTimer = true;
    }

    public void ResetTimer()
    {
        elapsedTime = 0f;
        isOnTimer = false;
    }


    /// <summary>
    /// MonoBehaviour 업데이트 문에 종속 호출 시켜서 구현(파라미터는 Time.deltaTime)
    /// </summary>
    /// <param name="elapsedTime"></param>
    public void UpdateTimer(float elapsedTime)
    {
        if(isOnTimer == false)
        {
            return;
        }

        this.elapsedTime += elapsedTime;
        OnTick?.Invoke();

        if(this.elapsedTime >= limitTime)
        {
            isOnTimer = false;
            OnTimeOver?.Invoke();
        }   
    }

    public float GetRemainingTime()
    {
        return Mathf.Max(0, limitTime - elapsedTime);
    }
}
