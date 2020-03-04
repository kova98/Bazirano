using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public interface IGoogleRecaptchaHelper
    {
        Task<bool> VerifyRecaptcha(HttpRequest request, ModelStateDictionary modelState);
    }
}
