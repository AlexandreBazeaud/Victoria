using System;
using System.Linq;
using System.Text;

namespace Victoria.Encoder {
    /// <summary>
    /// Helper class for encoding Lavalink's <see cref="LavaTrack"/>.
    /// </summary>
    public readonly struct TrackEncoder {
        private const int TRACK_VERSIONED = 1;
        private const int TRACK_VERSION = 2;

        /// <summary>
        ///     Encodes the specified track and sets its hash.
        /// </summary>
        /// <param name="track">The <see cref="LavaTrack"/> to encode.</param>
        /// <remarks>
        ///     This method sets the track's hash in place.
        /// </remarks>
        public static void Encode(LavaTrack track) {
            Span<byte> bytes = stackalloc byte[GetMaxByteCount(track)];

            var javaWriter = new JavaBinaryWriter(bytes);
            javaWriter.Write<byte>(TRACK_VERSION);
            javaWriter.Write(track.Title);
            javaWriter.Write(track.Author);
            javaWriter.Write((long)track.Duration.TotalMilliseconds);
            javaWriter.Write(track.Id);
            javaWriter.Write(track.IsStream);
            javaWriter.WriteNullableText(track.Url);
            javaWriter.Write(track.Source);
            javaWriter.Write((long)track.Position.TotalMilliseconds);
            javaWriter.WriteVersioned(TRACK_VERSIONED);

            track.Hash = Convert.ToBase64String(bytes[..javaWriter.Length]);
        }

        private static int GetMaxByteCount(LavaTrack track) {
            return 23 + typeof(LavaTrack).GetProperties()
                .Where(p => p.PropertyType == typeof(string) && p.Name != "Hash")
                .Sum(p => 2 + Encoding.UTF8.GetByteCount(p.GetValue(track).ToString()));
        }
    }
}
