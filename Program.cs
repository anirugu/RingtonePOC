using System.Diagnostics;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace RingtonePOC
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //string videoId = "SmiZ2gt5cH0";
            //await DownloadFiles(videoId);
            //GetRangeFromFile(videoId);

            await Search();
        }

        static async Task Search()
        {
            var youtube = new YoutubeClient();
            string keyword = "asp.net core";
            var result1 = youtube.Search.GetVideosAsync(keyword);

            await foreach (var result in result1)
            {
                await DownloadFiles(keyword, result.Id, false);
                //ConvertFile(result.Id);
            }
        }

        static async Task DownloadFiles(string folderName, string videoId, bool isAudio)
        {
            if (!System.IO.Directory.Exists($"Z:\\{ folderName}"))
                System.IO.Directory.CreateDirectory($"Z:\\{folderName}");
            if (!System.IO.File.Exists($"Z:\\{folderName}\\{videoId}.mp4"))
            {
                var youtube = new YoutubeClient();

                var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoId);
                IStreamInfo streamInfo;
                if (isAudio)
                {
                    streamInfo = streamManifest.GetAudioOnlyStreams().Where(x => x.Container == Container.Mp4).GetWithHighestBitrate();
                }
                else
                {
                    streamInfo = streamManifest.GetMuxedStreams().Where(x => x.Container == Container.Mp4).GetWithHighestBitrate();
                }
                await youtube.Videos.Streams.DownloadAsync(streamInfo, $"Z:\\{folderName}\\{videoId}.{streamInfo.Container}");
            }
        }

        static void ConvertFile(string videoId)
        {
            string args = $" -i {videoId}.mp4 {videoId}.wav";

            var dir = "Z:\\";
            var startInfo = new ProcessStartInfo(dir + "ffmpeg.exe");
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = dir;

            startInfo.Arguments = args;

            var processn = Process.Start(startInfo);
            processn.EnableRaisingEvents = true;

            processn.WaitForExit();
        }

        static string GetValidFileName(string illegal)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (char c in invalid)
            {
                illegal = illegal.Replace(c.ToString(), "");
            }
            return illegal;
        }

        static void GetRangeFromFile(string videoId)
        {
            string startingPoint = "00:02:11.800";
            string endPoint = "00:00:39.000";
            string newFileName = $"{GetValidFileName(videoId + startingPoint + "_" + endPoint)}.mp4";
            if (System.IO.File.Exists(newFileName))
                return;

            string args = $" -y -ss {startingPoint} -t {endPoint} -i {videoId}.mp4 {newFileName}";

            var dir = "Z:\\";
            var startInfo = new ProcessStartInfo(dir + "ffmpeg.exe");
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.WorkingDirectory = dir;

            startInfo.Arguments = args;

            var processn = Process.Start(startInfo);
            processn.EnableRaisingEvents = true;

            processn.WaitForExit();
        }

    }
}

