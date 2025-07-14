#pragma warning disable IDE0057 // Use range operator
#pragma warning disable IDE0305 // Simplify collection initialization

namespace SampleWebApp.Playwright.Utility;

/// <summary>
/// Represents a URL template that can contain constant parts, placeholders, and query parameters.
/// Supports parsing URL patterns like "/path/{id}?name=value" into structured parts.
/// Handles URL encoding/decoding for constant parts and query parameter values.
/// </summary>
public class UrlTemplate {
    /// <summary>
    /// Attempts to parse a URL template string into a <see cref="UrlTemplate"/> instance.
    /// </summary>
    /// <param name="value">The URL template string to parse. Can contain slashes, constants, placeholders ({name}), and query parameters.</param>
    /// <param name="result">When this method returns, contains the parsed <see cref="UrlTemplate"/> if parsing succeeded, or null if parsing failed.</param>
    /// <returns>true if the URL template was successfully parsed; otherwise, false.</returns>
    /// <remarks>
    /// <para>Supported URL template patterns:</para>
    /// <list type="bullet">
    /// <item><description>"/" - Slash separator</description></item>
    /// <item><description>"constant" - Constant text parts (URL decoded during parsing, encoded during reconstruction)</description></item>
    /// <item><description>"{name}" - Required placeholder for variable substitution</description></item>
    /// <item><description>"{name?}" - Optional placeholder for variable substitution (ending with ?)</description></item>
    /// <item><description>"?" - Query string separator</description></item>
    /// <item><description>"param=" - Query parameter name (URL decoded during parsing, encoded during reconstruction)</description></item>
    /// <item><description>"param?=" - Query parameter name the ? is not part of the name it's indicating that this is an optinal parameter (URL decoded during parsing, encoded during reconstruction)</description></item>
    /// <item><description>"value" - Query parameter value (URL decoded during parsing, encoded during reconstruction)</description></item>
    /// <item><description>"{value}" - Query parameter name without default value (optional different name)</description></item>
    /// <item><description>"{value?}" - Query parameter name without default value (optional different name)</description></item>
    /// <item><description>"&amp;" - Ampersand separator between query parameters</description></item>
    /// </list>
    /// <para>Examples:</para>
    /// <list type="bullet">
    /// <item><description>"/users/{id}/edit?tab=profile&amp;mode=advanced" - Required placeholder with query parameters</description></item>
    /// <item><description>"/api/{version?}/users/{id}" - Optional version placeholder</description></item>
    /// <item><description>"?debug&amp;verbose=true&amp;level=" - Mixed query parameters (flag, value, empty value)</description></item>
    /// </list>
    /// <para>URL encoding is automatically handled for constant parts and query parameters.</para>
    /// <para>Optional placeholders (ending with ?) allow for flexible URL patterns where certain path segments may or may not be present.</para>
    /// <para>Query parameters can be specified with or without values, supporting both flag-style (?debug) and key-value (?name=value) patterns.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Basic template with required placeholder
    /// if (UrlTemplate.TryParse("/users/{id}?name=john", out var template))
    /// {
    ///     // Successfully parsed template with parts:
    ///     // - Slash: "/"
    ///     // - Const: "users"
    ///     // - Slash: "/"
    ///     // - Placeholder: "id"
    ///     // - QuestionMark: "?"
    ///     // - VariableName: "name"
    ///     // - VariableValue: "john"
    /// }
    ///
    /// // Template with optional placeholder
    /// if (UrlTemplate.TryParse("/api/{version?}/users/{id}", out var optionalTemplate))
    /// {
    ///     // Optional version placeholder allows matching both:
    ///     // "/api/v1/users/123" and "/api/users/123"
    /// }
    ///
    /// // Template with query parameters
    /// if (UrlTemplate.TryParse("?verbose=true&amp;level=", out var queryTemplate))
    /// {
    ///     // Supports valued parameters (verbose=true),
    ///     // and empty value parameters (level=)
    /// }
    /// 
    /// // No support for flag parameters
    /// if (UrlTemplate.TryParse("?debug", out var queryTemplate))
    /// {
    ///     // flag parameters (debug)
    /// }
    /// </code>
    /// </example>
    public static bool TryParse(string value, [MaybeNullWhen(false)] out UrlTemplate result) {
        result = null;

        if (string.IsNullOrEmpty(value)) {
            return false;
        }

        var listUrlTemplatePart = new List<UrlTemplatePart>();

        try {
            var i = 0;
            var length = value.Length;

            // Parse path part (before query string)
            while (i < length && value[i] != '?') {
                if (value[i] == '/') {
                    listUrlTemplatePart.Add(new UrlTemplatePart(
                        Kind: UrlTemplatePartKind.Slash, 
                        Value: "/",
                        Name: null,
                        IsOptional: false));
                    i++;
                } else if (value[i] == '{') {
                    // Parse placeholder like {Name}
                    var start = i + 1;
                    i++;
                    while (i < length && value[i] != '}') {
                        i++;
                    }
                    if (i >= length) {
                        return false; // Unclosed placeholder
                    }
                    var placeholderName = value.Substring(start, i - start);
                    if (string.IsNullOrEmpty(placeholderName)) {
                        return false; // Empty placeholder
                    }
                    var decodedPlaceholderName = WebUtility.UrlDecode(placeholderName);
                    bool isOptional;
                    if (decodedPlaceholderName.EndsWith('?')) {
                        decodedPlaceholderName = decodedPlaceholderName.Substring(0, decodedPlaceholderName.Length - 1);
                        isOptional = true;
                    } else {
                        isOptional = false;
                    }
                    listUrlTemplatePart.Add(new UrlTemplatePart(
                        Kind: UrlTemplatePartKind.Placeholder,
                        Value: decodedPlaceholderName,
                        Name: decodedPlaceholderName,
                        IsOptional: isOptional));
                    if (i < length && value[i] == '}') {
                        i++; // Skip closing }
                    }
                } else if (value[i] == '}') {
                    result = default;
                    return false;
                } else {

                    // Parse constant part
                    var start = i;
                    while (i < length && value[i] != '/' && value[i] != '{' && value[i] != '?') {
                        i++;
                    }
                    var constantValue = value.Substring(start, i - start);
                    if (!string.IsNullOrEmpty(constantValue)) {
                        // URL decode constant parts to handle encoded characters
                        var decodedConstantValue = WebUtility.UrlDecode(constantValue);
                        listUrlTemplatePart.Add(new UrlTemplatePart(
                            Kind: UrlTemplatePartKind.Const,
                            Value: decodedConstantValue,
                            Name: null,
                            IsOptional: false));
                    }
                }
            }

            // Parse query string part (after ?)
            if (i < length && value[i] == '?') {
                listUrlTemplatePart.Add(new UrlTemplatePart(
                    Kind: UrlTemplatePartKind.QuestionMark, 
                    Value: "?",
                    Name: null,
                    IsOptional: false));
                i++;

                while (i < length) {
                    // Parse variable name (before =)
                    var start = i;
                    while (i < length && value[i] != '=' && value[i] != '&') {
                        i++;
                    }
                    var variableName = value.Substring(start, i - start);
                    if (!string.IsNullOrEmpty(variableName)) {
                        // URL decode query parameter names to handle encoded characters
                        var decodedVariableName = WebUtility.UrlDecode(variableName);
                        bool isOptional;
                        if (decodedVariableName.EndsWith('?')) {
                            decodedVariableName = decodedVariableName.Substring(0, decodedVariableName.Length - 1);
                            isOptional = true;
                        } else {
                            isOptional = false;
                        }
                        listUrlTemplatePart.Add(new UrlTemplatePart(
                            Kind: UrlTemplatePartKind.VariableName,
                            Value: decodedVariableName,
                            Name: decodedVariableName,
                            IsOptional: isOptional));
                    } else {
                        result = default;
                        return false;
                    }

                    if (i < length && value[i] == '=') {
                        i++; // Skip =

                        // Parse variable value (after =, before & or end)
                        start = i;
                        while (i < length && value[i] != '&') {
                            i++;
                        }
                        var variableValue = value.Substring(start, i - start);
                        // URL decode query parameter values to handle encoded characters
                        var decodedVariableValue = WebUtility.UrlDecode(variableValue);
                        bool isOptional;
                        string name;
                        if (decodedVariableValue == "{}") {
                            decodedVariableValue = "";
                            var urlTemplatePart = listUrlTemplatePart[^1];
                            name = urlTemplatePart.Value;
                            isOptional = urlTemplatePart.IsOptional;
                        } else if (decodedVariableValue.StartsWith('{')
                            && decodedVariableValue.EndsWith('}')) {
                            decodedVariableValue = decodedVariableValue.Substring(1, decodedVariableValue.Length - 2);
                            if (decodedVariableValue.EndsWith('?')) {
                                decodedVariableValue = decodedVariableValue.Substring(0, decodedVariableValue.Length - 1);
                                isOptional = true;
                                name = decodedVariableValue;
                            } else {
                                isOptional = false;
                                name = decodedVariableValue;
                            }
                        } else {
                            var urlTemplatePart = listUrlTemplatePart[^1];
                            name = urlTemplatePart.Value;
                            isOptional = urlTemplatePart.IsOptional;
                        }

                        listUrlTemplatePart.Add(new UrlTemplatePart(
                            Kind: UrlTemplatePartKind.VariableValue,
                            Value: decodedVariableValue,
                            Name: name,
                            IsOptional: isOptional));
                    } else {
                        var urlTemplatePart = listUrlTemplatePart[^1];
                        listUrlTemplatePart.Add(new UrlTemplatePart(
                            Kind: UrlTemplatePartKind.VariableValue,
                            Value: string.Empty,
                            Name: urlTemplatePart.Name,
                            IsOptional: urlTemplatePart.IsOptional));
                    }

                    // & is a separator
                    if (i == length) {
                        break;
                    } else if (i < length && value[i] == '&') {
                        listUrlTemplatePart.Add(new UrlTemplatePart(
                            Kind:UrlTemplatePartKind.Ampersand, 
                            Value: "&",
                            Name: null,
                            IsOptional: false));
                        i++; // Skip &
                    } else {
                        result = default;
                        return false;
                    }
                }
            }

            result = new UrlTemplate(listUrlTemplatePart.ToArray());
            return true;
        } catch {
            return false;
        }
    }

