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
    public class JutsuEntry : CustomData
    {
        public static JutsuEntry local;
        public GameObject coroutine = new GameObject();
        public CoroutineManager coroutineManager;
        private SpeechRecognitionEngine recognizer;
        internal ItemData logData;
        public GameObject shadow;
        public GameObject shadowSFX;
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
                coroutineManager = coroutine.AddComponent<CoroutineManager>();
                Choices jutsu = new Choices();
                jutsu.Add("Substitution");
                recognizer = new SpeechRecognitionEngine();
                Grammar servicesGrammar = new Grammar(new GrammarBuilder(jutsu));
                recognizer.RequestRecognizerUpdate();
                recognizer.LoadGrammarAsync(servicesGrammar);
                recognizer.SetInputToDefaultAudioDevice();
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                recognizer.SpeechRecognized += Recognizer_SpeechRecognized;
                Application.quitting += () => Process.GetCurrentProcess().Kill();
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