﻿using ColossalFramework.Plugins;
using ColossalFramework.UI;
using ICities;
using RoundaboutBuilder.Tools;
using RoundaboutBuilder.UI;
using System;
using System.Linq;
using UnityEngine;

/* By Strad, 01/2019 */

/* Version RELEASE 1.4.0+ */

/* Most of this is copied from Elektrix's Segment Slope Smoother.
 * The oter part is copied from somewhere as well, but unfortunately I don't remeber from where. */

namespace RoundaboutBuilder
{
    public class ModLoadingExtension : ILoadingExtension
    {
        public static bool appModeGame = false;
        public static bool LevelLoaded = false;
        public static bool tmpeDetected = false;
        public static bool fineRoadToolDetected = false;
        public static bool networkAnarchyDetected = false;
        public static bool undoItDetected = false;
        private static bool unifiedUiDetected = false;

        private static readonly UInt64[] TMPE_IDs = { 583429740, 1637663252, 1806963141 };
        private static readonly UInt64[] FINE_ROAD_ANARCHY_IDs = { 651322972, 1844442251 };
        private static readonly UInt64[] NETWORK_ANARCHY_IDs = { 2862881785 };
        private static readonly UInt64[] UNDO_IT_IDs = { 1890830956 };
        private static readonly UInt64[] UNIFIED_UI_IDs = { 2966990700 };

        // called when level loading begins
        public void OnCreated(ILoading loading)
        {
            appModeGame = loading.currentMode == AppMode.Game;
            tmpeDetected = false;
            fineRoadToolDetected = false;
            networkAnarchyDetected = false;
            unifiedUiDetected = false;

            foreach (PluginManager.PluginInfo current in PluginManager.instance.GetPluginsInfo())
            {
                //_string += current.name + " ";
                if (!tmpeDetected && current.isEnabled && (current.name.Contains("TrafficManager") || TMPE_IDs.Contains(current.publishedFileID.AsUInt64)))
                {
                    tmpeDetected = true;
                    //_string += "[TMPE Detected!]";
                }
                else if (!networkAnarchyDetected && current.isEnabled && (current.name.Contains("NetworkAnarchy") || NETWORK_ANARCHY_IDs.Contains(current.publishedFileID.AsUInt64)))
                {
                    networkAnarchyDetected = true;
                }
                else if (!fineRoadToolDetected && current.isEnabled && (current.name.Contains("FineRoadTool") || FINE_ROAD_ANARCHY_IDs.Contains(current.publishedFileID.AsUInt64)))
                {
                    fineRoadToolDetected = true;
                }
                else if (!undoItDetected && current.isEnabled && (current.name.Contains("UndoMod") || UNDO_IT_IDs.Contains(current.publishedFileID.AsUInt64)))
                {
                    undoItDetected = true;
                }
                // specifically check for UnifiedUI Continued as that's the mod that removed the built-in support for RAB
                // @see https://github.com/ssv2/Skylines-UnifiedUI/commit/d4bc2ceaf0c91377abe4a5cd1e37d2d307e64e43
                else if (!unifiedUiDetected && current.isEnabled && UNIFIED_UI_IDs.Contains(current.publishedFileID.AsUInt64))
                {
                    Debug.Log("RAB: UnifiedUI Continued detected!");
                    unifiedUiDetected = true;
                }
            }

            //Ads.Destroy();
            //Debug.Log(_string);
        }

        // called when level is loaded
        public void OnLevelLoaded(LoadMode mode)
        {
            //instatiate tools
            if (RoundaboutTool.Instance == null || EllipseTool.Instance == null)
            {
                ToolController toolController = GameObject.FindObjectOfType<ToolController>();

                RoundaboutTool.Instance = toolController.gameObject.AddComponent<RoundaboutTool>();
                RoundaboutTool.Instance.enabled = false;
                EllipseTool.Instance = toolController.gameObject.AddComponent<EllipseTool>();
                EllipseTool.Instance.enabled = false;
                FreeCursorTool.Instance = toolController.gameObject.AddComponent<FreeCursorTool>();
                FreeCursorTool.Instance.enabled = false;
            }

            //instatiate UI
            if (UIWindow.instance == null)
            {
                UIView.GetAView().AddUIComponent(typeof(UIWindow));
            }

            //update msg
            /*if (ShowUpdateMsg())
            {
                UIWindow2.instance.ThrowErrorMsg("Roundabout Builder now supports undo! Yaay! Moreover, building costs are now taken from your account.\n" +
                    "Please report any bugs on the Steam Workshop page.");
            }
            RoundAboutBuilder.SeenUpdateMsg.value = true;*/

            // we detected UUI Continued, register our own button
            if (unifiedUiDetected)
            {
                // add UnifiedUI button
                UIUtil.CreateUUIButton();
                RoundAboutBuilder._isUsingUnifiedUI = true;
            }

            LevelLoaded = true;
        }

        /*private bool ShowUpdateMsg()
        {
            return !RoundAboutBuilder.SeenUpdateMsg && 
        }*/

        /*private void debug()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            //Make an array for the list of assemblies.
            Assembly[] assems = currentDomain.GetAssemblies();

            //List the assemblies in the current application domain.
            Console.WriteLine("List of assemblies loaded in current appdomain:");
            foreach (Assembly assem in assems)
                Debug.Log(assem.ToString());
        }*/

        // called when unloading begins
        public void OnLevelUnloading()
        {
            if (UIWindow.instance != null)
            {
                UIWindow.instance.enabled = false;
            }

            if (unifiedUiDetected)
            {
                UIUtil.RemoveUUIButton();
            }
            
            LevelLoaded = false;
        }

        // called when unloading finished
        public void OnReleased()
        {
        }
    }
}
