using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FiveSpn.Clock.Server
{
    public class Service : BaseScript
    {
        private readonly bool _discordShare;
        private readonly bool _verboseLogs;
        
        private int _serverUtcOffset = 0;
        private float _timeScale = 0;
        
        public Service()
        {
            if (API.GetResourceMetadata(API.GetCurrentResourceName(), "discord_share", 0) == "true") _discordShare = true;
            if (API.GetResourceMetadata(API.GetCurrentResourceName(), "verbose_logging", 0) == "true") _verboseLogs = true;
            
            if(!int.TryParse(API.GetResourceMetadata(API.GetCurrentResourceName(), "utc_offset", 0),out _serverUtcOffset)) _serverUtcOffset = 0;
            _serverUtcOffset = _serverUtcOffset < 0 ? _serverUtcOffset *= -1: _serverUtcOffset; //Prevent admin setting to negative number.
            if(!float.TryParse(API.GetResourceMetadata(API.GetCurrentResourceName(), "time_scale", 0),out _timeScale)) _timeScale = 0;
            
            if (_verboseLogs)
            {
                TriggerEvent("FiveSPN-LogToServer", API.GetCurrentResourceName(),4,"Server offset is " + _serverUtcOffset);
                var currentHour = ((DateTime.UtcNow.Hour + _serverUtcOffset) % 24).ToString();
                TriggerEvent("FiveSPN-LogToServer", API.GetCurrentResourceName(),4,"New server hour is " + currentHour);
                TriggerEvent("FiveSPN-LogToDiscord", true, API.GetCurrentResourceName(),"The server time is " + currentHour +":"+ DateTime.UtcNow.Minute.ToString("00"));    
            }

            EventHandlers["FiveSPN-Clock-VerifyClientUtc"] += new Action<Player, int>(VerifyClientUtcHour);
            EventHandlers["FiveSPN-Clock-VerifyUtcOffset"] += new Action<Player>(VerifyUtcOffset);
            EventHandlers["FiveSPN-Clock-SetUtcOffset"] += new Action<Player, int>(SetUtcOffset);
        }
        
        private void SetUtcOffset([FromSource] Player player, int newOffset)
        {
            TriggerEvent("FiveSPN-LogToServer", API.GetCurrentResourceName(), 4, "Server UTC offset update requested by " + player.Name + " to " + newOffset.ToString());
            if (!CheckPermsNow(player)) return;
            UpdateOffset(newOffset);
        }
        
        private void UpdateOffset(int newOffset)
        {
            _serverUtcOffset = ValidateOffset(newOffset);
            TriggerClientEvent("FiveSPN-Clock-SetUtcOffset", _serverUtcOffset);
            TriggerEvent("FiveSPN-LogToServer", API.GetCurrentResourceName(),4,"New server hour is " + ((DateTime.UtcNow.Hour + _serverUtcOffset) %24).ToString());
            
        }

        private int ValidateOffset(int newOffset)
        {
            newOffset = newOffset < 0 ? newOffset * -1 : newOffset;
            return newOffset % 24;
        }
        
        private void VerifyUtcOffset([FromSource]Player player)
        {
            TriggerClientEvent(player, "FiveSPN-Clock-SetUtcOffset", _serverUtcOffset);
        }

        private void VerifyClientUtcHour([FromSource]Player player, int currentClientUtcHour)
        {
            int currentServerUtcHour = DateTime.UtcNow.Hour;
            if (currentServerUtcHour == currentClientUtcHour)
            {
                TriggerClientEvent(player,"FiveSPN-Clock-ClientUtcConfirm", 0);
            }
            else if (DateTime.UtcNow.Minute == 0 && DateTime.UtcNow.Second < 5) //Could the event tx at x:59 and rx x:00
            {
                if (currentServerUtcHour == 0 && currentClientUtcHour == 23 || currentServerUtcHour == currentClientUtcHour + 1)
                {
                    TriggerClientEvent(player,"FiveSPN-Clock-ClientUtcConfirm", 0);
                }
            }
            else
            {
                int returnOffset = (currentServerUtcHour + 24 - currentClientUtcHour) % 24;
                TriggerClientEvent(player,"FiveSPN-Clock-ClientUtcConfirm", returnOffset);
            }
        }
        
        private static bool CheckPermsNow([FromSource] Player player)
        {
            if (API.IsPlayerAceAllowed(player.Handle, "ServerAdmin") || API.IsPlayerAceAllowed(player.Handle, "ClockAdmin"))
            {
                return true;
            }
            TriggerEvent("FiveSPN-LogToServer", API.GetCurrentResourceName(), 4, "Server UTC offset update requested by " + player.Name + " but they are not an admin!");
            return false;
        }
    }
}