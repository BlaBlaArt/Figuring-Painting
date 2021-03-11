using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HomaGames.HomaBelly.Utilities;

namespace HomaGames.HomaBelly
{
    /// <summary>
    /// Homa Bridge is the main connector between the public facade (HomaBelly)
    /// and all the inner behaviour of the Homa Belly library. All features
    /// and callbacks will be centralized within this class.
    /// </summary>
    public class HomaBridge : IHomaBellyBridge
    {
        #region Private properties
        private List<IMediator> mediators;
        private List<IAttribution> attributions;
        private List<IAnalytics> analytics;
        private InitializationStatus initializationStatus = new InitializationStatus();
        #endregion

        #region Public properties

        public bool IsInitialized
        {
            get
            {
                return initializationStatus.IsInitialized;
            }
        }

        #endregion

        public void Initialize()
        {
            RemoteConfiguration.FetchRemoteConfiguration().ContinueWith((remoteConfiguration) =>
            {
                HomaGamesLog.Debug("[Homa Belly] Initializing Homa Belly after Remote Configuration fetch");
                InitializeRemoteConfigurationDependantComponents(remoteConfiguration.Result);
            }, TaskScheduler.FromCurrentSynchronizationContext());

            InitializeRemoteConfigurationIndependentComponents();
        }

        /// <summary>
        /// Initializes all those components that can be initialized
        /// before the Remote Configuration data is fetched
        /// </summary>
        private void InitializeRemoteConfigurationIndependentComponents()
        {
            // Instantiate
            InstantiateMediators();
            InstantiateAttributions();
            InstantiateAnalytics();

            // Apply Homa Games GDPR values
            GDPRUtils.ApplyHomaGamesGDPRValues();

            // Auto-track AdEvents
            RegisterAdEventsForAnalytics();

            // Try to auto configure analytics custom dimensions from NTesting
            // This is done before initializing to ensure all analytic events
            // properly gather the custom dimension
            AutoConfigureAnalyticsCustomDimensionsForNTesting();

            // Initialize
            InitializeMediators();
            InitializeAttributions();
            InitializeAnalytics();

            // Start initialization grace period timer
            initializationStatus.StartInitializationGracePeriod();
        }

        /// <summary>
        /// Initializes all those components that require from Remote Configuration
        /// data in order to initialize
        /// </summary>
        private void InitializeRemoteConfigurationDependantComponents(RemoteConfiguration.RemoteConfigurationSetup remoteConfigurationSetup)
        {
            CrossPromotionManager.Initialize(remoteConfigurationSetup);
        }

        public void SetDebug(bool enabled)
        {

        }

        public void ValidateIntegration()
        {
            // Mediators
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.ValidateIntegration();
                }
            }

            // Attributions
            if (attributions != null)
            {
                foreach (IAttribution attribution in attributions)
                {
                    attribution.ValidateIntegration();
                }
            }

