//----------------------------------------
// MIT License
// Copyright(c) 2021 Jonas Boetel
//----------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Lumpn.Matomo.Utils;
using UnityEngine.Networking;

namespace Lumpn.Matomo
{
    public sealed class MatomoSession
    {
        private readonly Random random = new Random();
        private readonly StringBuilder stringBuilder = new StringBuilder();
        private readonly string baseUrl;
        public readonly string websiteUrl;

        public static MatomoSession Create(string matomoUrl, string websiteUrl, int websiteId, byte[] userId)
        {
            var sb = new StringBuilder(matomoUrl);
            sb.Append("/matomo.php?apiv=1&rec=1&send_image=0&idsite=");
            sb.Append(websiteId);

            sb.Append("&_id=");
            HexUtils.AppendHex(sb, userId);

            var url = sb.ToString();
            return new MatomoSession(url, websiteUrl);
        }

        private MatomoSession(string baseUrl, string websiteUrl)
        {
            this.baseUrl = baseUrl;
            this.websiteUrl = websiteUrl;
        }

        public UnityWebRequest CreateWebRequest(IReadOnlyDictionary<string, string> parameters, bool debug)
        {
            var url = BuildUrl(parameters, debug);
            var downloadHandler = debug ? new DownloadHandlerBuffer() : null;
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET, downloadHandler, null);
            return request;
        }

        private string BuildUrl(IReadOnlyDictionary<string, string> parameters, bool debug)
        {
            var sb = stringBuilder;
            sb.Clear();

            sb.Append(baseUrl);

            foreach (var parameter in parameters)
            {
                sb.Append("&");
                sb.Append(parameter.Key);
                sb.Append("=");
                sb.Append(EscapeDataString(parameter.Value));
            }

            if (debug)
            {
                sb.Append("&debug=1");
            }

            sb.Append("&rand=");
            sb.Append(random.Next());

            var url = sb.ToString();
            sb.Clear();

            return url;
        }

        private static string EscapeDataString(string str)
        {
            return Uri.EscapeDataString(str);
        }
    }
}
