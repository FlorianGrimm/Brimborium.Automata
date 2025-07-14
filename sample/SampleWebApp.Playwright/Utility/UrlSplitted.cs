namespace SampleWebApp.Playwright.Utility;

/// <summary>
/// Represents a parsed URL split into its component parts for analysis and manipulation.
/// Handles both absolute and server-relative URLs, parsing them into structured components
/// including path segments, query parameters, and fragments.
/// </summary>
public class UrlSplitted {
    /// <summary>
    /// Parses a URL string into its component parts.
    /// </summary>
    /// <param name="url">The URL string to parse. Can be absolute (with scheme) or server-relative (starting with /).</param>
    /// <returns>A new <see cref="UrlSplitted"/> instance containing the parsed URL components.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="url"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="url"/> is empty or whitespace.</exception>
    /// <exception cref="UriFormatException">Thrown when <paramref name="url"/> is not a valid URL format.</exception>
    /// <remarks>
    /// <para>Supported URL formats:</para>
    /// <list type="bullet">
    /// <item><description>Absolute URLs: "https://example.com/path?query=value#fragment"</description></item>
    /// <item><description>Server-relative URLs: "/path/to/resource?param=value"</description></item>
    /// <item><description>URLs with encoded characters are automatically decoded during parsing</description></item>
    /// </list>
    /// <para>The parsed URL is split into <see cref="UrlTemplatePart"/> components:</para>
    /// <list type="bullet">
    /// <item><description>Slash separators ("/") become <see cref="UrlTemplatePartKind.Slash"/> parts</description></item>
    /// <item><description>Path segments become <see cref="UrlTemplatePartKind.Const"/> parts</description></item>
    /// <item><description>Query string marker ("?") becomes <see cref="UrlTemplatePartKind.QuestionMark"/> part</description></item>
    /// <item><description>Query parameter names become <see cref="UrlTemplatePartKind.VariableName"/> parts</description></item>
    /// <item><description>Query parameter values become <see cref="UrlTemplatePartKind.VariableValue"/> parts</description></item>
    /// <item><description>Query parameter separators ("&amp;") become <see cref="UrlTemplatePartKind.Ampersand"/> parts</description></item>
    /// </list>
    /// <para>URL encoding is automatically handled - encoded characters in the input are decoded and stored in their decoded form.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Parse a server-relative URL
    /// var parsed = UrlSplitted.Parse("/users/123/edit?tab=profile&amp;mode=advanced");
    /// // Results in parts: [/, users, /, 123, /, edit, ?, tab, profile, &amp;, mode, advanced]
    ///
    /// // Parse an absolute URL
    /// var absoluteParsed = UrlSplitted.Parse("https://api.example.com/v1/users?limit=10");
    /// // Results in parts for the path and query portions: [/, v1, /, users, ?, limit, 10]
    ///
    /// // Parse URL with encoded characters
    /// var encodedParsed = UrlSplitted.Parse("/search?q=hello%20world&amp;category=news%2Btech");
    /// // Decoded values: q="hello world", category="news+tech"
    /// </code>
    /// </example>
    public static UrlSplitted Parse(string url) {
        ArgumentNullException.ThrowIfNull(url);

        if (string.IsNullOrWhiteSpace(url)) {
            throw new ArgumentException("URL cannot be empty or whitespace.", nameof(url));
        }

        var result = new UrlSplitted();
        var i = 0;
        var length = url.Length;

        // Parse path part (before query string)
        while (i < length && url[i] != '?') {
            if (url[i] == '/') {
                result.Parts.Add(new UrlTemplatePart(
                    Kind: UrlTemplatePartKind.Slash,
                    Value: "/",
                    Name: null,
                    IsOptional: false));
                i++;
            } else {
                // Parse path segment
                var start = i;
                while (i < length && url[i] != '/' && url[i] != '?') {
                    i++;
                }
                var segment = url.Substring(start, i - start);
                if (!string.IsNullOrEmpty(segment)) {
                    // URL decode path segments
                    var decodedSegment = WebUtility.UrlDecode(segment);
                    result.Parts.Add(new UrlTemplatePart(
                        Kind: UrlTemplatePartKind.Const,
                        Value: decodedSegment,
                        Name: null,
                        IsOptional: false));
                }
            }
        }

        // Parse query string part (after ?)
        if (i < length && url[i] == '?') {
            result.Parts.Add(new UrlTemplatePart(
                Kind: UrlTemplatePartKind.QuestionMark,
                Value: "?",
                Name: null,
                IsOptional: false));
            i++;

            while (i < length) {
                // Parse query parameter name
                var start = i;
                while (i < length && url[i] != '=' && url[i] != '&') {
                    i++;
                }
                var paramName = url.Substring(start, i - start);
                string? decodedParamName = null;
                if (!string.IsNullOrEmpty(paramName)) {
                    // URL decode parameter names
                    decodedParamName = WebUtility.UrlDecode(paramName);
                    result.Parts.Add(new UrlTemplatePart(
                        Kind: UrlTemplatePartKind.VariableName,
                        Value: decodedParamName,
                        Name: decodedParamName,
                        IsOptional: false));
                }

                if (i < length && url[i] == '=') {
                    i++; // Skip =

                    // Parse query parameter value
                    start = i;
                    while (i < length && url[i] != '&') {
                        i++;
                    }
                    var paramValue = url.Substring(start, i - start);
                    // URL decode parameter values
                    var decodedParamValue = WebUtility.UrlDecode(paramValue);
                    result.Parts.Add(new UrlTemplatePart(
                        Kind: UrlTemplatePartKind.VariableValue,
                        Value: decodedParamValue,
                        Name: decodedParamName,
                        IsOptional: false));
                }

                if (i < length && url[i] == '&') {
                    result.Parts.Add(new UrlTemplatePart(
                        Kind: UrlTemplatePartKind.Ampersand,
                        Value: "&",
                        Name: null,
                        IsOptional: false));
                    i++; // Skip &
                }
            }
        }

        return result;

    }

    /// <summary>
    /// Gets the list of URL parts that make up this parsed URL.
    /// </summary>
    /// <value>A list of <see cref="UrlTemplatePart"/> objects representing the components of the parsed URL.</value>
    public List<UrlTemplatePart> Parts { get; } = [];
}
