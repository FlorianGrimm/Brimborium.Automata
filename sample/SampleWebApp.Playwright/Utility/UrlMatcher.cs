using System.Diagnostics.CodeAnalysis;

namespace SampleWebApp.Playwright.Utility;

/// <summary>
/// Provides URL pattern matching functionality using a decision tree structure.
/// Matches actual URLs against registered URL templates to find corresponding page definitions.
/// </summary>
public class UrlMatcher {
    /// <summary>
    /// Gets the root node of the decision tree used for URL matching.
    /// </summary>
    public readonly DecisionTreeNode Root;

    /// <summary>
    /// Initializes a new instance of the <see cref="UrlMatcher"/> class.
    /// </summary>
    public UrlMatcher() {
        this.Root = new DecisionTreeNode() {
            UrlTemplatePart = new UrlTemplatePart(Kind: UrlTemplatePartKind.Slash, Value: "/")
        };
    }

    /// <summary>
    /// Adds a URL template and its corresponding page definition to the matcher.
    /// </summary>
    /// <param name="urlTemplate">The URL template to register for matching.</param>
    /// <param name="page">The page definition associated with the URL template.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="urlTemplate"/> or <paramref name="page"/> is null.</exception>
    /// <remarks>
    /// <para>This method builds a decision tree by creating nodes for each part of the URL template.</para>
    /// <para>Multiple templates can share common prefixes, creating an efficient tree structure for matching.</para>
    /// <para>Placeholders in templates allow for dynamic matching of URL segments.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var matcher = new UrlMatcher();
    /// var template = UrlTemplate.Parse("/users/{id}/edit");
    /// var page = new PageDefinition(template);
    /// matcher.Add(template, page);
    /// </code>
    /// </example>
    public void Add(UrlTemplate urlTemplate, PageDefinition page) {
        ArgumentNullException.ThrowIfNull(urlTemplate);
        ArgumentNullException.ThrowIfNull(page);

        var node = this.Root;
        int idx = 0;
        int length = urlTemplate.Parts.Length;
        if (idx < length) {
            UrlTemplatePart part = urlTemplate.Parts[idx];
            if (part.Kind == UrlTemplatePartKind.Slash) {
                idx++;
            } else {
                throw new ArgumentException("should start with an slash.", nameof(urlTemplate));
            }
        }
        for (; idx < length; idx++) {
            UrlTemplatePart part = urlTemplate.Parts[idx];

            // Find existing child node that matches this part, or create a new one
            var childNode = node.Children.FirstOrDefault(child => TemplatePartsMatch(child.UrlTemplatePart, part));

            if (childNode == null) {
                // Create new child node for this part
                childNode = new DecisionTreeNode() {
                    UrlTemplatePart = part
                };
                node.Children.Add(childNode);
            }

            node = childNode;
        }

        // Set the page definition at the final node
        node.PageDefinition = page;
    }

