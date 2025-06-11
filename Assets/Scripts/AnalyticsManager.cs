using Unity.Services.Core;
using Unity.Services.Analytics;
using UnityEngine;

public class AnalyticsManager : MonoBehaviour {
    private async void Start() {
        await UnityServices.InitializeAsync();
        
        if(PlayerPrefs.GetInt("analyticsConsent") == 1) {
            Debug.Log("analytics enabled");
            AnalyticsService.Instance.StartDataCollection();
        }
    }

    public void AllowAnalytics() {
        PlayerPrefs.SetInt("analyticsConsent", 1);
        PlayerPrefs.Save(); 
        AnalyticsService.Instance.StartDataCollection();
    }

    public void DeclineAnalytics() {
        PlayerPrefs.SetInt("analyticsConsent", 0);
        PlayerPrefs.Save();
        AnalyticsService.Instance.StopDataCollection();
    }

    public void BeatLevelEvent(int levelNumber, int numberOfStars, int numberOfTries, bool firstTime) {
        AnalyticsService.Instance.RecordEvent(new LevelBeatEvent {
            LevelNumber = levelNumber,
            NumberOfStars = numberOfStars,
            NumberOfTries = numberOfTries,
            FirstTime = firstTime
        });
    }
}

public class LevelBeatEvent : Unity.Services.Analytics.Event {
    public LevelBeatEvent() : base("LevelBeaten") {}

    public int LevelNumber { set { SetParameter("levelNumber", value); } }
    public int NumberOfStars { set { SetParameter("numberOfStars", value); } }
    public int NumberOfTries { set { SetParameter("numberOfTries", value); } }
    public bool FirstTime { set { SetParameter("firstTime", value); } }
}