using BepInEx.Configuration;
using HarmonyLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BepInPluginSample
{
    public class MyWindowRect
    {
        private float windowSpace;
        private Rect windowRect;
        private Size windowRectOpen;
        private Size windowRectClose;
        private Position position;
        private string jsonPath;

        private static Harmony harmony;
        private static event Action actionSave;

        private ConfigEntry<bool> isOpen;

        /// <summary>
        /// 참이 열렸다 닫혔을때 창 위치랑 크기 조정
        /// </summary>
        public bool IsOpen
        {
            get => isOpen.Value;
            set
            {
                if (isOpen.Value = value)// 펼쳤을때
                {
                    windowRect.width = windowRectOpen.w;
                    windowRect.height = windowRectOpen.h;
                    windowRect.x -= windowRectOpen.w - windowRectClose.w;
                }
                else // 닫혔을때
                {
                    windowRect.width = windowRectClose.w;
                    windowRect.height = windowRectClose.h;
                    windowRect.x += windowRectOpen.w - windowRectClose.w;
                }
            }
        }

        struct Position
        {
            public float x;
            public float y;

            public Position(float x, float y) : this()
            {
                this.x = x;
                this.y = y;
            }
        } // 설정파일 저장용 규격. 이게 GUI 위치값 저장

        struct Size
        {
            public float w;
            public float h;

            public Size(float w, float h) : this()
            {
                this.w = w;
                this.h = h;
            }
        }

        public Rect WindowRect
        {
            get
            {
                // 윈도우 리사이즈시 밖으로 나가버리는거 방지
                windowRect.x = Mathf.Clamp(windowRect.x, -windowRect.width + windowSpace, Screen.width - windowSpace);
                windowRect.y = Mathf.Clamp(windowRect.y, -windowRect.height + windowSpace, Screen.height - windowSpace);
                return windowRect;
            }
            set => windowRect = value;
        }

        public float Height { get => windowRect.height; set => windowRect.height = value; }
        public float Width { get => windowRect.width; set => windowRect.width = value; }
        public float X { get => windowRect.x; set => windowRect.x = value; }
        public float Y { get => windowRect.y; set => windowRect.y = value; }

        /// <summary>
        /// 베핀 설정 파일 기능 이용
        /// </summary>
        /// <param name="config">베핀설정 파일</param>
        /// <param name="fileName"></param>
        /// <param name="wc"></param>
        /// <param name="wo"></param>
        /// <param name="hc"></param>
        /// <param name="ho"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="windowSpace"></param>
        public MyWindowRect(ConfigFile config, string fileName = "", float wc = 200f, float wo = 300f, float hc = 32f, float ho = 600f, float x = 32f, float y = 32f, float windowSpace = 32f)
        {
            // json으로 저장될 파일명
            jsonPath = Path.GetDirectoryName(config.ConfigFilePath) + $@"\{fileName}-windowRect.json";

            this.windowSpace = windowSpace;
            windowRect = new Rect(x, y, wo, ho);
            windowRectOpen = new Size(wo, ho);
            windowRectClose = new Size(wc, hc);
            isOpen = config.Bind("GUI", "isOpen", true);
            IsOpen = isOpen.Value;

            if (harmony == null)
            {
                harmony = Harmony.CreateAndPatchAll(typeof(MyWindowRect));
            }
            actionSave += save;// 최초 생성시 저장
        }

        public void load()
        {
            if (File.Exists(jsonPath))
            {
                position = JsonConvert.DeserializeObject<Position>(File.ReadAllText(jsonPath));
            }
            else
            {
                position = new Position(windowSpace, windowSpace);
                File.WriteAllText(jsonPath, JsonConvert.SerializeObject(position, Formatting.Indented)); // 자동 들여쓰기
            }
            windowRect.x = position.x;
            windowRect.y = position.y;
        }

        /// <summary>
        /// 이게 창위치를 저장하게 해주는 함수
        /// </summary>
        public void save()
        {
            position.x = windowRect.x;
            position.y = windowRect.y;
            File.WriteAllText(jsonPath, JsonConvert.SerializeObject(position, Formatting.Indented)); // 자동 들여쓰기
        }

        [HarmonyPatch(typeof(GameMain), "LoadScene")]
        [HarmonyPostfix]
        public static void LoadScene()
        {
            actionSave();
        }
    }
}
