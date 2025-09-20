using AmongUs.Data;
using UnityEngine;
using UnityEngine.Analytics;
using System;
using System.Security.Cryptography;
using Il2CppSystem.Net.Http.Headers;

namespace Monolith;
public class Blocker : MonoBehaviour
{
    public void Load()
    {
        Analytics.enabled = false;
        Analytics.enabledInternal = false;
        Analytics.deviceStatsEnabled = false;
        Analytics.deviceStatsEnabledInternal = false;
        Analytics.limitUserTracking = true;
        Analytics.limitUserTrackingInternal = true;
        Analytics.initializeOnStartup = false;
        Analytics.initializeOnStartupInternal = false;
        PerformanceReporting.enabled = false;
    }
};
