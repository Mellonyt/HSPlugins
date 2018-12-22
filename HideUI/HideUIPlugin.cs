﻿using IllusionPlugin;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HideUI
{
    public class HideUIPlugin : IEnhancedPlugin
    {
        public const string PLUGIN_NAME = "HideUI";
        public const string PLUGIN_VERSION = "1.0";
        public string Name { get; } = PLUGIN_NAME;
        public string Version { get; } = PLUGIN_VERSION;

        public string[] Filter { get; } = new string[]
        {
            "HoneySelect_32",
            "HoneySelect_64",
            "StudioNEO_32",
            "StudioNEO_64",
        };

        public static string[] SceneFilter = new string[]
        {
            "Studio",
            "HScene",
            "CustomScene",
        };

        public void OnLevelWasLoaded(int level)
        {
            StartMod();
        }

        public static void StartMod()
        {
            if(SceneFilter.Contains(SceneManager.GetActiveScene().name)) new GameObject(PLUGIN_NAME).AddComponent<HideUI>();
        }

        public static void Bootstrap()
        {
            var gameobject = GameObject.Find(PLUGIN_NAME);
            if(gameobject != null) GameObject.DestroyImmediate(gameobject);
            StartMod();
        }

        public void OnApplicationStart(){}
        public void OnUpdate(){}
        public void OnLateUpdate(){}
        public void OnApplicationQuit(){}
        public void OnLevelWasInitialized(int level){}
        public void OnFixedUpdate(){}
    }
}
