using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TypeWriter
{
    public class RichTextParser
    {

        public string[] RTOpenTag = new string[] { "<b>", "<i>", "<size=", "<color=" };
        public string[] RTCloseTag = new string[] { "</b>", "</i>", "</size>", "</color>" };


        public class RTTag
        {
            public int tag;
            public string value;
        }

        public class RTCharacter
        {
            public string character;
            public List<RTTag> activeTags;
        }

        public List<RTCharacter> processedText = new List<RTCharacter>();
        public string cleanText;

        string completeText;
        List<RTTag> activeTags = new List<RTTag>();

        int currentPos = 0;

        public void InitializeWithString(string text)
        {
            completeText = text;
            currentPos = 0;
            activeTags.Clear();
            processedText.Clear();
            ProcessText();
            CreateCleanText();
        }

        void CreateCleanText()
        {
            for (int i = 0; i < processedText.Count; i++)
                cleanText += processedText[i].character;
        }

        void ProcessText()
        {
            while (currentPos < completeText.Length)
            {
                //Look for starting tag at current pos
                //if found... parse value if needed and add to active tags
                RTTag openFound = GetTagInPos();
                if (openFound != null)
                {
                    activeTags.Add(openFound);
                    continue;
                }

                //Look for ending tag at current pos
                //If found... remove from active tags
                int closeFound = ClosingTagFound();
                if (closeFound != -1)
                {
                    for (int i = activeTags.Count - 1; i >= 0; i--)
                    {
                        if (activeTags[i].tag == closeFound)
                        {
                            activeTags.RemoveAt(i);
                            break;
                        }
                    }
                    continue;
                }

                //if no start and no end
                //add character at current pos to processed text list
                AddCurrentToList();
            }
        }

        void AddCurrentToList()
        {
            RTCharacter rtc = new RTCharacter();
            rtc.character = completeText.Substring(currentPos, 1);
            rtc.activeTags = new List<RTTag>(activeTags);
            processedText.Add(rtc);
            currentPos++;
        }

        RTTag GetTagInPos()
        {
            for (int i = 0; i < RTOpenTag.Length; i++)
            {
                if (completeText.Length - currentPos < RTOpenTag[i].Length)
                    continue;
                if (completeText.Substring(currentPos, RTOpenTag[i].Length) == RTOpenTag[i])
                {
                    //Opening tag found... parsing
                    if (i < 2)
                    {
                        currentPos += RTOpenTag[i].Length;
                        RTTag rtg = new RTTag();
                        rtg.tag = i;
                        rtg.value = "";
                        return rtg;
                    }
                    else
                    {
                        //Find close ">" and parse internal value
                        int pos = completeText.IndexOf(">", currentPos);
                        if (pos > currentPos)
                        {
                            int valueLength = pos - (currentPos + RTOpenTag[i].Length);
                            RTTag rtg = new RTTag();
                            rtg.tag = i;
                            rtg.value = completeText.Substring(currentPos + RTOpenTag[i].Length, valueLength);
                            currentPos = pos + 1;
                            return rtg;
                        }
                    }
                }
            }
            return null;
        }

        int ClosingTagFound()
        {
            for (int i = 0; i < RTCloseTag.Length; i++)
            {
                if (completeText.Length - currentPos < RTCloseTag[i].Length)
                    continue;
                if (completeText.Substring(currentPos, RTCloseTag[i].Length) == RTCloseTag[i])
                {
                    currentPos += RTCloseTag[i].Length;
                    return i;
                }
            }
            return -1;
        }

        public int Length()
        {
            return processedText.Count;
        }

        public string CharAtPos(int pos)
        {
            if (pos < 0 || pos >= processedText.Count)
                return "";
            string value = "";
            for (int i = 0; i < processedText[pos].activeTags.Count; i++)
            {
                value += GetOpenTag(processedText[pos].activeTags[i]);
            }
            value += processedText[pos].character;
            for (int i = processedText[pos].activeTags.Count - 1; i >= 0; i--)
            {
                value += RTCloseTag[processedText[pos].activeTags[i].tag];
            }
            return value;
        }

        string GetOpenTag(RTTag tg)
        {
            if (tg.tag < 2)
                return RTOpenTag[tg.tag];
            return RTOpenTag[tg.tag] + tg.value + ">";
        }
    }
}
