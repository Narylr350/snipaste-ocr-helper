using SnipasteOcrHelper.Watching;

namespace SnipasteOcrHelper.Tests;

public sealed class ImageFileFilterTests
{
    [Theory]
    [InlineData("capture.png")]
    [InlineData("capture.JPG")]
    [InlineData("capture.jpeg")]
    [InlineData("capture.bmp")]
    [InlineData("capture.webp")]
    public void IsSupported_ReturnsTrue_ForImageExtensions(string fileName)
    {
        Assert.True(ImageFileFilter.IsSupported(fileName));
    }

    [Theory]
    [InlineData("notes.txt")]
    [InlineData("capture.tmp")]
    [InlineData("capture")]
    public void IsSupported_ReturnsFalse_ForUnsupportedExtensions(string fileName)
    {
        Assert.False(ImageFileFilter.IsSupported(fileName));
    }
}
