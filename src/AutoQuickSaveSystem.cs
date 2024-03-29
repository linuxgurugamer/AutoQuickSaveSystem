﻿using System;
using UnityEngine;
using KSP.IO;

using KSP_Log;

namespace AutoQuickSaveSystem
{
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class AutoQuickSaveSystem : MonoBehaviour
    {

        private MainMenuGui gui;

        public static KSP_Log.Log Log = null;

        public AutoQuickSaveSystem()
        {
            Log.Info("A.Q.S.S.");
        }

        public void Awake()
        {
            Log.Info("awake");

            DontDestroyOnLoad(this);
        }

        public void Start()
        {
            Log.Info("Start");
            Configuration.StartUp();
            ConfigNodeIO.LoadData();
            Log.SetLevel(Configuration.LogLevel);


            if (this.gui == null)
            {
                this.gui = this.gameObject.AddComponent<MainMenuGui>();                
            }
        }




        internal void OnDestroy()
        {
            Log.Info("destroying A.Q.S.S.");
            ConfigNodeIO.excludes.Clear();
            ConfigNodeIO.excludes = null;
            Configuration.Save();
        }

    }

}
