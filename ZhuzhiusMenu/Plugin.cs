using BepInEx;
using HarmonyLib;
using Photon.Realtime;
using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

[BepInPlugin("com.Artemius466.ZhuzhiusLoader", "Zhuzhius Loader", "1.0.0")]
public class ZhuzhiusLoader : BaseUnityPlugin
{
    private const string DownloadUrl = "https://github.com/artemius466/ZhuzhiusPublic/raw/refs/heads/main/Zhuzhius.dll"; // URL к .dll
    private const string TargetPluginPath = "BepInEx/plugins/Zhuzhius.dll";

    void Start()
    {
        Logger.LogInfo("Harmony Dynamic Loader started.");
        DownloadAndLoadPlugin();
    }

    private void DownloadFile()
    {
        using (WebClient client = new WebClient())
        {
            client.DownloadFile(DownloadUrl, TargetPluginPath);
        }
    }

    private void DownloadAndLoadPlugin()
    {
        try
        {
            try
            {
                File.Delete(Path.GetFullPath(TargetPluginPath));
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex);
            }

            DownloadFile();

            Assembly assembly = Assembly.LoadFile(Path.GetFullPath(TargetPluginPath));

            foreach (var type in assembly.GetTypes())
            {
                var injectMethod = type.GetMethod("Inject", BindingFlags.Public | BindingFlags.Static);
                if (injectMethod != null)
                {
                    Logger.LogInfo("Found ZhuzhiusMain. Attempting to inject...");
                    injectMethod?.Invoke(null, null);
                    Logger.LogInfo("Injection complete!");
                    return;
                }
            }

            Logger.LogWarning("ZhuzhiusMain not found in the assembly. Could not inject.");
        }
        catch (Exception ex)
        {
            Logger.LogError($"Failed to download or load plugin: {ex.Message}");
        }
    }
}