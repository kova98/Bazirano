﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public static class ViewResultHelper
    {
        // TODO: Add option for buttons
        // In view: foreach button in Alert.Buttons
        // 

        public static ViewResult WithAlert(this ViewResult viewResult, AlertType alertType, string message)
        {
            viewResult.ViewData["Alert.Type"] = GetBootstrapClassForAlertType(alertType);
            viewResult.ViewData["Alert.Message"] = message;

            return viewResult;
        }

        public static ViewResult WithAlert(this ViewResult viewResult, AlertType alertType, string title, string message)
        {
            viewResult.ViewData["Alert.Title"] = title;

            return WithAlert(viewResult, alertType, message);
        }

        public static ViewResult WithAlert(this ViewResult viewResult, AlertType alertType, string title, string subtitle, string message)
        {
            viewResult.ViewData["Alert.Subtitle"] = subtitle;

            return WithAlert(viewResult, alertType, title, message);
        }

        private static object GetBootstrapClassForAlertType(AlertType alertType)
        {
            switch (alertType)
            {
                case AlertType.Info: return "alert-info";
                case AlertType.Warning: return "alert-warning";
                case AlertType.Error: return "alert-danger";
                case AlertType.Success: return "alert-success";

                default: return "bg-primary";
            }
        }

    }

    public enum AlertType
    {
        Info,
        Warning,
        Success,
        Error
    }
}
