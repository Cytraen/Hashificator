using Hashificator.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hashificator.OptimizeTest;

internal class Program
{
    private static void Main(string[] args)
    {
        const string tinyFile = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Halo The Master Chief Collection\\mcclauncher.exe"; // 1.11 MiB
        const string smallFile = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Halo The Master Chief Collection\\halo1\\maps\\a50.map"; // 52 MiB
        const string mediumFile = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Halo The Master Chief Collection\\haloreach\\maps\\m20.map"; // 585 MiB
        const string largeFile = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Warframe\\Cache.Windows\\F.Misc.cache"; // 2.76 GiB
        const string hugeFile = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Halo The Master Chief Collection\\halo2\\preload\\paks\\shared.pck"; // 12.2 GiB

        const byte numOfRuns = 21;

        var selection = new HashSelection
        {
            Sha1 = true,
            Sha224 = true,
            Sha256 = true,
            Sha384 = true,
            Sha512 = true,
            Sha3_224 = true,
            Sha3_256 = true,
            Sha3_384 = true,
            Sha3_512 = true
        };

        var files = new string[] { tinyFile, smallFile, mediumFile };//, largeFile, hugeFile };

        var results = new Dictionary<string, Dictionary<byte, Dictionary<int, double>>>();

        foreach (var file in files)
        {
            results[file] = new Dictionary<byte, Dictionary<int, double>>();
            foreach (var threadCount in ThreadCounts())
            {
                results[file][threadCount] = new Dictionary<int, double>();
                foreach (var bufferSize in BufferSizes().Select(x => x - 1))
                {
                    var expandBufferSize = bufferSize + 1;

                    var bufferString = expandBufferSize / 1024 >= 1
                        ? expandBufferSize / 1024 / 1024 >= 1
                            ? $"{expandBufferSize / 1024 / 1024}MiB"
                            : $"{expandBufferSize / 1024}KiB"
                        : $"{expandBufferSize}B";

                    var testResults = new List<TimeSpan>();

                    for (byte i = 0; i < numOfRuns; i++)
                    {
                        Console.WriteLine($"Starting {file.Split('\\').Last()} TC: {threadCount} BS: {bufferString} run #: {i}");

                        var startTime = DateTime.UtcNow;
                        Crypto.CalculateHashes(file, selection, threadCount, bufferSize);
                        var endTime = DateTime.UtcNow;
                        var elapsed = endTime - startTime;

                        Console.WriteLine($"Completed {file.Split('\\').Last()} TC: {threadCount} BS: {bufferString} run #: {i} in {Math.Round(elapsed.TotalSeconds, 2)}");

                        if (i == 0) continue;

                        testResults.Add(elapsed);
                    }

                    results[file][threadCount][bufferSize] = testResults.Select(x => x.TotalSeconds).Average();
                }
            }
        }

        var resultDict = new Dictionary<string, List<Tuple<byte, int, double>>>();

        foreach (var (file, fileResults) in results)
        {
            var fileName = file.Split('\\').Last();
            var thisFileResults = new List<Tuple<byte, int, double>>();

            foreach (var (threadCount, threadResults) in fileResults)
            {
                foreach (var (bufferSize, time) in threadResults)
                {
                    thisFileResults.Add(new Tuple<byte, int, double>(threadCount, bufferSize, time));
                }
            }

            thisFileResults.Sort((a, b) => a.Item3.CompareTo(b.Item3));
            resultDict[fileName] = thisFileResults;
        }

        Console.WriteLine("Done. Press ENTER three times to exit.");

        _ = Console.ReadLine();
        _ = Console.ReadLine();
        _ = Console.ReadLine();
    }

    private static IEnumerable<int> BufferSizes(int minSize = 4096, int maxSize = 1024 * 1024 * 32, float multiplier = 2)
    {
        for (int i = minSize; i < maxSize + 1; i = (int)Math.Floor(i * multiplier)) { yield return i; }
    }

    private static IEnumerable<byte> ThreadCounts(byte minThreads = 1, byte maxThreads = byte.MaxValue, float multiplier = 2)
    {
        if (maxThreads > Environment.ProcessorCount) maxThreads = (byte)Environment.ProcessorCount;
        for (byte i = minThreads; i < maxThreads + 1; i = (byte)Math.Floor(i * multiplier)) { yield return i; }
    }
}