using System;
using System.Linq;
using System.Reflection;

namespace HomaGames.HomaBelly.Utilities
{
    /// <summary>
    /// Utils to auto apply GDPR user preferences (if any).
    /// This class uses reflection to obtain values.
    /// </summary>
    public static class GDPRUtils
    {
        #region Constants

        /*
         *  These constants are `public` to ease testeability. These constants
         *  correspond to Homa Games GDPR module facade
         */
        public const string GDPR_NAMESPACE = "HomaGames.GDPR";
        public const string GDPR_MANAGER = "Manager";
        public const string METHOD_REQUIRED_AGE = "IsAboveRequiredAge";
        public const string METHOD_TERMS_AND_CONDITIONS = "IsTermsAndConditionsAccepted";
        public const string METHOD_ANALYTICS_GRANTED = "IsAnalyticsGranted";
        public const string METHOD_TAILORED_ADS = "IsTailoredAdsGranted";

        #endregion

        /// <summary>
        /// Try to apply Homa Games GDPR values (if any). If not,
        /// does nothing
        /// </summary>
        public static void ApplyHomaGamesGDPRValues()
        {
            ApplyUserIsAboveRequiredAge();
            ApplyTermsAndConditionsAcceptance();
            ApplyAnalyticsTrackingConsentGranted();
            ApplyTailoredAdsConsentGranted();
        }

        private static void ApplyUserIsAboveRequiredAge()
        {
            bool found = GetGDPRValueForMethod(METHOD_REQUIRED_AGE, out bool value);
            if (found)
            {

                HomaBelly.Instance.SetUserIsAboveRequiredAge(value);
            }
        }

        private static void ApplyTermsAndConditionsAcceptance()
        {
            bool found = GetGDPRValueForMethod(METHOD_TERMS_AND_CONDITIONS, out bool value);
            if (found)
            {

                HomaBelly.Instance.SetTermsAndConditionsAcceptance(value);
            }
        }

        private static void ApplyAnalyticsTrackingConsentGranted()
        {
            bool found = GetGDPRValueForMethod(METHOD_ANALYTICS_GRANTED, out bool value);
            if (found)
            {

                HomaBelly.Instance.SetAnalyticsTrackingConsentGranted(value);
            }
        }

        private static void ApplyTailoredAdsConsentGranted()
        {
            bool found = GetGDPRValueForMethod(METHOD_TAILORED_ADS, out bool value);
            if (found)
            {

                HomaBelly.Instance.SetTailoredAdsConsentGranted(value);
            }
        }

        /// <summary>
        /// Try to obtain a bool value from GDPR `methodName` method
        /// </summary>
        /// <param name="methodName">The method name to be invoked</param>
        /// <param name="result">The boolean result of the invoked method, as `out` parameter</param>
        /// <returns>True if method is invoked successfully, false otherwise</returns>
        private static bool GetGDPRValueForMethod(string methodName, out bool result)
        {
            try
            {
                HomaGamesLog.Debug($"Getting GDPR value for {methodName}");
                Type gdprManagerType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                        from type in assembly.GetTypes()
                                        where type.Namespace == GDPR_NAMESPACE && type.Name == GDPR_MANAGER
                                        select type).FirstOrDefault();
                if (gdprManagerType != null)
                {
                    PropertyInfo gdprManagerInstanceProperty = gdprManagerType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
                    if (gdprManagerInstanceProperty != null)
                    {
                        MethodInfo methodInfo = gdprManagerInstanceProperty.PropertyType.GetMethod(methodName);
                        if (methodInfo != null)
                        {
                            var gdprManager = gdprManagerInstanceProperty.GetValue(null, null);
                            var boolResult = methodInfo.Invoke(gdprManagerInstanceProperty.GetValue(null, null), null);

                            if (boolResult != null && bool.TryParse(boolResult.ToString(), out bool finalValue))
                            {
                                HomaGamesLog.Debug($"GDPR {methodName}: {finalValue}");
                                result = finalValue;
                                return true;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                HomaGamesLog.Warning($"GDPR method {methodName} not found: {e.Message}");
            }

            result = false;
            return false;
        }
    }
}

