using System;
using System.IO;
using System.ComponentModel;
using BepInEx;
using BepInEx.Configuration;
using GravityMonke;
using UnityEngine;
using Utilla;


namespace GravityMonke
{
	/// <summary>
	/// This is your mod's main class.
	/// </summary>

	/* This attribute tells Utilla to look for [ModdedGameJoin] and [ModdedGameLeave] */
	[ModdedGamemode]
	[Description("HauntedModMenu")]
	[BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
	[BepInDependency("org.legoandmars.gorillatag.utilla", "1.6.0")]
	public class Plugin : BaseUnityPlugin
	{

		public Vector3 defaultGravity = Physics.gravity;
		public static bool inRoom;
		public static ConfigEntry<float> gravity;
		public static bool Allowed => inRoom;

		void OnEnable() {
			/* Set up your mod here */
			/* Code here runs at the start and whenever your mod is enabled*/

			HarmonyPatches.ApplyHarmonyPatches();

			var customFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "GravityMonke.cfg"), true);
			gravity = customFile.Bind("Configuration", "Gravity", -9.8f, "How much gravity do you want? Default is -9.8");
		}

		void OnDisable() {
			/* Undo mod setup here */
			/* This provides support for toggling mods with ComputerInterface, please implement it :) */
			/* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

			Physics.gravity = defaultGravity;
			HarmonyPatches.RemoveHarmonyPatches();
		}

		void OnGameInitialized(object sender, EventArgs e)
		{
			/* Code here runs after the game initializes (i.e. GorillaLocomotion.Player.Instance != null) */
			Physics.gravity = defaultGravity;
		}

		void Update()
		{
			/* Code here runs every frame when the mod is enabled */
			if (inRoom) 
			{

				Physics.gravity = new Vector3(Physics.gravity.x, gravity.Value, Physics.gravity.z);
					
			}
			if (inRoom == false) 
			{
				Physics.gravity = defaultGravity;
			}

		}

		/* This attribute tells Utilla to call this method when a modded room is joined */
		[ModdedGamemodeJoin]
		public void OnJoin(string gamemode)
		{
			/* Activate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = true;
		}

		/* This attribute tells Utilla to call this method when a modded room is left */
		[ModdedGamemodeLeave]
		public void OnLeave(string gamemode)
		{
			/* Deactivate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = false;
		}
	}
}
