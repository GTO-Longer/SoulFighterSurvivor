using System;
using UnityEngine;

namespace Utilities
{
    public struct Choice
    {
        public string choiceTitle;
        public string choiceContent;
        public Sprite choiceIcon;
        public Action OnSelected;
        public Quality choiceQuality;

        public Choice(string title, string content, Sprite icon, Action onSelected, Quality quality)
        {
            choiceTitle = title;
            choiceContent = content;
            choiceIcon = icon;
            OnSelected = onSelected;
            choiceQuality = quality;
        }
    }
}