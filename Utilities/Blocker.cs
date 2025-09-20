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
        /* 
         * AS OF 20TH OF SEP 2025 THERE IS NO AMONG US ANTICHEAT,
         * I DISABLED UNITY ANALYTICS ABOVE,
         * ONLY REAL DETECTIONS ARE ALL SERVERSIDE.
        */
    }
};