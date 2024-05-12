using System;
using System.Collections;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    private AppOpenAd _appOpenAd;

    [SerializeField] private string appOpenAdUnitId;
    [SerializeField] private bool useTestAds;
    private const string TestAdId = "ca-app-pub-3940256099942544/9257395921";
    [SerializeField] private bool showDebugLog;
    

    private void Awake()
    {
        // Use the AppStateEventNotifier to listen to application open/close events.
        // This is used to launch the loaded ad when we open the app.
        AppStateEventNotifier.AppStateChanged += OnAppStateChanged;
    }

    private void Start()
    {
        
        MobileAds.Initialize(initStatus =>
        {
            LoadAppOpenAd();
        });
        
        
    }

    private void OnDestroy()
    {
        // Always un-listen to events when complete.
        AppStateEventNotifier.AppStateChanged -= OnAppStateChanged;
    }
    
    private void OnAppStateChanged(AppState state)
    {
        DebugLog("App State changed to : "+ state);

        // if the app is Foregrounded and the ad is available, show it.
        if (state == AppState.Foreground)
        {
            ShowAppOpenAd();
            // StartCoroutine(ShowAppOpenAdWithDelay());
        }
    }

    IEnumerator ShowAppOpenAdWithDelay()
    {
        yield return new WaitForSeconds(0.7f);
        ShowAppOpenAd();
    }
    
    /// <summary>
    /// Shows the app open ad.
    /// </summary>
    public void ShowAppOpenAd()
    {
        if (_appOpenAd != null && _appOpenAd.CanShowAd())
        {
            DebugLog("Showing app open ad.");
            _appOpenAd.Show();
        }
        else
        {
            DebugLogError("App open ad is not ready yet.");
        }
    }
    
    /// <summary>
      /// Loads the app open ad.
      /// </summary>
      public void LoadAppOpenAd()
      {
          // Clean up the old ad before loading a new one.
          if (_appOpenAd != null)
          {
                _appOpenAd.Destroy();
                _appOpenAd = null;
          }

          DebugLog("Loading the app open ad.");

          // Create our request used to load the ad.
          var adRequest = new AdRequest();

          // send the request to load the ad.
          AppOpenAd.Load(useTestAds ? TestAdId : appOpenAdUnitId, adRequest,
              (AppOpenAd ad, LoadAdError error) =>
              {
                  // if error is not null, the load request failed.
                  if (error != null || ad == null)
                  {
                      DebugLogError("app open ad failed to load an ad " +
                                     "with error : " + error);
                      return;
                  }

                  DebugLog("App open ad loaded with response : "
                            + ad.GetResponseInfo());

                  _appOpenAd = ad;
                  RegisterEventHandlers(ad);
              });
      }
    
    private void RegisterEventHandlers(AppOpenAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            DebugLog(String.Format("App open ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            DebugLog("App open ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            DebugLog("App open ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            DebugLog("App open ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            DebugLog("App open ad full screen content closed.");
            LoadAppOpenAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            DebugLogError("App open ad failed to open full screen content " +
                           "with error : " + error);
            LoadAppOpenAd();
        };
    }

    private void DebugLog(string log)
    {
        if(showDebugLog) Debug.Log(log);
    }


    private void DebugLogError(string log)
    {
        if (showDebugLog) Debug.LogError(log);
    }
}
