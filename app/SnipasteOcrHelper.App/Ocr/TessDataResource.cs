using System.IO;

namespace SnipasteOcrHelper.Ocr;

public sealed record TessDataResource(string FileName, Func<Stream> OpenStream);
