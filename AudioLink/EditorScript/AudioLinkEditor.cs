using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

using ABI.CCK.Components;

using UnityEngine.Rendering;


namespace AudioLink {
    public class AudioLinkEditor : EditorWindow {
        public CVRAudioMaterialParser audioMaterialParser;
        public AudioSource audioSource;
        public string microphoneName = " --- select microphone --- ";


        public void OnGUI() {
            int total_height = 2;
            int margin_bottom = 2;
            int margin_left = 10;
            Rect R(int height) {
                // Add a row and return a new Rect
                float y = total_height;
                total_height += height;
                total_height += margin_bottom;
                return new Rect(margin_left, y, position.width - margin_left, height);
            }

            R(0);
            audioMaterialParser = EditorGUI.ObjectField(R(20), "Controller Sync", audioMaterialParser, typeof(CVRAudioMaterialParser), true) as CVRAudioMaterialParser;
            if (!audioMaterialParser) {
                audioMaterialParser = GameObject.FindObjectOfType<CVRAudioMaterialParser>();
            }

            // Microphone
            EditorGUI.LabelField(R(20), "Microphone device:");
            void handleDropdownItemClicked(object parameter) {
                microphoneName = parameter as string;
                StartMicrophone();
            }
            GenericMenu menu = new GenericMenu();
            foreach (var device in Microphone.devices) {
                menu.AddItem(new GUIContent($"{device}"), false, handleDropdownItemClicked, device);
            }
            if (EditorGUI.DropdownButton(R(20), new GUIContent(microphoneName), FocusType.Keyboard)) {
                menu.ShowAsContext();
            }


            // audioSource = EditorGUI.ObjectField(R(20), "AudioSource", audioSource, typeof(AudioSource), true) as AudioSource;
            if (audioMaterialParser) {
                audioSource = audioMaterialParser.GetComponentInChildren<AudioSource>();
            }
            if (audioSource) {
                StartMicrophone();
            }

            EditorGUI.HelpBox(R(50), "You can use something like Virtual Audio Cable to create a microphone device which plays your music that you jam to while working on your shaders.", MessageType.Info);
        }

        void StartMicrophone() {
            audioSource.clip = Microphone.Start(microphoneName, true, 1, 44100);
            audioSource.Play();
        }

        public void Update() {
            SendAudioOutputData();
            CustomRenderTexture customRenderTexture = audioMaterialParser.GetComponent<CVRCustomRenderTextureUpdater>().customRenderTexture;
            customRenderTexture.Update();
            Shader.SetGlobalTexture("_AudioTexture", customRenderTexture, RenderTextureSubElement.Default);
        }

        [MenuItem("Tools/AudioLinkEditor")]
        public static void ShowMyEditor()
        {
          EditorWindow wnd = GetWindow<AudioLinkEditor>();
          wnd.titleContent = new GUIContent("AudioLinkEditor");
        }

        // -------- below this, we're adating code from AudioLink.cs, found in the VRChat version --------

        private float[] _spectrumValues = new float[1024];
        private float[] _spectrumValuesTrim = new float[1023];
        private float[] _audioFramesL = new float[1023 * 4];
        private float[] _audioFramesR = new float[1023 * 4];
        private float[] _samples = new float[1023];
        // Fix for AVPro mono game output bug (if running the game with a mono output source like a headset)
        private int _rightChannelTestDelay = 300;
        private int _rightChannelTestCounter;
        private bool _ignoreRightChannel = false;



        void SendAudioOutputData()
        {
            Material audioMaterial = audioMaterialParser.processingMaterial;

            audioSource.GetOutputData(_audioFramesL, 0);                // left channel
            // Debug.Log(_audioFramesL[1]);

            if (_rightChannelTestCounter > 0)
            {
                if (_ignoreRightChannel) {
                    System.Array.Copy(_audioFramesL, 0, _audioFramesR, 0, 4092);
                } else {
                    audioSource.GetOutputData(_audioFramesR, 1);
                }
                _rightChannelTestCounter--;
            } else {
                _rightChannelTestCounter = _rightChannelTestDelay;      // reset test countdown
                _audioFramesR[0] = 0f;                                  // reset tested array element to zero just in case
                audioSource.GetOutputData(_audioFramesR, 1);            // right channel test
                _ignoreRightChannel = (_audioFramesR[0] == 0f) ? true : false;
            }

            System.Array.Copy(_audioFramesL, 0, _samples, 0, 1023); // 4092 - 1023 * 4
            audioMaterial.SetFloatArray("_Samples0L", _samples);
            System.Array.Copy(_audioFramesL, 1023, _samples, 0, 1023); // 4092 - 1023 * 3
            audioMaterial.SetFloatArray("_Samples1L", _samples);
            System.Array.Copy(_audioFramesL, 2046, _samples, 0, 1023); // 4092 - 1023 * 2
            audioMaterial.SetFloatArray("_Samples2L", _samples);
            System.Array.Copy(_audioFramesL, 3069, _samples, 0, 1023); // 4092 - 1023 * 1
            audioMaterial.SetFloatArray("_Samples3L", _samples);

            System.Array.Copy(_audioFramesR, 0, _samples, 0, 1023); // 4092 - 1023 * 4
            audioMaterial.SetFloatArray("_Samples0R", _samples);
            System.Array.Copy(_audioFramesR, 1023, _samples, 0, 1023); // 4092 - 1023 * 3
            audioMaterial.SetFloatArray("_Samples1R", _samples);
            System.Array.Copy(_audioFramesR, 2046, _samples, 0, 1023); // 4092 - 1023 * 2
            audioMaterial.SetFloatArray("_Samples2R", _samples);
            System.Array.Copy(_audioFramesR, 3069, _samples, 0, 1023); // 4092 - 1023 * 1
            audioMaterial.SetFloatArray("_Samples3R", _samples);
        }


    }
}