            // Analytics
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.ValidateIntegration();
                }
            }
        }

        public void OnApplicationPause(bool pause)
        {
            // Mediators
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.OnApplicationPause(pause);
                }
            }

            // Attributions
            if (attributions != null)
            {
                foreach (IAttribution attribution in attributions)
                {
                    attribution.OnApplicationPause(pause);
                }
            }
        }

        #region IHomaBellyBridge

        public void ShowRewardedVideoAd(string placementId = null)
        {
            TrackAdEvent(AdAction.Request, AdType.RewardedVideo, "homagames.homabelly.default", placementId);

            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.ShowRewardedVideoAd(placementId);
                }
            }
        }

        public bool IsRewardedVideoAdAvailable(string placementId = null)
        {
            bool available = false;
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    available |= mediator.IsRewardedVideoAdAvailable();
                }
            }

            return available;
        }

        // Banners
        public void LoadBanner(BannerSize size, BannerPosition position, string placementId = null, UnityEngine.Color bannerBackgroundColor = default)
        {
            TrackAdEvent(AdAction.Request, AdType.Banner, "homagames.homabelly.default", placementId);

            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.LoadBanner(size, position, placementId, bannerBackgroundColor);
                }
            }
        }

        public void ShowBanner(string placementId = null)
        {
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.ShowBanner();
                }
            }
        }

        public void HideBanner(string placementId = null)
        {
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.HideBanner();
                }
            }
        }

        public void DestroyBanner(string placementId = null)
        {
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.DestroyBanner();
                }
            }
        }

        public void ShowInsterstitial(string placementId = null)
        {
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.ShowInterstitial(placementId);
                }
            }
        }

        public bool IsInterstitialAvailable(string placementId = null)
        {
            bool available = false;
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    available |= mediator.IsInterstitialAvailable();
                }
            }

            return available;
        }

        public void SetUserIsAboveRequiredAge(bool consent)
        {
            // For mediators
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.SetUserIsAboveRequiredAge(consent);
                }
            }

            // For attributions
            if (attributions != null)
            {
                foreach (IAttribution attribution in attributions)
                {
                    attribution.SetUserIsAboveRequiredAge(consent);
                }
            }

            // For analytics
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.SetUserIsAboveRequiredAge(consent);
                }
            }
        }

        public void SetTermsAndConditionsAcceptance(bool consent)
        {
            // For mediators
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.SetTermsAndConditionsAcceptance(consent);
                }
            }

            // For attributions
            if (attributions != null)
            {
                foreach (IAttribution attribution in attributions)
                {
                    attribution.SetTermsAndConditionsAcceptance(consent);
                }
            }

            // For analytics
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.SetTermsAndConditionsAcceptance(consent);
                }
            }
        }

        public void SetAnalyticsTrackingConsentGranted(bool consent)
        {
            // For mediators
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.SetAnalyticsTrackingConsentGranted(consent);
                }
            }

            // For attributions
            if (attributions != null)
            {
                foreach (IAttribution attribution in attributions)
                {
                    attribution.SetAnalyticsTrackingConsentGranted(consent);
                }
            }

            // For analytics
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.SetAnalyticsTrackingConsentGranted(consent);
                }
            }
        }

        public void SetTailoredAdsConsentGranted(bool consent)
        {
            // For mediators
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    mediator.SetTailoredAdsConsentGranted(consent);
                }
            }

            // For attributions
            if (attributions != null)
            {
                foreach (IAttribution attribution in attributions)
                {
                    attribution.SetTailoredAdsConsentGranted(consent);
                }
            }

            // For analytics
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.SetTailoredAdsConsentGranted(consent);
                }
            }
        }

        public void TrackInAppPurchaseEvent(string productId, string currencyCode, double unitPrice, string transactionId = null, string payload = null)
        {
            // IAP events are applicable to Attributions and Analytics

            // For attributions
            if (attributions != null)
            {
                foreach (IAttribution attribution in attributions)
                {
                    attribution.TrackInAppPurchaseEvent(productId, currencyCode, unitPrice, transactionId, payload);
                }
            }

            // For analytics
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.TrackInAppPurchaseEvent(productId, currencyCode, unitPrice, transactionId, payload);
                }
            }
        }

        public void TrackResourceEvent(ResourceFlowType flowType, string currency, float amount, string itemType, string itemId)
        {
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.TrackResourceEvent(flowType, currency, amount, itemType, itemId);
                }
            }
        }

        public void TrackProgressionEvent(ProgressionStatus progressionStatus, string progression, int score = 0)
        {
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.TrackProgressionEvent(progressionStatus, progression, score);
                }
            }
        }

        public void TrackErrorEvent(ErrorSeverity severity, string message)
        {
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.TrackErrorEvent(severity, message);
                }
            }
        }

        public void TrackDesignEvent(string eventName, float eventValue = 0f)
        {
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.TrackDesignEvent(eventName, eventValue);
                }
            }
        }

        public void TrackAdEvent(AdAction adAction, AdType adType, string adNetwork, string adPlacementId)
        {
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    analytic.TrackAdEvent(adAction, adType, adNetwork, adPlacementId);
                }
            }
        }

        public void TrackAdRevenue(AdRevenueData adRevenueData)
        {
            if (attributions != null)
            {
                foreach (IAttribution attribution in attributions)
                {
                    attribution.TrackAdRevenue(adRevenueData);
                }
            }
        }

        public void TrackAttributionEvent(string eventName, Dictionary<string, object> arguments = null)
        {
            if (attributions != null)
            {
                foreach (IAttribution attribution in attributions)
                {
                    attribution.TrackEvent(eventName, arguments);
                }
            }
        }

        public void SetCustomDimension01(string customDimension)
        {
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    if (typeof(ICustomDimensions).IsInstanceOfType(analytic))
                    {
                        ((ICustomDimensions) analytic).SetCustomDimension01(customDimension);
                    }
                }
            }
        }

        public void SetCustomDimension02(string customDimension)
        {
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    if (typeof(ICustomDimensions).IsInstanceOfType(analytic))
                    {
                        ((ICustomDimensions)analytic).SetCustomDimension02(customDimension);
                    }
                }
            }
        }

        public void SetCustomDimension03(string customDimension)
        {
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    if (typeof(ICustomDimensions).IsInstanceOfType(analytic))
                    {
                        ((ICustomDimensions)analytic).SetCustomDimension03(customDimension);
                    }
                }
            }
        }

        #endregion

        #region Private helpers

        private void RegisterAdEventsForAnalytics()
        {
            // Banner
            Events.onBannerAdClickedEvent += (id) =>
            {
                TrackAdEvent(AdAction.Clicked, AdType.Banner, "homagames.homabelly.default", id);
            };

            Events.onBannerAdLoadedEvent += (id) =>
            {
                TrackAdEvent(AdAction.Loaded, AdType.Banner, "homagames.homabelly.default", id);
            };

            Events.onBannerAdLoadFailedEvent += (id) =>
            {
                TrackAdEvent(AdAction.FailedShow, AdType.Banner, "homagames.homabelly.default", id);
            };

            Events.onBannerAdScreenPresentedEvent += (id) =>
            {
                TrackAdEvent(AdAction.Show, AdType.Banner, "homagames.homabelly.default", id);
            };

            // Interstitial
            Events.onInterstitialAdClickedEvent += (id) =>
            {
                TrackAdEvent(AdAction.Clicked, AdType.Interstitial, "homagames.homabelly.default", id);
            };

            Events.onInterstitialAdShowFailedEvent += (id) =>
            {
                TrackAdEvent(AdAction.FailedShow, AdType.Interstitial, "homagames.homabelly.default", id);
            };

            Events.onInterstitialAdShowSucceededEvent += (id) =>
            {
                TrackAdEvent(AdAction.Show, AdType.Interstitial, "homagames.homabelly.default", id);
            };

            // Rewarded Video
            Events.onRewardedVideoAdRewardedEvent += (reward) =>
            {
                TrackAdEvent(AdAction.RewardReceived, AdType.RewardedVideo, "homagames.homabelly.default", reward.getPlacementName());
            };

            Events.onRewardedVideoAdShowFailedEvent += (id) =>
            {
                TrackAdEvent(AdAction.FailedShow, AdType.RewardedVideo, "homagames.homabelly.default", id);
            };

            Events.onRewardedVideoAdStartedEvent += (id) =>
            {
                TrackAdEvent(AdAction.Show, AdType.RewardedVideo, "homagames.homabelly.default", id);
            };

            Events.onRewardedVideoAvailabilityChangedEvent += (available,id) =>
            {
                if (available)
                {
                    TrackAdEvent(AdAction.Loaded, AdType.RewardedVideo, "homagames.homabelly.default", id);
                }
            };

            Events.onRewardedVideoAdClickedEvent += (id) =>
            {
                TrackAdEvent(AdAction.Clicked, AdType.RewardedVideo, "homagames.homabelly.default", id);
            };
        }

        private void AutoConfigureAnalyticsCustomDimensionsForNTesting()
        {
            // Wait for Geryon initialization to complete.
            // This is required after implementing Geryon <> Analytics automatic integration
            // and assign ExternalTokens to Custom Dimensions
            GeryonUtils.GetGeryonTestingId()
                .ContinueWith((result) =>
                {
                    // If the obtained NTesting ID is not empty
                    // means NTesting is present and initialized
                    if (!string.IsNullOrEmpty(result.Result))
                    {
                        string customDimension01 = GeryonUtils.GetNTestingExternalToken("ExternalToken0");
                        string customDimension02 = GeryonUtils.GetNTestingExternalToken("ExternalToken1");

                        if (!string.IsNullOrEmpty(customDimension01))
                        {
                            HomaGamesLog.Debug(string.Format("Setting Game Analytics custom dimension 01 to: {0}", customDimension01));
                            SetCustomDimension01(customDimension01);
                        }

                        if (!string.IsNullOrEmpty(customDimension02))
                        {
                            HomaGamesLog.Debug(string.Format("Setting Game Analytics custom dimension 02 to: {0}", customDimension02));
                            SetCustomDimension02(customDimension02);
                        }
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion

        #region Mediators

        private void InstantiateMediators()
        {
            // LINQ all available classes implementing IMediator interface
            List<Type> availableMediators = Reflection.GetHomaBellyAvailableClasses(typeof(IMediator));

            // If available mediators found, instantiate them
            if (availableMediators != null && availableMediators.Count > 0)
            {
                mediators = new List<IMediator>();
                foreach (Type type in availableMediators)
                {
                    HomaGamesLog.Debug($"[Homa Belly] Instantiating {type}");
                    mediators.Add(Activator.CreateInstance(type) as IMediator);
                }
            }
            else
            {
                HomaGamesLog.Warning("No available mediators found");
            }
        }

        private void InitializeMediators()
        {
            if (mediators != null)
            {
                foreach (IMediator mediator in mediators)
                {
                    // For Homa Belly v1.2.0+
                    if (typeof(IMediatorWithInitializationCallback).IsInstanceOfType(mediator))
                    {
                        ((IMediatorWithInitializationCallback) mediator).Initialize(initializationStatus.OnInnerComponentInitialized);
                    }
                    else
                    {
                        // For Homa Belly prior 1.2.0
                        mediator.Initialize();
                    }
                    
                    mediator.RegisterEvents();
                }
            }
        }

#endregion

#region Attributions

        private void InstantiateAttributions()
        {
            // LINQ all available classes implementing IAttribution interface
            List<Type> availableAttributions = Reflection.GetHomaBellyAvailableClasses(typeof(IAttribution));

            // If available mediators found, instantiate them
            if (availableAttributions != null && availableAttributions.Count > 0)
            {
                attributions = new List<IAttribution>();
                foreach (Type type in availableAttributions)
                {
                    HomaGamesLog.Debug($"[Homa Belly] Instantiating {type}");
                    attributions.Add(Activator.CreateInstance(type) as IAttribution);
                }
            }
            else
            {
                HomaGamesLog.Warning("No available attributions found");
            }
        }

        private void InitializeAttributions()
        {
            if (attributions != null)
            {
                // If Geryon NTESTING_ID is found, report it to all Attribution
                GeryonUtils.GetGeryonTestingId()
                    .ContinueWith((geryonTestingId) =>
                    {
                        foreach (IAttribution attribution in attributions)
                        {
                            // For Homa Belly v1.2.0+
                            if (typeof(IAttributionWithInitializationCallback).IsInstanceOfType(attribution))
                            {
                                ((IAttributionWithInitializationCallback)attribution).Initialize(geryonTestingId.Result, initializationStatus.OnInnerComponentInitialized);
                            }
                            else
                            {
                                // For Homa Belly prior 1.2.0
                                attribution.Initialize(geryonTestingId.Result);
                            }
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

#endregion

#region Analytics

        private void InstantiateAnalytics()
        {
            // LINQ all available classes implementing IAnalytics interface
            List<Type> availableAnalytics = Reflection.GetHomaBellyAvailableClasses(typeof(IAnalytics));

            // If available mediators found, instantiate them
            if (availableAnalytics != null && availableAnalytics.Count > 0)
            {
                analytics = new List<IAnalytics>();
                foreach (Type type in availableAnalytics)
                {
                    HomaGamesLog.Debug($"[Homa Belly] Instantiating {type}");
                    analytics.Add(Activator.CreateInstance(type) as IAnalytics);
                }
            }
            else
            {
                HomaGamesLog.Warning("No available analytics found");
            }
        }

        private void InitializeAnalytics()
        {
            if (analytics != null)
            {
                foreach (IAnalytics analytic in analytics)
                {
                    // For Homa Belly v1.2.0+
                    if (typeof(IAnalyticsWithInitializationCallback).IsInstanceOfType(analytic))
                    {
                        ((IAnalyticsWithInitializationCallback)analytic).Initialize(initializationStatus.OnInnerComponentInitialized);
                    }
                    else
                    {
                        // For Homa Belly prior 1.2.0
                        analytic.Initialize();
                    }
                }
            }
        }

#endregion
    }
}