    /// <summary>
    /// <see cref="TryParse(string, out UrlTemplate)"/>.
    /// </summary>
    /// <param name="value">a server-relative url - absolut paths are not supported.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static UrlTemplate Parse(string value) {
        if (TryParse(value, out var result)) {
            return result;
        } else {
            throw new ArgumentException("Invalid UrlTemplate", nameof(value));
        }
    }
    /// <summary>
    /// Initializes a new instance of the <see cref="UrlTemplate"/> class with the specified parts.
    /// </summary>
    /// <param name="parts">An array of <see cref="UrlTemplatePart"/> objects that make up this URL template.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parts"/> is null.</exception>
    public UrlTemplate(UrlTemplatePart[] parts) {
        this.Parts = parts ?? throw new ArgumentNullException(nameof(parts));
    }

    /// <summary>
    /// Gets the array of parts that make up this URL template.
    /// </summary>
    /// <value>An array of <see cref="UrlTemplatePart"/> objects representing the parsed components of the URL template.</value>
    public UrlTemplatePart[] Parts { get; }

    /// <summary>
    /// Reconstructs the URL template string from the parsed parts.
    /// </summary>
    /// <returns>A URL template string that represents the current instance.</returns>
    /// <remarks>
    /// <para>This method reconstructs the original URL template by combining all parts in order.</para>
    /// <para>URL encoding is applied to constant parts and query parameter names/values during reconstruction.</para>
    /// <para>The reconstructed string should be functionally equivalent to the original input when parsed again.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// if (UrlTemplate.TryParse("/users/{id}?name=john%20doe", out var template))
    /// {
    ///     var reconstructed = template.ToString();
    ///     // reconstructed will be "/users/{id}?name=john%20doe" (with proper URL encoding)
    /// }
    /// </code>
    /// </example>
    public override string ToString() {
        var len = 0;
        foreach (var part in this.Parts) { len += part.GetUrlTemplateTextLength(); }
        var sb = new StringBuilder(1 + (len >> 12) << 12);
        foreach (var part in this.Parts) { part.BuildUrlTemplateText(sb); }
        return sb.ToString();
    }
}

