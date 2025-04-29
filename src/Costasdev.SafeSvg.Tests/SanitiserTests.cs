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

    [TestMethod]
    public void TestParsingInsideGroup()
    {
        // Arrange
        // Licence: Icon sourced from https://tabler.io/icons, under the MIT licence
        const string input = "<svg    xmlns=\"http://www.w3.org/2000/svg\"    width=\"24\" height=\"24\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" class=\"icon icon-tabler icons-tabler-outline icon-tabler-file-type-svg\"> <g> <script>alert(\"XSS\");</script> <path stroke=\"none\" d=\"M0 0h24v24H0z\" fill=\"none\" /> <path d=\"M14 3v4a1 1 0 0 0 1 1h4\" /> <path d=\"M5 12v-7a2 2 0 0 1 2 -2h7l5 5v4\" /> <path d=\"M4 20.25c0 .414 .336 .75 .75 .75h1.25a1 1 0 0 0 1 -1v-1a1 1 0 0 0 -1 -1h-1a1 1 0 0 1 -1 -1v-1a1 1 0 0 1 1 -1h1.25a.75 .75 0 0 1 .75 .75\" /> <path d=\"M10 15l2 6l2 -6\" /> <path d=\"M20 15h-1a2 2 0 0 0 -2 2v2a2 2 0 0 0 2 2h1v-3\" /> </g> </svg>";
        const string expected = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"24\" height=\"24\" viewBox=\"0 0 24 24\" fill=\"none\" stroke=\"currentColor\" stroke-width=\"2\" stroke-linecap=\"round\" stroke-linejoin=\"round\" class=\"icon icon-tabler icons-tabler-outline icon-tabler-file-type-svg\"><g><path stroke=\"none\" d=\"M0 0h24v24H0z\" fill=\"none\" /><path d=\"M14 3v4a1 1 0 0 0 1 1h4\" /><path d=\"M5 12v-7a2 2 0 0 1 2 -2h7l5 5v4\" /><path d=\"M4 20.25c0 .414 .336 .75 .75 .75h1.25a1 1 0 0 0 1 -1v-1a1 1 0 0 0 -1 -1h-1a1 1 0 0 1 -1 -1v-1a1 1 0 0 1 1 -1h1.25a.75 .75 0 0 1 .75 .75\" /><path d=\"M10 15l2 6l2 -6\" /><path d=\"M20 15h-1a2 2 0 0 0 -2 2v2a2 2 0 0 0 2 2h1v-3\" /></g></svg>";
        
        // Act
        var actual = SvgSanitiser.Sanitise(input, DefaultOptions);
        
        // Assert
        Assert.IsNotNull(actual);
        Assert.AreEqual(expected, actual);
    }
}