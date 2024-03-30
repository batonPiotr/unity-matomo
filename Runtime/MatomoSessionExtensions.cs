// MIT License
// Copyright(c) 2021 Jonas Boetel
//----------------------------------------
using System.Collections;
using System.Collections.Generic;
using Lumpn.Matomo.Utils;
using UnityEngine;

namespace Lumpn.Matomo
{
    public static class MatomoSessionExtensions
    {
        private static readonly Dictionary<string, string> emptyParameters = new Dictionary<string, string>();

            /*
            e_c — The event category. Must not be empty. (eg. Videos, Music, Games...)
e_a — The event action. Must not be empty. (eg. Play, Pause, Duration, Add Playlist, Downloaded, Clicked...)
e_n — The event name. (eg. a Movie name, or Song name, or File name...)
e_v — The event value. Must be a float or integer value (numeric), not a string.
Note: Trailing and leading whitespaces will be trimmed from parameter values for e_c, e_a and e_n. Strings filled with whitespaces will be considered as (invalid) empty values.
            */
        public static IEnumerator RecordSystemInfo(this MatomoSession session)
        {
            var parameters = new Dictionary<string, string>
            {
                { "new_visit", "1"},
                { "ua", GetUserAgent(Application.unityVersion, Application.platform) },
                { "lang", LanguageUtils.GetLanguageCode(Application.systemLanguage) },
                { "res", string.Format("{0}x{1}", Screen.width, Screen.height)},
                { "dimension1", SystemInfo.processorType},
                { "dimension2", SystemInfo.graphicsDeviceName},
                { "e_c",  "SystemInfo" },
                { "e_a", "initial" }
            };

            using (var request = session.CreateWebRequest(parameters, false))
            {
                yield return request.SendWebRequest();

                Debug.Assert(request.responseCode == 204, request.error);
            }
        }

        public static IEnumerator SendCustomEvent(this MatomoSession session, string category, string action, string name = null, string value = null, bool debug = false)
        {
            var parameters = new Dictionary<string, string>
            {
                { "e_c", category },
                { "e_a", action }
            };
            if(name != null)
            {
                parameters["e_n"] = name;
            }
            if(value != null)
            {
                parameters["e_v"] = value;
            }

            using (var request = session.CreateWebRequest(parameters, debug))
            {
                yield return request.SendWebRequest();

                Debug.Assert(request.responseCode == 204, request.error);
            }
        }

        public static IEnumerator SendPageView(this MatomoSession session, string url, bool debug = false)
        {
            var parameters = new Dictionary<string, string>
            {
                { "url", session.websiteUrl + url },
                { "action_name", url }
            };

            using (var request = session.CreateWebRequest(parameters, debug))
            {
                yield return request.SendWebRequest();

                Debug.Assert(request.responseCode == 204, request.error);
            }
        }

        private static string GetUserAgent(string unityVersion, RuntimePlatform platform)
        {
            return string.Format("UnityPlayer/{0} ({1})", GetUnityVersion(unityVersion), PlatformUtils.GetDevice(platform));
        }

        private static string GetUnityVersion(string unityVersion)
        {
            return unityVersion.Substring(0, 6);
        }
    }
}
