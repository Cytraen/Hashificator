using Org.BouncyCastle.Crypto.Digests;

namespace Hashificator.Common;

internal class HashCalculation
{
    internal MD5Digest MD5;
    internal Sha1Digest Sha1;
    internal Sha224Digest Sha224;
    internal Sha256Digest Sha256;

    internal Sha512tDigest Sha512_224;
    internal Sha512tDigest Sha512_256;
    internal Sha384Digest Sha384;
    internal Sha512Digest Sha512;

    internal Sha3Digest Sha3_224;
    internal Sha3Digest Sha3_256;
    internal Sha3Digest Sha3_384;
    internal Sha3Digest Sha3_512;

    internal Blake2sDigest Blake2s;
    internal Blake2bDigest Blake2b;
    internal Blake3.Hasher? Blake3;
}