using ICities;
using UnityEngine;

using System;
using System.IO;

using ColossalFramework.UI;

namespace RealisticVehicleSpeed
{
    public class ModInfo : IUserMod
    {
        public string Name
        {
            get { return "Realistic Vehicle Speed " + version; }
        }

        public string Description
        {
            get { return "Slightly randomize the speed of vehicles"; }
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            try
            {
                RealisticVehicleSpeed.LoadConfig();
                Detour.RandomSpeed.highwaySpeed = RealisticVehicleSpeed.config.highwaySpeed;

                UICheckBox highway = null;

                UIHelperBase group = helper.AddGroup(Name);

                highway = (UICheckBox)group.AddCheckbox("Realistic highway speeds", Detour.RandomSpeed.highwaySpeed, (b) =>
                {
                    if (Detour.RandomSpeed.highwaySpeed != b)
                    {
                        Detour.RandomSpeed.highwaySpeed = b;
                        RealisticVehicleSpeed.SaveConfig();
                    }
                });
                highway.tooltip = "On highways, vehicles will go faster in the inside lane and slower in the outside lane\n\nWARNING: It might slow down the simulation";

                //highway.enabled = Detour.RandomSpeed.enabled;
            }
            catch (Exception e)
            {
                DebugUtils.Log("OnSettingsUI failed");
                Debug.LogException(e);
            }
        }

        public const string version = "1.0";
    }
    
    public class RealisticVehicleSpeed : LoadingExtensionBase
    {
        private const string m_fileName = "RealisticVehicleSpeed.xml";
        public static Configuration config = new Configuration();

                
        #region LoadingExtensionBase overrides
        /// <summary>
        /// Called when the level (game, map editor, asset editor) is loaded
        /// </summary>
        public override void OnLevelLoaded(LoadMode mode)
        {
            // Is it an actual game ?
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame) return;

            try
            {
                Detour.RandomSpeed.activated = true;
                Detour.RandomSpeed.enabled = true;
            }
            catch
            {
                DebugUtils.Log("Could not initialize");

                return;
            }
        }

        /// <summary>
        /// Called when the level is unloaded
        /// </summary>
        public override void OnLevelUnloading()
        {
            try
            {
                Detour.RandomSpeed.Restore();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion

        /// <summary>
        /// Load and apply the configuration file
        /// </summary>
        public static void LoadConfig()
        {
            if (File.Exists(m_fileName))
            {
                config.Deserialize(m_fileName);
                DebugUtils.Log("Configuration loaded");
                return;
            }

            // Store modded values
            DebugUtils.Log("Configuration file not found. Creating new configuration file.");

            SaveConfig();
            return;
            
        }

        /// <summary>
        /// Save the configuration file
        /// </summary>
        public static void SaveConfig()
        {
            config.version = ModInfo.version;
            config.highwaySpeed = Detour.RandomSpeed.highwaySpeed;
            config.Serialize(m_fileName);
        }
    }
}
