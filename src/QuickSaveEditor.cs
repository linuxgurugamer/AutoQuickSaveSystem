﻿/* 
QuickStart
Copyright 2017 Malah

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>. 
*/

using System;
using System.Collections;
using System.Collections.Generic;
using KSP.Localization;
using UnityEngine;

using static AutoQuickSaveSystem.AutoQuickSaveSystem;

namespace AutoQuickSaveSystem
{
	[KSPAddon(KSPAddon.Startup.EditorAny, false)]
	public class QuickSaveEditor:MonoBehaviour
	{

		public static readonly string shipFilename = "Auto-Saved Ship";

		public static string shipPath
		{
			get
			{
				return string.Format("{0}/Ships/{1}/{2}.craft", HighLogic.SaveFolder, ShipConstruction.GetShipsSubfolderFor(EditorDriver.editorFacility), shipFilename);
			}
		}


		void Start()
		{
			if (Configuration.saveVesselInEditor)
				StartCoroutine("AutoSaveShip");
		}

		IEnumerator AutoSaveShip()
		{
			Log.Info("Starting Coroutein AutoSaveShip");
			while (true)
			{
				yield return new WaitForSeconds(Configuration.editorTimeIntervalToSave);
				Log.Info("AutoSaveShip, after: "+ Configuration.editorTimeIntervalToSave);
				List<Part> parts = EditorLogic.fetch.ship != null ? EditorLogic.fetch.ship.Parts : new List<Part>();

				if (parts.Count > 0)
				{
					Log.Info("AutoSaveShip, parts.Count: " + parts.Count);
					ShipConstruction.SaveShip(shipFilename);
				}
			}
		}

		void OnDestroy()
		{
			Log.Info("OnDestroy");
			StopCoroutine("AutoSaveShip");
		}
	}
}

