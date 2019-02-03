using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace TypeWriter
{
    public class Typewriter : MonoBehaviour
    {
        public enum EventType
        {
            MessageStarted, MessageEnded, InitialAudioStarted, InitialAudioEnded, TextStarted, TextEnded, AllMesagesEnded
        }

        public class EventInfo
        {
            public EventType messageInfo;
            public int extraInfo;
            public Typewriter sender;
        }

        public delegate void CallbackEventHandler(EventInfo eventInfo);
        public CallbackEventHandler CallBackFunction;

        public bool autoStart = true;
        public string cursorChar = "#";
        public float typeDuration = 0.1f;
        public float newLinePause = 0.3f;
        public float cursorBlinkRate = 0.3f;
        public GameObject AudioSourceClicks;
        public GameObject AudioSourceMessages;
        public List<AudioClip> typeSound;
        public List<AudioClip> audioMessage;

        int currentPos = 0;

        public AudioClip initialAudio;
        public bool cursorOnPlay = true;
        public bool cursorOnPauses = true;
        public float initialPause = 0.5f;
        public float finalPause = 1.0f;
        public bool RichTextEnabled = false;

        //Add any other language you need
        //Remember to add also to the Language enum
        [TextArea(5, 10)]
        public string text = "";
        [HideInInspector]
        public RichTextParser rtp;

        public Text messageUI;


        AudioSource typeSource;
        AudioSource initialSource;

        bool playingInitialAudio = false;
        bool initialAudioPlayed = false;
        bool initialPausePlayed = false;
        bool finalPausePlayed = false;
        bool playEnded = false;

        bool showCursor = false;
        bool cursorBlink = false;
        string originalMsg = "";

        public void InitialiceText()
        {
            text = text.Replace("\\n", "\n");
            //Debug.Log(text);
            if (RichTextEnabled)
            {
                rtp = new RichTextParser();
                rtp.InitializeWithString(text);
                text = rtp.cleanText;
            }
            //Debug.Log(text);
        }

        void Start()
        {
            //InitialiceText();
            typeSource = (AudioSource)AudioSourceClicks.transform.GetComponent<AudioSource>();
            initialSource = (AudioSource)AudioSourceMessages.transform.GetComponent<AudioSource>();

            if (autoStart)
            {
                InitialiceText();
                StartCoroutine(PlayMessage());
                StartCoroutine(CursorBlink());
            }
        }

        public void StartMesage()
        {
            InitialiceText();
            StopCoroutine(PlayMessage());
            StopCoroutine(CursorBlink());
            playingInitialAudio = false;
            initialAudioPlayed = false;
            initialPausePlayed = false;
            finalPausePlayed = false;
            playEnded = false;
            showCursor = false;
            cursorBlink = false;
            originalMsg = "";
            currentPos = 0;
            StartCoroutine(PlayMessage());
            StartCoroutine(CursorBlink());
        }

        public IEnumerator CursorBlink()
        {
            while (true)
            {
                if (showCursor)
                {
                    if (cursorBlink)
                    {
                        cursorBlink = false;
                        messageUI.text = originalMsg;
                    }
                    else
                    {
                        cursorBlink = true;
                        originalMsg = messageUI.text;
                        messageUI.text = messageUI.text + cursorChar;
                    }
                }
                if (playEnded)
                {
                    messageUI.text = "";
                    originalMsg = "";
                    yield break;
                }

                yield return new WaitForSeconds(cursorBlinkRate);
            }
        }

        public IEnumerator PlayMessage()
        {
            while (true)
            {

                if (currentPos == 0 && !initialAudioPlayed && !playingInitialAudio && CallBackFunction != null)
                {
                    EventInfo myEvent = new EventInfo();
                    myEvent.sender = this;
                    myEvent.messageInfo = EventType.MessageStarted;
                    CallBackFunction(myEvent);
                }

                //Has initial audio and has not beeb played yet
                if (!initialAudioPlayed && initialAudio != null && !playingInitialAudio)
                {
                    //Plays initial audio
                    //Debug.Log("Initil Audio Ini");
                    playingInitialAudio = true;
                    showCursor = cursorOnPlay;
                    initialSource = (AudioSource)AudioSourceMessages.transform.GetComponent<AudioSource>();
                    //Debug.Log(initialSourceObject + " - " + initialSource);
                    initialSource.clip = initialAudio;
                    initialSource.Play();
                }

                //Initial audio has been just finished
                if (playingInitialAudio && !initialSource.isPlaying)
                {
                    initialAudioPlayed = true;
                    playingInitialAudio = false;
                    showCursor = false;
                    //Debug.Log("Initil Audio Fin");
                }

                //Initial audio already played or no initial audio at all
                //Play text message
                if (initialAudioPlayed || initialAudio == null)
                {
                    //Play initial pause
                    if (currentPos == 0 && !initialPausePlayed && initialPause > 0f)
                    {
                        initialPausePlayed = true;
                        showCursor = cursorOnPauses;
                        yield return new WaitForSeconds(initialPause);
                        messageUI.text = "";
                        originalMsg = "";
                        cursorBlink = false;
                    }

                    if (initialPausePlayed)
                    {
                        if (currentPos == 0 && CallBackFunction != null)
                        {
                            EventInfo myEvent = new EventInfo();
                            myEvent.sender = this;
                            myEvent.messageInfo = EventType.TextStarted;
                            CallBackFunction(myEvent);
                        }

                        showCursor = false;



                        if (RichTextEnabled)
                        {
                            if (rtp.CharAtPos(currentPos) == "[" && rtp.CharAtPos(currentPos + 3) == "]")
                            {
                                string val = rtp.CharAtPos(currentPos + 1) + rtp.CharAtPos(currentPos + 2);
                                initialSource.clip = audioMessage[int.Parse(val)];
                                initialSource.Play();
                                currentPos += 4;
                            }
                            messageUI.text = messageUI.text + rtp.CharAtPos(currentPos);
                        }
                        else
                        {
                            if (text.Substring(currentPos, 1) == "[" && text.Substring(currentPos + 3, 1) == "]")
                            {
                                string val = text.Substring(currentPos + 1, 2);
                                initialSource.clip = audioMessage[int.Parse(val)];
                                initialSource.Play();
                                currentPos += 4;
                            }
                            messageUI.text = messageUI.text + text.Substring(currentPos, 1);
                        }

                        currentPos++;
                        typeSource.clip = typeSound[Random.Range(0, typeSound.Count - 1)];
                        typeSource.Play();

                        if (text.Substring(currentPos - 1, 1) == "\n")
                            yield return new WaitForSeconds(newLinePause);

                        if (currentPos == text.Length && CallBackFunction != null)
                        {
                            EventInfo myEvent = new EventInfo();
                            myEvent.sender = this;
                            myEvent.messageInfo = EventType.TextEnded;
                            CallBackFunction(myEvent);
                        }
                    }

                    //Play final pause
                    if (currentPos == text.Length && !finalPausePlayed && finalPause > 0f)
                    {
                        finalPausePlayed = true;
                        showCursor = cursorOnPauses;
                        yield return new WaitForSeconds(finalPause);
                    }

                    //Playing all messages, typewriter ended
                    if (currentPos == text.Length && finalPausePlayed)
                    {
                        if (CallBackFunction != null)
                        {
                            EventInfo myEvent = new EventInfo();
                            myEvent.sender = this;
                            myEvent.extraInfo = -1;
                            myEvent.messageInfo = EventType.MessageEnded;
                            CallBackFunction(myEvent);
                            myEvent.messageInfo = EventType.AllMesagesEnded;
                            CallBackFunction(myEvent);
                        }
                        playEnded = true;
                        yield break;
                    }

                    //Current massage ended, go for next
                    if (currentPos == text.Length && finalPausePlayed)
                    {
                        if (CallBackFunction != null)
                        {
                            EventInfo myEvent = new EventInfo();
                            myEvent.sender = this;
                            myEvent.messageInfo = EventType.MessageEnded;
                            CallBackFunction(myEvent);
                        }
                        playingInitialAudio = false;
                        initialAudioPlayed = false;
                        initialPausePlayed = false;
                        finalPausePlayed = false;
                        showCursor = false;
                        cursorBlink = false;
                        originalMsg = "";
                        currentPos = 0;
                        messageUI.text = "";
                        yield return new WaitForSeconds(typeDuration);
                    }
                }
                yield return new WaitForSeconds(typeDuration);
            }
        }
    }
}
