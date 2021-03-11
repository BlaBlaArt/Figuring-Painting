using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace HomaGames.HomaBelly.Utilities
{
    public static class GeryonUtils
    {
        private const int WAIT_ITERATION_MS = 250;      // ms to wait for 'initialized' check
        private const int N_MAX_WAIT_ITERATIONS = 8;    // 8 iterations of 250ms => 2 seconds

        /// <summary>
        /// Try to obtain Geryon NTesting ID with reflection. If not found,
        /// returns an empty string.
        ///
        /// Upon Geryon v3.0.0+, it is initialized asynchronously. Hence, this method
        /// asynchronously awaits for its initialization (2 seconds) and then tries to
        /// obtian the NTESTING_ID
        /// </summary>
        /// <returns>The Geryon NTESTING_ID if found, or an empty string if not</returns>
        public static async Task<string> GetGeryonTestingId()
		{
            string geryonNtestingId = "";
            try
            {
                HomaGamesLog.Debug($"Looking for Geryon NTESTING_ID");
                Type geryonConfig = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                     from type in assembly.GetTypes()
                                     where type.Namespace == "HomaGames.Geryon" && type.Name == "Config"
                                     select type).FirstOrDefault();
                if (geryonConfig != null)
                {
                    // Run the reflection asynchronously
                    await Task.Run(() =>
                    {
                        // For NTesting 3.0.0+, 'Initialized' property is available after asynchronous initialization
                        System.Reflection.PropertyInfo ntestingInitializedPropertyInfo = geryonConfig.GetProperty("Initialized", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (ntestingInitializedPropertyInfo != null)
                        {
                            var propertyValue = ntestingInitializedPropertyInfo.GetValue(null, null);
                            if (propertyValue != null)
                            {
                                // Wait for `Initialized` property to be `true`
                                bool initialized = false;
                                int iterationCount = 0;
                                bool.TryParse(propertyValue.ToString(), out initialized);
                                while (!initialized && iterationCount < N_MAX_WAIT_ITERATIONS)
                                {
                                    // Not yet initialized. Debug log and wait a bit more
                                    HomaGamesLog.Debug($"NTesting not yet initialized. Iteration: {iterationCount}. Waiting...");
                                    Thread.Sleep(WAIT_ITERATION_MS);

                                    // Update value
                                    iterationCount++;
                                    propertyValue = ntestingInitializedPropertyInfo.GetValue(null, null);
                                    bool.TryParse(propertyValue.ToString(), out initialized);
                                }

                                // `Initialized` went `true` or the maximum iterations reached
                            }
                        }

                        // After waiting for `Initialized` propery (either becoming `true` or iterations finished)
                        // try to fecth NTESTING_ID. For Geryon prior v3.0.0 this will be executed right away
                        // without waiting for `Initialized`
                        System.Reflection.PropertyInfo ntestingIdPropertyInfo = geryonConfig.GetProperty("NTESTING_ID", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (ntestingIdPropertyInfo != null)
                        {
                            var ntestingPropertyValue = ntestingIdPropertyInfo.GetValue(null, null);
                            if (ntestingPropertyValue != null)
                            {
                                geryonNtestingId = ntestingPropertyValue.ToString();
                                HomaGamesLog.Debug($"Geryon NTESTING_ID found: {geryonNtestingId}");
                            }
                        }
                    });
                }
            }
            catch (Exception e)
            {
                HomaGamesLog.Warning($"Geryon not found: {e.Message}");
            }

            return geryonNtestingId;
        }

        /// <summary>
        /// Try to obtain Geryon dynamic variable
        /// </summary>
        /// <param name="propertyName">The property name of the variable. All in caps and without type prefix: ie. IDFA_CONSENT_POPUP_DELAY_S</param>
        /// <returns></returns>
        public static string GetGeryonDynamicVariableValue(string propertyName)
        {
            string value = null;
            try
            {
                Type geryonDvrType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                      from type in assembly.GetTypes()
                                      where type.Name == "DVR"
                                      select type).FirstOrDefault();
                if (geryonDvrType != null)
                {
                    FieldInfo field = geryonDvrType.GetField(propertyName);
                    if (field != null)
                    {
                        value = field.GetValue(null).ToString();
                        UnityEngine.Debug.Log($"{propertyName} value from Geryon: {value}");
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Could not obtain {propertyName} value from Geryon: {e.Message}");
            }

            return value;
        }

        /// <summary>
        /// Obtains by reflection the external token value for the given property name:
        /// - ExternalToken0
        /// - ExternalToken1
        /// </summary>
        /// <param name="externalTokenPropertyName"></param>
        /// <param name="externalToken"></param>
        public static string GetNTestingExternalToken(string externalTokenPropertyName)
        {
            string externalToken = "";
            try
            {
                Type geryonConfigType = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                      from type in assembly.GetTypes()
                                      where type.Name == "Config"
                                      select type).FirstOrDefault();
                if (geryonConfigType != null)
                {
                    PropertyInfo propertyInfo = geryonConfigType.GetProperty(externalTokenPropertyName, BindingFlags.Static | BindingFlags.Public);
                    if (propertyInfo != null)
                    {
                        var propertyValue = propertyInfo.GetValue(null, null);
                        if (propertyValue != null)
                        {
                            externalToken = propertyValue.ToString();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogWarning($"Could not obtain {externalTokenPropertyName} value from NTesting: {e.Message}");
            }

            return externalToken;
        }
    }
}