/// <summary>
/// Specifies the type of a URL template part.
/// </summary>
public enum UrlTemplatePartKind {
    /// <summary>
    /// Represents a slash ("/") separator in the URL path.
    /// </summary>
    Slash,

    /// <summary>
    /// Represents a constant text part in the URL (e.g., "users", "api").
    /// </summary>
    Const,

    /// <summary>
    /// Represents a placeholder for variable substitution (e.g., "{id}", "{name}", "{version?}").
    /// The Value contains the placeholder name without the braces and optional marker.
    /// Can be optional (IsOptional = true) when the original template contains a '?' suffix.
    /// </summary>
    Placeholder,

    /// <summary>
    /// Represents the question mark ("?") that separates the path from the query string.
    /// </summary>
    QuestionMark,

    /// <summary>
    /// Represents a query parameter name (the part before "=" in "name=value").
    /// Can be optional (IsOptional = true) for flag-style parameters without values.
    /// </summary>
    VariableName,

    /// <summary>
    /// Represents a query parameter value (the part after "=" in "name=value").
    /// Can be empty for parameters with no value (e.g., "param=") or optional parameters.
    /// Inherits the IsOptional property from its associated VariableName.
    /// </summary>
    VariableValue,

    /// <summary>
    /// Represents an ampersand ("&") separator between query parameters.
    /// </summary>
    Ampersand
}

