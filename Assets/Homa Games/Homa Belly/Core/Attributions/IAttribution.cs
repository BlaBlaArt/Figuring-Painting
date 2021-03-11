using System;
using System.Collections.Generic;

namespace HomaGames.HomaBelly
{
    public interface IAttribution
    {
        // Base methods
        void Initialize(string appSubversion = "");
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

        /// <summary>
        /// Tracks an Ad Revenue event
        /// </summary>
        /// <param name="adRevenueData">Object holding all ad revenue data to be sent</param>
        void TrackAdRevenue(AdRevenueData adRevenueData);

        /// <summary>
        /// Tracks an event on the attribution platform
        /// </summary>
        /// <param name="eventName">The event name</param>
        /// <param name="arguments">(Optional) Additional arguments. Dictionary values must have one of these types: string, int, long, float, double, null, ArrayList, Dictionary<String,object></param>
        void TrackEvent(string eventName, Dictionary<string, object> arguments = null);
    }
}
