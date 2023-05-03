using System.Collections.Generic;

namespace Watermelon
{
    public static partial class GameSettingsPrefs
    {
        private static Dictionary<string, object> settings = new Dictionary<string, object>()
        {            
            { "tutorial", false },
            { "last_open_level_id", 0 },

            { "coins", 0 },
            { "gems", 0 },

            { "last_ads_coins", double.MinValue },

            { "current_level", 0 },
            { "actual_level", 0 },

            { "music", 1.0f },
            { "sound", 1.0f },
            { "vibration", true },

            { "rate_us_already_shown", false },
        };
    }
}