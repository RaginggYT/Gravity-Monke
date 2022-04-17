using System;
using System.IO;
using System.ComponentModel;
using BepInEx;
using Bepinject;
using BepInEx.Configuration;
using GravityMonke;
using UnityEngine;
using System.Text;
using Utilla;
using UnityEngine.UI;
using System.Reflection;
using HarmonyLib;
using UnityEngine.XR;
using System.Collections.Generic;
using System.Collections;

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
		private XRNode rightHandNode = XRNode.RightHand;

		public Vector3 defaultGravity = Physics.gravity;
		public static bool inRoom;
		public static float gravity;
		public static bool Allowed;
		public static bool canSpawn = true;
		public static int gorillaLayer;
		protected float _saveGravity;
		protected bool _zeroEnabled;

		public void Awake()
		{
			new Harmony(PluginInfo.GUID).PatchAll(Assembly.GetExecutingAssembly());
			Zenjector.Install<ComputerInterface.MainInstaller>().OnProject();
		}
		void OnEnable() {
			/* Set up your mod here */
			/* Code here runs at the start and whenever your mod is enabled*/

			HarmonyPatches.ApplyHarmonyPatches();
			Allowed = true;
		}

		void OnDisable() {
			/* Undo mod setup here */
			/* This provides support for toggling mods with ComputerInterface, please implement it :) */
			/* Code here runs whenever your mod is disabled (including if it disabled on startup)*/

			Physics.gravity = defaultGravity;
			HarmonyPatches.RemoveHarmonyPatches();
			Allowed = false;
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
				if (Allowed)
				{
					Physics.gravity = new Vector3(Physics.gravity.x, gravity, Physics.gravity.z);

					InputDevices.GetDeviceAtXRNode(rightHandNode).TryGetFeatureValue(CommonUsages.primaryButton, out var rPrimary);

					if (rPrimary && !_zeroEnabled)
					{
						_saveGravity = Physics.gravity.y;

						gravity = 0f;

						_zeroEnabled = true;
					}

					if (!rPrimary && _zeroEnabled)
					{
						gravity = _saveGravity;

						_zeroEnabled = false;
					}
				}
			}
			if (inRoom == false)
			{
				Physics.gravity = defaultGravity;

				_zeroEnabled = false;
			}

			if (!Allowed)
			{
				Physics.gravity = defaultGravity;

				_zeroEnabled = false;
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
