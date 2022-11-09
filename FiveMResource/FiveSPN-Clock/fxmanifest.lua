fx_version 'bodacious'

name 'FiveSPN-Clock'
description 'Manages the time of the server.'
author 'SourPatchNom'
version 'v1.0'
url 'https://itsthenom.com'

utc_offset '6' -- The offset in hours from UTC time for the server - Always positive number less than 24 or 0
time_scale '0' -- The speed scale of time from real world time.

discord_share 'false'
verbose_logs 'false'

games { 'gta5' }

server_script {
	"FiveSpn.Clock.Server.net.dll",
}

client_script { 
	"FiveSpn.Clock.Client.net.dll",
}

dependancy 'FiveSPN-Logger'