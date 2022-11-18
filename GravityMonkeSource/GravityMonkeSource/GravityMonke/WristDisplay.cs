using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR;
using TMPLoader;
using TMPro;
using Utilla;
using GorillaLocomotion;

using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

namespace GravityMonke.WD
{
	public class WristDisplay : MonoBehaviour
	{
		public static WristDisplay instance;
		private GorillaLocomotion.Player player = GorillaLocomotion.Player.Instance;
		private bool alreadySetRotation = false;

		private GameObject wristDisplay;

		void Awake()
		{
			instance = this;
			wristDisplay = CreateObject();

			wristDisplay.transform.SetParent(player.leftHandTransform, false);
			wristDisplay.transform.localPosition = new Vector3(-0.075f, 0.13f, 0.047f);
			wristDisplay.transform.Rotate(45,0,0);
			wristDisplay.gameObject.name = "WristDisplay";
			TextMeshPro text = wristDisplay.AddComponent<TextMeshPro>();
		}

		public void SpawnWristObject()
		{
			Instantiate(wristDisplay);
			UpdateText();
		}

		public void DisableObject()
		{
			wristDisplay.transform.localPosition = new Vector3(0, -999, 0);
		}

		public void EnableObject()
		{
			wristDisplay.transform.localPosition = new Vector3(-0.075f, 0.13f, 0.047f);
		}

		GameObject CreateObject()
		{
			GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
			obj.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
			obj.transform.localScale = new Vector3(0.03f, 0.03f, 0.03f);

			Collider col = obj.GetComponent<Collider>();
			if (col != null)
				UnityEngine.Object.Destroy(col);

			return obj;
		}

		public void UpdateText()
		{
			TextMeshPro _text = wristDisplay.GetComponent<TextMeshPro>();
			_text.text = Plugin.gravity.ToString();

			if (!alreadySetRotation)
			{
				wristDisplay.GetComponentInChildren<TMP_SubMesh>().gameObject.transform.Rotate(new Vector3(0, 98, 90));
				wristDisplay.GetComponentInChildren<TMP_SubMesh>().gameObject.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
				alreadySetRotation = true;
			}
		}
	}
}