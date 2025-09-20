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
        Analytics.deviceStatsEnabled = false;
        PerformanceReporting.enabled = false;
    }
};
