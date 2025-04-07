using System.Text.RegularExpressions;
using System.Diagnostics;

namespace EnvironmentManager.Services
{
    public class ValidationService
    {
        // Updated pattern to allow more characters including numbers mixed with text
        private static readonly Regex TextPattern = new Regex(@"^[a-zA-Z0-9\s\-_.,/()&+#@!%]*[a-zA-Z][a-zA-Z0-9\s\-_.,/()&+#@!%]*$");
        
        // Regular expression for URLs
        private static readonly Regex UrlPattern = new Regex(@"^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-zA-Z0-9]+([\-\.]{1}[a-zA-Z0-9]+)*\.[a-zA-Z]{2,5}(:[0-9]{1,5})?(\/.*)?$");
        
        // Regular expression for version numbers
        private static readonly Regex VersionPattern = new Regex(@"^v?\d+(\.\d+)*[a-zA-Z]?$");

        public static (bool IsValid, string ErrorMessage) ValidateTextField(string fieldName, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                Debug.WriteLine($"Validation failed for {fieldName}: Empty value");
                return (false, $"{fieldName} cannot be empty");
            }

            if (value.All(char.IsDigit))
            {
                Debug.WriteLine($"Validation failed for {fieldName}: Numeric only value '{value}'");
                return (false, $"{fieldName} cannot contain only numbers");
            }

            if (!TextPattern.IsMatch(value))
            {
                Debug.WriteLine($"Validation failed for {fieldName}: Invalid characters in value '{value}'");
                return (false, $"{fieldName} contains invalid characters");
            }

            Debug.WriteLine($"Validation passed for {fieldName}: '{value}'");
            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidateUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                Debug.WriteLine("URL validation passed: Empty URL is allowed");
                return (true, string.Empty); // URL is optional
            }

            if (!UrlPattern.IsMatch(url))
            {
                Debug.WriteLine($"URL validation failed: Invalid format '{url}'");
                return (false, "Invalid URL format");
            }

            Debug.WriteLine($"URL validation passed: '{url}'");
            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidateVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                Debug.WriteLine("Version validation passed: Empty version is allowed");
                return (true, string.Empty); // Version is optional
            }

            if (!VersionPattern.IsMatch(version))
            {
                Debug.WriteLine($"Version validation failed: Invalid format '{version}'");
                return (false, "Invalid version format (e.g., v1.0.0 or 1.0.0)");
            }

            Debug.WriteLine($"Version validation passed: '{version}'");
            return (true, string.Empty);
        }

        public static (bool IsValid, string ErrorMessage) ValidateBatteryLevel(float? batteryLevel)
        {
            if (!batteryLevel.HasValue)
            {
                Debug.WriteLine("Battery level validation passed: No value provided");
                return (true, string.Empty); // Battery level is optional
            }

            if (batteryLevel.Value < 0 || batteryLevel.Value > 100)
            {
                Debug.WriteLine($"Battery level validation failed: Value {batteryLevel.Value} out of range");
                return (false, "Battery level must be between 0 and 100");
            }

            Debug.WriteLine($"Battery level validation passed: {batteryLevel.Value}");
            return (true, string.Empty);
        }

        public static (bool IsValid, Dictionary<string, string> Errors) ValidateSensor(
            string sensorName,
            string model,
            string manufacturer,
            string sensorType,
            string firmwareVersion,
            string sensorUrl,
            float? batteryLevel)
        {
            Debug.WriteLine("Starting sensor validation...");
            var errors = new Dictionary<string, string>();
            bool isValid = true;

            // Required text fields
            var (isNameValid, nameError) = ValidateTextField("Sensor Name", sensorName);
            if (!isNameValid)
            {
                errors["SensorName"] = nameError;
                isValid = false;
            }

            // Optional text fields
            if (!string.IsNullOrWhiteSpace(model))
            {
                var (isModelValid, modelError) = ValidateTextField("Model", model);
                if (!isModelValid)
                {
                    errors["Model"] = modelError;
                    isValid = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(manufacturer))
            {
                var (isManufacturerValid, manufacturerError) = ValidateTextField("Manufacturer", manufacturer);
                if (!isManufacturerValid)
                {
                    errors["Manufacturer"] = manufacturerError;
                    isValid = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(sensorType))
            {
                var (isTypeValid, typeError) = ValidateTextField("Sensor Type", sensorType);
                if (!isTypeValid)
                {
                    errors["SensorType"] = typeError;
                    isValid = false;
                }
            }

            // Version validation
            var (isVersionValid, versionError) = ValidateVersion(firmwareVersion);
            if (!isVersionValid)
            {
                errors["FirmwareVersion"] = versionError;
                isValid = false;
            }

            // URL validation
            var (isUrlValid, urlError) = ValidateUrl(sensorUrl);
            if (!isUrlValid)
            {
                errors["SensorUrl"] = urlError;
                isValid = false;
            }

            // Battery level validation
            var (isBatteryValid, batteryError) = ValidateBatteryLevel(batteryLevel);
            if (!isBatteryValid)
            {
                errors["BatteryLevel"] = batteryError;
                isValid = false;
            }

            Debug.WriteLine($"Sensor validation completed. IsValid: {isValid}, Error count: {errors.Count}");
            if (!isValid)
            {
                Debug.WriteLine("Validation errors:");
                foreach (var error in errors)
                {
                    Debug.WriteLine($"- {error.Key}: {error.Value}");
                }
            }

            return (isValid, errors);
        }
    }
} 