/// <summary>
/// Represents a single part of a URL template with its kind, value, and optional properties.
/// </summary>
/// <param name="Kind">The type of this URL template part.</param>
/// <param name="Value">The string value of this part. For placeholders, this is the name without braces and optional marker.</param>
/// <param name="Name">The name of the placeholder or variable. Used for placeholders and query parameters.</param>
/// <param name="IsOptional">Indicates whether this part is optional. True for placeholders ending with '?' and optional query parameters.</param>
/// <remarks>
/// <para>Optional placeholders (IsOptional = true) are denoted by a '?' suffix in the original template (e.g., "{version?}").</para>
/// <para>Optional query parameters can exist without values or with empty values.</para>
/// <para>The Name property is used to associate variable values with their corresponding variable names in query strings.</para>
/// </remarks>
public record class UrlTemplatePart(
    UrlTemplatePartKind Kind,
    string Value,
    string? Name = default,
    bool IsOptional = false) {

    /// <summary>
    /// Gets the approximate length of text this part would contribute to the reconstructed URL template string.
    /// </summary>
    /// <returns>The number of characters this part would add to the URL template.</returns>
    public int GetUrlTemplateTextLength() {
        return this.Kind switch {
            UrlTemplatePartKind.Slash => 1, // "/"
            UrlTemplatePartKind.Const => WebUtility.UrlEncode(this.Value).Length, // Account for URL encoding
            UrlTemplatePartKind.Placeholder => this.Value.Length + 2 + (this.IsOptional ? 1 : 0), // "{" + Value + "}" or "{" + Value + "?}"
            UrlTemplatePartKind.QuestionMark => 1, // "?"
            UrlTemplatePartKind.VariableName => WebUtility.UrlEncode(this.Value).Length, // Account for URL encoding
            UrlTemplatePartKind.VariableValue => WebUtility.UrlEncode(this.Value).Length + 1, // "=" + encoded value
            UrlTemplatePartKind.Ampersand => 1, // "&"
            _ => 0
        };
    }

    /// <summary>
    /// Appends this part's text representation to the provided StringBuilder.
    /// </summary>
    /// <param name="sb">The StringBuilder to append to.</param>
    /// <remarks>
    /// <para>Optional placeholders are reconstructed with a '?' suffix (e.g., "{version?}").</para>
    /// <para>Optional query parameters without values are represented as placeholder patterns when empty.</para>
    /// <para>URL encoding is applied to constant parts and query parameter names/values.</para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="sb"/> is null.</exception>
    public void BuildUrlTemplateText(StringBuilder sb) {
        ArgumentNullException.ThrowIfNull(sb);

        switch (this.Kind) {
            case UrlTemplatePartKind.Slash:
                sb.Append('/');
                break;
            case UrlTemplatePartKind.Const:
                // URL encode constant parts to handle special characters
                sb.Append(WebUtility.UrlEncode(this.Value));
                break;
            case UrlTemplatePartKind.Placeholder:
                sb.Append('{').Append(this.Value);
                if (this.IsOptional) {
                    sb.Append('?');
                }
                sb.Append('}');
                break;
            case UrlTemplatePartKind.QuestionMark:
                sb.Append('?');
                break;
            case UrlTemplatePartKind.VariableName:
                // URL encode query parameter names to handle special characters
                sb.Append(WebUtility.UrlEncode(this.Value)).Append('=');
                break;
            case UrlTemplatePartKind.VariableValue:
                // URL encode query parameter values to handle special characters
                if (this.Value is { Length:> 0 }) {
                    sb.Append(WebUtility.UrlEncode(this.Value));
                    break;
                }
                if (this.IsOptional) {
                    sb.Append('{').Append(WebUtility.UrlEncode(this.Name)).Append("?}");
                } else { 
                    sb.Append("{}");
                }
                break;
            case UrlTemplatePartKind.Ampersand:
                sb.Append('&');
                break;
        }
    }
}