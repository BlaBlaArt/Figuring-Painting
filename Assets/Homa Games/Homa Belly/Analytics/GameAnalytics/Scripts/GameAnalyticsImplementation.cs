using System;
using System.IO;
using GameAnalyticsSDK;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class GameAnalyticsImplementation : IAnalyticsWithInitializationCallback, ICustomDimensions
    {
        #region Public methods
        public void Initialize()
        {
            Initialize(null);
        }

        public void Initialize(Action onInitialized = null)
        {
            // Try to find any previous GameAnalytics object and destroy it
            UnityEngine.Object gameAnalyticsObject = UnityEngine.Object.FindObjectOfType(typeof(GameAnalytics));
            if (gameAnalyticsObject != null)
            {
                UnityEngine.Object.Destroy(gameAnalyticsObject);
            }

            // Create GameObject for GameAnalytics in runtime and attach script
            GameObject gameAnalyticsGameObject = new GameObject("Game Analytics");
            gameAnalyticsGameObject.AddComponent<GameAnalytics>();
            GameAnalytics.Initialize();

            // Invoke intialization
            if (onInitialized != null)
            {
                onInitialized.Invoke();
            }
        }

        public void OnApplicationPause(bool pause)
        {
            // N/A
        }

        public void SetUserIsAboveRequiredAge(bool consent)
        {
            // NO-OP
        }

        public void SetTermsAndConditionsAcceptance(bool consent)
        {
            // NO-OP
        }

        public void SetAnalyticsTrackingConsentGranted(bool consent)
        {
            GameAnalytics.SetEnabledEventSubmission(consent);
        }

        public void SetTailoredAdsConsentGranted(bool consent)
        {
            // NO-OP
        }

        public void ValidateIntegration()
        {
#if UNITY_EDITOR
            string gameAnalyticsSettingsPath = Application.dataPath + "/Resources/GameAnalytics/Settings.asset";
            if (File.Exists(gameAnalyticsSettingsPath))
            {
#endif
                if (GameAnalytics.SettingsGA != null && GameAnalytics.SettingsGA.Platforms != null)
                {
                    if (GameAnalytics.SettingsGA.Platforms.Count > 0)
                    {
                        HomaGamesLog.Debug($"[Validate Integration] Game Analytics successfully integrated");
                    }
                }
                else
                {
                    HomaGamesLog.Warning($"[Validate Integration] Wrong configuration for Game Analytics");
                }
#if UNITY_EDITOR
            }
            else
            {
                HomaGamesLog.Warning($"[Validate Integration] Game Analytics Settings not found. Please see {gameAnalyticsSettingsPath}");
            }
#endif
        }

        public void TrackInAppPurchaseEvent(string productId, string currencyCode, double unitPrice, string transactionId = null, string payload = null)
        {
            // For the seek of Homa Belly standarization, we do not
            // inform `itemType` nor `cartType`, as attribution products
            // do not take care of them
            int unitPriceInCents = (int)(unitPrice * 100);
            string itemType = "";
            string cartType = "";

#if UNITY_ANDROID
            GameAnalytics.NewBusinessEventGooglePlay(currencyCode, unitPriceInCents, itemType, productId, cartType, transactionId, payload);
#elif UNITY_IOS
            GameAnalytics.NewBusinessEventIOS(currencyCode, unitPriceInCents, itemType, productId, cartType, transactionId);
#endif
        }

        public void TrackInAppPurchaseEvent(string currency, int unitPrice, string itemType, string itemId, string cartType, string receipt, string signature = null)
        {
#if UNITY_ANDROID
            GameAnalytics.NewBusinessEventGooglePlay(currency, unitPrice, itemType, itemId, cartType, receipt, signature);
#elif UNITY_IOS
            GameAnalytics.NewBusinessEventIOS(currency, unitPrice, itemType, itemId, cartType, receipt);
#endif
        }

        public void TrackResourceEvent(ResourceFlowType flowType, string currency, float amount, string itemType, string itemId)
        {
            GameAnalytics.NewResourceEvent((GAResourceFlowType)flowType, currency, amount, itemType, itemId);
        }

        public void TrackProgressionEvent(ProgressionStatus progressionStatus, string progression, int score = 0)
        {
            GameAnalytics.NewProgressionEvent((GAProgressionStatus)progressionStatus, progression, score);
        }

        public void TrackErrorEvent(ErrorSeverity severity, string message)
        {
            GameAnalytics.NewErrorEvent((GAErrorSeverity)severity, message);
        }

        public void TrackDesignEvent(string eventName, float eventValue = 0f)
        {
            GameAnalytics.NewDesignEvent(eventName, eventValue);
        }

        public void TrackAdEvent(AdAction adAction, AdType adType, string adNetwork, string adPlacementId)
        {
            GameAnalytics.NewAdEvent((GAAdAction)adAction, (GAAdType)adType, adNetwork, string.IsNullOrEmpty(adPlacementId) ? "default" : adPlacementId);
        }

        #endregion

        #region ICustomDimensions

        public void SetCustomDimension01(string customDimension)
        {
            GameAnalytics.SetCustomDimension01(customDimension);
        }

        public void SetCustomDimension02(string customDimension)
        {
            GameAnalytics.SetCustomDimension02(customDimension);
        }

        public void SetCustomDimension03(string customDimension)
        {
            GameAnalytics.SetCustomDimension03(customDimension);
        }

        #endregion
    }
}

