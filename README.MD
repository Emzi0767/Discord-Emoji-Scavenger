# Discord Emoji Scavenger [![Discord](https://img.shields.io/discord/207879549394878464.svg?style=flat-square&label=discord)](https://discord.gg/rGKrJDR)
A simple tool that will download all the emojis from all the Discord servers you are in.

## Why?
One day I just woke up and decided I was in way too many servers. I hit the server cap (100 servers) three times 
already.

I was in most of these servers for emotes, so I never spoke in any of them, and the only thing these servers generated 
was spam and traffic. Furthermore, most of these servers had maybe 1-2 good emotes of all the ones they offered, so I 
decided there was little no point in sticking around.

The idea was to download all the emotes, sift through them, pick the interesting ones, then create enough emote-only 
servers with just the good ones.

## How?
I am one of the developers of [DSharpPlus](https://github.com/NaamloosDT/DSharpPlus) (which this tool uses). This tool 
simply extracts the user token from running Discord instance, then logs in using said token. Probes all the guilds for 
available emotes, then downloads them all.

Emote download happens in parallel. The tool will spawn twice as many download threads as there are logical processors 
available on the machine (so on a 2 core, 4 thread machine, it will use 8 threads).

## Requirements
The tool has some minimal requirements. First and foremost, it was only tested on Windows 10 x64, it is not guaranteed 
it will run on other tools. 

You will also need a running Discord instance.

Complete list of requirements is as follows:

- [.NET Core 2.0 Runtime](https://www.microsoft.com/net/download/thank-you/dotnet-runtime-2.0.4-windows-x64-installer) 
  ([other platforms](https://www.microsoft.com/net/download/windows), make sure you're downloading .NET Core 2.0 
  runtime).
- Running Discord instance. Stable, PTB, Canary, or Development *should* work, but it was only tested against Stable. 
  Furthermore, the application picks the first Discord instance available, which means when multiple instances are 
  running (e.g. stable and canary), the tool will pick one nondeterministically.
- Hard drive space (duh).

The download and token extraction process themselves are cross-platform. The Discord instance location prcess 
potentially isn't.

## Usage
In order to use the tool, [download](https://github.com/Emzi0767/Discord-Emoji-Scavenger/releases/latest) and extract 
it first. Then run the `scavenge.bat` script. The tool will begin operation. The emotes will be downloaded to *emotes* 
directory where the tool is located.

When the tool is done, the target directory will contain directories, which correspond to guilds which the particular 
emotes were obtained from. Inside will be emote files, file name of each will correspond to the name of the emote.

### Advanced usage
In the event you want to download the emotes somewhere else, you need to open up command prompt, and then navigate to 
the location of the tool. Then simply invoke `dotnet EmoteScavenger.dll "path/to/target/directory"`. The tool will 
download all emoji to the target directory.

## Building
If you want to make changes of the tool or simply don't trust the provided binary, you can build the tool yourself. You 
will need some tools.

### Requirements
- [Visual Studio 2017](https://www.visualstudio.com/downloads/). Make sure to install *.NET Core cross-platform 
  development* workload.
- [.NET Core SDK 2.0 or newer](https://www.microsoft.com/net/download/windows).
- .NET Core 2.0 Runtime (see above link).
- [Git for Windows](http://gitforwindows.org/).
- DSharpPlus nightly MyGet feed in NuGet sources (https://www.myget.org/F/dsharpplus-nightly/api/v3/index.json).

### Building using Visual Studio
- Clone the repository using Git. 
- Open the solution In Visual Studio.
- Select *Release* configuration.
- Hit Ctrl+Shift+B to build the solution. 
- When the build is successful, select *EmoteScavenger* project from the Solution Explorer.
- Go to Build menu, and select *Publish EmoteScavenger*.
- When the publish is completed, your artifacts will be available in 
  `<solution directory>/EmoteScavenger/bin/Release/PublishOutput`.

### Building using Command Line
- Clone the repository using Git. 
- Open command line and navigate to the directory you cloned the solution to.
- Restore all NuGet packages using `dotnet restore -c Release`.
- Build the solution in Release configuration `dotnet build -c Release`.
- Navigate to EmoteScavenger project directory.
- Publish the project in Release configuration `dotnet publish -c Release`.
- Your artifacts are available in `<soluction directory>/EmoteScavenger/bin/Release/netcoreapp2.0/publish`.

## Disclaimer
Usage of selfbots is prohibited by Discord terms of service. Using this application is a violation of Discord's Terms 
of Service, and might lead to your Discord account being terminated. The author of this tool is not responsible for 
any such problems, should they arise.

You were warned.

## Support me
If you feel like supporting me by providing me with currency that I can exchange for goods and services, you can do so 
on [my Patreon](https://www.patreon.com/emzi0767).
