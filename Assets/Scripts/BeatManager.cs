using System;
using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour {
    [SerializeField] private float _bpm;
    [SerializeField] private AudioSource _track, _countInBeat, _audioStartLevel;
    [SerializeField] private Intervals[] _intervals;
    private Coroutine _countInCoroutine;
    public Action<int> OnCountInBeat;
    private AudioLowPassFilter _filter;
    private AudioReverbFilter _reverb;
    private float _currentBeat;
    private int _nextStartingBeat;

    private void Start() {
        _filter = _track.GetComponent<AudioLowPassFilter>();
        _reverb = _track.gameObject.GetComponent<AudioReverbFilter>();
    }

    private bool IsPaused() {
        return _reverb.isActiveAndEnabled;
    }

    private void Update() {
        var newBeat = _track.timeSamples * _bpm / (_track.clip.frequency * 60f);
        if (newBeat < _currentBeat) {
            foreach (var interval in _intervals) {
                if(interval.IsExpired()) continue;
                interval.ReduceNextTrigger(_currentBeat);
            }
        }
        
        _currentBeat = _track.timeSamples * _bpm / (_track.clip.frequency * 60f);
        if(IsPaused()) return;
        foreach (var interval in _intervals) {
            if(interval.IsExpired()) continue;
            
            interval.CheckForNewInterval(_currentBeat);
        }
    }

    public void Stop() {
        _track.Stop();
        StopCoroutine(_countInCoroutine);
    }

    public void Pause() {
        StopCoroutine(_countInCoroutine);
        _filter.enabled = true;
        _reverb.enabled = true;
        _track.volume = 0.7f;
    }
    
    private System.Collections.IEnumerator PlayCountInBeats(Action onComplete) {
        if (!_track.isPlaying) {
            yield return new WaitForSeconds(0.2f); // wait 0.2s on first start, as otherwise intervals are uneven
        } else {
            var beatsToNextEvenBeat = Mathf.Ceil(_currentBeat / 2f) * 2f - _currentBeat;
            var secondsToNextEven = beatsToNextEvenBeat * (60f / _bpm);
            _nextStartingBeat = (int)Mathf.Ceil(_currentBeat / 2f) * 2 + PlayerPrefs.GetInt("countInBeats");
            foreach(var interval in _intervals) interval.Reset(_nextStartingBeat);
            yield return new WaitForSeconds(secondsToNextEven);
        }

        for (var i = 0; i < PlayerPrefs.GetInt("countInBeats"); i++) {
            _countInBeat.Play();
            OnCountInBeat?.Invoke(i+1);
            yield return new WaitForSeconds(0.75f);
        }
        _audioStartLevel.Play();
        onComplete?.Invoke();
    }

    public void Play() {
        _countInCoroutine = StartCoroutine(PlayCountInBeats(() => {
            _reverb.enabled = false;
            _filter.enabled = false;
            _track.volume = 1f;
            
            if (!_track.isPlaying) _track.Play();
        }));
       
    }
}

[Serializable]
public class Intervals {
    [SerializeField] private float _beatOffset; // the beat offset at which event is triggered (excluding count in beats)
    [SerializeField] private float _repeat; // the number of beats after which event is triggered again
    [SerializeField] private UnityEvent _trigger;
    
    private bool expired;
    private float nextBeatToTrigger = -1f;
    public bool IsExpired() {
        return expired;
    }

    public void Reset(int startBeat) {
        expired = false;
        nextBeatToTrigger = startBeat + _beatOffset;
    }
    
    public void CheckForNewInterval(float beat) {
        if (nextBeatToTrigger < 0) {
            nextBeatToTrigger = _beatOffset;
        }
        if (beat <= nextBeatToTrigger) return;
        if (_repeat > 0) {
            nextBeatToTrigger += _repeat;
        } else expired = true;
        _trigger.Invoke();
    }
    
    public void ReduceNextTrigger(float beat) {
        nextBeatToTrigger -= beat;
    }
}