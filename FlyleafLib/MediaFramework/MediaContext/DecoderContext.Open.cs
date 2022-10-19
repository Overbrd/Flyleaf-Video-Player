﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FlyleafLib.MediaFramework.MediaDemuxer;
using FlyleafLib.MediaFramework.MediaPlaylist;
using FlyleafLib.MediaFramework.MediaStream;

using static FlyleafLib.Logger;

namespace FlyleafLib.MediaFramework.MediaContext
{
    public partial class DecoderContext
    {
        #region Events
        public event EventHandler<OpenCompletedArgs>                        OpenCompleted;
        public event EventHandler<OpenSessionCompletedArgs>                 OpenSessionCompleted;
        public event EventHandler<OpenSubtitlesCompletedArgs>               OpenSubtitlesCompleted;
        public event EventHandler<OpenPlaylistItemCompletedArgs>            OpenPlaylistItemCompleted;

        public event EventHandler<OpenAudioStreamCompletedArgs>             OpenAudioStreamCompleted;
        public event EventHandler<OpenVideoStreamCompletedArgs>             OpenVideoStreamCompleted;
        public event EventHandler<OpenSubtitlesStreamCompletedArgs>         OpenSubtitlesStreamCompleted;

        public event EventHandler<OpenExternalAudioStreamCompletedArgs>     OpenExternalAudioStreamCompleted;
        public event EventHandler<OpenExternalVideoStreamCompletedArgs>     OpenExternalVideoStreamCompleted;
        public event EventHandler<OpenExternalSubtitlesStreamCompletedArgs> OpenExternalSubtitlesStreamCompleted;

        public class OpenCompletedArgs
        {
            public string       Url;
            public Stream       IOStream;
            public string       Error;
            public bool         Success => Error == null;
            public OpenCompletedArgs(string url = null, Stream iostream = null, string error = null) { Url = url; IOStream = iostream; Error = error; }
        }
        public class OpenSubtitlesCompletedArgs
        {
            public string       Url;
            public string       Error;
            public bool         Success => Error == null;
            public OpenSubtitlesCompletedArgs(string url = null, string error = null) { Url = url; Error = error; }
        }
        public class OpenSessionCompletedArgs
        {
            public Session      Session;
            public string       Error;
            public bool         Success => Error == null;
            public OpenSessionCompletedArgs(Session session = null, string error = null) { Session = session; Error = error; }
        }
        public class OpenPlaylistItemCompletedArgs
        {
            public PlaylistItem Item;
            public PlaylistItem OldItem;
            public string       Error;
            public bool         Success => Error == null;
            public OpenPlaylistItemCompletedArgs(PlaylistItem item = null, PlaylistItem oldItem = null, string error = null) {  Item = item; OldItem = oldItem; Error = error; }
        }
        public class StreamOpenedArgs
        {
            public StreamBase   Stream;
            public StreamBase   OldStream;
            public string       Error;
            public bool         Success => Error == null;
            public StreamOpenedArgs(StreamBase stream = null, StreamBase oldStream = null, string error = null) { Stream = stream; OldStream= oldStream; Error = error; }
        }
        public class OpenAudioStreamCompletedArgs : StreamOpenedArgs 
        {
            public new AudioStream Stream   => (AudioStream)base.Stream;
            public new AudioStream OldStream=> (AudioStream)base.OldStream;
            public OpenAudioStreamCompletedArgs(AudioStream stream = null, AudioStream oldStream = null, string error = null): base(stream, oldStream, error) { }
        }
        public class OpenVideoStreamCompletedArgs : StreamOpenedArgs
        {
            public new VideoStream Stream   => (VideoStream)base.Stream;
            public new VideoStream OldStream=> (VideoStream)base.OldStream;
            public OpenVideoStreamCompletedArgs(VideoStream stream = null, VideoStream oldStream = null, string error = null): base(stream, oldStream, error) { }
        }
        public class OpenSubtitlesStreamCompletedArgs : StreamOpenedArgs
        {
            public new SubtitlesStream Stream   => (SubtitlesStream)base.Stream;
            public new SubtitlesStream OldStream=> (SubtitlesStream)base.OldStream;
            public OpenSubtitlesStreamCompletedArgs(SubtitlesStream stream = null, SubtitlesStream oldStream = null, string error = null): base(stream, oldStream, error) { }
        }
        public class ExternalStreamOpenedArgs : EventArgs
        {
            public ExternalStream   ExtStream;
            public ExternalStream   OldExtStream;
            public string           Error;
            public bool             Success => Error == null;
            public ExternalStreamOpenedArgs(ExternalStream extStream = null, ExternalStream oldExtStream = null, string error = null) { ExtStream = extStream; OldExtStream= oldExtStream; Error = error; }
        } 
        public class OpenExternalAudioStreamCompletedArgs : ExternalStreamOpenedArgs
        {
            public new ExternalAudioStream ExtStream   => (ExternalAudioStream)base.ExtStream;
            public new ExternalAudioStream OldExtStream=> (ExternalAudioStream)base.OldExtStream;
            public OpenExternalAudioStreamCompletedArgs(ExternalAudioStream extStream = null, ExternalAudioStream oldExtStream = null, string error = null) : base(extStream, oldExtStream, error) { } 
        }
        public class OpenExternalVideoStreamCompletedArgs : ExternalStreamOpenedArgs
        {
            public new ExternalVideoStream ExtStream   => (ExternalVideoStream)base.ExtStream;
            public new ExternalVideoStream OldExtStream=> (ExternalVideoStream)base.OldExtStream;
            public OpenExternalVideoStreamCompletedArgs(ExternalVideoStream extStream = null, ExternalVideoStream oldExtStream = null, string error = null) : base(extStream, oldExtStream, error) { }
        }
        public class OpenExternalSubtitlesStreamCompletedArgs : ExternalStreamOpenedArgs
        {
            public new ExternalSubtitlesStream ExtStream   => (ExternalSubtitlesStream)base.ExtStream;
            public new ExternalSubtitlesStream OldExtStream=> (ExternalSubtitlesStream)base.OldExtStream;
            public OpenExternalSubtitlesStreamCompletedArgs(ExternalSubtitlesStream extStream = null, ExternalSubtitlesStream oldExtStream = null, string error = null) : base(extStream, oldExtStream, error) { }
        }

