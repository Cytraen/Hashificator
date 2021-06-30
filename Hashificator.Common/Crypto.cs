using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Hashificator.Common
{
    public class Crypto
    {
        public static HashCollection CalculateHashes(string path, HashSelection hashes, int maxThreads)
        {
            var hashDict = new Dictionary<string, IDigest>();
            var results = new HashCollection();

            if (hashes.MD2) hashDict["MD2"] = new MD2Digest();
            if (hashes.MD4) hashDict["MD4"] = new MD4Digest();
            if (hashes.MD5) hashDict["MD5"] = new MD5Digest();

            if (hashes.Sha1) hashDict["Sha1"] = new Sha1Digest();
            if (hashes.Sha224) hashDict["Sha224"] = new Sha224Digest();
            if (hashes.Sha256) hashDict["Sha256"] = new Sha256Digest();
            if (hashes.Sha384) hashDict["Sha384"] = new Sha384Digest();
            if (hashes.Sha512) hashDict["Sha512"] = new Sha512Digest();

            if (hashes.Sha3_224) hashDict["Sha3_224"] = new Sha3Digest(224);
            if (hashes.Sha3_256) hashDict["Sha3_256"] = new Sha3Digest(256);
            if (hashes.Sha3_384) hashDict["Sha3_384"] = new Sha3Digest(384);
            if (hashes.Sha3_512) hashDict["Sha3_512"] = new Sha3Digest(512);

            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Delete | FileShare.ReadWrite))
            {
                byte[] buffer = new byte[4096];
                int bytesRead;

                while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
                {
                    _ = Parallel.ForEach(hashDict, new ParallelOptions { MaxDegreeOfParallelism = maxThreads }, hashPair =>
                    {
                        hashPair.Value.BlockUpdate(buffer, 0, bytesRead);
                    });
                }

                foreach (var (method, hash) in hashDict)
                {
                    var result = new byte[hash.GetDigestSize()];
                    _ = hash.DoFinal(result, 0);
                    var resultString = BitConverter.ToString(result).Replace("-", "");

                    switch (method)
                    {
                        case "MD2": results.MD2 = resultString; break;
                        case "MD4": results.MD4 = resultString; break;
                        case "MD5": results.MD5 = resultString; break;

                        case "Sha1": results.Sha1 = resultString; break;
                        case "Sha224": results.Sha224 = resultString; break;
                        case "Sha256": results.Sha256 = resultString; break;
                        case "Sha384": results.Sha384 = resultString; break;
                        case "Sha512": results.Sha512 = resultString; break;

                        case "Sha3_224": results.Sha3_224 = resultString; break;
                        case "Sha3_256": results.Sha3_256 = resultString; break;
                        case "Sha3_384": results.Sha3_384 = resultString; break;
                        case "Sha3_512": results.Sha3_512 = resultString; break;

                        default:
                            throw new NotSupportedException();
                    }
                }
            }
            return results;
        }
    }
}