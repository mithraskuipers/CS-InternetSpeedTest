using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;

namespace InternetSpeedTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string defaultUrl = "http://speedtest.ftp.otenet.gr/files/test100Mb.db";
            string url = defaultUrl;
            bool verbose = false;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--verbose":
                    {
                        verbose = true;
                        break;
                    }
                    case "--dfile":
                    {
                        if (i + 1 < args.Length)
                        {
                            url = args[i + 1];
                            i++;
                        }
                        break;
                    }
                }
            }

            Console.WriteLine($"Starting download speed test for URL: {url}");

            double downloadSpeed = await MeasureDownloadSpeedAsync(url, verbose);
            Console.WriteLine($"\nFinal running mean download speed: {downloadSpeed:F2} Mbps");

            Console.WriteLine("Internet speed test completed.");
        }
        static async Task<double> MeasureDownloadSpeedAsync(string url, bool verbose)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; InternetSpeedTest/1.0)");

                HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                long totalBytesRead = 0;
                int readCount = 0;
                double runningMeanSpeed = 0;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                long fileSizeInBytes = response.Content.Headers.ContentLength ?? -1;

                using (var contentStream = await response.Content.ReadAsStreamAsync())
                {
                    byte[] buffer = new byte[8192];
                    int bytesRead;
                    double totalElapsedSeconds = 0;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                    {
                        totalBytesRead += bytesRead;
                        readCount++;

                        double elapsedSeconds = stopwatch.Elapsed.TotalSeconds - totalElapsedSeconds;
                        totalElapsedSeconds += elapsedSeconds;

                        double currentSpeed = (bytesRead * 8) / (elapsedSeconds * 1024 * 1024);
                        runningMeanSpeed = ((runningMeanSpeed * (readCount - 1)) + currentSpeed) / readCount;

                        if (verbose)
                        {
                            Console.WriteLine($"\nRead {bytesRead} bytes, Total bytes read: {totalBytesRead}, Current running mean speed: {runningMeanSpeed:F2} Mbps");
                        }

                        PrintProgressBar(totalBytesRead, fileSizeInBytes);
                    }
                }

                stopwatch.Stop();

                return runningMeanSpeed;
            }
        }
        static void PrintProgressBar(long totalBytesRead, long fileSizeInBytes)
        {
            if (fileSizeInBytes <= 0)
            {
                return;
            }

            int progressBarWidth = 50;
            double progress = (double)totalBytesRead / fileSizeInBytes;
            int filledLength = (int)(progressBarWidth * progress);

            Console.CursorLeft = 0;
            Console.Write("[");
            Console.Write(new string('#', filledLength));
            Console.Write(new string('-', progressBarWidth - filledLength));
            Console.Write($"] {progress * 100:F2}%");
        }
    }
}
