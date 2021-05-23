using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LastRefreshAPI : MonoBehaviour
{

    public float m_time;
    public float m_previousTime;
    public Text m_timeDebug;
    public Text m_timePreviousDebug;


    public void ResetToZero() {


        m_previousTime = m_time;
        m_time = 0;
        if (m_timePreviousDebug != null)
            m_timePreviousDebug.text = string.Format("{0:0.0}", m_previousTime);
    }

    void Update()
    {
        m_time += Time.deltaTime;
        if (m_timeDebug != null)
            m_timeDebug.text = string.Format("{0:0.0}", m_time);
    }
}
