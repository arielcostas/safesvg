using System.Xml;

namespace Costasdev.SafeSvg;

/// <summary>
/// Provides functionality to sanitise SVG content for safe use in various contexts.
/// The sanitisation process includes validating the SVG structure, removing
/// disallowed attributes and elements, optionally adding an SVG namespace, and ensuring
/// the output conforms to the expectations of safe SVG usage.
/// This class is designed to handle potential vulnerabilities in SVG content arising from
/// malicious or malformed input.
/// </summary>
public abstract class SvgSanitiser
{
    private static readonly string[] AllowedSvgTags =
    [
        "svg", "g", "path", "circle", "rect", "line", "polyline", "polygon",
        "ellipse", "text", "tspan"
    ];

    private static readonly string[] AllowedSvgAttributes =
    [
        "id", "class", "d", "cx", "cy", "r", "x", "y", "width", "height",
        "fill", "stroke", "stroke-width", "transform", "font-size", "font-family",
        "text-anchor", "viewBox", "preserveAspectRatio", "style", "opacity"
    ];

    private const string AllowedSvgNamespace = "http://www.w3.org/2000/svg";

    /// <summary>
    /// Sanitises the input SVG content by removing any disallowed elements and attributes,
    /// and optionally adding a namespace if required.
    /// </summary>
    /// <param name="content">The SVG content to sanitise as a string.</param>
    /// <param name="options">The sanitisation options to apply, or null to use default options.</param>
    /// <returns>The sanitised SVG content as a string, or null if the input is invalid or cannot be sanitised.</returns>
    public static string? Sanitise(string content, SanitiserOptions? options = null)
    {
        options ??= SanitiserOptions.Default;

        // Load the SVG content into an XmlDocument
        XmlDocument doc = new();
        try
        {
            using var textReader = new StringReader(content);
            using var reader = XmlReader.Create(textReader, new XmlReaderSettings
            {
                IgnoreWhitespace = options.IndentOutput,
                IgnoreComments = options.RemoveComments,
                DtdProcessing = DtdProcessing.Ignore
            });
            
            doc.Load(reader);
        }
        catch (XmlException)
        {
            return null;
        }

        // If the root element is not an SVG element, return null
        if (doc.DocumentElement?.LocalName != "svg")
        {
            return null;
        }

        // Remove all elements and attributes that are not allowed
        foreach (XmlNode node in doc.DocumentElement.ChildNodes)
        {
            if (node is not XmlElement element)
            {
                continue;
            }

            if (!AllowedSvgTags.Contains(element.LocalName))
            {
                doc.DocumentElement.RemoveChild(node);
                continue;
            }

            var attributesInElementToRemove = new List<XmlAttribute>();
            foreach (XmlAttribute attribute in element.Attributes)
            {
                if (!AllowedSvgAttributes.Contains(attribute.LocalName))
                {
                    attributesInElementToRemove.Add(attribute);
                }
            }
            
            foreach (XmlAttribute attribute in attributesInElementToRemove)
            {
                element.Attributes.Remove(attribute);
            }
        }

        // Remove all attributes that are not allowed from the root element
        List<XmlAttribute> attributesToRemove = [];
        foreach (XmlAttribute attribute in doc.DocumentElement.Attributes)
        {
            // If the attribute is the SVG namespace, leave it
            if (attribute.Name == "xmlns" && attribute.Value == AllowedSvgNamespace)
            {
                continue;
            }
            
            if (attribute.Prefix == "xmlns" && attribute.Value != AllowedSvgNamespace)
            {
                attributesToRemove.Add(attribute);
            }
            
            if (!AllowedSvgAttributes.Contains(attribute.LocalName))
            {
                attributesToRemove.Add(attribute);
            }
        }
        
        foreach (XmlAttribute attribute in attributesToRemove)
        {
            doc.DocumentElement.Attributes.Remove(attribute);
        }
        
        // If the SVG namespace is not defined, add it
        if (options.AddNamespace && !doc.DocumentElement.HasAttribute("xmlns"))
        {
            doc.DocumentElement.SetAttribute("xmlns", AllowedSvgNamespace);
        }

        // Return the sanitised SVG as a string
        using StringWriter stringWriter = new();
        using XmlTextWriter xmlWriter = new(stringWriter);
        xmlWriter.Formatting = options.IndentOutput ? Formatting.Indented : Formatting.None;
        doc.WriteTo(xmlWriter);
        
        xmlWriter.Flush();
        return stringWriter.ToString();
    }
}