using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public struct TimedEvent
{
    public string name;
    public Color color;
    public float unscaledTime;
    public float gameTime;
    public float realTime;

    public override string ToString()
    {
        return string.Format("Real: {0,-5} Unscaled: {1,-5} Game: {2,-5} {3}\n",
            realTime,
            unscaledTime,
            gameTime,
            name);
    }
}

public class TimeAnalyzer : MonoBehaviour
{
    public TimedEvent[] timedEvents = new TimedEvent[1000];

    int index;
    float avgRelativeEventTime;

    void OnEnable()
    {
        InputSystem.eventHandler += OnInput;
    }

    void OnDisable()
    {
        InputSystem.eventHandler -= OnInput;
    }

    protected bool OnInput(InputEvent inputEvent)
    {
        timedEvents[index] = new TimedEvent()
        {
            name = "Event",
            color = new Color(0.9f, 0, 0),
            unscaledTime = (float)inputEvent.time,
            gameTime = Time.time,
            realTime = Time.realtimeSinceStartup
        };
        index = (index + 1) % timedEvents.Length;
        return false;
    }

    void Update()
    {
        timedEvents[index] = new TimedEvent()
        {
            name = "Update",
            color = new Color(0, 0.3f, 1),
            unscaledTime = Time.unscaledTime,
            gameTime = Time.time,
            realTime = Time.realtimeSinceStartup
        };
        index = (index + 1) % timedEvents.Length;

        float relativeEventTimeSum = 0;
        int eventCount = 0;
        for (int i = 0; i < timedEvents.Length; i++)
        {
            var evt = timedEvents[i];
            if (evt.name == null)
                continue;
            float unscaled = -1000 * (Time.unscaledTime - evt.unscaledTime);
            float real     = -1000 * (Time.unscaledTime - evt.realTime);
            Debug.DrawLine(
                new Vector3(unscaled, Screen.height * 0.5f, 0),
                new Vector3(real    , Screen.height * 0.5f + 30, 0),
                evt.color
                );

            if (evt.name == "Event")
            {
                relativeEventTimeSum += evt.unscaledTime - evt.realTime;
                eventCount++;
            }
        }
        avgRelativeEventTime = relativeEventTimeSum / eventCount;
    }

    void OnGUI()
    {
        GUILayout.Label("Average event time compared to realtime: " + avgRelativeEventTime);
    }

    void FixedUpdate()
    {
        timedEvents[index] = new TimedEvent()
        {
            name = "FixedUpdate",
            color = new Color(0, 0.8f, 0),
            unscaledTime = Time.unscaledTime,
            gameTime = Time.time,
            realTime = Time.realtimeSinceStartup
        };
        index = (index + 1) % timedEvents.Length;
    }
}
