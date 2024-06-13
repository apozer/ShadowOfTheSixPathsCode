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
        public GameObject coroutine = new GameObject();
        public CoroutineManager coroutineManager;
        
        //Speech Recognition Engine object
        private SpeechRecognitionEngine recognizer;
        
        //Gameobject for Substitution Jutsu
        internal ItemData logData;
        
        //GameObjects for Shadow Possession Jutsu
        public GameObject shadow;
        public GameObject shadowSFX;
        
        //GameObjects for Chidori 
        public GameObject chidori;
        public GameObject chidoriStartSFX;
        public GameObject chidoriLoopSFX;
        
        public override void OnCatalogRefresh()
        {
            //Only want one instance of the loader running
            if (local != null) return;
            local = this;
            AsyncSetup();
            
        }

        async void AsyncSetup()
        {
            await Task.Run(() =>
            {
                //Add new component of Coroutine Manager to coroutine manager reference
                coroutineManager = coroutine.AddComponent<CoroutineManager>();
                
                //Setup speech recognition with choice words
                Choices jutsu = new Choices();
                jutsu.Add("Substitution");
                recognizer = new SpeechRecognitionEngine();
                Grammar servicesGrammar = new Grammar(new GrammarBuilder(jutsu));
                recognizer.RequestRecognizerUpdate();
                recognizer.LoadGrammarAsync(servicesGrammar);
                recognizer.SetInputToDefaultAudioDevice();
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
                
                //Prevents game from getting hung up when using speech recognition engine.
                Application.quitting += () => Process.GetCurrentProcess().Kill();
                
                //get GameObjects for VFX or SFX for jutsus
                logData = Catalog.GetData<ItemData>("JutsuLog");
                Catalog.LoadAssetAsync<GameObject>("apoz123.Jutsu.YinRelease.VFX.ShadowPosession",
                    gameobject => { shadow = gameobject;}, "ShadowPossesion");
                Catalog.LoadAssetAsync<GameObject>("apoz123.Jutsu.YinRelease.VFX.ShadowPossessionJutsu.SFX",
                    go => { shadowSFX = go;}, "ShadowPossessionSFX");
                Catalog.LoadAssetAsync<GameObject>("apoz123.LightningStyle.Chidori", go => { chidori = go;}, "ChidoriVFX");
                Catalog.LoadAssetAsync<GameObject>("apoz123.LightningStyle.Chidori.ChidoriStartSFX", go => { chidoriStartSFX = go;}, "ChidoriStartSFX");
                Catalog.LoadAssetAsync<GameObject>("apoz123.LightningStyle.Chidori.ChidoriLoopSFX",
                    go => { chidoriLoopSFX = go;}, "ChidoriLoopSFX");
            });
        }

        private void Recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text != null && e.Result.Confidence > 0.93f)
            {
                Debug.Log("Jutsu Result: " + e.Result.Text);
                Debug.Log("Jutsu Confidence: " + e.Result.Confidence);

                if (e.Result.Text.Equals("Substitution"))
                {
                    if (!Player.currentCreature.gameObject.GetComponent<SubstitutionJutsu>())
                    {
                        Player.currentCreature.gameObject.AddComponent<SubstitutionJutsu>();
                    }
                }

            }
        }
    }
    
    
}