using System.Xml;

namespace Costasdev.SafeSvg;

/// <summary>
/// Represents options to configure the behaviour of the SVG sanitization process.
/// </summary>
public class SanitiserOptions
{
    /// <summary>
    /// A default instance of <see cref="SanitiserOptions"/> with "sane" defaults.
    /// </summary>
    public static readonly SanitiserOptions Default = new()
    {
        AddNamespace = true,
        IndentOutput = true,
        RemoveComments = true
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
}