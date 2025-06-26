using NAudio.Wave;
using Newtonsoft.Json.Linq;
using Vosk;

namespace AlsProjects.Speech {
    public static class Utility {
        private static readonly object recognizerLock = new object();

        public static async Task<string> Listen(string modelPath, int timeoutMilliseconds = 8000) {
            VoskRecognizer recognizer = null!;
            Vosk.Model model = null!;

            // Load the model and create the recognizer
            model = new Vosk.Model(modelPath);
            recognizer = new VoskRecognizer(model, 16000.0f);
            var waveIn = new WaveInEvent();
            waveIn.BufferMilliseconds = 1000;
            waveIn.WaveFormat = new WaveFormat(16000, 1);
            string resultText = string.Empty;
            var tcs = new TaskCompletionSource<string>();

            waveIn.DataAvailable += (s, e) => {
                lock (recognizerLock) {
                    if (recognizer.AcceptWaveform(e.Buffer, e.BytesRecorded)) {
                        resultText = JObject.Parse(recognizer.Result())?["text"]?.ToString() ?? "";
                        tcs.TrySetResult(resultText);
                    }
                }
            };

            // When the recording stops, we complete the task if not already done
            waveIn.RecordingStopped += (s, e) => {
                if (!tcs.Task.IsCompleted) {
                    tcs.TrySetResult(resultText);
                }
            };

            waveIn.StartRecording();
            Thread.Sleep(1000);
            string finalResult = await tcs.Task;
            waveIn.StopRecording();

            return finalResult;
        }

        public static void SpeakText(string text) {
            using var synthesizer = new System.Speech.Synthesis.SpeechSynthesizer();
            synthesizer.Speak(text);
        }
    }
}