        private void OnOpenCompleted(OpenCompletedArgs args = null)
        {
            VideoDecoder.Renderer?.ClearScreen();
            if (CanInfo) Log.Info($"[Open] {(args.Url != null ? args.Url : "None")} {(!args.Success ? " [Error: " + args.Error  + "]": "")}");
            OpenCompleted?.Invoke(this, args);
        }
        private void OnOpenSessionCompleted(OpenSessionCompletedArgs args = null)
        {
            VideoDecoder.Renderer?.ClearScreen();
            if (CanInfo) Log.Info($"[OpenSession] {(args.Session.Url != null ? args.Session.Url : "None")} - Item: {args.Session.PlaylistItem} {(!args.Success ? " [Error: " + args.Error  + "]": "")}");
            OpenSessionCompleted?.Invoke(this, args);
        }
        private void OnOpenSubtitles(OpenSubtitlesCompletedArgs args = null)
        {
            if (CanInfo) Log.Info($"[OpenSubtitles] {(args.Url != null ? args.Url : "None")} {(!args.Success ? " [Error: " + args.Error  + "]": "")}");
            OpenSubtitlesCompleted?.Invoke(this, args);
        }
        private void OnOpenPlaylistItemCompleted(OpenPlaylistItemCompletedArgs args = null)
        {
            VideoDecoder.Renderer?.ClearScreen();
            if (CanInfo) Log.Info($"[OpenPlaylistItem] {(args.OldItem != null ? args.OldItem.Title : "None")} => {(args.Item != null ? args.Item.Title : "None")}{(!args.Success ? " [Error: " + args.Error  + "]": "")}");
            OpenPlaylistItemCompleted?.Invoke(this, args);
        }
        private void OnOpenAudioStreamCompleted(OpenAudioStreamCompletedArgs args = null)
        {
            ClosedAudioStream = null;
            MainDemuxer = !VideoDemuxer.Disposed ? VideoDemuxer : AudioDemuxer;

            if (CanInfo) Log.Info($"[OpenAudioStream] #{(args.OldStream != null ? args.OldStream.StreamIndex.ToString() : "_")} => #{(args.Stream != null ? args.Stream.StreamIndex.ToString() : "_")}{(!args.Success ? " [Error: " + args.Error  + "]": "")}");
            OpenAudioStreamCompleted?.Invoke(this, args);
        }
        private void OnOpenVideoStreamCompleted(OpenVideoStreamCompletedArgs args = null)
        {
            ClosedVideoStream = null;
            MainDemuxer = !VideoDemuxer.Disposed ? VideoDemuxer : AudioDemuxer;

            if (CanInfo) Log.Info($"[OpenVideoStream] #{(args.OldStream != null ? args.OldStream.StreamIndex.ToString() : "_")} => #{(args.Stream != null ? args.Stream.StreamIndex.ToString() : "_")}{(!args.Success ? " [Error: " + args.Error  + "]": "")}");
            OpenVideoStreamCompleted?.Invoke(this, args);
        }
        private void OnOpenSubtitlesStreamCompleted(OpenSubtitlesStreamCompletedArgs args = null)
        {
            ClosedSubtitlesStream = null;

            if (CanInfo) Log.Info($"[OpenSubtitlesStream] #{(args.OldStream != null ? args.OldStream.StreamIndex.ToString() : "_")} => #{(args.Stream != null ? args.Stream.StreamIndex.ToString() : "_")}{(!args.Success ? " [Error: " + args.Error  + "]": "")}");
            OpenSubtitlesStreamCompleted?.Invoke(this, args);
        }
        private void OnOpenExternalAudioStreamCompleted(OpenExternalAudioStreamCompletedArgs args = null)
        {
            ClosedAudioStream = null;
            MainDemuxer = !VideoDemuxer.Disposed ? VideoDemuxer : AudioDemuxer;

            if (CanInfo) Log.Info($"[OpenExternalAudioStream] {(args.OldExtStream != null ? args.OldExtStream.Url : "None")} => {(args.ExtStream != null ? args.ExtStream.Url : "None")}{(!args.Success ? " [Error: " + args.Error  + "]": "")}");
            OpenExternalAudioStreamCompleted?.Invoke(this, args);
        }
        private void OnOpenExternalVideoStreamCompleted(OpenExternalVideoStreamCompletedArgs args = null)
        {
            ClosedVideoStream = null;
            MainDemuxer = !VideoDemuxer.Disposed ? VideoDemuxer : AudioDemuxer;

            if (CanInfo) Log.Info($"[OpenExternalVideoStream] {(args.OldExtStream != null ? args.OldExtStream.Url : "None")} => {(args.ExtStream != null ? args.ExtStream.Url : "None")}{(!args.Success ? " [Error: " + args.Error  + "]": "")}");
            OpenExternalVideoStreamCompleted?.Invoke(this, args);
        }
        private void OnOpenExternalSubtitlesStreamCompleted(OpenExternalSubtitlesStreamCompletedArgs args = null)
        {
            ClosedSubtitlesStream = null;

            if (CanInfo) Log.Info($"[OpenExternalSubtitlesStream] {(args.OldExtStream != null ? args.OldExtStream.Url : "None")} => {(args.ExtStream != null ? args.ExtStream.Url : "None")}{(!args.Success ? " [Error: " + args.Error  + "]": "")}");
            OpenExternalSubtitlesStreamCompleted?.Invoke(this, args);
        }
        #endregion

