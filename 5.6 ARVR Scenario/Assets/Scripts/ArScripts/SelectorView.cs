//Selector Helper script to guid the selector(crosshair) in the FOV
using System.Collections;
using UnityEngine;

public class SelectorView : MonoBehaviour {

    public Transform m_parent { get; set; }
    public float m_defaultTrackSpeed = 6.5f;
    public float m_hoveringTrackSpeed = 10;
    public float m_selectingTrackSpeed = 20;
    float m_currentTrackSpeed;

    public RectTransform m_innerCircle;
    public Vector3 m_innerCircleDefault = new Vector3(0.5f, 0.5f, 0.5f);
    public Vector3 m_innerCircleSelect = new Vector3(1f, 1f, 1f);
    public float m_selectorTweenSpeed = 0.4f;

    void Start()
    {
        m_currentTrackSpeed = m_defaultTrackSpeed;

        if (m_parent == null)
        {
            m_parent = FindObjectOfType<Camera>().transform;
        }
    }

    void Update()
    {
        Vector3 to = m_parent.forward * 2.5f + m_parent.position;
        transform.position = Vector3.Lerp(transform.position, to, Time.deltaTime * m_currentTrackSpeed);
        transform.rotation = Quaternion.Lerp(transform.rotation, m_parent.rotation, Time.deltaTime * m_currentTrackSpeed);
    }

    public void OnSelect()
    {
        StartCoroutine(LerpSizeFrom(m_innerCircleSelect, m_innerCircleDefault, m_selectorTweenSpeed));
    }

    public void OnHover()
    {
        m_currentTrackSpeed = m_hoveringTrackSpeed;
    }

    public void OffHover()
    {
        m_currentTrackSpeed = m_defaultTrackSpeed;
    }

    IEnumerator LerpSizeFrom(Vector3 start, Vector3 end, float t)
    {
        float startTime = Time.time;
        while (Time.time < startTime + t)
        {
            Vector3 v = Vector3.Lerp(start, end, (Time.time - startTime) / t);
            m_innerCircle.localScale = v;
            yield return null;
        }
        m_innerCircle.localScale = end;
    }
}
