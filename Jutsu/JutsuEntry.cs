using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Speech.Recognition;
using System.Threading.Tasks;
using ThunderRoad;
using UnityEngine;
using static UnityEngine.AddressableAssets.Addressables;
using Debug = UnityEngine.Debug;

namespace Jutsu
{
    /**
     * Jutsu Entry - Entry point to the Jutsu mod
     * CustomData class from ThunderRoad
     * Use this to import VFX GameObjects, or for more complex things such as voice activation
     */
    public class JutsuEntry : CustomData
    {
        //static reference for Singleton structure
        public static JutsuEntry local;
        
        //Reference to global coroutine manager (Useful for non monbehaviour classes)
        //public GameObject coroutine = new GameObject();
        //public CoroutineManager coroutineManager;
        public SDFBakeTool bakeTool; // = new GameObject().AddComponent<SDFBakeTool>();*/
        
        //Speech Recognition Engine object
        //private SpeechRecognitionEngine recognizer;
        
        //Gameobject for Substitution Jutsu
        internal ItemData logData;

        internal ItemData chidoriItemData;
        
        //GameObjects for Shadow Possession Jutsu
        public GameObject shadow;
        public GameObject shadowSFX;
        
        //GameObjects for Chidori 
        //public GameObject chidori;
        public GameObject chidoriStartSFX;
        public GameObject chidoriLoopSFX;
        //VFX for Vacuum Blade
        public GameObject vacuumBlade;
        public GameObject debugObject;
        public override void OnCatalogRefresh()
        {
            //Only want one instance of the loader running
            if (local != null) return;
            local = this;
            AsyncSetup();
            
        }

        public override IEnumerator LoadAddressableAssetsCoroutine()
        {
            AsyncSetup();
            return base.LoadAddressableAssetsCoroutine();
            
        }

        async void AsyncSetup()
        {
            await Task.Run(() =>
            {
                //coroutineManager = coroutine.AddComponent<CoroutineManager>();
                //Add new component of Coroutine Manager to coroutine manager reference
                //Prevents game from getting hung up when using speech recognition engine.
                Application.quitting += () => Process.GetCurrentProcess().Kill();
                
                //get GameObjects for VFX or SFX for jutsus
                /*logData = Catalog.GetData<ItemData>("JutsuLog");
                Catalog.LoadAssetAsync<GameObject>("apoz123.Jutsu.YinRelease.VFX.ShadowPosession",
                    gameobject => { shadow = gameobject;}, "ShadowPossesion");
                Catalog.LoadAssetAsync<GameObject>("apoz123.Jutsu.YinRelease.VFX.ShadowPossessionJutsu.SFX",
                    go => { shadowSFX = go;}, "ShadowPossessionSFX");*/
                //Catalog.LoadAssetAsync<GameObject>("apoz123.LightningStyle.Chidori", go => { chidori = go;}, "ChidoriVFX");
                GameManager.local.StartCoroutine(Catalog.LoadAddressableAssetsCoroutine());
               Catalog.LoadAssetAsync<GameObject>("SOTSP.Jutsu.LightningRelease.Chidori.SFX.start", go => { chidoriStartSFX = go;}, "ChidoriStartSFX");
                Catalog.LoadAssetAsync<GameObject>("SOTSP.Jutsu.LightningRelease.Chidori.SFX.loop",
                    go => { chidoriLoopSFX = go;}, "ChidoriLoopSFX");

               // GameManager.local.StartCoroutine(loadAssetCoroutine);
                //GameManager.local.StartCoroutine(assetCoroutine);
                /*Catalog.LoadAssetAsync<GameObject>("apoz123.Jutsu.WindRelease.VFX.VacuumBlade", data =>{this.vacuumBlade = data;}, "VacuumBladeVFX");*/
                /*Catalog.LoadAssetAsync<GameObject>("debugObject", data =>{this.debugObject = data;}, "DebugObject");*/
            });
        }
    }
    
    
}