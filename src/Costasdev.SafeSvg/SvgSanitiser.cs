﻿using System.Xml;

namespace Costasdev.SafeSvg;

/// <summary>
/// Provides functionality to sanitise SVG content for safe use in various contexts.
/// The sanitisation process includes validating the SVG structure, removing
/// disallowed attributes and elements, optionally adding an SVG namespace, and ensuring
/// the output conforms to the expectations of safe SVG usage.
/// This class is designed to handle potential vulnerabilities in SVG content arising from
/// malicious or malformed input.
/// </summary>
public sealed class SvgSanitiser
{
    private static readonly string[] AllowedSvgTags =
    [
        "svg", "g", "path", "circle", "rect", "line", "polyline", "polygon",
        "ellipse", "text", "tspan", "style"
    ];

    private static readonly string[] AllowedSvgAttributes =
    [
        "id", "class", "d", "cx", "cy", "r", "x", "y", "width", "height",
        "fill", "stroke", "stroke-width", "stroke-linecap", "stroke-linejoin",
        "transform", "font-size", "font-family",
        "text-anchor", "viewBox", "preserveAspectRatio", "style", "opacity"
    ];

    private const string SvgStandardNamespace = "http://www.w3.org/2000/svg";

    private readonly SanitiserOptions _options;
    
    /// <summary>
    /// Initialises a new instance of the <see cref="SvgSanitiser"/> class with the specified sanitisation options.
    /// </summary>
    /// <param name="options"></param>
    public SvgSanitiser(SanitiserOptions options)
    {
        _options = options;
    }
    
    /// <summary>
    /// Initialises a new instance of the <see cref="SvgSanitiser"/> class with default sanitisation options.
    /// </summary>
    public SvgSanitiser() : this(SanitiserOptions.Default)
    {
    }
    
    /// <summary>
    /// Sanitises the provided SVG content by removing any disallowed elements and attributes.
    /// </summary>
    /// <param name="content">The SVG content to sanitise as a string.</param>
    /// <param name="options">The sanitisation options to apply, or null to use default options.</param>
    /// <returns>The sanitised SVG content as a string, or null if the input is invalid or cannot be sanitised.</returns>
    public static string? Sanitise(string content, SanitiserOptions? options = null)
    {
        options ??= SanitiserOptions.Default;
        
        var sanitiser = new SvgSanitiser(options);
        return sanitiser.CleanSvg(content);
    }
    
    /// <summary>
    /// Sanitises the provided SVG content by removing any disallowed elements and attributes, using the options
    /// provided in the constructor.
    /// </summary>
    /// <param name="content">The SVG content to sanitise as a string.</param>
    /// <returns>The sanitised SVG content as a string, or null if the input is invalid or cannot be sanitised.</returns>
    public string? CleanSvg(string content)
    {
        // Load the SVG content into an XmlDocument
        XmlDocument doc = new();
        try
        {
            using var textReader = new StringReader(content);
            using var reader = XmlReader.Create(textReader, new XmlReaderSettings
            {
                IgnoreWhitespace = _options.IndentOutput,
                IgnoreComments = _options.RemoveComments,
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

        SanitizeSvgElementContent(doc);

        // Remove all attributes that are not allowed from the root element
        List<XmlAttribute> attributesToRemove = [];
        foreach (XmlAttribute attribute in doc.DocumentElement.Attributes)
        {
            // If the attribute is the SVG namespace, leave it
            if (attribute.Name == "xmlns" && attribute.Value == SvgStandardNamespace)
            {
                continue;
            }
            
            if (attribute.Prefix == "xmlns" && attribute.Value != SvgStandardNamespace)
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
        if (_options.AddNamespace && !doc.DocumentElement.HasAttribute("xmlns"))
        {
            XmlAttribute namespaceAttribute = doc.CreateAttribute("xmlns");
            namespaceAttribute.Value = SvgStandardNamespace;
            
            doc.DocumentElement.Attributes.Prepend(namespaceAttribute);
        }

        // Return the sanitised SVG as a string
        using StringWriter stringWriter = new();
        using XmlTextWriter xmlWriter = new(stringWriter);
        xmlWriter.Formatting = _options.IndentOutput ? Formatting.Indented : Formatting.None;
        doc.WriteTo(xmlWriter);
        
        xmlWriter.Flush();
        return stringWriter.ToString();
    }

    private void SanitizeSvgElementContent(XmlDocument doc)
    {
        if (doc.DocumentElement == null)
        {
            return;
        }

        // Start the recursive traversal from the document element
        SanitizeXmlNode(doc.DocumentElement);
    }
    
    private void SanitizeXmlNode(XmlNode node)
    {
        if (node is not XmlElement element) return;
        
        var attributesToRemove = new List<XmlAttribute>();
        foreach (XmlAttribute attribute in element.Attributes)
        {
            if (!AllowedSvgAttributes.Contains(attribute.LocalName))
            {
                attributesToRemove.Add(attribute);
            }
            
            if (_options.AllowStyle == false && attribute.LocalName == "style")
            {
                attributesToRemove.Add(attribute);
            }
        }

        foreach (XmlAttribute attribute in attributesToRemove)
        {
            element.Attributes.Remove(attribute);
        }

        // Remove elements not in the allowed list
        var childNodesToRemove = new List<XmlNode>();
        foreach (XmlNode child in element.ChildNodes)
        {
            if (child is not XmlElement childElement)
            {
                continue;
            }
            
            if (!AllowedSvgTags.Contains(childElement.LocalName))
            {
                childNodesToRemove.Add(child);
            }
            
            if (_options.AllowStyle == false && childElement.LocalName == "style")
            {
                childNodesToRemove.Add(child);
            }
        }

        foreach (XmlNode child in childNodesToRemove)
        {
            element.RemoveChild(child);
        }

        // Recursively process child nodes
        foreach (XmlNode child in element.ChildNodes)
        {
            SanitizeXmlNode(child);
        }
    }
}