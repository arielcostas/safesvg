using System.Xml;

namespace Costasdev.SafeSvg;

/// <summary>
/// Represents options to configure the behaviour of the SVG sanitization process.
/// </summary>
public sealed class SanitiserOptions
{
    /// <summary>
    /// Initialises a new instance of the <see cref="SanitiserOptions"/> class with default values.
    /// </summary>
    public SanitiserOptions()
    {
        AddNamespace = true;
        IndentOutput = true;
        RemoveComments = false;
        AllowStyle = true;
    }

    /// <summary>
    /// A predefined instance of <see cref="SanitiserOptions"/> with the default settings you'd get from the constructor.
    /// </summary>
    public static readonly SanitiserOptions Default = new();
    
    /// <summary>
    /// A default instance of <see cref="SanitiserOptions"/> with "cautious" defaults.
    /// </summary>
    public static readonly SanitiserOptions Cautious = new()
    {
        AddNamespace = true,
        IndentOutput = true,
        RemoveComments = true,
        AllowStyle = false
    };
    
    /// <summary>
    /// Whether to add the SVG namespace to the root element.
    ///
    /// If true, the namespace will be added to the root element of the SVG document.
    ///
    /// If false, the namespace  will not be added, but will be preserved if included in the input.
    /// </summary>
    public bool AddNamespace { get; init; }
    
    /// <summary>
    /// Whether to indent the output SVG using <see cref="Formatting.Indented"/>.
    /// </summary>
    public bool IndentOutput { get; init; }
    
    /// <summary>
    /// Whether to remove comments from the SVG document.
    /// </summary>
    public bool RemoveComments { get; init; }
    
    /// <summary>
    /// Whether to allow the style attribute and style element in the SVG document.
    /// </summary>
    public bool AllowStyle { get; init; }
}