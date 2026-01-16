using Microsoft.Win32;
using System.Runtime.Versioning;

namespace GameArchiver.Services
{
    // RegistryWorker (c) 2023-2026 elNino0916.
    // https://github.com/elnino0916/reShut-CLI
    [SupportedOSPlatform("windows")]
    class RegistryWorker
{

    public static void WriteToRegistry(string registryPath, string keyName, string type, string content)
    {
        try
        {
            // Determine the registry base key and subkey
            string[] pathParts = registryPath.Split('\\', 2);
            string baseKey = pathParts[0];
            string subKey = pathParts[1];

            // Get the appropriate base key
            RegistryKey registryKey = baseKey switch
            {
                "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                "HKEY_CURRENT_USER" => Registry.CurrentUser,
                "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                "HKEY_USERS" => Registry.Users,
                "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                _ => throw new ArgumentException("Invalid base registry key."),
            };

            // Open the subkey for writing
            using RegistryKey key = registryKey.OpenSubKey(subKey, writable: true) ?? registryKey.CreateSubKey(subKey);

            if (key == null)
            {
                return;
            }

            // Determine the value type and write to the registry
            switch (type.ToUpper())
            {
                case "STRING":
                    key.SetValue(keyName, content, RegistryValueKind.String);
                    break;
                case "DWORD":
                    if (int.TryParse(content, out int intValue))
                    {
                        key.SetValue(keyName, intValue, RegistryValueKind.DWord);
                    }
                    break;
                case "QWORD":
                    if (long.TryParse(content, out long longValue))
                    {
                        key.SetValue(keyName, longValue, RegistryValueKind.QWord);
                    }
                    break;
                case "BINARY":
                    try
                    {
                        byte[] binaryData = Convert.FromBase64String(content);
                        key.SetValue(keyName, binaryData, RegistryValueKind.Binary);
                    }
                    catch (FormatException)
                    {
                    }
                    break;
                case "MULTI_STRING":
                    string[] multiStringData = content.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    key.SetValue(keyName, multiStringData, RegistryValueKind.MultiString);
                    break;
                default:
                    break;
            }
        }
        catch
        {
        }
    }


    public static string ReadFromRegistry(string registryPath, string keyName)
    {
        try
        {
            // Determine the registry base key and subkey
            string[] pathParts = registryPath.Split('\\', 2);
            string baseKey = pathParts[0];
            string subKey = pathParts[1];

            // Get the appropriate base key
            RegistryKey registryKey = baseKey switch
            {
                "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                "HKEY_CURRENT_USER" => Registry.CurrentUser,
                "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                "HKEY_USERS" => Registry.Users,
                "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                _ => throw new ArgumentException("Invalid base registry key."),
            };

            // Open the subkey for reading
            using RegistryKey key = registryKey.OpenSubKey(subKey, writable: false);

            if (key == null)
            {
                return null;
            }

            // Read the value from the registry
            object value = key.GetValue(keyName);

            if (value == null)
            {
                return null;
            }

            return value.ToString();
        }
        catch
        {
            return null;
        }
    }


    public static void DeleteFromRegistry(string registryPath, string keyName)
    {
        try
        {
            // Determine the registry base key and subkey
            string[] pathParts = registryPath.Split('\\', 2);
            string baseKey = pathParts[0];
            string subKey = pathParts[1];

            // Get the appropriate base key
            RegistryKey registryKey = baseKey switch
            {
                "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                "HKEY_CURRENT_USER" => Registry.CurrentUser,
                "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                "HKEY_USERS" => Registry.Users,
                "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                _ => throw new ArgumentException("Invalid base registry key."),
            };

            // Open the subkey for writing
            using RegistryKey key = registryKey.OpenSubKey(subKey, writable: true);

            if (key != null)
            {
                key.DeleteValue(keyName, throwOnMissingValue: false);
            }
        }
        catch
        {
            return;
        }
    }


    public static bool Exists(string registryPath, string keyName)
    {
        try
        {
            // Determine the registry base key and subkey
            string[] pathParts = registryPath.Split('\\', 2);
            string baseKey = pathParts[0];
            string subKey = pathParts[1];

            // Get the appropriate base key
            RegistryKey registryKey = baseKey switch
            {
                "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                "HKEY_CURRENT_USER" => Registry.CurrentUser,
                "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                "HKEY_USERS" => Registry.Users,
                "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                _ => throw new ArgumentException("Invalid base registry key."),
            };

            // Open the subkey for reading
            using RegistryKey key = registryKey.OpenSubKey(subKey, writable: false);

            // Check if the key or value exists
            return key?.GetValue(keyName) != null;
        }
        catch
        {
            return false;
        }
    }
}
}
