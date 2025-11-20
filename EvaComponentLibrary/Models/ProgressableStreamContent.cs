using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EvaComponentLibrary.Models
{
    public class ProgressableStreamContent : HttpContent
    {
        private const int DefaultBufferSize = 4096;
        private readonly Stream _content;
        public Stream Content { get { return _content; } }
        private readonly int _bufferSize;
        private readonly Action<long, long> _progressCallback; // Callback for reporting progress
        private bool _contentConsumed;

        public ProgressableStreamContent(Stream content, int bufferSize, Action<long, long> progressCallback)
        {
            _content = content ?? throw new ArgumentNullException(nameof(content));
            _bufferSize = bufferSize <= 0 ? DefaultBufferSize : bufferSize;
            _progressCallback = progressCallback ?? throw new ArgumentNullException(nameof(progressCallback));
        }

        protected override async Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (_contentConsumed)
                throw new InvalidOperationException("The content has already been consumed.");

            _contentConsumed = true;

            var buffer = new byte[_bufferSize];
            long totalBytesRead = 0;
            var totalLength = _content.Length;

            using (_content)
            {
                while (true)
                {
                    var bytesRead = await _content.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    await stream.WriteAsync(buffer, 0, bytesRead);
                    await stream.FlushAsync();

                    totalBytesRead += bytesRead;
                    _progressCallback(totalBytesRead, totalLength); // Report progress
                }
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _content.Length;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _content.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
