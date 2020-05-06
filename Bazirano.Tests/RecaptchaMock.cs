using Bazirano.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Bazirano.Tests
{
    public class RecaptchaMock : IGoogleRecaptchaHelper
    {
        private readonly bool validationResult = true;

        public async Task<bool> VerifyRecaptcha(HttpRequest request, ModelStateDictionary modelState)
        {
            return await Task.FromResult(validationResult);
        }

        public RecaptchaMock(bool validationResult = true)
        {
            this.validationResult = validationResult;
        }
    }
}
