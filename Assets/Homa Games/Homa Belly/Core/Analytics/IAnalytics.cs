using System;

namespace HomaGames.HomaBelly
{
    public interface IAnalytics
    {
        // Base methods
        void Initialize();
        void OnApplicationPause(bool pause);
        void ValidateIntegration();

        #region GDPR/CCPA
        /// <summary>
        /// Specifies if the user asserted being above the required age
        /// </summary>
        /// <param name="consent">true if user accepted, false otherwise</param>
        void SetUserIsAboveRequiredAge(bool consent);

        /// <summary>
        /// Specifies if the user accepted privacy policy and terms and conditions
        /// </summary>
        /// <param name="consent">true if user accepted, false otherwise</param>
        void SetTermsAndConditionsAcceptance(bool consent);

        /// <summary>
        /// Specifies if the user granted consent for analytics tracking
        /// </summary>
        /// <param name="consent">true if user accepted, false otherwise</param>
        void SetAnalyticsTrackingConsentGranted(bool consent);

        /// <summary>
        /// Specifies if the user granted consent for showing tailored ads
        /// </summary>
        /// <param name="consent">true if user accepted, false otherwise</param>
        void SetTailoredAdsConsentGranted(bool consent);

        #endregion

        /// <summary>
        /// Tracks an IAP. IAP validation is optional: just provide
        /// `transactionId` and `payload` values and validation will be enabled.
        /// </summary>
        /// <param name="productId">The IAP product id</param>
        /// <param name="currencyCode">The currency code</param>
        /// <param name="unitPrice">The unit price</param>
        /// <param name="transactionId">(Optional) Transaciton ID for iOS IAP validation</param>
        /// <param name="payload">(Optional) IAP payload for validation</param>
        void TrackInAppPurchaseEvent(string productId, string currencyCode, double unitPrice, string transactionId = null, string payload = null);
        void TrackResourceEvent(ResourceFlowType flowType, string currency, float amount, string itemType, string itemId);
        void TrackProgressionEvent(ProgressionStatus progressionStatus, string progression, int score = 0);
        void TrackErrorEvent(ErrorSeverity severity, string message);
        void TrackDesignEvent(string eventName, float eventValue = 0f);
        void TrackAdEvent(AdAction adAction, AdType adType, string adNetwork, string adPlacementId);
    }
}
