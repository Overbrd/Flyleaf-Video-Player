## Flyleaf Video Player using C#, .Net 6.0 and Visual Studio 2022

Run the app and right click in the video player to choose a file for playback.

A sample .mp4 and a sample .mov video file is included in the download in the main folder.

# *Flyleaf v3.4*: Media Player .NET Library for WPF/WinForms (based on FFmpeg/DirectX)

![video-player-screenshot](https://user-images.githubusercontent.com/10948817/196687448-e7a4814b-0d6b-4160-a5a9-3f8930264fea.jpg)

---

>Notes<br/>
>1. FlyleafLib's releases will be on [NuGet](https://www.nuget.org/packages?q=flyleaf)
>2. Compiled samples will be on [GitHub releases](https://github.com/SuRGeoNix/Flyleaf/releases)
>3. Documentation will be on [Wiki](https://github.com/SuRGeoNix/Flyleaf/wiki) and [Samples](https://github.com/SuRGeoNix/Flyleaf/tree/master/Samples) within the solution

### [Supported Features]
* ***Light***: for GPU/CPU/RAM as it supports video acceleration, post-processing with pixel shaders and it is coded from scratch
* ***High Performance***: threading implementation and efficient aborting/cancelation allows to achieve smooth playback and fast seeking
* ***Stable***: library which started as a hobby about 2 years ago and finally became a stable (enough) media engine
* ***Formats Support***: All formats and protocols that FFmpeg supports with the additional supported by plugins (currently torrent (bitswarm) / web (youtube-dl) streaming)
* ***Pluggable***: Focusing both on allowing custom inputs (such as a user defined stream) and support for 3rd party plugins (such as scrappers / channels / playlists etc.)
* ***Configurable***: Exposes low level parameters for customization (demuxing/buffering & decoding parameters). Supports save and load of an existing config file.
* ***UI Controls***: Supports both WinForms and WPF with a large number of embedded functionality (Activity Mode/FullScreen/Mouse & Key Bindings/ICommands/Properties/UI Updates)
* ***Multiple Instances***: Supports multiple players with fast swap to each other and different configurations (such as audio devices/video aspect ratio etc.)

### [Extra Features]
* ***HLS Player***: supports live seeking (might the 1st FFmpeg player which does that)
* ***RTSP***: supports RTSP cameras with low latency
* ***AudioOnly***: supports Audio Player only without the need of Control/Rendering
* ***HDR to SDR***: supports BT2020 / SMPTE 2084 to BT709 with Aces, Hable and Reinhard methods (still in progress, HDR native not supported yet)
* ***Slow/Fast Speed***: Change playback speed (x0.0 - x1.0 and x1 - x4)
* ***Reverse Playback***: Change playback mode to reverse and still keep the same frame rate (speed x0.0 - x1.0)
* ***Recorder***: record the currently watching video
* ***Snapshots***: Take a snapshot of the current frame
* ***Zoom In/Out***: Zoom In/Out the rendering surface
* ***Key Bindings***: Assign embedded or custom actions to keys (check [default](https://github.com/SuRGeoNix/Flyleaf/wiki/Player#mouse--key-bindings))
* ***Themes***: WPF Control is based on [Material Design In XAML](http://materialdesigninxaml.net/) and supports already some basic Color Themes
* ***Downloader***: supports also the plugins so you can download any yt-dlp supported url as well
* ***Frame Extractor***: Extract video frames to Bmp, Jpeg, Png etc. (All, Specific or by Step)

### [Thanks to]
*Flyleaf wouldn't exist without them!*

* ***[FFmpeg](http://ffmpeg.org/)***
* ***[FFmpeg.AutoGen](https://github.com/Ruslan-B/FFmpeg.AutoGen/)***
* ***[Vortice](https://github.com/amerkoleci/Vortice.Windows)***
* Major open source media players ***[VLC](https://github.com/videolan/vlc)***, ***[Kodi](https://github.com/xbmc/xbmc)***, ***[MPV](https://github.com/mpv-player/mpv)***, ***[FFplay](https://github.com/FFmpeg/FFmpeg/blob/master/fftools/ffplay.c)***
* For plugins thanks to ***[YT-DLP](https://github.com/yt-dlp/yt-dlp)*** and ***[OpenSubtitles.org](https://www.opensubtitles.org/)***
