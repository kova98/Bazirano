using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public class GoogleRecaptchaHelper : IGoogleRecaptchaHelper
    {
        private IConfiguration config;

        public GoogleRecaptchaHelper(IConfiguration config)
        {
            this.config = config;
        }

        public async Task<bool> VerifyRecaptcha(HttpRequest request, ModelStateDictionary modelState)
        {
            if (request != null) // Only when unit testing
            {
                try
                {
                    if (string.IsNullOrEmpty(request.Form["g-recaptcha-response"]) || !await IsRecaptchaValid(request.Form["g-recaptcha-response"]))
                    {
                        modelState.AddModelError("captchaError", "CAPTCHA provjera neispravna.");
                        return false;
                    }
                }
                catch (InvalidOperationException)
                {
                    modelState.AddModelError("captchaError", "CAPTCHA forma neispravna.");
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> IsRecaptchaValid(string gRecaptchaResponse)
        {
            string secret = config["GoogleReCaptcha:secret"];

            HttpClient httpClient = new HttpClient();
            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("secret", secret),
                new KeyValuePair<string, string>("response", gRecaptchaResponse)
            });

            var res = await httpClient.PostAsync($"https://www.google.com/recaptcha/api/siteverify", content);
            if (res.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);
            if (JSONdata.success != "true")
            {
                return false;
            }

            return true;
        }
    }
}

