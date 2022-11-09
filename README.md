# FiveSPN-Clock

FiveSPN-Clock is a FiveM server resource that will sync all players game time to a real world time, or a scaled time. Want the server to be evening everyday at noon? Want longer day night cycles? This is your answer! 

## Installation

Copy the folder inside of the FiveMResource folder into your server's resources folder and add it to the server configuration file.

### In server config
```
ensure FiveSPN-Clock
```

A complete server use package with all FiveSPN resources is available [here](https://github.com/SourPatchNom/FiveSPN---Suite)

## Settings

You can update the settings in the fxmanifest.
```lua
utc_offset '0' -- The offset in hours from UTC time for the server - Always positive number less than 24 or 0
time_scale '0' -- The speed scale of time from real world time. (IE '2' Means two days for every one real day.) - Always greater than one. 
```

## Commands

All players:
```lua
VerifyTime - Will force an update to the client time.
```
Players in group ServerAdmin or ClockAdmin:
```lua
SetTimeOffset # - Will set the time offset from within the game.
SetTimeScale # - Will set the time scale from within the game.
```

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

## Discord

FiveM development can be even more amazing if we work together to grow the open source community! 

Lets Collab! Join the project discord at [itsthenom.com!](http://itsthenom.com/)

## Licensing

    Copyright ©2022 Owen Dolberg (SourPatchNom)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published
    by the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

In the hopes that the greater community may benefit, you may use this code under the [GNU Affero General Public License v3.0](LICENSE).

This resource distribution utilizes the [Newtonsoft.JSON Library](https://github.com/JamesNK/Newtonsoft.Json) under the [MIT License](https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md).

This software references the CitizenFX.Core.Server and CitizenFX.Core.Client nuget packages (c) 2017-2020 the CitizenFX Collective used under [license](https://github.com/citizenfx/fivem/blob/master/code/LICENSE) and under the [FiveM Service Agreement](https://fivem.net/terms)

Never heard of FiveM? Learn more about the CitizenFX FiveM project [here](https://fivem.net/)