#pragma warning disable IDE0301 // Simplify collection initialization

using SampleWebApp.Playwright.Utility;
using System.Text;
using System.Threading.Tasks;
using TUnit.Assertions.AssertConditions.Throws;

namespace SampleWebApp.Playwright.Test;

/// <summary>
/// Unit tests for the UrlTemplate class and its TryParse method.
/// </summary>
public class UrlTemplateTests {
    
    [Test]
    public async Task TryParse_WithNullInput_ReturnsFalse() {
        // Act
        var result = UrlTemplate.TryParse(null!, out var template);
        
        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(template).IsNull();
    }
    
    [Test]
    public async Task TryParse_WithEmptyInput_ReturnsFalse() {
        // Act
        var result = UrlTemplate.TryParse("", out var template);
        
        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(template).IsNull();
    }
    
    [Test]
    public async Task TryParse_WithWhitespaceInput_ReturnsFalse() {
        // Act
        var result = UrlTemplate.TryParse("   ", out var template);
        
        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
    }
    
    [Test]
    public async Task TryParse_WithSingleSlash_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("/", out var template);
        
        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(1);
        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[0].Value).IsEqualTo("/");
    }
    
    [Test]
    public async Task TryParse_WithConstantOnly_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("users", out var template);
        
        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(1);
        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[0].Value).IsEqualTo("users");
    }
    
    [Test]
    public async Task TryParse_WithSlashAndConstant_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("/users", out var template);
        
        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(2);
        
        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[0].Value).IsEqualTo("/");
        
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[1].Value).IsEqualTo("users");
    }
    
    [Test]
    public async Task TryParse_WithPlaceholder_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("{id}", out var template);
        
        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(1);
        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Placeholder);
        await Assert.That(template.Parts[0].Value).IsEqualTo("id");
    }
    
    [Test]
    public async Task TryParse_WithUnclosedPlaceholder_ReturnsFalse() {
        // Act
        var result = UrlTemplate.TryParse("{id", out var template);
        
        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(template).IsNull();
    }
    
    [Test]
    public async Task TryParse_WithEmptyPlaceholder_ReturnsFalse() {
        // Act
        var result = UrlTemplate.TryParse("{}", out var template);
        
        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(template).IsNull();
    }
    
    [Test]
    public async Task TryParse_WithComplexPath_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("/users/{id}/edit", out var template);
        
        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(6);
        
        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[0].Value).IsEqualTo("/");
        
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[1].Value).IsEqualTo("users");
        
        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[2].Value).IsEqualTo("/");
        
        await Assert.That(template.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Placeholder);
        await Assert.That(template.Parts[3].Value).IsEqualTo("id");
        
        await Assert.That(template.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[4].Value).IsEqualTo("/");

        await Assert.That(template.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[5].Value).IsEqualTo("edit");
    }

    [Test]
    public async Task TryParse_WithQueryString_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("?name=value", out var template);
        
        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(3);
        
        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(template.Parts[0].Value).IsEqualTo("?");
        
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[1].Value).IsEqualTo("name");
        
        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[2].Value).IsEqualTo("value");
    }
    
    [Test]
    public async Task TryParse_WithMultipleQueryParameters_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("?name=john&age=25&active=true", out var template);
        
        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(9); // Updated count to include ampersands

        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(template.Parts[0].Value).IsEqualTo("?");

        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[1].Value).IsEqualTo("name");

        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[2].Value).IsEqualTo("john");

        await Assert.That(template.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Ampersand);
        await Assert.That(template.Parts[3].Value).IsEqualTo("&");

        await Assert.That(template.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[4].Value).IsEqualTo("age");

        await Assert.That(template.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[5].Value).IsEqualTo("25");

        await Assert.That(template.Parts[6].Kind).IsEqualTo(UrlTemplatePartKind.Ampersand);
        await Assert.That(template.Parts[6].Value).IsEqualTo("&");

        await Assert.That(template.Parts[7].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[7].Value).IsEqualTo("active");

        await Assert.That(template.Parts[8].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[8].Value).IsEqualTo("true");
    }
    
    [Test]
    public async Task TryParse_WithCompleteUrlTemplate_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("/users/{id}/edit?tab=profile&mode=advanced", out var template);
        
        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(12); // Updated count to include ampersand

        // Path parts
        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[1].Value).IsEqualTo("users");
        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Placeholder);
        await Assert.That(template.Parts[3].Value).IsEqualTo("id");
        await Assert.That(template.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[5].Value).IsEqualTo("edit");

        // Query parts
        await Assert.That(template.Parts[6].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(template.Parts[7].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[7].Value).IsEqualTo("tab");
        await Assert.That(template.Parts[8].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[8].Value).IsEqualTo("profile");
        await Assert.That(template.Parts[9].Kind).IsEqualTo(UrlTemplatePartKind.Ampersand);
        await Assert.That(template.Parts[9].Value).IsEqualTo("&");
        await Assert.That(template.Parts[10].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[10].Value).IsEqualTo("mode");
        await Assert.That(template.Parts[11].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[11].Value).IsEqualTo("advanced");
    }
    
    [Test]
    public async Task TryParse_WithQueryParameterWithoutValue_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("?debug&verbose=true", out var template);
        
        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(6); // Updated count to include ampersand

        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[1].Value).IsEqualTo("debug");
        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[2].Name).IsEqualTo("debug");
        await Assert.That(template.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Ampersand);
        await Assert.That(template.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[4].Value).IsEqualTo("verbose");
        await Assert.That(template.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[5].Value).IsEqualTo("true");
    }
    
    [Test]
    public async Task Constructor_WithNullParts_ThrowsArgumentNullException() {
        // Act & Assert
        await Assert.That(() => new UrlTemplate(null!)).Throws<ArgumentNullException>();
    }
    
    [Test]
    public async Task Constructor_WithValidParts_SetsPartsProperty() {
        // Arrange
        var parts = new[] {
            new UrlTemplatePart(UrlTemplatePartKind.Slash, "/"),
            new UrlTemplatePart(UrlTemplatePartKind.Const, "users")
        };

        // Act
        var template = new UrlTemplate(parts);

        // Assert
        await Assert.That(template.Parts).IsEqualTo(parts);
    }

    [Test]
    public async Task TryParse_WithEmptyQueryValue_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("?name=", out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(3);

        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[1].Value).IsEqualTo("name");
        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[2].Value).IsEqualTo("");
    }

    [Test]
    public async Task TryParse_WithSpecialCharactersInConstant_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("/api-v2/user_profile", out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(4);

        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[1].Value).IsEqualTo("api-v2");
        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[3].Value).IsEqualTo("user_profile");
    }

    [Test]
    public async Task TryParse_WithMultiplePlaceholders_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("/{category}/{id}/{action}", out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(6);

        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Placeholder);
        await Assert.That(template.Parts[1].Value).IsEqualTo("category");
        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Placeholder);
        await Assert.That(template.Parts[3].Value).IsEqualTo("id");
        await Assert.That(template.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.Placeholder);
        await Assert.That(template.Parts[5].Value).IsEqualTo("action");
    }

    [Test]
    public async Task TryParse_WithTrailingSlash_ReturnsTrue() {
        // Act
        var result = UrlTemplate.TryParse("/users/{id}/", out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(5);

        await Assert.That(template.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[4].Value).IsEqualTo("/");
    }

    [Test]
    [Arguments("/UIEbbes/{id}?name=value")]
    [Arguments("/users/{userId}/posts/{postId}")]
    [Arguments("?search=test&page=1&limit=10")]
    [Arguments("/api/v1/resources")]
    [Arguments("/{controller}/{action}")]
    public async Task TryParse_WithVariousValidInputs_ReturnsTrue(string input) {
        // Act
        var result = UrlTemplate.TryParse(input, out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().GreaterThan(0);
    }

    [Test]
    [Arguments("{unclosed")]
    [Arguments("{}")]
    [Arguments("{")]
    [Arguments("}")]
    public async Task TryParse_WithInvalidPlaceholders_ReturnsFalse(string input) {
        // Act
        var result = UrlTemplate.TryParse(input, out var template);

        // Assert
        await Assert.That(result).IsFalse();
        await Assert.That(template).IsNull();
    }

    [Test]
    public async Task ToString_WithSimplePath_ReturnsOriginalString() {
        // Arrange
        var input = "/users/{id}";
        await Assert.That(UrlTemplate.TryParse(input, out var template)).IsTrue();

        // Act
        var result = template!.ToString();

        // Assert
        await Assert.That(result).IsEqualTo(input);
    }

    [Test]
    public async Task ToString_WithQueryParameters_ReturnsCorrectString() {
        // Arrange
        var input = "/users?name=john&age=25";
        await Assert.That(UrlTemplate.TryParse(input, out var template)).IsTrue();

        // Act
        var result = template!.ToString();

        // Assert
        await Assert.That(result).IsEqualTo(input);
    }

    [Test]
    public async Task ToString_WithComplexTemplate_ReturnsCorrectString() {
        // Arrange
        var input = "/api/{version}/users/{id}/posts?limit=10&offset=0";
        await Assert.That(UrlTemplate.TryParse(input, out var template)).IsTrue();
        // Act
        var result = template!.ToString();

        // Assert
        await Assert.That(result).IsEqualTo(input);
    }

    [Test]
    public async Task GetUrlTemplateTextLength_WithDifferentPartKinds_ReturnsCorrectLength() {
        // Arrange & Act & Assert
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.Slash, "/").GetUrlTemplateTextLength()).IsEqualTo(1);
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.Const, "users").GetUrlTemplateTextLength()).IsEqualTo(5);
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.Placeholder, "id").GetUrlTemplateTextLength()).IsEqualTo(4); // {id}
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.QuestionMark, "?").GetUrlTemplateTextLength()).IsEqualTo(1);
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.VariableName, "name").GetUrlTemplateTextLength()).IsEqualTo(4);
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.VariableValue, "john").GetUrlTemplateTextLength()).IsEqualTo(5); // =john
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.Ampersand, "&").GetUrlTemplateTextLength()).IsEqualTo(1);
    }

    [Test]
    public async Task BuildUrlTemplateText_WithDifferentPartKinds_AppendsCorrectText() {
        // Arrange
        var sb = new StringBuilder();

        // Act & Assert
        new UrlTemplatePart(UrlTemplatePartKind.Slash, "/").BuildUrlTemplateText(sb);
        await Assert.That(sb.ToString()).IsEqualTo("/");

        sb.Clear();
        new UrlTemplatePart(UrlTemplatePartKind.Const, "users").BuildUrlTemplateText(sb);
        await Assert.That(sb.ToString()).IsEqualTo("users");

        sb.Clear();
        new UrlTemplatePart(UrlTemplatePartKind.Placeholder, "id").BuildUrlTemplateText(sb);
        await Assert.That(sb.ToString()).IsEqualTo("{id}");

        sb.Clear();
        new UrlTemplatePart(UrlTemplatePartKind.QuestionMark, "?").BuildUrlTemplateText(sb);
        await Assert.That(sb.ToString()).IsEqualTo("?");

        sb.Clear();
        new UrlTemplatePart(UrlTemplatePartKind.VariableName, "name").BuildUrlTemplateText(sb);
        await Assert.That(sb.ToString()).IsEqualTo("name=");

        sb.Clear();
        new UrlTemplatePart(UrlTemplatePartKind.VariableValue, "john").BuildUrlTemplateText(sb);
        await Assert.That(sb.ToString()).IsEqualTo("john");

        sb.Clear();
        new UrlTemplatePart(UrlTemplatePartKind.Ampersand, "&").BuildUrlTemplateText(sb);
        await Assert.That(sb.ToString()).IsEqualTo("&");
    }

    [Test]
    public async Task BuildUrlTemplateText_WithNullStringBuilder_ThrowsArgumentNullException() {
        // Arrange
        var part = new UrlTemplatePart(UrlTemplatePartKind.Const, "test");

        // Act & Assert
        await Assert.That(() => part.BuildUrlTemplateText(null!)).Throws<ArgumentNullException>();
    }

    // URL Encoding Tests
    [Test]
    public async Task TryParse_WithUrlEncodedConstant_DecodesCorrectly() {
        // Act
        var result = UrlTemplate.TryParse("/hello%20world", out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(2);

        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[1].Value).IsEqualTo("hello world"); // Decoded
    }

    [Test]
    public async Task TryParse_WithUrlEncodedQueryParameterName_DecodesCorrectly() {
        // Act
        var result = UrlTemplate.TryParse("?my%20param=value", out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(3);

        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[1].Value).IsEqualTo("my param"); // Decoded
        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[2].Value).IsEqualTo("value");
    }

    [Test]
    public async Task TryParse_WithUrlEncodedQueryParameterValue_DecodesCorrectly() {
        // Act
        var result = UrlTemplate.TryParse("?name=john%20doe", out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(3);

        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(template.Parts[1].Value).IsEqualTo("name");
        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(template.Parts[2].Value).IsEqualTo("john doe"); // Decoded
    }

    [Test]
    public async Task TryParse_WithSpecialCharactersInConstant_DecodesCorrectly() {
        // Act
        var result = UrlTemplate.TryParse("/api%2Fv1/users%2Bgroups", out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(4);

        await Assert.That(template.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[1].Value).IsEqualTo("api/v1"); // Decoded
        await Assert.That(template.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(template.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(template.Parts[3].Value).IsEqualTo("users+groups"); // Decoded
    }

    [Test]
    public async Task ToString_WithSpecialCharactersInConstant_EncodesCorrectly() {
        // Arrange
        var parts = new[] {
            new UrlTemplatePart(UrlTemplatePartKind.Slash, "/"),
            new UrlTemplatePart(UrlTemplatePartKind.Const, "hello world"),
            new UrlTemplatePart(UrlTemplatePartKind.Slash, "/"),
            new UrlTemplatePart(UrlTemplatePartKind.Const, "api/v1")
        };
        var template = new UrlTemplate(parts);

        // Act
        var result = template.ToString();

        // Assert
        await Assert.That(result).IsEqualTo("/hello+world/api%2Fv1");
    }

    [Test]
    public async Task ToString_WithSpecialCharactersInQueryParameters_EncodesCorrectly() {
        // Arrange
        var parts = new[] {
            new UrlTemplatePart(UrlTemplatePartKind.QuestionMark, "?"),
            new UrlTemplatePart(UrlTemplatePartKind.VariableName, "my param"),
            new UrlTemplatePart(UrlTemplatePartKind.VariableValue, "john doe"),
            new UrlTemplatePart(UrlTemplatePartKind.Ampersand, "&"),
            new UrlTemplatePart(UrlTemplatePartKind.VariableName, "filter"),
            new UrlTemplatePart(UrlTemplatePartKind.VariableValue, "name=value")
        };
        var template = new UrlTemplate(parts);

        // Act
        var result = template.ToString();

        // Assert
        await Assert.That(result).IsEqualTo("?my+param=john+doe&filter=name%3Dvalue");
    }

    // Round-trip Tests
    [Test]
    [Arguments("/users/{id}")]
    [Arguments("/api/v1/resources")]
    [Arguments("?name=value&age=25")]
    [Arguments("/users/{id}/posts?limit=10&offset=0")]
    [Arguments("/{controller}/{action}?tab=settings")]
    public async Task RoundTrip_SimpleTemplates_PreservesStructure(string input) {
        // Act - Parse and then convert back to string
        var parseResult = UrlTemplate.TryParse(input, out var template);
        var reconstructed = template!.ToString();
        var reparseResult = UrlTemplate.TryParse(reconstructed, out var reparsedTemplate);

        // Assert - Both parsing operations should succeed
        await Assert.That(parseResult).IsTrue();
        await Assert.That(reparseResult).IsTrue();

        // Assert - Both templates should have the same structure
        await Assert.That(template.Parts).HasCount().EqualTo(reparsedTemplate!.Parts.Length);

        for (int i = 0; i < template.Parts.Length; i++) {
            await Assert.That(template.Parts[i].Kind).IsEqualTo(reparsedTemplate.Parts[i].Kind);
            await Assert.That(template.Parts[i].Value).IsEqualTo(reparsedTemplate.Parts[i].Value);
        }
    }

    [Test]
    public async Task RoundTrip_WithUrlEncodedContent_PreservesDecodedValues() {
        // Arrange
        var input = "/hello%20world?my%20param=john%20doe&filter=name%3dvalue";

        // Act - Parse, convert to string, and parse again
        var parseResult = UrlTemplate.TryParse(input, out var template);
        var reconstructed = template!.ToString();
        var reparseResult = UrlTemplate.TryParse(reconstructed, out var reparsedTemplate);

        // Assert - Both parsing operations should succeed
        await Assert.That(parseResult).IsTrue();
        await Assert.That(reparseResult).IsTrue();

        // Assert - Decoded values should be preserved
        await Assert.That(template.Parts[1].Value).IsEqualTo("hello world");
        await Assert.That(reparsedTemplate!.Parts[1].Value).IsEqualTo("hello world");

        await Assert.That(template.Parts[3].Value).IsEqualTo("my param");
        await Assert.That(reparsedTemplate.Parts[3].Value).IsEqualTo("my param");

        await Assert.That(template.Parts[4].Value).IsEqualTo("john doe");
        await Assert.That(reparsedTemplate.Parts[4].Value).IsEqualTo("john doe");

        await Assert.That(template.Parts[7].Value).IsEqualTo("name=value");
        await Assert.That(reparsedTemplate.Parts[7].Value).IsEqualTo("name=value");
    }

    [Test]
    public async Task RoundTrip_WithComplexUrlEncoding_MaintainsFunctionality() {
        // Arrange - URL with various encoded characters
        var input = "/api%2Fv1/users%2Bgroups/{id}?search=name%3A%22john%22&tags=tag1%2Ctag2";

        // Act
        var parseResult = UrlTemplate.TryParse(input, out var template);
        var reconstructed = template!.ToString();
        var reparseResult = UrlTemplate.TryParse(reconstructed, out var reparsedTemplate);

        // Assert
        await Assert.That(parseResult).IsTrue();
        await Assert.That(reparseResult).IsTrue();

        // Verify that the decoded values are correct
        var constantPart = template.Parts.First(p => p.Kind == UrlTemplatePartKind.Const && p.Value.Contains("api"));
        await Assert.That(constantPart.Value).IsEqualTo("api/v1");

        var groupsPart = template.Parts.First(p => p.Kind == UrlTemplatePartKind.Const && p.Value.Contains("users"));
        await Assert.That(groupsPart.Value).IsEqualTo("users+groups");

        var searchValue = template.Parts.First(p => p.Kind == UrlTemplatePartKind.VariableValue && p.Value.Contains("john"));
        await Assert.That(searchValue.Value).IsEqualTo("name:\"john\"");

        var tagsValue = template.Parts.First(p => p.Kind == UrlTemplatePartKind.VariableValue && p.Value.Contains("tag"));
        await Assert.That(tagsValue.Value).IsEqualTo("tag1,tag2");

        // Verify that reparsed template has the same decoded values
        var reparsedConstantPart = reparsedTemplate!.Parts.First(p => p.Kind == UrlTemplatePartKind.Const && p.Value.Contains("api"));
        await Assert.That(reparsedConstantPart.Value).IsEqualTo("api/v1");

        var reparsedSearchValue = reparsedTemplate.Parts.First(p => p.Kind == UrlTemplatePartKind.VariableValue && p.Value.Contains("john"));
        await Assert.That(reparsedSearchValue.Value).IsEqualTo("name:\"john\"");
    }

    [Test]
    public async Task RoundTrip_WithEmptyQueryValues_HandlesCorrectly() {
        // Arrange
        var input = "?debug&verbose=&active=true";

        // Act
        var parseResult = UrlTemplate.TryParse(input, out var template);
        var reconstructed = template!.ToString();
        var reparseResult = UrlTemplate.TryParse(reconstructed, out var reparsedTemplate);

        // Assert
        await Assert.That(parseResult).IsTrue();
        await Assert.That(reparseResult).IsTrue();

        // Find the empty value part (verbose=)
        var emptyValuePart = template.Parts.Where(p => p.Kind == UrlTemplatePartKind.VariableValue).First();
        await Assert.That(emptyValuePart.Value).IsEqualTo("");

        var reparsedEmptyValuePart = reparsedTemplate!.Parts.Where(p => p.Kind == UrlTemplatePartKind.VariableValue).First();
        await Assert.That(reparsedEmptyValuePart.Value).IsEqualTo("");
    }

    [Test]
    public async Task ToString_WithPlaceholders_DoesNotEncodeContent() {
        // Arrange - Placeholders should not be URL encoded
        var parts = new[] {
            new UrlTemplatePart(UrlTemplatePartKind.Slash, "/"),
            new UrlTemplatePart(UrlTemplatePartKind.Const, "users"),
            new UrlTemplatePart(UrlTemplatePartKind.Slash, "/"),
            new UrlTemplatePart(UrlTemplatePartKind.Placeholder, "user-id"),
            new UrlTemplatePart(UrlTemplatePartKind.Slash, "/"),
            new UrlTemplatePart(UrlTemplatePartKind.Placeholder, "action+type")
        };
        var template = new UrlTemplate(parts);

        // Act
        var result = template.ToString();

        // Assert - Placeholder content should not be encoded
        await Assert.That(result).IsEqualTo("/users/{user-id}/{action+type}");
    }

    [Test]
    public async Task ToString_WithEmptyTemplate_ReturnsEmptyString() {
        // Arrange
        var template = new UrlTemplate(Array.Empty<UrlTemplatePart>());

        // Act
        var result = template.ToString();

        // Assert
        await Assert.That(result).IsEqualTo("");
    }

    [Test]
    public async Task ToString_WithSingleSlash_ReturnsSlash() {
        // Arrange
        var parts = new[] { new UrlTemplatePart(UrlTemplatePartKind.Slash, "/") };
        var template = new UrlTemplate(parts);

        // Act
        var result = template.ToString();

        // Assert
        await Assert.That(result).IsEqualTo("/");
    }

    [Test]
    public async Task GetUrlTemplateTextLength_WithUrlEncodedValues_ReturnsEncodedLength() {
        // Arrange & Act & Assert
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.Const, "hello world").GetUrlTemplateTextLength()).IsEqualTo(11); // "hello+world"
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.VariableName, "my param").GetUrlTemplateTextLength()).IsEqualTo(8); // "my+param"
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.VariableValue, "john doe").GetUrlTemplateTextLength()).IsEqualTo(9); // "=john+doe" (8 chars + 1 for =)
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.Const, "api/v1").GetUrlTemplateTextLength()).IsEqualTo(8); // "api%2Fv1"
    }

    [Test]
    [Arguments("hello world", "hello+world")]
    [Arguments("api/v1", "api%2Fv1")]
    [Arguments("name=value", "name%3Dvalue")]
    [Arguments("tag1,tag2", "tag1%2Ctag2")]
    [Arguments("search:\"test\"", "search%3A%22test%22")]
    public async Task BuildUrlTemplateText_WithSpecialCharacters_EncodesCorrectly(string input, string expectedEncoded) {
        // Arrange
        var sb = new StringBuilder();
        var part = new UrlTemplatePart(UrlTemplatePartKind.Const, input);

        // Act
        part.BuildUrlTemplateText(sb);

        // Assert
        await Assert.That(sb.ToString()).IsEqualTo(expectedEncoded);
    }

    [Test]
    public async Task RoundTrip_WithUnicodeCharacters_PreservesContent() {
        // Arrange
        var input = "/café/{id}?name=José&city=São%20Paulo";

        // Act
        var parseResult = UrlTemplate.TryParse(input, out var template);
        var reconstructed = template!.ToString();
        var reparseResult = UrlTemplate.TryParse(reconstructed, out var reparsedTemplate);

        // Assert
        await Assert.That(parseResult).IsTrue();
        await Assert.That(reparseResult).IsTrue();

        // Verify Unicode characters are preserved
        var cafePart = template.Parts.First(p => p.Kind == UrlTemplatePartKind.Const && p.Value.Contains("café"));
        await Assert.That(cafePart.Value).IsEqualTo("café");

        var josePart = template.Parts.First(p => p.Kind == UrlTemplatePartKind.VariableValue && p.Value.Contains("José"));
        await Assert.That(josePart.Value).IsEqualTo("José");

        var saoPauloPart = template.Parts.First(p => p.Kind == UrlTemplatePartKind.VariableValue && p.Value.Contains("São"));
        await Assert.That(saoPauloPart.Value).IsEqualTo("São Paulo");

        // Verify reparsed template preserves Unicode
        var reparsedCafePart = reparsedTemplate!.Parts.First(p => p.Kind == UrlTemplatePartKind.Const && p.Value.Contains("café"));
        await Assert.That(reparsedCafePart.Value).IsEqualTo("café");
    }

    // Optional Placeholder Tests
    [Test]
    public async Task TryParse_WithOptionalPlaceholder_ParsesCorrectly() {
        // Act
        var result = UrlTemplate.TryParse("/api/{version?}/users/{id}", out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();
        await Assert.That(template!.Parts).HasCount().EqualTo(8);

        // Check the optional placeholder
        var optionalPlaceholder = template.Parts.First(p => p.Kind == UrlTemplatePartKind.Placeholder && p.Value == "version");
        await Assert.That(optionalPlaceholder.IsOptional).IsTrue();
        await Assert.That(optionalPlaceholder.Name).IsEqualTo("version");

        // Check the required placeholder
        var requiredPlaceholder = template.Parts.First(p => p.Kind == UrlTemplatePartKind.Placeholder && p.Value == "id");
        await Assert.That(requiredPlaceholder.IsOptional).IsFalse();
        await Assert.That(requiredPlaceholder.Name).IsEqualTo("id");
    }

    [Test]
    public async Task ToString_WithOptionalPlaceholder_ReconstructsCorrectly() {
        // Arrange
        var parts = new[] {
            new UrlTemplatePart(UrlTemplatePartKind.Slash, "/"),
            new UrlTemplatePart(UrlTemplatePartKind.Const, "api"),
            new UrlTemplatePart(UrlTemplatePartKind.Slash, "/"),
            new UrlTemplatePart(UrlTemplatePartKind.Placeholder, "version", "version", true), // Optional
            new UrlTemplatePart(UrlTemplatePartKind.Slash, "/"),
            new UrlTemplatePart(UrlTemplatePartKind.Const, "users")
        };
        var template = new UrlTemplate(parts);

        // Act
        var result = template.ToString();

        // Assert
        await Assert.That(result).IsEqualTo("/api/{version?}/users");
    }

    [Test]
    public async Task RoundTrip_WithOptionalPlaceholders_PreservesOptionalFlags() {
        // Arrange
        var input = "/api/{version?}/users/{id}";

        // Act
        var parseResult = UrlTemplate.TryParse(input, out var template);
        var reconstructed = template!.ToString();
        var reparseResult = UrlTemplate.TryParse(reconstructed, out var reparsedTemplate);

        // Assert
        await Assert.That(parseResult).IsTrue();
        await Assert.That(reparseResult).IsTrue();
        await Assert.That(reconstructed).IsEqualTo(input);

        // Verify optional flags are preserved
        var originalOptional = template.Parts.First(p => p.Kind == UrlTemplatePartKind.Placeholder && p.Value == "version");
        var reparsedOptional = reparsedTemplate!.Parts.First(p => p.Kind == UrlTemplatePartKind.Placeholder && p.Value == "version");
        await Assert.That(originalOptional.IsOptional).IsTrue();
        await Assert.That(reparsedOptional.IsOptional).IsTrue();

        var originalRequired = template.Parts.First(p => p.Kind == UrlTemplatePartKind.Placeholder && p.Value == "id");
        var reparsedRequired = reparsedTemplate.Parts.First(p => p.Kind == UrlTemplatePartKind.Placeholder && p.Value == "id");
        await Assert.That(originalRequired.IsOptional).IsFalse();
        await Assert.That(reparsedRequired.IsOptional).IsFalse();
    }

    [Test]
    public async Task TryParse_WithOptionalQueryParameters_ParsesCorrectly() {
        // Act
        var result = UrlTemplate.TryParse("?debug={debug?}&verbose=true&level=", out var template);

        // Assert
        await Assert.That(result).IsTrue();
        await Assert.That(template).IsNotNull();

        // Check flag parameter (debug)
        var debugParam = template!.Parts.First(p => p.Kind == UrlTemplatePartKind.VariableValue&& p.Name == "debug");
        await Assert.That(debugParam.IsOptional).IsTrue();

        // Check valued parameter (verbose=true)
        var verboseParam = template.Parts.First(p => p.Kind == UrlTemplatePartKind.VariableValue && p.Name == "verbose");
        await Assert.That(verboseParam.IsOptional).IsFalse();

        // Check empty value parameter (level=)
        var levelParam = template.Parts.First(p => p.Kind == UrlTemplatePartKind.VariableValue && p.Name == "level");
        await Assert.That(levelParam.IsOptional).IsFalse();
    }

    [Test]
    public async Task GetUrlTemplateTextLength_WithOptionalPlaceholder_ReturnsCorrectLength() {
        // Arrange & Act & Assert
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.Placeholder, "version", "version", true).GetUrlTemplateTextLength()).IsEqualTo(10); // "{version?}"
        await Assert.That(new UrlTemplatePart(UrlTemplatePartKind.Placeholder, "id", "id", false).GetUrlTemplateTextLength()).IsEqualTo(4); // "{id}"
    }
}
