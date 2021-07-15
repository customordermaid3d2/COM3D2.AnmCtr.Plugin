
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
				var tag = fileName.GetHashCode().ToString();
				//AnmCtrPatch.maids[seleted].CrossFade(fileName, false, true, false, 0f, 1f);
				AnmCtrPatch.maids[seleted].body0.CrossFade(tag, array, false, true, false, 0f, 1f);
				AnmCtrPatch.maids[seleted].GetAnimation().Play();
				AnmCtrPatch.motionTags[seleted] = tag;
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

        internal static void TimeRnd(int seleted)
        {
			var maid = AnmCtrPatch.maids[seleted];
			if (maid == null)
			{
				return;
			}
			// AnmCtrPatch.maids[seleted].GetAnimation().Play();
			// AnmCtrPatch.motionNames[seleted] = fileName;
			var tag = AnmCtrPatch.motionTags[seleted];
            if (tag==null)
            {
				MyLog.LogMessage("AnmCtrUtill.TimeRnd"
				, "nm null"
				);
				return;
            }
			var anm= maid.GetAnimation()[tag];
			if (anm == null)
			{
				MyLog.LogMessage("AnmCtrUtill.TimeRnd"
				, "anm null"
				);
				return;
			}
			anm.time = UnityEngine.Random.Range(0, anm.length);
		}
    }
}
