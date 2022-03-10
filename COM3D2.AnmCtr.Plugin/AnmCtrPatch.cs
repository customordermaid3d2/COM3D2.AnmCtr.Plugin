using HarmonyLib;
using LillyUtill.MyMaidActive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace COM3D2.AnmCtr.Plugin
{
    class AnmCtrPatch
    {
        public static string[] motionTags = new string[18];
        
        public static void maidCntChg(int slot)
        {
            Array.Resize(ref motionTags, slot);            
        }

        /// <summary>
        /// 메이드가 슬롯에서 빠졌을때
        /// </summary>
        /// <param name="f_nActiveSlotNo"></param>
        /// <param name="f_bMan"></param>
        [HarmonyPatch(typeof(CharacterMgr), "Deactivate")]
        [HarmonyPrefix] // CharacterMgr의 SetActive가 실행 전에 아래 메소드 작동
        public static void Deactivate(int f_nActiveSlotNo, bool f_bMan)
        {
            if (!f_bMan&& motionTags.Length> f_nActiveSlotNo)
            {
                //maids[f_nActiveSlotNo] = null;
                //maidNames[f_nActiveSlotNo] = string.Empty;
                motionTags[f_nActiveSlotNo] = null;
            }
            //AnmCtr.log.LogMessage("CharacterMgr.Deactivate", f_nActiveSlotNo, f_bMan);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(TBody), "CrossFade", typeof(string), typeof(AFileSystemBase), typeof(bool), typeof(bool), typeof(bool), typeof(float), typeof(float))]
        public static void CrossFade(TBody __instance, string filename, AFileSystemBase fileSystem, bool additive = false, bool loop = false, bool boAddQue = false, float fade = 0.5f, float weight = 1f)
        {

            int i = MaidActiveUtill.maids.ToList().IndexOf(__instance.maid);
            if (i >= 0&& motionTags.Length > i)
            {
                motionTags[i] = filename;
            }

        }

        [HarmonyPostfix, HarmonyPatch(typeof(TBody), "CrossFade", typeof(string), typeof(byte[]), typeof(bool), typeof(bool), typeof(bool), typeof(float), typeof(float))]
        public static void CrossFade(TBody __instance, string tag, byte[] byte_data, bool additive = false, bool loop = false, bool boAddQue = false, float fade = 0.5f, float weight = 1f)
        {
            //if (config["CrossFade", false])
            //AnmCtr.log.LogMessage("TBody.CrossFade2"
            //, tag
            //, additive
            //, loop
            //, boAddQue
            //, fade
            //, weight
            //);
            int i = MaidActiveUtill.maids.ToList().IndexOf(__instance.maid);
            if (i >= 0 && motionTags.Length > i)
            {
                motionTags[i] = tag;
            }
        }
    }
}
