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
        AudioPlaybackHandler audioHandler;

        public MultipleCharactersInteraction(InputField playerText, Text AIText, LLMCharacter llmCharacter, System.Action<string> onResponseComplete, string voice_id,
            ElevenlabsAPI api = null)
        {
            this.playerText = playerText;
            this.AIText = AIText;
            this.llmCharacter = llmCharacter;
            this.onResponseComplete = onResponseComplete;
            this.api = api;
            this.voice_id = voice_id;
            this.audioHandler = api?.GetComponent<AudioPlaybackHandler>();
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
            ProcessInteraction(message);
        }

        public void ProcessInteraction(string message)
        {
            if (interactionCount < maxInteractions)
            {
                _ = llmCharacter.Chat(message, SetAIText, () => onChatComplete(message));
            }
            else
            {
                // Reset counter when max interactions reached
                interactionCount = 0;
                playerText.interactable = true;
                playerText.Select();
                playerText.text = "";
            }
        }

        public void SetAIText(string text)
        {
            AIText.text = text;
        }

        public async void onChatComplete(string message)
        {
            if (api != null)
            {
                api.AudioReceived.AddListener(OnAudioReceived);
                api.GetAudio(AIText.text, voice_id);
            }
            else
            {
                await Task.Delay(500); // Small delay if no audio
                CompleteInteraction();
            }
        }

        private void OnAudioReceived(AudioClip audioClip)
        {
            api.AudioReceived.RemoveListener(OnAudioReceived); // Remove listener immediately
            
            if (audioHandler != null)
            {
                audioHandler.PlayAudioClip(audioClip);
                WaitForAudioToFinish().ConfigureAwait(false);
            }
            else
            {
                CompleteInteraction();
            }
        }

        private async Task WaitForAudioToFinish()
        {
            while (audioHandler != null && audioHandler.IsPlaying())
            {
                await Task.Yield();
            }
            CompleteInteraction();
        }

        private void CompleteInteraction()
        {
            interactionCount++; // Increment counter here instead
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
            interaction2 = new MultipleCharactersInteraction(playerText2, AIText2, llmCharacter2, ResponseFromCharacter2, voiceId2, _elevenLabsAPI);
            interaction1.Start();
        }

        void ResponseFromCharacter1(string response)
        {
            // Don't reset playerText1 here - let the interaction handle it
            interaction2.ProcessInteraction(response);
        }

        void ResponseFromCharacter2(string response)
        {
            // Don't reset playerText2 here - let the interaction handle it
            interaction1.ProcessInteraction(response);
        }

        public void CancelRequests()
        {
            llmCharacter1.CancelRequests();
            llmCharacter2.CancelRequests();
        }
    }
}