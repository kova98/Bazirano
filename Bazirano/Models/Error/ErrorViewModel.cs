using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Models.Error
{
    public class ErrorViewModel
    {
        public string RedirectActionName { get; set; }

        public string RedirectControllerName { get; set; }

        public string Message { get; set; }

        public string ButtonText { get; set; }
    }
}