    /// <summary>
    /// Attempts to find a page definition that matches the given URL.
    /// </summary>
    /// <param name="url">The URL to match against registered templates. Can be absolute or server-relative.</param>
    /// <returns>A <see cref="UrlMatch"/> containing the matched page definition and extracted parameter values.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="url"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="url"/> is empty or whitespace.</exception>
    /// <remarks>
    /// <para>This method strips the scheme and host from absolute URLs, focusing on the path and query portions.</para>
    /// <para>The URL is parsed into parts and matched against the decision tree to find the best matching template.</para>
    /// <para>Placeholder values are extracted and included in the result for dynamic URL segments.</para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var matcher = new UrlMatcher();
    /// // ... add templates ...
    /// var match = matcher.GetPageDefinitionFromUrl("/users/123/edit?tab=profile");
    /// if (match.Found != null) {
    ///     // Found matching page definition
    ///     var userId = match.ListUrlPartsFound.FirstOrDefault(p => p.Name == "id")?.Value;
    /// }
    /// </code>
    /// </example>
    public UrlMatch GetPageDefinitionFromUrl(string url) {
        ArgumentNullException.ThrowIfNull(url);

        if (string.IsNullOrWhiteSpace(url)) {
            throw new ArgumentException("URL cannot be empty or whitespace.", nameof(url));
        }

        // Strip scheme and host from absolute URLs
        if (url.StartsWith("https://")) {
            url = url[8..];
            var index = url.IndexOf('/');
            url = index >= 0 ? url[index..] : "/";
        } else if (url.StartsWith("http://")) {
            url = url[7..];
            var index = url.IndexOf('/');
            url = index >= 0 ? url[index..] : "/";
        }

        var urlSplitted = UrlSplitted.Parse(url);
        var result = new UrlMatch();

        // Walk down the decision tree to find matching page definition
        var currentNode = this.Root;
        var urlParts = urlSplitted.Parts;
        int idx = 0;
        if (idx < urlParts.Count) {
            var urlPart = urlParts[idx];
            if (urlPart.Kind == UrlTemplatePartKind.Slash) {
                idx++;
            } else {
                return result;
            }
        }
        for (; idx < urlParts.Count; idx++) {
            var urlPart = urlParts[idx];
            DecisionTreeNode? matchingChild = null;

            // Try to find a matching child node
            foreach (var child in currentNode.Children) {
                if (PartsMatch(child.UrlTemplatePart, urlPart)) {
                    matchingChild = child;

                    // If this is a placeholder, capture the value
                    if (child.UrlTemplatePart.Kind == UrlTemplatePartKind.Placeholder) {
                        result.ListUrlPartsFound.Add(new UrlPartValueString(
                            Name: child.UrlTemplatePart.Value,
                            Value: urlPart.Value));
                    }
                    break;
                }
            }

            if (matchingChild == null) {
                // No matching path found
                return result;
            }

            currentNode = matchingChild;
        }

        // Check if we found a complete match (node has a page definition)
        result.Found = currentNode.PageDefinition;
        return result;
    }

    /// <summary>
    /// Determines whether two template parts match when building the decision tree.
    /// Used during the Add operation to find existing nodes that can be reused.
    /// </summary>
    /// <param name="existingPart">The template part from an existing node in the tree.</param>
    /// <param name="newPart">The template part from the new template being added.</param>
    /// <returns>true if the parts match and the node can be reused; otherwise, false.</returns>
    /// <remarks>
    /// <para>Template-to-template matching rules:</para>
    /// <list type="bullet">
    /// <item><description>Exact match for slashes, question marks, and ampersands</description></item>
    /// <item><description>Exact value match for constant parts</description></item>
    /// <item><description>Placeholders match other placeholders with the same name</description></item>
    /// <item><description>Query parameter names must match exactly</description></item>
    /// <item><description>Query parameter values match if names correspond</description></item>
    /// </list>
    /// </remarks>
    private static bool TemplatePartsMatch(UrlTemplatePart existingPart, UrlTemplatePart newPart) {
        return existingPart.Kind switch {
            // Structural parts must match exactly
            UrlTemplatePartKind.Slash or
            UrlTemplatePartKind.QuestionMark or
            UrlTemplatePartKind.Ampersand =>
                existingPart.Kind == newPart.Kind,

            // Constant parts must have exact value match
            UrlTemplatePartKind.Const =>
                newPart.Kind == UrlTemplatePartKind.Const &&
                string.Equals(existingPart.Value, newPart.Value, StringComparison.OrdinalIgnoreCase),

            // Placeholders match other placeholders with the same name
            UrlTemplatePartKind.Placeholder =>
                newPart.Kind == UrlTemplatePartKind.Placeholder &&
                string.Equals(existingPart.Value, newPart.Value, StringComparison.OrdinalIgnoreCase),

            // Query parameter names must match exactly
            UrlTemplatePartKind.VariableName =>
                newPart.Kind == UrlTemplatePartKind.VariableName &&
                string.Equals(existingPart.Value, newPart.Value, StringComparison.OrdinalIgnoreCase),

            // Query parameter values match if they belong to the same parameter
            UrlTemplatePartKind.VariableValue =>
                newPart.Kind == UrlTemplatePartKind.VariableValue &&
                string.Equals(existingPart.Name, newPart.Name, StringComparison.OrdinalIgnoreCase),

            _ => false
        };
    }

