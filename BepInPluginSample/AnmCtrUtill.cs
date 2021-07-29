
using COM3D2.LillyUtill;
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
            if (MaidActivePatch.maids[seleted]==null)
            {
				return;
            }
			if (0 < array.Length)
			{
				var tag = fileName.GetHashCode().ToString();
				//AnmCtrPatch.maids[seleted].CrossFade(fileName, false, true, false, 0f, 1f);
				MaidActivePatch.maids[seleted].body0.CrossFade(tag, array, false, true, false, 0f, 1f);
				MaidActivePatch.maids[seleted].GetAnimation().Play();
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
				AnmCtr.log.LogMessage("AnmCtrUtill.Load"
					, e.ToString()
					);
			}
			return array;
		}

        internal static void TimeRnd(int seleted)
        {
			AnimationState anm = GetAnm(seleted);
			if (anm == null)
			{
				AnmCtr.log.LogMessage("AnmCtrUtill.TimeRnd"
				, "anm null"
				);
				return;
			}
			anm.time = UnityEngine.Random.Range(0, anm.length);
		}

		internal static void TimeReset(int seleted)
		{
			AnimationState anm = GetAnm(seleted);
			if (anm == null)
			{
				AnmCtr.log.LogMessage("AnmCtrUtill.TimeRnd"
				, "anm null"
				);
				return;
			}
			anm.time = 0;
		}

		internal static AnimationState GetAnm(int seleted)
        {
			var maid = MaidActivePatch.maids[seleted];
			if (maid == null)
			{
				return null;
			}
			// AnmCtrPatch.maids[seleted].GetAnimation().Play();
			// AnmCtrPatch.motionNames[seleted] = fileName;
			var tag = AnmCtrPatch.motionTags[seleted];
			if (tag == null)
			{
				AnmCtr.log.LogMessage("AnmCtrUtill.TimeRnd"
				, "nm null"
				);
				return null;
			}
			AnimationState anm = maid.GetAnimation()[tag];
			if (anm == null)
			{
				AnmCtr.log.LogMessage("AnmCtrUtill.TimeRnd"
				, "anm null"
				);
				return null;
			}
			return anm;
		}





		internal static void seletedCopy(int seleted)
		{
			//AnimationClip clip = animation.GetClip(tag);
			AnimationState anm = GetAnm(seleted);
			if (anm == null)
			{
				AnmCtr.log.LogMessage("AnmCtrUtill.seletedCopy"
				, "anm null"
				);
				return;
			}
			//AnmCtrPatch.motionTags[seleted];

			Animation animation = MaidActivePatch.maids[seleted].GetAnimation();
			AnimationClip clip = animation.GetClip(AnmCtrPatch.motionTags[seleted]);
			Maid maid;
			var tag = AnmCtrPatch.motionTags[seleted];

			for (int i = 0; i < 18; i++)
			{
				maid = MaidActivePatch.maids[i];
				if (maid == null)
				{
					continue;
				}
				animation = maid.GetAnimation();
				if (animation == null)
				{
					continue;
				}
				//AnimationClip clip = animation.GetClip(tag);
				animation.AddClip(clip, tag);
				animation.Play();
				anm = animation[tag];
				anm.blendMode = AnimationBlendMode.Blend;
				anm.wrapMode = WrapMode.Loop;
				anm.speed = 1f;
				anm.time = 0f;
				anm.weight = 0f;
				anm.enabled = true;
				AnmCtr.log.LogMessage("AnmCtrUtill.seletedCopy"
				, maid.status.fullNameEnStyle
				);
			}

		}


	}
}
