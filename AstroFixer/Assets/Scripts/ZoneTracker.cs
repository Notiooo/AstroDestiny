using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

public interface IZoneListener
{
    void OnZoneEnter();
    void OnZoneExit();
}

public class ZoneTracker : MonoBehaviour
{
    public static ZoneTracker Instance { get; private set; }

    private List<IZoneListener> listeners = new List<IZoneListener>();
    private int insideZoneCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void RegisterListener(IZoneListener listener)
    {
        if (!listeners.Contains(listener))
        {
            listeners.Add(listener);
        }
    }

    public void UnregisterListener(IZoneListener listener)
    {
        if (listeners.Contains(listener))
        {
            listeners.Remove(listener);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("InsideZone"))
        {
            insideZoneCount++;
            if (insideZoneCount == 1)
            {
                NotifyEnterZone();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("InsideZone"))
        {
            insideZoneCount--;
            if (insideZoneCount == 0)
            {
                NotifyExitZone();
            }
        }
    }

    private void NotifyEnterZone()
    {
        foreach (var listener in listeners)
        {
            listener.OnZoneEnter();
        }
    }

    private void NotifyExitZone()
    {
        foreach (var listener in listeners)
        {
            listener.OnZoneExit();
        }
    }

    public bool IsInZone()
    {
        return insideZoneCount > 0;
    }
}

