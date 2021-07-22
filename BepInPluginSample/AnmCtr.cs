using BepInEx;
using BepInEx.Configuration;
using COM3D2.LillyUtill;
using COM3D2API;
using HarmonyLib;
using Newtonsoft.Json;
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
    [BepInPlugin(MyAttribute.PLAGIN_FULL_NAME, MyAttribute.PLAGIN_NAME, MyAttribute.PLAGIN_VERSION)]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    //[BepInPlugin("COM3D2.Sample.Plugin", "COM3D2.Sample.Plugin", "21.6.6")]// 버전 규칙 잇음. 반드시 2~4개의 숫자구성으로 해야함. 미준수시 못읽어들임
    [BepInProcess("COM3D2x64.exe")]
    public class AnmCtr : BaseUnityPlugin
    {
        // 단축키 설정파일로 연동
        //private ConfigEntry<BepInEx.Configuration.KeyboardShortcut> ShowCounter;

        Harmony harmony;

        public static AnmCtr sample;
        public static MyLog log=new MyLog(MyAttribute.PLAGIN_NAME);

        /// <summary>
        /// 0.
        /// 플러그인 로딩시 이부분이 가장 먼저 실행됨
        /// </summary>
        public AnmCtr()
        {
            sample = this;
        }


        /// <summary>
        /// 2. 그다음 플러그인을 켰을때 작동되는 함수
        /// 나중에 플러그인 끄고 다시 켰을때 이함수가 다시 작동되니 여기에 뭘 넣을지는 잘 생각해야함
        /// </summary>
        public void OnEnable()
        {
            AnmCtr.log.LogMessage("OnEnable");

            SceneManager.sceneLoaded += this.OnSceneLoaded;

            // 하모니 패치
            // 이게 게임 원래 메소들을 해킹해서 값을 바꿔주게 해주는 역활
            harmony = Harmony.CreateAndPatchAll(typeof(AnmCtrPatch));

        }

        /// <summary>
        /// 3. OnEnable 실행된후에 한번만 실행
        /// OnEnable 이 몇번이나 실행되도 이건 플러그인 로딩 후에 한번만 실행되고 두번다시 실행 안됨
        /// 보통 이게 작동되는 시점은 모든 플러그인이 초기화 되고 데이터를 다 불러온 후에 작동함
        /// Awake 가 게임 킨 직후라면
        /// Start 는 데이터를 다 불러온 후
        /// 게임 실행시 한번만 실행됨
        /// </summary>
        public void Start()
        {
            AnmCtr.log.LogMessage("Start");

            // 어쨋든 기본 유니티 앤진 구조는 이러함
            // 그리고 유니티 앤진 자체가 오브젝트 안에 오브젝트를 추가할수 있음
            // 이게 그걸 이용한 방식
            // 여기 안으로 들어가 보면
            AnmCtrGUI.Install(gameObject, Config);

            //SystemShortcutAPI.AddButton(MyAttribute.PLAGIN_FULL_NAME, new Action(delegate () { enabled = !enabled; }), MyAttribute.PLAGIN_NAME, MyUtill.ExtractResource(COM3D2.PresetExpresetXmlLoader.Plugin.Properties.Resources.icon));
        }

        public string scene_name = string.Empty;

        /// <summary>
        /// 이건 장면이 바귀었을때마다 작동할 코드 작성
        /// 보통 OnEnable 에다가 += this.OnSceneLoaded; 를 해주고
        /// OnDisable 에다가 -= this.OnSceneLoaded; 를 해줌
        /// 어지간하면 scene.buildIndex 는 쓰지 말자
        /// scene.name 를 쓰자
        /// buildIndex 는 게임 버전 바귈때마다 번호가 바귈 위험성이 매우큼
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="mode"></param>
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            AnmCtr.log.LogMessage("OnSceneLoaded", scene.name, scene.buildIndex);
            //  scene.buildIndex 는 쓰지 말자 제발
            scene_name = scene.name;
        }

        public void FixedUpdate()
        {

        }

        /// <summary>
        /// 여기는 게임 로직 부분
        /// </summary>
        public void Update()
        {
            //if (ShowCounter.Value.IsDown())
            //{
            //    MyLog.LogMessage("IsDown", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            //if (ShowCounter.Value.IsPressed())
            //{
            //    MyLog.LogMessage("IsPressed", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
            //if (ShowCounter.Value.IsUp())
            //{
            //    MyLog.LogMessage("IsUp", ShowCounter.Value.Modifiers, ShowCounter.Value.MainKey);
            //}
        }

        /// <summary>
        /// 여기도 마찬가지
        /// </summary>
        public void LateUpdate()
        {

        }

        
        /// <summary>
        /// 여기가 GUI 작성 부분
        /// 지금 이 플러그인은 약간 특이하게 다른족에서 GUI 불러들임
        /// 자세한건 위에 다시 설명해줌
        /// </summary>
        public void OnGUI()
        {
          
        }


        /// <summary>
        /// 플러그인 껏을때 작동
        /// 게임에서 F1 누르면 나오는 베핀플러그인 메니저 띄우면
        /// 활성 비활성 시켜주는 옵션도 있는데 내가 이 플러그인은 그걸 추가 안했네..
        /// </summary>
        public void OnDisable()
        {
            AnmCtr.log.LogMessage("OnDisable");

            SceneManager.sceneLoaded -= this.OnSceneLoaded;

            harmony.UnpatchSelf();// ==harmony.UnpatchAll(harmony.Id);
            //harmony.UnpatchAll(); // 정대 사용 금지. 다름 플러그인이 패치한것까지 다 풀려버림
        }

    }
}
