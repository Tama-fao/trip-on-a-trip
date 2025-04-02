using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

/**
 * Optional utility class to fetch all the available Voices in the ElevenLabs account.
 * This may be useful if you want to allow the player to customize the voice at runtime.
 */
public class ElevenVoices : MonoBehaviour
{

    public List<VoiceExposed> Voices = new List<VoiceExposed>();

    [SerializeField]
    private string _apiKey;

    [SerializeField]
    private string _apiUrl = "https://api.elevenlabs.io";

    void Start()
    {
        // In this example we populate the Voices list on Start. But you can do that on demand
        // if you prefer. After all, we do not need to do this every time we start the game.
        StartCoroutine(DoRequest());
    }

    IEnumerator DoRequest()
    {
        var url = $"{_apiUrl}/v1/voices";
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.SetRequestHeader("xi-api-key", _apiKey);
            yield return request.SendWebRequest();
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error fetching voices: " + request.error);
                yield break;
            }
            var jsonResponse = request.downloadHandler.text;
            var response = JsonUtility.FromJson<ApiResponse>(jsonResponse);
            foreach (var voice in response.voices)
            {
                Voices.Add(new VoiceExposed
                {
                    voice_id = voice.voice_id,
                    name = voice.name
                });
            }
        }
    }

    [Serializable]
    public class VoiceExposed
    {
        public string voice_id;
        public string name;
    }

    [Serializable]
    public class Voice
    {
        public string voice_id;
        public string name;
        public List<Sample> samples;
        // Define other properties as needed
    }

    [Serializable]
    public class Sample
    {
        public string sample_id;
        public string file_name;
        public string mime_type;
        public int size_bytes;
        public string hash;
        // Define other properties as needed
    }

    [Serializable]
    public class ApiResponse
    {
        public List<Voice> voices;
    }

}
