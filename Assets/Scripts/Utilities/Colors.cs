using UnityEngine;

namespace Utilities
{
    public static class Colors
    {
        public const string Health = "#0ACD02";
        public const string HealthGeneration = "#20FF20";
        public const string Magic = "#00B4FF";
        public const string MagicRegeneration = "#80D8FF";

        public const string AttackDamage = "#FF8A00";
        public const string AbilityPower = "#C97BFF";
        public const string AttackPenetration = "#FF6A6A";
        public const string MagicPenetration = "#9E4EFF";
        
        public const string CriticalRate = "#FF4D4D";
        public const string CriticalDamage = "#FF1A1A";
        public const string AttackSpeed = "#E0C300";

        public const string AbilityHaste = "#7DFFFF";
        public const string AttackDefense = "#FFD700";
        public const string MagicDefense = "#6AA2FF";

        public const string MoveSpeed = "#FFFFFF";
        public const string LifeSteal = "#FF6666";
        public const string OmniVamp = "#D14BFF";
        
        public const string SilverBorder = "#55656B";
        public const string Gold = "#FFEDB6";
        public const string GoldBorder = "#9D9C7A";
        public const string Prismatic = "#E6B6FF";
        public const string PrismaticBorder = "#DCC3E9";

        public static Color GetColor(string color)
        {
            ColorUtility.TryParseHtmlString(color, out var newColor);
            return newColor;
        }
    }
}