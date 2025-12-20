using System;
using UnityEngine;

namespace Utilities
{
    public struct Choice
    {
        public string choiceTitle;
        public string choiceContent;
        public Sprite choiceIcon;
        public Sprite choiceIconBorder;
        public Action OnSelected;
        public Quality choiceQuality;

        public Choice(string title, string content, Sprite icon, Sprite iconBorder, Action onSelected, Quality quality)
        {
            choiceTitle = title;
            choiceContent = content;
            choiceIcon = icon;
            choiceIconBorder = iconBorder;
            OnSelected = onSelected;
            choiceQuality = quality;
        }
    }
}