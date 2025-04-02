using UnityEngine;
using LLMUnity;
using UnityEngine.UI;
using System.Threading.Tasks;

namespace LLMUnitySamples
{
    public class MultipleCharactersInteraction
    {
        InputField playerText;
        Text AIText;
        LLMCharacter llmCharacter;
        System.Action<string> onResponseComplete;
        int interactionCount = 0;
        const int maxInteractions = 3;


        ElevenlabsAPI api;

        string voice_id;

        public MultipleCharactersInteraction(InputField playerText, Text AIText, LLMCharacter llmCharacter, System.Action<string> onResponseComplete,  string voice_id,
            ElevenlabsAPI api = null)
        {
            this.playerText = playerText;
            this.AIText = AIText;
            this.llmCharacter = llmCharacter;
            this.onResponseComplete = onResponseComplete;
                 this.api = api;
            this.voice_id = voice_id;
        }

        public void Start()
        {
            playerText.onSubmit.AddListener(onInputFieldSubmit);
            playerText.Select();
        }

        public void onInputFieldSubmit(string message)
        {
            playerText.interactable = false;
            AIText.text = "...";
            interactionCount = 0;
            ProcessInteraction(message);
        }

        public void ProcessInteraction(string message)
        {
            if (interactionCount < maxInteractions)
            {
                _ = llmCharacter.Chat(message, SetAIText, () => onChatComplete(message));
            }
        }


        public void SetAIText(string text)
        {
            AIText.text = text;
        }


             public async void onChatComplete(string message)
        {
            interactionCount++;

            api.AudioReceived.AddListener(OnAudioReceived);
            api.GetAudio(AIText.text, voice_id);


        }

 private void OnAudioReceived(AudioClip audioClip)
        {
            var audioHandler = api.GetComponent<AudioPlaybackHandler>();
            if (audioHandler != null)
            {
                audioHandler.PlayAudioClip(audioClip);
                WaitForAudioToFinish(audioHandler).ConfigureAwait(false);
            }
        }
        private async Task WaitForAudioToFinish(AudioPlaybackHandler audioHandler)
        {
            while (audioHandler.IsPlaying())
            {
                await Task.Yield(); // Yield control back to the main thread
            }

            playerText.interactable = true;
            playerText.Select();
            playerText.text = "";
            onResponseComplete(AIText.text);
        }



    }

   public class MultipleCharacters : MonoBehaviour
    {
        public LLMCharacter llmCharacter1;
        public InputField playerText1;
        public Text AIText1;
        MultipleCharactersInteraction interaction1;
        public string voiceId1;

        public LLMCharacter llmCharacter2;
        public InputField playerText2;
        public Text AIText2;
        MultipleCharactersInteraction interaction2;
        public string voiceId2;

        [SerializeField]
        private ElevenlabsAPI _elevenLabsAPI;

        void Start()
        {
            interaction1 = new MultipleCharactersInteraction(playerText1, AIText1, llmCharacter1, ResponseFromCharacter1, voiceId1, _elevenLabsAPI);
            interaction2 = new MultipleCharactersInteraction(null, AIText2, llmCharacter2, ResponseFromCharacter2, voiceId2, _elevenLabsAPI);
            interaction1.Start();
        }

        void ResponseFromCharacter1(string response)
        {
            interaction2.ProcessInteraction(response);
        }

        void ResponseFromCharacter2(string response)
        {
            interaction1.ProcessInteraction(response);
        }

        public void CancelRequests()
        {
            llmCharacter1.CancelRequests();
            llmCharacter2.CancelRequests();
        }
    }
}