    /// <summary>
    /// Determines whether two URL template parts match for routing purposes.
    /// </summary>
    /// <param name="templatePart">The template part from the decision tree.</param>
    /// <param name="urlPart">The actual URL part to match against.</param>
    /// <returns>true if the parts match; otherwise, false.</returns>
    /// <remarks>
    /// <para>Matching rules:</para>
    /// <list type="bullet">
    /// <item><description>Exact match for slashes, question marks, and ampersands</description></item>
    /// <item><description>Exact value match for constant parts</description></item>
    /// <item><description>Placeholders match any constant part</description></item>
    /// <item><description>Query parameter names must match exactly</description></item>
    /// <item><description>Query parameter values match if names correspond</description></item>
    /// </list>
    /// </remarks>
    private static bool PartsMatch(UrlTemplatePart templatePart, UrlTemplatePart urlPart) {
        return templatePart.Kind switch {
            // Structural parts must match exactly
            UrlTemplatePartKind.Slash or
            UrlTemplatePartKind.QuestionMark or
            UrlTemplatePartKind.Ampersand =>
                templatePart.Kind == urlPart.Kind,

            // Constant parts must have exact value match
            UrlTemplatePartKind.Const =>
                urlPart.Kind == UrlTemplatePartKind.Const &&
                string.Equals(templatePart.Value, urlPart.Value, StringComparison.OrdinalIgnoreCase),

            // Placeholders match any constant value
            UrlTemplatePartKind.Placeholder =>
                urlPart.Kind == UrlTemplatePartKind.Const,

            // Query parameter names must match exactly
            UrlTemplatePartKind.VariableName =>
                urlPart.Kind == UrlTemplatePartKind.VariableName &&
                string.Equals(templatePart.Value, urlPart.Value, StringComparison.OrdinalIgnoreCase),

            // Query parameter values match if they belong to the same parameter
            UrlTemplatePartKind.VariableValue =>
                urlPart.Kind == UrlTemplatePartKind.VariableValue &&
                string.Equals(templatePart.Name, urlPart.Name, StringComparison.OrdinalIgnoreCase),

            _ => false
        };
    }
}

/// <summary>
/// Represents a node in the URL matching decision tree.
/// Each node corresponds to a part of a URL template and can have child nodes for subsequent parts.
/// </summary>
public class DecisionTreeNode {
    /// <summary>
    /// Gets or initializes the URL template part that this node represents.
    /// </summary>
    /// <value>The <see cref="UrlTemplatePart"/> associated with this node in the decision tree.</value>
    public required UrlTemplatePart UrlTemplatePart { get; init; }

    /// <summary>
    /// Gets the list of child nodes representing possible next parts in the URL pattern.
    /// </summary>
    /// <value>A list of <see cref="DecisionTreeNode"/> objects that are children of this node.</value>
    public List<DecisionTreeNode> Children { get; } = [];

    /// <summary>
    /// Gets or sets the page definition associated with this node, if this node represents a complete URL pattern.
    /// </summary>
    /// <value>The <see cref="PageDefinition"/> for complete URL patterns, or null for intermediate nodes.</value>
    /// <remarks>
    /// This property is set when a complete URL template path ends at this node,
    /// indicating that URLs matching this path should resolve to the associated page definition.
    /// </remarks>
    public PageDefinition? PageDefinition { get; set; }
}

/// <summary>
/// Represents the result of matching a URL against registered URL templates.
/// Contains the matched page definition and any extracted parameter values.
/// </summary>
public class UrlMatch {
    /// <summary>
    /// Gets the list of URL parameter values that were extracted during the matching process.
    /// </summary>
    /// <value>A list of <see cref="UrlPartValueString"/> objects containing parameter names and their extracted values.</value>
    /// <remarks>
    /// This list contains values extracted from placeholder segments in the URL template.
    /// For example, matching "/users/123" against "/users/{id}" would result in a parameter with Name="id" and Value="123".
    /// </remarks>
    public List<UrlPartValueString> ListUrlPartsFound { get; } = [];

    /// <summary>
    /// Gets or sets the page definition that was found to match the URL, if any.
    /// </summary>
    /// <value>The <see cref="PageDefinition"/> that matches the URL, or null if no match was found.</value>
    /// <remarks>
    /// This property is null when no registered URL template matches the provided URL.
    /// When not null, it indicates a successful match and the page definition can be used for routing.
    /// </remarks>
    public PageDefinition? Found { get; set; }
}