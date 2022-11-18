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
using Photon.Pun;
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
		private string fileLocation = string.Format("{0}/SaveData", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        private string[] fileArray = new string[3];
		private XRNode rightHandNode = XRNode.RightHand;
		private XRNode leftHandNode = XRNode.LeftHand;

		public Vector3 defaultGravity = Physics.gravity;
		public static bool inRoom;
		public static float gravity;
		public static float resetGravity;
		public static bool Allowed;
		public static bool canSpawn = true;
		protected float _saveGravity;
		protected bool _zeroEnabled;
		private bool canIncreaseGravity = true;
		private bool canDecreaseGravity = true;
		private GorillaLocomotion.Player player = GorillaLocomotion.Player.Instance;
		private bool canSpawnWristObj = true;

		public void Awake()
		{
			new Harmony(PluginInfo.GUID).PatchAll(Assembly.GetExecutingAssembly());
			Zenjector.Install<ComputerInterface.MainInstaller>().OnProject();

			 if (File.Exists(fileLocation))
            {
                fileArray = File.ReadAllText(fileLocation).Split(',');
                Allowed = bool.Parse(fileArray[0]);
                gravity = float.Parse(fileArray[1]);
				resetGravity = float.Parse(fileArray[2]);
            }
            else
            {
                Allowed = true;
                gravity = -9;
				resetGravity = -9;
				fileArray[0] = Allowed.ToString();
                fileArray[1] = gravity.ToString();
				fileArray[2] = resetGravity.ToString();
			}
		}

		void OnGameInitialized()
		{
			if (canSpawnWristObj)
			{
				GravityMonke.WD.WristDisplay.instance.SpawnWristObject();
				canSpawnWristObj = false;
			}
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
					InputDevices.GetDeviceAtXRNode(rightHandNode).TryGetFeatureValue(CommonUsages.secondaryButton, out var rSecondary);
					InputDevices.GetDeviceAtXRNode(leftHandNode).TryGetFeatureValue(CommonUsages.primaryButton, out var lPrimary);
					InputDevices.GetDeviceAtXRNode(leftHandNode).TryGetFeatureValue(CommonUsages.secondaryButton, out var lSecondary);
					InputDevices.GetDeviceAtXRNode(leftHandNode).TryGetFeatureValue(CommonUsages.triggerButton, out var lTrigger);

					if (rPrimary && !_zeroEnabled)
					{
						_saveGravity = Physics.gravity.y;

						gravity = 0f;

						_zeroEnabled = true;

						GravityMonke.WD.WristDisplay.instance.UpdateText();
					}

					if (!rPrimary && _zeroEnabled)
					{
						gravity = _saveGravity;

						_zeroEnabled = false;
						GravityMonke.WD.WristDisplay.instance.UpdateText();
					}

					if (!lPrimary && !canIncreaseGravity)
					{
						canIncreaseGravity = true;
					}

					if (!lSecondary && !canDecreaseGravity)
					{
						canDecreaseGravity = true;
					}

					if (lPrimary && canIncreaseGravity && lTrigger && !_zeroEnabled)
					{
						canIncreaseGravity = false;

						gravity = gravity + 1f;
						GravityMonke.ComputerInterface.GravityView.instance.gravity = GravityMonke.ComputerInterface.GravityView.instance.gravity + 1f;
						GravityMonke.WD.WristDisplay.instance.UpdateText();
						GravityMonke.ComputerInterface.GravityView.instance.UpdateScreen();

						fileArray[1] = gravity.ToString();
					}

					if (rSecondary && lTrigger && !_zeroEnabled)
					{
						gravity = resetGravity;
						GravityMonke.ComputerInterface.GravityView.instance.gravity = resetGravity;
						GravityMonke.WD.WristDisplay.instance.UpdateText();
						GravityMonke.ComputerInterface.GravityView.instance.UpdateScreen();

						fileArray[1] = gravity.ToString();
					}

					if (lSecondary && canDecreaseGravity && lTrigger && !_zeroEnabled)
					{
						canDecreaseGravity = false;

						gravity = gravity - 1f;
						GravityMonke.ComputerInterface.GravityView.instance.gravity = GravityMonke.ComputerInterface.GravityView.instance.gravity - 1f;
						GravityMonke.WD.WristDisplay.instance.UpdateText();
						GravityMonke.ComputerInterface.GravityView.instance.UpdateScreen();

						fileArray[1] = gravity.ToString();
					}

					if (lTrigger)
					{
						GravityMonke.WD.WristDisplay.instance.EnableObject();
						GravityMonke.WD.WristDisplay.instance.UpdateText();
					}

					if (!lTrigger)
					{
						GravityMonke.WD.WristDisplay.instance.DisableObject();
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

		GameObject CreateObject()
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
            obj.transform.localScale = new Vector3(0.05f, 0.05f, 0.035f);

			Collider col = obj.GetComponent<Collider>();
			if (col != null)
				UnityEngine.Object.Destroy(col);

            return obj;
        }

		/* This attribute tells Utilla to call this method when a modded room is joined */
		[ModdedGamemodeJoin]
		public void OnJoin(string gamemode)
		{
			/* Activate your mod here */
			/* This code will run regardless of if the mod is enabled*/

			inRoom = true;

			fileArray[0] = Allowed.ToString();
            File.WriteAllText(fileLocation, string.Join(",", fileArray));
			fileArray[1] = gravity.ToString();
            File.WriteAllText(fileLocation, string.Join(",", fileArray));
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
