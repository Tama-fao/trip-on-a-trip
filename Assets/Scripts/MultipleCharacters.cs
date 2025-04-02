using UnityEngine;
using LLMUnity;
using UnityEngine.UI;

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

        public MultipleCharactersInteraction(InputField playerText, Text AIText, LLMCharacter llmCharacter, System.Action<string> onResponseComplete)
        {
            this.playerText = playerText;
            this.AIText = AIText;
            this.llmCharacter = llmCharacter;
            this.onResponseComplete = onResponseComplete;
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


        public void onChatComplete(string message)
        {
            interactionCount++;
            onResponseComplete(AIText.text);
        }
    }

    public class MultipleCharacters : MonoBehaviour
    {
        public LLMCharacter llmCharacter1;
        public InputField playerText1;
        public Text AIText1;
        MultipleCharactersInteraction interaction1;

        public LLMCharacter llmCharacter2;
        public Text AIText2;
        MultipleCharactersInteraction interaction2;

        void Start()
        {
            interaction1 = new MultipleCharactersInteraction(playerText1, AIText1, llmCharacter1, ResponseFromCharacter1);
            interaction2 = new MultipleCharactersInteraction(null, AIText2, llmCharacter2, ResponseFromCharacter2);
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
