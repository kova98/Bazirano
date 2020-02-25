using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public interface IGoogleRecaptchaHelper
    {
        Task<bool> IsRecaptchaValid(string gRecaptchaResponse, string secret);
    }
}
