//----------------------------------------
// MIT License
// Copyright(c) 2021 Jonas Boetel
//----------------------------------------
using System.Collections;
using UnityEngine;

namespace Lumpn.Matomo.Samples
{
    public class EventSequenceSender : MonoBehaviour
    {
        [Header("Matomo")]
        [SerializeField] private MatomoTrackerData trackerData;

        [Header("Event sequence to record over time in play mode")]
        [SerializeField] private string[] events;

        IEnumerator Start()
        {
            var tracker = trackerData.CreateTracker();
            var session = tracker.CreateSession();

            Debug.Log("Recording system info");
            yield return session.RecordSystemInfo();
            yield return new WaitForSeconds(1f);

            foreach (var eventName in events)
            {
                Debug.LogFormat(this, "Recording event '{0}'", eventName);
                yield return session.RecordEvent(eventName, Time.time);
                yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            }
        }
    }
}