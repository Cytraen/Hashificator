using Org.BouncyCastle.Crypto.Digests;
using System;
using System.IO;

namespace Hashificator.Common;

public class Crypto
{
    public static HashCollection CalculateHashes(string path, HashSelection hashes, int maxThreads, int bufferSize)
    {
        var hashCalc = new HashCalculation();
        var results = new HashCollection();

        if (hashes.MD5) hashCalc.MD5 = new MD5Digest();
        if (hashes.Sha1) hashCalc.Sha1 = new Sha1Digest();
        if (hashes.Sha224) hashCalc.Sha224 = new Sha224Digest();
        if (hashes.Sha256) hashCalc.Sha256 = new Sha256Digest();

        if (hashes.Sha512_224) hashCalc.Sha512_224 = new Sha512tDigest(224);
        if (hashes.Sha512_256) hashCalc.Sha512_256 = new Sha512tDigest(256);
        if (hashes.Sha384) hashCalc.Sha384 = new Sha384Digest();
        if (hashes.Sha512) hashCalc.Sha512 = new Sha512Digest();

        if (hashes.Sha3_224) hashCalc.Sha3_224 = new Sha3Digest(224);
        if (hashes.Sha3_256) hashCalc.Sha3_256 = new Sha3Digest(256);
        if (hashes.Sha3_384) hashCalc.Sha3_384 = new Sha3Digest(384);
        if (hashes.Sha3_512) hashCalc.Sha3_512 = new Sha3Digest(512);

        if (hashes.Blake2s) hashCalc.Blake2s = new Blake2sDigest();
        if (hashes.Blake2b) hashCalc.Blake2b = new Blake2bDigest();
        if (hashes.Blake3) hashCalc.Blake3 = Blake3.Hasher.New();

        using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            while ((bytesRead = fs.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (hashCalc.MD5 is not null) { hashCalc.MD5.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Sha1 is not null) { hashCalc.Sha1.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Sha224 is not null) { hashCalc.Sha224.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Sha256 is not null) { hashCalc.Sha256.BlockUpdate(buffer, 0, bytesRead); };

                if (hashCalc.Sha512_224 is not null) { hashCalc.Sha512_224.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Sha512_256 is not null) { hashCalc.Sha512_256.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Sha384 is not null) { hashCalc.Sha384.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Sha512 is not null) { hashCalc.Sha512.BlockUpdate(buffer, 0, bytesRead); };

                if (hashCalc.Sha3_224 is not null) { hashCalc.Sha3_224.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Sha3_256 is not null) { hashCalc.Sha3_256.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Sha3_384 is not null) { hashCalc.Sha3_384.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Sha3_512 is not null) { hashCalc.Sha3_512.BlockUpdate(buffer, 0, bytesRead); };

                if (hashCalc.Blake2s is not null) { hashCalc.Blake2s.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Blake2b is not null) { hashCalc.Blake2b.BlockUpdate(buffer, 0, bytesRead); };
                if (hashCalc.Blake3 is not null) { hashCalc.Blake3.Value.Update(buffer); };
            }

            if (hashCalc.MD5 is not null)
            {
                var result = new byte[hashCalc.MD5.GetDigestSize()];
                _ = hashCalc.MD5.DoFinal(result, 0);
                results.MD5 = BitConverter.ToString(result).Replace("-", "");
            }
            if (hashCalc.Sha1 is not null)
            {
                var result = new byte[hashCalc.Sha1.GetDigestSize()];
                _ = hashCalc.Sha1.DoFinal(result, 0);
                results.Sha1 = BitConverter.ToString(result).Replace("-", "");
            }
            if (hashCalc.Sha224 is not null)
            {
                var result = new byte[hashCalc.Sha224.GetDigestSize()];
                _ = hashCalc.Sha224.DoFinal(result, 0);
                results.Sha224 = BitConverter.ToString(result).Replace("-", "");
            }
            if (hashCalc.Sha256 is not null)
            {
                var result = new byte[hashCalc.Sha256.GetDigestSize()];
                _ = hashCalc.Sha256.DoFinal(result, 0);
                results.Sha256 = BitConverter.ToString(result).Replace("-", "");
            }

            if (hashCalc.Sha512_224 is not null)
            {
                var result = new byte[hashCalc.Sha512_224.GetDigestSize()];
                _ = hashCalc.Sha512_224.DoFinal(result, 0);
                results.Sha512_224 = BitConverter.ToString(result).Replace("-", "");
            }
            if (hashCalc.Sha512_256 is not null)
            {
                var result = new byte[hashCalc.Sha512_256.GetDigestSize()];
                _ = hashCalc.Sha512_256.DoFinal(result, 0);
                results.Sha512_256 = BitConverter.ToString(result).Replace("-", "");
            }
            if (hashCalc.Sha384 is not null)
            {
                var result = new byte[hashCalc.Sha384.GetDigestSize()];
                _ = hashCalc.Sha384.DoFinal(result, 0);
                results.Sha384 = BitConverter.ToString(result).Replace("-", "");
            }
            if (hashCalc.Sha512 is not null)
            {
                var result = new byte[hashCalc.Sha512.GetDigestSize()];
                _ = hashCalc.Sha512.DoFinal(result, 0);
                results.Sha512 = BitConverter.ToString(result).Replace("-", "");
            }

            if (hashCalc.Sha3_224 is not null)
            {
                var result = new byte[hashCalc.Sha3_224.GetDigestSize()];
                _ = hashCalc.Sha3_224.DoFinal(result, 0);
                results.Sha3_224 = BitConverter.ToString(result).Replace("-", "");
            }
            if (hashCalc.Sha3_256 is not null)
            {
                var result = new byte[hashCalc.Sha3_256.GetDigestSize()];
                _ = hashCalc.Sha3_256.DoFinal(result, 0);
                results.Sha3_256 = BitConverter.ToString(result).Replace("-", "");
            }
            if (hashCalc.Sha3_384 is not null)
            {
                var result = new byte[hashCalc.Sha3_384.GetDigestSize()];
                _ = hashCalc.Sha3_384.DoFinal(result, 0);
                results.Sha3_384 = BitConverter.ToString(result).Replace("-", "");
            }
            if (hashCalc.Sha3_512 is not null)
            {
                var result = new byte[hashCalc.Sha3_512.GetDigestSize()];
                _ = hashCalc.Sha3_512.DoFinal(result, 0);
                results.Sha3_512 = BitConverter.ToString(result).Replace("-", "");
            }

            if (hashCalc.Blake2s is not null)
            {
                var result = new byte[hashCalc.Blake2s.GetDigestSize()];
                _ = hashCalc.Blake2s.DoFinal(result, 0);
                results.Blake2s = BitConverter.ToString(result).Replace("-", "");
            }
            if (hashCalc.Blake2b is not null)
            {
                var result = new byte[hashCalc.Blake2b.GetDigestSize()];
                _ = hashCalc.Blake2b.DoFinal(result, 0);
                results.Blake2b = BitConverter.ToString(result).Replace("-", "");
            }

            if (hashCalc.Blake3 != null)
            {
                results.Blake3 = hashCalc.Blake3.Value.Finalize().ToString().ToUpper();
                hashCalc.Blake3.Value.Dispose();
            }
        }
        return results;
    }
}