﻿using Microsoft.ClearScript.V8;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VideoLibrary.Helpers;

namespace VideoLibrary
{
    public partial class YouTubeVideo
    {
        private async Task<string> NDescrambleAsync(string uri, Func<DelegatingClient> makeClient)
        {
            var query = new Query(uri);

            if (!query.TryGetValue("n", out var signature))
                return uri;

            if (string.IsNullOrWhiteSpace(signature))
                throw new Exception("N Signature not found.");

            if (jsPlayer == null)
            {
                jsPlayer = await makeClient()
                    .GetStringAsync(jsPlayerUrl)
                    .ConfigureAwait(false);
            }

            query["n"] = DescrambleNSignature(jsPlayer, signature);
            return query.ToString();
        }

        private string DescrambleNSignature(string js, string signature)
        {
            var descrambleFunction = GetDescrambleFunctionLines(js);

            if (descrambleFunction?.Item1 != null && descrambleFunction?.Item2 != null)
            {
                string sign;
                using (var engine = new V8ScriptEngine())
                {
                    string v = descrambleFunction.Item1;
                    string f = descrambleFunction.Item2.Insert(12, " descramble");
                    f = f.Replace($"{v}=", "");
                    engine.Execute($"{f};");
                    sign = engine.Script.descramble(signature);
                };
                signature = sign;
            }

            return signature;
        }

        private Tuple<string, string> GetDescrambleFunctionLines(string js)
        {
            string functionName = null;
            var functionLine = Regex.Match(js, @"\.get\(""n""\)\)&&\(b=([a-zA-Z0-9$]+)(?:\[(\d+)\])?\([a-zA-Z0-9]\)");

            if (functionLine.Success && !functionLine.Groups[2].Success)
            {
                functionName = functionLine.Groups[1].Value;
            }
            else
            {
                var fname = Regex.Match(js, $@"var {functionLine.Groups[1]}\s*=\s*(\[.+?\]);");
                if (fname.Success && fname.Groups[1].Success)
                {
                    functionName = fname.Groups[1].Value
                        .Replace("[", string.Empty)
                        .Replace("]", string.Empty)
                        .Split(',')[int.Parse(functionLine.Groups[2].Value)];
                }
            }

            if (!string.IsNullOrWhiteSpace(functionName))
            {
                var decipherDefinitionBody = Regex.Match(js, $@"{Regex.Escape(functionName)}=function\(\w+(,\w+)?\)\{{(?s:.*?)return b\.join\(\""\""\)\}};", RegexOptions.Singleline);

                if (decipherDefinitionBody.Success)
                {
                    return new Tuple<string, string>(functionName, decipherDefinitionBody.Groups[0].Value);
                }
            }

            return new Tuple<string, string>(functionName, null);
        }
    }
}