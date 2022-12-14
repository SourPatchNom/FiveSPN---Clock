using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace FiveSpn.Clock.Client
{
    public class Service : BaseScript
    {
        private readonly bool _verboseLogs;
        private int _utcOffsetServerSetting = 0; //Offset for in game time
        private int _utcOffsetClientFromGameServer = 0; //Users offset from game servers UTC
        private float _timeScale;

        public Service()
        {
            if (API.GetResourceMetadata(API.GetCurrentResourceName(), "verbose_logs", 0) == "true") _verboseLogs = true;
            if (!int.TryParse(API.GetResourceMetadata(API.GetCurrentResourceName(), "utc_offset", 0), out _utcOffsetServerSetting)) _utcOffsetServerSetting = 0;
            _utcOffsetServerSetting = _utcOffsetServerSetting < 0 ? _utcOffsetServerSetting *= -1: _utcOffsetServerSetting; //Prevent admin setting to negative number.
                
            if(!float.TryParse(API.GetResourceMetadata(API.GetCurrentResourceName(), "time_scale", 0),out _timeScale)) _timeScale = 0;
            
            EventHandlers.Add("playerSpawned", new Action<Vector3>(OnPlayerSpawned));
            EventHandlers["FiveSPN-Clock-SetUtcOffset"] += new Action<int>(SetGeneralUtcOffset);
            EventHandlers["FiveSPN-Clock-SetTimeScale"] += new Action<float>(SetTimeScale);
            EventHandlers["FiveSPN-Clock-ClientUtcConfirm"] += new Action<int>(SetClientServerUtcOffset);

            API.RegisterCommand("VerifyTime", new Action<int, List<object>, string>((source, args, raw) =>
            {
                TriggerServerEvent("FiveSPN-Clock-VerifyUtcOffset");
                TriggerServerEvent("FiveSPN-Clock-VerifyClientUtc", DateTime.UtcNow.Hour);
                TriggerServerEvent("FiveSPN-Clock-VerifyTimeScale");
            }), false);

            API.RegisterCommand("SetTimeOffset", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count == 1)
                {
                    try
                    {
                        if (int.TryParse(args[0].ToString(), out int setOffset))
                        {
                            TriggerServerEvent("FiveSPN-Clock-SetUtcOffset", setOffset);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }), false);

            API.RegisterCommand("SetTimeScale", new Action<int, List<object>, string>((source, args, raw) =>
            {
                if (args.Count == 1)
                {
                    try
                    {
                        if (float.TryParse(args[0].ToString(), out float scale))
                        {
                            TriggerServerEvent("FiveSPN-Clock-SetTimeScale", scale);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
            }), false);
            
            Tick += OnTick;
        }

        private void SetTimeScale(float newScale)
        {
            _timeScale = newScale;
        }

        private void SetClientServerUtcOffset(int setOffset)
        {
            _utcOffsetClientFromGameServer = setOffset;
        }

        private void SetGeneralUtcOffset(int setOffset)
        {
            _utcOffsetServerSetting = setOffset;
        }
        
                
        private void OnPlayerSpawned([FromSource]Vector3 a)
        {
            TriggerServerEvent("FiveSPN-Clock-VerifyUtcOffset");
            TriggerServerEvent("FiveSPN-Clock-VerifyClientUtc", DateTime.UtcNow.Hour);
        }
        
        private async Task OnTick()
        {
            try
            {
                DateTime utcTime = DateTime.UtcNow;
                int hourNoScale = (utcTime.Hour + _utcOffsetServerSetting + _utcOffsetClientFromGameServer) % 24;
                hourNoScale = hourNoScale == 24 ? hourNoScale = 0 : hourNoScale;
                
                if (_timeScale <= 1)
                {
                    API.NetworkOverrideClockTime(hourNoScale,utcTime.Minute,utcTime.Second);
                    await Task.FromResult(1);
                    return;
                }
                
                int totalSecondsElapsedIrl = (hourNoScale * 60 * 60) + (utcTime.Minute * 60) + utcTime.Second;
                float timeScaleMultiplier = _timeScale <= 1 ? 1 : _timeScale;
                DateTime dateTime = utcTime.Date.AddSeconds(totalSecondsElapsedIrl * timeScaleMultiplier);

                if (_verboseLogs) Debug.WriteLine(dateTime.Hour+"|"+dateTime.Minute+"|"+dateTime.Second);
                API.NetworkOverrideClockTime(dateTime.Hour,dateTime.Minute,dateTime.Second);
                await Task.FromResult(1);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        
    }
}