        #region Open
        public OpenCompletedArgs Open(string url, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        {
            return Open((object)url, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);
        }
        public OpenCompletedArgs Open(Stream iostream, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        {
            return Open((object)iostream, defaultPlaylistItem, defaultVideo, defaultAudio, defaultSubtitles);
        }

        internal OpenCompletedArgs Open(object input, bool defaultPlaylistItem = true, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        {
            OpenCompletedArgs args = new OpenCompletedArgs();

            try
            {
                Initialize();

                if (input is Stream)
                {
                    Playlist.IOStream = (Stream)input;
                }
                else
                    Playlist.Url = input.ToString(); // TBR: check UI update

                args.Url = Playlist.Url;
                args.IOStream = Playlist.IOStream;
                args.Error = Open().Error;

                if (Playlist.Items.Count == 0 && args.Success)
                    args.Error = "No playlist items were found";

                if (!args.Success)
                    return args;

                if (!defaultPlaylistItem)
                    return args;

                args.Error = Open(SuggestItem(), defaultVideo, defaultAudio, defaultSubtitles).Error;

                return args;

            } catch (Exception e)
            {
                args.Error = !args.Success ? args.Error + "\r\n" + e.Message : e.Message;
                return args;
            }
            finally
            {
                OnOpenCompleted(args);
            }
        }
        public new OpenSubtitlesCompletedArgs OpenSubtitles(string url)
        {
            OpenSubtitlesCompletedArgs args = new OpenSubtitlesCompletedArgs();

            try
            {
                var res = base.OpenSubtitles(url);
                args.Error = res == null ? "No external subtitles stream found" : res.Error;

                if (args.Success)
                    args.Error = Open(res.ExternalSubtitlesStream).Error;

                return args;

            } catch (Exception e)
            {
                args.Error = !args.Success ? args.Error + "\r\n" + e.Message : e.Message;
                return args;
            }
            finally
            {
                OnOpenSubtitles(args);
            }
        }
        public OpenSessionCompletedArgs Open(Session session)
        {
            OpenSessionCompletedArgs args = new OpenSessionCompletedArgs(session);

            try
            {
                // Open
                if (session.Url != null && session.Url != Playlist.Url) // && session.Url != Playlist.DirectUrl)
                {
                    args.Error = Open(session.Url, false, false, false, false).Error;
                    if (!args.Success)
                        return args;
                }

                // Open Item
                if (session.PlaylistItem != -1)
                {
                    args.Error = Open(Playlist.Items[session.PlaylistItem], false, false, false).Error;
                    if (!args.Success)
                        return args;
                }

                // Open Streams
                if (session.ExternalVideoStream != -1)
                {
                    args.Error = Open(Playlist.Selected.ExternalVideoStreams[session.ExternalVideoStream], false, session.VideoStream).Error;
                    if (!args.Success)
                        return args;
                }
                else if (session.VideoStream != -1)
                {
                    args.Error = Open(VideoDemuxer.AVStreamToStream[session.VideoStream], false).Error;
                    if (!args.Success)
                        return args;
                }

                string tmpErr = null;
                if (session.ExternalAudioStream != -1)
                    tmpErr = Open(Playlist.Selected.ExternalAudioStreams[session.ExternalAudioStream], false, session.AudioStream).Error;
                else if (session.AudioStream != -1)
                    tmpErr = Open(VideoDemuxer.AVStreamToStream[session.AudioStream], false).Error;

                if (tmpErr != null & VideoStream == null)
                {
                    args.Error = tmpErr;
                    return args;
                }

                if (session.ExternalSubtitlesUrl != null)
                    OpenSubtitles(session.ExternalSubtitlesUrl);
                else if (session.SubtitlesStream != -1)
                    Open(VideoDemuxer.AVStreamToStream[session.SubtitlesStream]);

                Config.Audio.SetDelay(session.AudioDelay);
                Config.Subtitles.SetDelay(session.SubtitlesDelay);

                if (session.CurTime > 1 * (long)1000 * 10000)
                    Seek(session.CurTime / 10000);

                return args;
            } catch (Exception e)
            {
                args.Error = !args.Success ? args.Error + "\r\n" + e.Message : e.Message;
                return args;
            } finally
            {
                OnOpenSessionCompleted(args);
            }
        }
        public OpenPlaylistItemCompletedArgs Open(PlaylistItem item, bool defaultVideo = true, bool defaultAudio = true, bool defaultSubtitles = true)
        {
            OpenPlaylistItemCompletedArgs args = new OpenPlaylistItemCompletedArgs(item);
            
            try
            {
                long stoppedTime = GetCurTime();
                InitializeSwitch();

                // Disables old item
                if (Playlist.Selected != null)
                {
                    args.OldItem = Playlist.Selected;
                    Playlist.Selected.Enabled = false;
                }

                Playlist.Selected = item;
                Playlist.Selected.Enabled = true;

                // We reset external streams of the current item and not the old one
                if (Playlist.Selected.ExternalAudioStream != null)
                {
                    Playlist.Selected.ExternalAudioStream.Enabled = false;
                    Playlist.Selected.ExternalAudioStream = null;
                }

                if (Playlist.Selected.ExternalVideoStream != null)
                {
                    Playlist.Selected.ExternalVideoStream.Enabled = false;
                    Playlist.Selected.ExternalVideoStream = null;
                }

                if (Playlist.Selected.ExternalSubtitlesStream != null)
                {
                    Playlist.Selected.ExternalSubtitlesStream.Enabled = false;
                    Playlist.Selected.ExternalSubtitlesStream = null;
                }

                args.Error = OpenItem().Error;

                if (!args.Success)
                    return args;

                if (Playlist.Selected.Url != null || Playlist.Selected.IOStream != null)
                    args.Error = OpenDemuxerInput(VideoDemuxer, Playlist.Selected);
                
                if (!args.Success)
                    return args;

                if (defaultVideo && Config.Video.Enabled)
                    args.Error = OpenSuggestedVideo(defaultAudio);
                else if (defaultAudio && Config.Audio.Enabled)
                    args.Error = OpenSuggestedAudio();

                if ((defaultVideo || defaultAudio) && AudioStream == null && VideoStream == null)
                {
                    if (args.Error == null)
                        args.Error = "No audio/video found";

                    return args;
                }

                if (defaultSubtitles && Config.Subtitles.Enabled)
                {
                    if (Playlist.Selected.ExternalSubtitlesStream != null)
                        Open(Playlist.Selected.ExternalSubtitlesStream);
                    else
                        OpenSuggestedSubtitles();
                }

                return args;
            } catch (Exception e)
            {
                args.Error = !args.Success ? args.Error + "\r\n" + e.Message : e.Message;
                return args;
            } finally
            {
                OnOpenPlaylistItemCompleted(args);
            }
        }
        public ExternalStreamOpenedArgs Open(ExternalStream extStream, bool defaultAudio = false, int streamIndex = -1) // -2: None, -1: Suggest, >=0: specified
        {
            ExternalStreamOpenedArgs args = null;

            try
            {
                Demuxer demuxer;

                if (extStream is ExternalVideoStream)
                {
                    args = new OpenExternalVideoStreamCompletedArgs((ExternalVideoStream) extStream, Playlist.Selected.ExternalVideoStream);

                    if (args.OldExtStream != null)
                        args.OldExtStream.Enabled = false;

                    Playlist.Selected.ExternalVideoStream = (ExternalVideoStream) extStream;

                    foreach(var plugin in Plugins.Values)
                        plugin.OnOpenExternalVideo();

                    demuxer = VideoDemuxer;
                }
                else if (extStream is ExternalAudioStream)
                {
                    args = new OpenExternalAudioStreamCompletedArgs((ExternalAudioStream) extStream, Playlist.Selected.ExternalAudioStream);

                    if (args.OldExtStream != null)
                        args.OldExtStream.Enabled = false;

                    Playlist.Selected.ExternalAudioStream = (ExternalAudioStream) extStream;

                    foreach(var plugin in Plugins.Values)
                        plugin.OnOpenExternalAudio();

                    demuxer = AudioDemuxer;
                }
                else
                {
                    args = new OpenExternalSubtitlesStreamCompletedArgs((ExternalSubtitlesStream) extStream, Playlist.Selected.ExternalSubtitlesStream);

                    if (args.OldExtStream != null)
                        args.OldExtStream.Enabled = false;

                    Playlist.Selected.ExternalSubtitlesStream = (ExternalSubtitlesStream) extStream;

                    if (!Playlist.Selected.ExternalSubtitlesStream.Downloaded)
                        DownloadSubtitles(Playlist.Selected.ExternalSubtitlesStream);
                        
                    foreach(var plugin in Plugins.Values)
                        plugin.OnOpenExternalSubtitles();

                    demuxer = SubtitlesDemuxer;
                }

                // Open external stream
                args.Error = OpenDemuxerInput(demuxer, extStream);

                if (!args.Success)
                    return args;

                // Update embedded streams with the external stream pointer
                foreach (var embStream in demuxer.VideoStreams)
                    embStream.ExternalStream = extStream;
                foreach (var embStream in demuxer.AudioStreams)
                    embStream.ExternalStream = extStream;
                foreach (var embStream in demuxer.SubtitlesStreams)
                    embStream.ExternalStream = extStream;

                // Open embedded stream
                if (streamIndex != -2)
                {
                    StreamBase suggestedStream = null;
                    if (streamIndex != -1 && (streamIndex >= demuxer.AVStreamToStream.Count || streamIndex < 0 || demuxer.AVStreamToStream[streamIndex].Type != extStream.Type))
                    {
                        args.Error = $"Invalid stream index {streamIndex}";
                        demuxer.Dispose();
                        return args;
                    }

                    if (demuxer.Type == MediaType.Video)
                        suggestedStream = streamIndex == -1 ? SuggestVideo(demuxer.VideoStreams) : demuxer.AVStreamToStream[streamIndex];
                    else if (demuxer.Type == MediaType.Audio)
                        suggestedStream = streamIndex == -1 ? SuggestAudio(demuxer.AudioStreams) : demuxer.AVStreamToStream[streamIndex];
                    else
                    {
                        var langs = Config.Subtitles.Languages.ToList();
                        langs.Add(Language.Unknown);
                        suggestedStream = streamIndex == -1 ? SuggestSubtitles(demuxer.SubtitlesStreams, langs) : demuxer.AVStreamToStream[streamIndex];
                    }

                    if (suggestedStream == null)
                    {
                        demuxer.Dispose();
                        args.Error = "No embedded streams found";
                        return args;
                    }

                    args.Error = Open(suggestedStream, defaultAudio).Error;
                    if (!args.Success)
                        return args;
                }

                extStream.Enabled = true;

                return args;
            } catch (Exception e)
            {
                args.Error = !args.Success ? args.Error + "\r\n" + e.Message : e.Message;
                return args;
            } finally
            {
                if (extStream is ExternalVideoStream)
                    OnOpenExternalVideoStreamCompleted((OpenExternalVideoStreamCompletedArgs)args);
                else if (extStream is ExternalAudioStream)
                    OnOpenExternalAudioStreamCompleted((OpenExternalAudioStreamCompletedArgs)args);
                else
                    OnOpenExternalSubtitlesStreamCompleted((OpenExternalSubtitlesStreamCompletedArgs)args);
            }
        }

        public StreamOpenedArgs OpenVideoStream(VideoStream stream, bool defaultAudio = true)
        {
            return Open(stream, defaultAudio);
        }
        public StreamOpenedArgs OpenAudioStream(AudioStream stream)
        {
            return Open(stream);
        }
        public StreamOpenedArgs OpenSubtitlesStream(SubtitlesStream stream)
        {
            return Open(stream);
        }
        private StreamOpenedArgs Open(StreamBase stream, bool defaultAudio = false)
        {
            StreamOpenedArgs args = null;
            
            try
            {
                lock (stream.Demuxer.lockFmtCtx)
                {
                    StreamBase oldStream = stream.Type == MediaType.Video ? (StreamBase)VideoStream : (stream.Type == MediaType.Audio ? (StreamBase)AudioStream : (StreamBase)SubtitlesStream);

                    // Close external demuxers when opening embedded
                    if (stream.Demuxer.Type == MediaType.Video)
                    {
                        // TBR: if (stream.Type == MediaType.Video) | We consider that we can't have Embedded and External Video Streams at the same time
                        if (stream.Type == MediaType.Audio) // TBR: && VideoStream != null)
                        {
                            if (!EnableDecoding) AudioDemuxer.Dispose();
                            if (Playlist.Selected.ExternalAudioStream != null)
                            {
                                Playlist.Selected.ExternalAudioStream.Enabled = false;
                                Playlist.Selected.ExternalAudioStream = null;
                            }
                        }
                        else if (stream.Type == MediaType.Subs)
                        {
                            if (!EnableDecoding) SubtitlesDemuxer.Dispose();
                            if (Playlist.Selected.ExternalSubtitlesStream != null)
                            {
                                Playlist.Selected.ExternalSubtitlesStream.Enabled = false;
                                Playlist.Selected.ExternalSubtitlesStream = null;
                            }
                        }
                    }
                    else if (!EnableDecoding)
                    {
                        // Disable embeded audio when enabling external audio (TBR)
                        if (stream.Demuxer.Type == MediaType.Audio && stream.Type == MediaType.Audio && AudioStream != null && AudioStream.Demuxer.Type == MediaType.Video)
                        {
                            foreach (var aStream in VideoDemuxer.AudioStreams)
                                VideoDemuxer.DisableStream(aStream);
                        }
                    }

                    // Open Codec / Enable on demuxer
                    if (EnableDecoding)
                    {
                        string ret = GetDecoderPtr(stream.Type).Open(stream);

                        if (ret != null)
                        {
                            if (stream.Type == MediaType.Video)
                                return args = new OpenVideoStreamCompletedArgs((VideoStream)stream, (VideoStream)oldStream, $"Failed to open video stream #{stream.StreamIndex}\r\n{ret}");
                            else if (stream.Type == MediaType.Audio)
                                return args = new OpenAudioStreamCompletedArgs((AudioStream)stream, (AudioStream)oldStream, $"Failed to open audio stream #{stream.StreamIndex}\r\n{ret}");
                            else
                                return args = new OpenSubtitlesStreamCompletedArgs((SubtitlesStream)stream, (SubtitlesStream)oldStream, $"Failed to open subtitles stream #{stream.StreamIndex}\r\n{ret}");
                        }
                    }
                    else
                        stream.Demuxer.EnableStream(stream);

                    // Open Audio based on new Video Stream (if not the same suggestion)
                    if (defaultAudio && stream.Type == MediaType.Video && Config.Audio.Enabled)
                    {
                        bool requiresChange = true;
                        SuggestAudio(out AudioStream aStream, out ExternalAudioStream aExtStream, VideoDemuxer.AudioStreams);

                        if (AudioStream != null)
                        {
                            // External audio streams comparison
                            if (Playlist.Selected.ExternalAudioStream != null && aExtStream != null && aExtStream.Index == Playlist.Selected.ExternalAudioStream.Index)
                                requiresChange = false;
                            // Embedded audio streams comparison
                            else if (Playlist.Selected.ExternalAudioStream == null && aStream != null && aStream.StreamIndex == AudioStream.StreamIndex)
                                requiresChange = false;
                        }

                        if (!requiresChange)
                        {
                            if (CanDebug) Log.Debug($"Audio no need to follow video");
                        }
                        else
                        {
                             if (aStream != null)
                                Open(aStream);
                            else if (aExtStream != null)
                                Open(aExtStream);

                             //RequiresResync = true;
                        }
                    }

                    if (stream.Type == MediaType.Video)
                        return args = new OpenVideoStreamCompletedArgs((VideoStream)stream, (VideoStream)oldStream);
                    else if (stream.Type == MediaType.Audio)
                        return args = new OpenAudioStreamCompletedArgs((AudioStream)stream, (AudioStream)oldStream);
                    else
                        return args = new OpenSubtitlesStreamCompletedArgs((SubtitlesStream)stream, (SubtitlesStream)oldStream);
                }
            } catch(Exception e)
            {
                return args = new StreamOpenedArgs(null, null, e.Message);
            } finally
            {
                if (stream.Type == MediaType.Video)
                    OnOpenVideoStreamCompleted((OpenVideoStreamCompletedArgs)args);
                else if (stream.Type == MediaType.Audio)
                    OnOpenAudioStreamCompleted((OpenAudioStreamCompletedArgs)args);
                else
                    OnOpenSubtitlesStreamCompleted((OpenSubtitlesStreamCompletedArgs)args);
            }
        }

        public string OpenSuggestedVideo(bool defaultAudio = false)
        {
            VideoStream stream;
            ExternalVideoStream extStream;
            string error = null;

            if (ClosedVideoStream != null)
            {
                Log.Debug("[Video] Found previously closed stream");

                extStream = ClosedVideoStream.Item1;
                if (extStream != null)
                    return Open(extStream, false, ClosedVideoStream.Item2 >= 0 ? ClosedVideoStream.Item2 : -1).Error;

                stream = ClosedVideoStream.Item2 >= 0 ? (VideoStream)VideoDemuxer.AVStreamToStream[ClosedVideoStream.Item2] : null;
            }
            else
                SuggestVideo(out stream, out extStream, VideoDemuxer.VideoStreams);

            if (stream != null)
                error = Open(stream, defaultAudio).Error;
            else if (extStream != null)
                error = Open(extStream, defaultAudio).Error;
            else if (defaultAudio)
                error = OpenSuggestedAudio(); // We still need audio if no video exists

            return error;
        }
        public string OpenSuggestedAudio()
        {
            AudioStream stream = null;
            ExternalAudioStream extStream = null;
            string error = null;

            if (ClosedAudioStream != null)
            {
                Log.Debug("[Audio] Found previously closed stream");

                extStream = ClosedAudioStream.Item1;
                if (extStream != null)
                    return Open(extStream, false, ClosedAudioStream.Item2 >= 0 ? ClosedAudioStream.Item2 : -1).Error;

                stream = ClosedAudioStream.Item2 >= 0 ? (AudioStream)VideoDemuxer.AVStreamToStream[ClosedAudioStream.Item2] : null;
            }
            else
                SuggestAudio(out stream, out extStream, VideoDemuxer.AudioStreams);

            if (stream != null)
                error = Open(stream).Error;
            else if (extStream != null)
                error = Open(extStream).Error;

            return error;
        }
        public void OpenSuggestedSubtitles()
        {
            long sessionId = OpenItemCounter;

            try
            {
                // 1. Closed / History / Remember last user selection? Probably application must handle this
                if (ClosedSubtitlesStream != null)
                {
                    Log.Debug("[Subtitles] Found previously closed stream");

                    ExternalSubtitlesStream extStream = ClosedSubtitlesStream.Item1;
                    if (extStream != null)
                    {
                        Open(extStream, false, ClosedSubtitlesStream.Item2 >= 0 ? ClosedSubtitlesStream.Item2 : -1);
                        return;
                    }

                    SubtitlesStream stream = ClosedSubtitlesStream.Item2 >= 0 ? (SubtitlesStream)VideoDemuxer.AVStreamToStream[ClosedSubtitlesStream.Item2] : null;

                    if (stream != null)
                    {
                        Open(stream);
                        return;
                    }
                    else if (extStream != null)
                    {
                        Open(extStream);
                        return;
                    }
                    else
                        ClosedSubtitlesStream = null;
                }

                // High Suggest (first lang priority + high rating + already converted/downloaded)
                // 2. Check embedded steams for high suggest
                if (Config.Subtitles.Languages.Count > 0)
                {
                    foreach (var stream in VideoDemuxer.SubtitlesStreams)
                        if (stream.Language == Config.Subtitles.Languages[0])
                        {
                            Log.Debug("[Subtitles] Found high suggested embedded stream");
                            Open(stream);
                            return;
                        }
                }

                // 3. Check external streams for high suggest
                if (Playlist.Selected.ExternalSubtitlesStreams.Count > 0)
                {
                    ExternalSubtitlesStream extStream = SuggestBestExternalSubtitles();
                    if (extStream != null)
                    {
                        Log.Debug("[Subtitles] Found high suggested external stream");
                        Open(extStream);
                        return;
                    }
                }

            } catch (Exception e)
            {
                Log.Debug($"OpenSuggestedSubtitles canceled? [{e.Message}]");
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    if (sessionId != OpenItemCounter)
                    {
                        Log.Debug("OpenSuggestedSubtitles canceled");
                        return;
                    }

                    ExternalSubtitlesStream extStream;

                    // 4. Search offline if allowed (not async)
                    if (!Playlist.Selected.SearchedLocal && Config.Subtitles.SearchLocal && (Config.Subtitles.SearchLocalOnInputType == null || Config.Subtitles.SearchLocalOnInputType.Count == 0 || Config.Subtitles.SearchLocalOnInputType.Contains(Playlist.InputType)))
                    {
                        Log.Debug("[Subtitles] Searching Local");
                        SearchLocalSubtitles();

                        // 4.1 Check external streams for high suggest (again for the new additions if any)
                        extStream = SuggestBestExternalSubtitles();
                        if (extStream != null)
                        {
                            Log.Debug("[Subtitles] Found high suggested external stream");
                            Open(extStream);
                            return;
                        }
                    }

                    if (sessionId != OpenItemCounter)
                    {
                        Log.Debug("OpenSuggestedSubtitles canceled");
                        return;
                    }

                    // 5. Search online if allowed (not async)
                    if (!Playlist.Selected.SearchedOnline && Config.Subtitles.SearchOnline && (Config.Subtitles.SearchOnlineOnInputType == null || Config.Subtitles.SearchOnlineOnInputType.Count == 0 || Config.Subtitles.SearchOnlineOnInputType.Contains(Playlist.InputType)))
                    {
                        Log.Debug("[Subtitles] Searching Online");
                        SearchOnlineSubtitles();
                    }

                    if (sessionId != OpenItemCounter)
                    {
                        Log.Debug("OpenSuggestedSubtitles canceled");
                        return;
                    }

                    // 6. (Any) Check embedded/external streams for config languages (including 'undefined')
                    SuggestSubtitles(out SubtitlesStream stream, out extStream);

                    if (stream != null)
                        Open(stream);
                    else if (extStream != null)
                        Open(extStream);
                } catch (Exception e)
                {
                    Log.Debug($"OpenSuggestedSubtitles canceled? [{e.Message}]");
                }
            });
        }

        public string OpenDemuxerInput(Demuxer demuxer, DemuxerInput demuxerInput)
        {
            OpenedPlugin?.OnBuffering();

            string error = null;

            SerializableDictionary<string, string> formatOpt = null;
            SerializableDictionary<string, string> copied = null;

            try
            {
                // Set HTTP Config
                if (Playlist.InputType == InputType.Web)
                {
                    formatOpt = Config.Demuxer.GetFormatOptPtr(demuxer.Type);
                    copied = new SerializableDictionary<string, string>();

                    foreach (var opt in formatOpt)
                        copied.Add(opt.Key, opt.Value);

                    if (demuxerInput.UserAgent != null)
                        formatOpt["user_agent"] = demuxerInput.UserAgent;

                    if (demuxerInput.Referrer != null)
                        formatOpt["referer"] = demuxerInput.Referrer;
                    else if (!formatOpt.ContainsKey("referer") && Playlist.Url != null)
                        formatOpt["referer"] = Playlist.Url;

                    if (demuxerInput.HTTPHeaders != null)
                    {
                        formatOpt["headers"] = "";
                        foreach(var header in demuxerInput.HTTPHeaders)
                            formatOpt["headers"] += header.Key + ": " + header.Value + "\r\n";
                    }
                }

                // Open Demuxer Input
                if (demuxerInput.Url != null)
                {
                    error = demuxer.Open(demuxerInput.Url);
                    if (error != null)
                        return error;

                    if (!string.IsNullOrEmpty(demuxerInput.UrlFallback))
                    {
                        Log.Warn($"Fallback to {demuxerInput.UrlFallback}");
                        error = demuxer.Open(demuxerInput.UrlFallback);
                    }
                }
                else if (demuxerInput.IOStream != null)
                    error = demuxer.Open(demuxerInput.IOStream);

                return error;
            } finally
            {
                // Restore HTTP Config
                if (Playlist.InputType == InputType.Web)
                {
                    formatOpt.Clear();
                    foreach(var opt in copied)
                        formatOpt.Add(opt.Key, opt.Value);
                }

                OpenedPlugin?.OnBufferingCompleted();
            }
        }
        #endregion

        #region Close (Only For EnableDecoding)
        public void CloseAudio()
        {
            ClosedAudioStream = new Tuple<ExternalAudioStream, int>(Playlist.Selected.ExternalAudioStream, AudioStream != null ? AudioStream.StreamIndex : -1);

            if (Playlist.Selected.ExternalAudioStream != null)
            {
                Playlist.Selected.ExternalAudioStream.Enabled = false;
                Playlist.Selected.ExternalAudioStream = null;
            }

            AudioDecoder.Dispose(true);
        }
        public void CloseVideo()
        {
            ClosedVideoStream = new Tuple<ExternalVideoStream, int>(Playlist.Selected.ExternalVideoStream, VideoStream != null ? VideoStream.StreamIndex : -1);

            if (Playlist.Selected.ExternalVideoStream != null)
            {
                Playlist.Selected.ExternalVideoStream.Enabled = false;
                Playlist.Selected.ExternalVideoStream = null;
            }

            VideoDecoder.Dispose(true);
            VideoDecoder.Renderer?.ClearScreen();
        }
        public void CloseSubtitles()
        {
            ClosedSubtitlesStream = new Tuple<ExternalSubtitlesStream, int>(Playlist.Selected.ExternalSubtitlesStream, SubtitlesStream != null ? SubtitlesStream.StreamIndex : -1);

            if (Playlist.Selected.ExternalSubtitlesStream != null)
            {
                Playlist.Selected.ExternalSubtitlesStream.Enabled = false;
                Playlist.Selected.ExternalSubtitlesStream = null;
            }

            SubtitlesDecoder.Dispose(true);
        }
        #endregion
    }
}
