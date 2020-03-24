using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bazirano.Infrastructure
{
    public static class Alert
    {
        public static void Add(Controller controller, AlertType alertType, string message)
        {
            controller.TempData["Alert.Type"] = GetBootstrapClassForAlertType(alertType);
            controller.TempData["Alert.Message"] = message;
        }

        public static void Add(Controller controller, AlertType alertType, string title, string message)
        {
            controller.TempData["Alert.Title"] = title;
            Add(controller, alertType, message);
        }

        public static void Add(Controller controller, AlertType alertType, string title, string subtitle, string message)
        {
            controller.TempData["Alert.Subtitle"] = subtitle;
            Add(controller, alertType, title, message);
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
