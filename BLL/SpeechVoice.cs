
using DotNetSpeech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public class SpeechVoice
    {
        private static SpVoice  voice = new SpVoice();
        private static SpeechVoiceSpeakFlags sFlags = SpeechVoiceSpeakFlags.SVSFlagsAsync;
        public static void speack(string text)
        {
            voice.Rate = 0;
            voice.Volume = 100;
            voice.Speak(text, sFlags);
        }
    }
}
