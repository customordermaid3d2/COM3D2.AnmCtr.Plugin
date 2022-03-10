using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using COM3D2API;
using HarmonyLib;
using LillyUtill.MyMaidActive;
using LillyUtill.MyWindowRect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace COM3D2.AnmCtr.Plugin
{
    class MyAttribute
    {
        public const string PLAGIN_NAME = "AnmCtr";
        public const string PLAGIN_VERSION = "22.3.10";
        public const string PLAGIN_FULL_NAME = "COM3D2.AnmCtr.Plugin";
    }

    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임  
    [BepInProcess("COM3D2x64.exe")]
    public class AnmCtr : BaseUnityPlugin
    {

        Harmony harmony;

        internal static ManualLogSource log;

        public WindowRectUtill myWindowRect;
        Vector2 scrollPosition;

        string[] type = new string[] { "one", "all" };

        public static System.Windows.Forms.OpenFileDialog openDialog;
        public static System.Windows.Forms.SaveFileDialog saveDialog;

        private int seleted;

        private static ConfigEntry<int> option;

        public int Option
        {
            get => option.Value;
            set => option.Value = value;
        }

        public void Awake()
        {
            log = Logger;

            AnmCtr.log.LogMessage("Awake");
            AnmCtr.log.LogMessage("https://github.com/customordermaid3d2/COM3D2.AnmCtr.Plugin");

            myWindowRect = new WindowRectUtill(Config, log, MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, "AC2");
            option = Config.Bind("GUI", "all", 0); // 이건 베핀 설정값 지정용

            // 파일 열기창 설정 부분. 이런건 구글링 하기
            openDialog = new System.Windows.Forms.OpenFileDialog()
            {
                // 기본 확장자
                DefaultExt = "anm",
                // 기본 디렉토리
                InitialDirectory = Path.Combine(UTY.gameProjectPath, @"PhotoModeData\MyPose"),
                // 선택 가능 확장자
                Filter = "anm files (*.anm)|*.anm|All files (*.*)|*.*"
            };
            saveDialog = new System.Windows.Forms.SaveFileDialog()
            {
                DefaultExt = "anm",
                InitialDirectory = Path.Combine(UTY.gameProjectPath, @"PhotoModeData\MyPose"),
                Filter = "anm files (*.anm)|*.anm|All files (*.*)|*.*"
            };
        }

        public void OnEnable()
        {
            AnmCtr.log.LogMessage("OnEnable");

            //SceneManager.sceneLoaded += this.OnSceneLoaded;

            // 하모니 패치
            // 이게 게임 원래 메소들을 해킹해서 값을 바꿔주게 해주는 역활
            harmony = Harmony.CreateAndPatchAll(typeof(AnmCtrPatch));

            MaidActiveUtill.maidCntChg += AnmCtrPatch.maidCntChg;

        }

        public void Start()
        {
            AnmCtr.log.LogMessage("Start");

            SystemShortcutAPI.AddButton(
                MyAttribute.PLAGIN_FULL_NAME
                , new Action(delegate ()
                { // 기어메뉴 아이콘 클릭시 작동할 기능
                                myWindowRect.IsGUIOn = !myWindowRect.IsGUIOn;
                })
                , MyAttribute.PLAGIN_FULL_NAME // 표시될 툴팁 내용                               
            , Properties.Resources.icon);// 표시될 아이콘

        }

        private void OnGUI()
        {
            if (!myWindowRect.IsGUIOn)
                return;

            //GUI.skin.window = ;

            //myWindowRect.WindowRect = GUILayout.Window(windowId, myWindowRect.WindowRect, WindowFunction, MyAttribute.PLAGIN_NAME + " " + ShowCounter.Value.ToString(), GUI.skin.box);
            // 별도 창을 띄우고 WindowFunction 를 실행함. 이건 스킨 설정 부분인데 따로 공부할것
            myWindowRect.WindowRect = GUILayout.Window(myWindowRect.winNum, myWindowRect.WindowRect, WindowFunction, "", GUI.skin.box);
        }

        public void WindowFunction(int id)
        {
            GUI.enabled = true; // 기능 클릭 가능

            GUILayout.BeginHorizontal();
            GUILayout.Label(myWindowRect.windowName, GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("-", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsOpen = !myWindowRect.IsOpen; }
            if (GUILayout.Button("x", GUILayout.Width(20), GUILayout.Height(20))) { myWindowRect.IsGUIOn = false; }
            GUILayout.EndHorizontal();// 가로 정렬 끝

            if (!myWindowRect.IsOpen)
            {

            }
            else
            {
                // 스크롤 영역
                scrollPosition = GUILayout.BeginScrollView(scrollPosition);

                if (GUI.enabled)
                {
                    GUILayout.Label("");
                }
                else
                {
                    GUILayout.Label("maid null");
                }

                GUILayout.Label("file set");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("load"))
                {
                    if (openDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)// 오픈했을때
                    {
                        byte[] array = AnmCtrUtill.Load(openDialog.FileName);
                        seletedRun(AnmCtrUtill.Load, openDialog.FileName, array);
                    }
                }
                GUILayout.EndHorizontal();

                {
                    GUILayout.Label("Time");
                    GUILayout.BeginHorizontal();

                    if (GUILayout.Button("Time Reset"))
                    {
                        seletedRun(AnmCtrUtill.TimeReset);
                    }

                    if (GUILayout.Button("Time Rnd"))
                    {
                        seletedRun(AnmCtrUtill.TimeRnd);
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.Label("option");

                Option = GUILayout.SelectionGrid(Option, type, 2);
      
                seleted = MaidActiveUtill.SelectionGrid(seleted);

                GUILayout.EndScrollView();
            }
            GUI.enabled = true;
            GUI.DragWindow(); // 창 드레그 가능하게 해줌. 마지막에만 넣어야함
        }


        public void OnDisable()
        {
            AnmCtr.log.LogMessage("OnDisable");

           // SceneManager.sceneLoaded -= this.OnSceneLoaded;

            harmony.UnpatchSelf();// ==harmony.UnpatchAll(harmony.Id);
        }


        public void seletedRun(Action<int> action)
        {
            if (Option == 0)
            {
                action(seleted);
            }
            else
            {
                for (int i = 0; i < 18; i++)
                {
                    action(i);
                }
            }
        }

        public void seletedRun<T1>(Action<int, T1> action, T1 t1)
        {
            if (Option == 0)
            {
                action(seleted, t1);
            }
            else
            {
                for (int i = 0; i < 18; i++)
                {
                    action(i, t1);
                }
            }
        }

        public void seletedRun<T1, T2>(Action<int, T1, T2> action, T1 t1, T2 t2)
        {
            if (Option == 0)
            {
                action(seleted, t1, t2);
            }
            else
            {
                for (int i = 0; i < 18; i++)
                {
                    action(i, t1, t2);
                }
            }
        }


    }
}
