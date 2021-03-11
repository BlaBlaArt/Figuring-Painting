using System;
using System.Collections.Generic;
using System.IO;
using HomaGames.HomaBelly.Utilities;
using UnityEngine;

namespace HomaGames.HomaBelly
{
    public class SingularAttribution : IAttributionWithInitializationCallback
    {
        private Dictionary<string, object> configurationData;

        #region Public methods

        public void Initialize(string appSubversion = "")
        {
            Initialize(appSubversion, null);
        }

        public void Initialize(string appSubversion = "", Action onInitialized = null)
        {
            configurationData = LoadConfigurationData();
            if (configurationData != null)
            {
                InstantiateSingularSdkGameObject((string)configurationData["s_api_key"], (string)configurationData["s_api_secret"]);

                // Set the app subversion as a Global Property.
                // Always after instantiation but before intialization.
                // See https://support.singular.net/hc/en-us/articles/360038355151#Adding_Global_Properties
                if (!string.IsNullOrEmpty(appSubversion) && appSubversion != "0")
                {
                    bool result = SingularSDK.SetGlobalProperty("s_app_subversion", appSubversion, true);
                    HomaGamesLog.Debug($"Singular Global Property s_app_subversion set to {appSubversion}: {result}");
                }
            }

#if UNITY_EDITOR
            HomaGamesLog.Debug("Singular SDK initialized");
#else
            SingularSDK.InitializeSingularSDK();
#endif

            if (onInitialized != null)
            {
                onInitialized.Invoke();
            }
        }

        public void OnApplicationPause(bool pause)
        {
            // N/A
        }

        public void ValidateIntegration()
        {
            // TODO: Validate Singular integration
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
#if UNITY_EDITOR
            return;
#endif

#pragma warning disable CS0162 // Remove inaccessible code warning
            SingularSDK.LimitDataSharing(!consent);

            if (consent)
            {
                if (SingularSDK.IsAllTrackingStopped())
                {
                    SingularSDK.ResumeAllTracking();
                }

                SingularSDK.TrackingOptIn();
            }
            else
            {
                SingularSDK.StopAllTracking();
            }
#pragma warning restore CS0162
        }

        public void SetTailoredAdsConsentGranted(bool consent)
        {
            // NO-OP
        }

        public void TrackInAppPurchaseEvent(string productId, string currencyCode, double unitPrice, string transactionId = null, string receipt = null)
        {
#if UNITY_EDITOR
            return;
#elif UNITY_ANDROID
            string googlePlayJson = "";
            string googlePlaySignature = "";

            if (receipt != null)
            {
                Dictionary<string, object> receiptDictionary = Json.Deserialize(receipt) as Dictionary<string, object>;

                if (receiptDictionary != null && receiptDictionary.ContainsKey("Payload"))
                {
                    Dictionary<string, object> googlePlayPayloadDictionary = receiptDictionary["Payload"] as Dictionary<string, object>;
                    if (googlePlayPayloadDictionary != null)
                    {
                        googlePlayJson = googlePlayPayloadDictionary.ContainsKey("json") ? (string)googlePlayPayloadDictionary["json"] : "";
                        googlePlaySignature = googlePlayPayloadDictionary.ContainsKey("signature") ? (string)googlePlayPayloadDictionary["signature"] : "";
                    }
                }
            }

            SingularSDK.CustomRevenue("__iap__", currencyCode, unitPrice, googlePlayJson, googlePlaySignature);
#elif UNITY_IOS
            SingularSDK.CustomRevenue("__iap__", currencyCode, 1, productId, "", "", 1, unitPrice);
#endif
        }

        public void TrackAdRevenue(AdRevenueData adRevenueData)
        {
            HomaGamesLog.Debug($"Tracking Ad Revenue for Singular: {adRevenueData}");
#if UNITY_EDITOR
            return;
#endif

#pragma warning disable CS0162 // Remove inaccessible code warning
            SingularAdData data = new SingularAdData(adRevenueData.AdPlatform, adRevenueData.Currency, adRevenueData.Revenue);

            data.WithAdGroupId(adRevenueData.AdGroupId)
                .WithAdGroupName(adRevenueData.AdGroupName)
                .WithAdGroupType(adRevenueData.AdGroupType)
                .WithAdGroupPriority(adRevenueData.AdGroupPriority)
                .WithAdUnitId(adRevenueData.AdUnitId)
                .WithAdUnitName(adRevenueData.AdUnitName)
                .WithImpressionId(adRevenueData.ImpressionId)
                .WithNetworkName(adRevenueData.NetworkName)
                .WithPlacementId(adRevenueData.PlacementId)
                .WithAdType(adRevenueData.AdType)
                .WithAdPlacmentName(adRevenueData.AdPlacamentName)
                .WithPrecision(adRevenueData.Precision);

            SingularSDK.AdRevenue(data);
#pragma warning restore CS0162
        }

        public void TrackEvent(string eventName, Dictionary<string, object> arguments = null)
        {
            HomaGamesLog.Debug($"Tracking Event on Singular: {eventName}");
            if (arguments != null)
            {
                HomaGamesLog.Debug($"With arguments: {JsonUtility.ToJson(arguments)}");
            }

#if UNITY_EDITOR
            return;
#endif

#pragma warning disable CS0162 // Remove inaccessible code warning
            if (arguments != null)
            { 
                SingularSDK.Event(arguments, eventName);
            }
            else
            {
                SingularSDK.Event(eventName);
            }
#pragma warning restore CS0162
        }

        #endregion

        #region Private methods

        private Dictionary<string, object> LoadConfigurationData()
        {
#if UNITY_EDITOR
            if (!File.Exists(HomaBellySingularConstants.CONFIG_FILE))
            {
                return null;
            }
#endif
             
            string configJson = FileUtilities.ReadAllText(HomaBellySingularConstants.CONFIG_FILE);
            return Json.Deserialize(configJson) as Dictionary<string, object>;
        }

        /// <summary>
        /// Creates a GameObject attaching the Singular SDK and setting keys
        /// </summary>
        /// <param name="apiKey">Singular API key</param>
        /// <param name="apiSecret">Singular API secret</param>
        private static void InstantiateSingularSdkGameObject(string apiKey, string apiSecret)
        {
            SingularSDK singularSdk = GameObject.FindObjectOfType<SingularSDK>();
            if (singularSdk == null)
            {
                GameObject singularSdkGameObject = new GameObject("SingularSDK");
                singularSdkGameObject.SetActive(false);
                singularSdk = singularSdkGameObject.AddComponent<SingularSDK>();

                if (singularSdk != null)
                {
                    singularSdk.SingularAPIKey = apiKey;
                    singularSdk.SingularAPISecret = apiSecret;
                    singularSdk.InitializeOnAwake = false;
#if UNITY_IOS
                    singularSdk.SKANEnabled = true;
                    singularSdk.manualSKANConversionManagement = false;

                    // Try to delay Singular initialization
                    string geryonIdfaConsentPopupDelayInSecodsString = GeryonUtils.GetGeryonDynamicVariableValue("IDFA_CONSENT_POPUP_DELAY_S");
                    if (!string.IsNullOrEmpty(geryonIdfaConsentPopupDelayInSecodsString))
                    {
                        int delayInSeconds = 0;
                        int.TryParse(geryonIdfaConsentPopupDelayInSecodsString, out delayInSeconds);
                        singularSdk.waitForTrackingAuthorizationWithTimeoutInterval = delayInSeconds + 30;  // Give 30 seconds margin
                    }
#endif
                }

                singularSdkGameObject.SetActive(true);
            }
        }

#endregion
    }
}
