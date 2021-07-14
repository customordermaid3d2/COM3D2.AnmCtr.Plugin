
using COM3D2.Lilly.Plugin;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.AnmCtr.Plugin
{
    public class AnmCtrUtill
    {

        internal static void Load(int seleted, string fileName, byte[] array)
        {
            if (AnmCtrPatch.maids[seleted]==null)
            {
				return;
            }
			if (0 < array.Length)
			{
				//AnmCtrPatch.maids[seleted].CrossFade(fileName, false, true, false, 0f, 1f);
				AnmCtrPatch.maids[seleted].body0.CrossFade(fileName.GetHashCode().ToString(), array, false, true, false, 0f, 1f);
				AnmCtrPatch.maids[seleted].GetAnimation().Play();
			}           
        }

		internal static byte[] Load(string fileName)
		{
			//public string CrossFade(string fn, bool additive = false, bool loop = false, bool boAddQue = false, float val = 0.5f, float weight = 1f)
			byte[] array = new byte[0];
			try
			{
				using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
				{
					array = new byte[fileStream.Length];
					fileStream.Read(array, 0, array.Length);
				}
			}
			catch (Exception e)
			{
				MyLog.LogMessage("AnmCtrUtill.Load"
					, e.ToString()
					);
			}
			return array;
		}

	}
}
