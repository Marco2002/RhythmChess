using System;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour {
    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;
    
    private void Update() {
        foreach (var interval in _intervals) {
            if(interval.IsExpired()) continue;
            
            var currentBeat = _audioSource.timeSamples * _bpm / (_audioSource.clip.frequency * 60f);
            interval.CheckForNewInterval(currentBeat);
        }
    }

    public void Stop() {
        _audioSource.Stop();
    }

    public void Reset() {
        _audioSource.Play();
        foreach(var interval in _intervals) interval.Reset();
    }
}

[System.Serializable]
public class Intervals {
    [SerializeField] private float _beatOffset; // the beat offset at which event is triggered
    [SerializeField] private float _repeat; // the number of beats after which event is triggered again
    [SerializeField] private UnityEvent _trigger;
    private bool expired;
    private float nextBeatToTrigger = -1f;
    public bool IsExpired() {
        return expired;
    }

    public void Reset() {
        expired = false;
        nextBeatToTrigger = _beatOffset;
    }
    public void CheckForNewInterval(float beat) {
        if (nextBeatToTrigger < 0) nextBeatToTrigger = _beatOffset;
        if (beat < nextBeatToTrigger) return;

        if (_repeat > 0) nextBeatToTrigger += _repeat;
        else expired = true;
        _trigger.Invoke();
    }
}