using System;

namespace AntiPatternsDemo.Configuration
{
    // Bad: Static class with global state
    public static class BadAppConfig
    {
        // Bad: Hardcoded configuration values
        public static string DatabaseConnectionString = "Server=localhost;Database=shop;User=admin;Password=123456";
        public static string SmtpServer = "smtp.gmail.com";
        public static int SmtpPort = 587;
        public static string SmtpUsername = "shop@gmail.com";
        public static string SmtpPassword = "password123";
        public static string ManagerEmail = "manager@shop.com";

        // Bad: Magic numbers
        public static int MaxRetryAttempts = 3;
        public static int RetryDelayMs = 1000;
        public static int SessionTimeoutMinutes = 30;
        public static int MaxConcurrentConnections = 100;

        // Bad: Mixed concerns - logging configuration
        public static string LogFilePath = "C:\\Logs\\app.log";
        public static bool EnableDebugLogging = true;
        public static int MaxLogFileSizeMB = 10;
        public static int MaxLogFiles = 5;

        // Bad: Business rules in configuration
        public static decimal HighValueOrderThreshold = 1000;
        public static decimal VipDiscountPercentage = 10;
        public static int MinimumOrderQuantity = 1;
        public static int MaximumOrderQuantity = 100;

        // Bad: No proper validation
        public static void Validate()
        {
            if (string.IsNullOrEmpty(DatabaseConnectionString))
                throw new Exception("Database connection string is required");

            if (string.IsNullOrEmpty(SmtpServer))
                throw new Exception("SMTP server is required");

            if (SmtpPort <= 0)
                throw new Exception("SMTP port must be greater than 0");

            // Bad: Inconsistent validation
            if (MaxRetryAttempts < 0)
                throw new Exception("Max retry attempts cannot be negative");

            // Bad: Missing validation for other properties
        }

        // Bad: Mixed responsibilities - logging
        public static void LogError(string message)
        {
            // Bad: Implementation details
            System.IO.File.AppendAllText(LogFilePath, $"{DateTime.Now}: ERROR - {message}\n");
        }
    }
} 