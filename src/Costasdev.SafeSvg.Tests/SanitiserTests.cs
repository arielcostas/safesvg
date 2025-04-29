namespace Costasdev.SafeSvg.Tests;

[TestClass]
public sealed class SanitiserTests
{
    private static SanitiserOptions DefaultOptions => new()
    {
        AddNamespace = true,
        IndentOutput = false,
        RemoveComments = true
    };
    
    [TestMethod]
    public void GoodSvgDoesntChange()
    {
        // Arrange
        const string goodSvg = "<svg xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        
        // Act
        var actual = SvgSanitiser.Sanitise(goodSvg, DefaultOptions);
        
        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(goodSvg, actual);
    }
    
    [TestMethod]
    public void SvgMissesNamespace()
    {
        // Arrange
        const string input = "<svg><path d=\"M10 10 H 90 V 90 H 10 L 10 10\"/></svg>";
        const string expected = "<svg xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        
        // Act
        var actual = SvgSanitiser.Sanitise(input, DefaultOptions);
        
        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }
    
    [TestMethod]
    public void RootNamespacesShouldBeRemoved()
    {
        // Arrange
        const string input = "<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\"><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        const string expected = "<svg xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        
        // Act
        var actual = SvgSanitiser.Sanitise(input, DefaultOptions);
        
        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestRemoveScriptTag()
    {
        // Arrange
        const string input = "<svg xmlns=\"http://www.w3.org/2000/svg\"><script>alert('XSS');</script><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        const string expected = "<svg xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        
        // Act
        var actual = SvgSanitiser.Sanitise(input, DefaultOptions);
        
        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }
    
    [TestMethod]
    public void TestRemoveScriptTagWithNamespace()
    {
        // Arrange
        const string input = "<svg xmlns=\"http://www.w3.org/2000/svg\"><script xmlns=\"http://www.w3.org/2000/svg\">alert('XSS');</script><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        const string expected = "<svg xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        
        // Act
        var actual = SvgSanitiser.Sanitise(input, DefaultOptions);
        
        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestRemoveXhtmlScript()
    {
        // Arrange
        const string input = "<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\"><xhtml:script>alert('XSS');</xhtml:script><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        const string expected = "<svg xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        
        // Act
        var actual = SvgSanitiser.Sanitise(input, DefaultOptions);
        
        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestRemoveJavascriptListener()
    {
        // Arrange
        const string input = "<svg xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" onmouseover=\"alert('XSS');\" /></svg>";
        const string expected = "<svg xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M10 10 H 90 V 90 H 10 L 10 10\" /></svg>";
        
        // Act
        var actual = SvgSanitiser.Sanitise(input, DefaultOptions);
        
        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }
}