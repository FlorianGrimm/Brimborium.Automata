namespace SampleWebApp.Playwright.Test;

/// <summary>
/// Unit tests for the UrlSplitted class and its Parse method.
/// </summary>
public class UrlSplittedTests {
    
    [Test]
    public async Task Parse_WithNullInput_ThrowsArgumentNullException() {
        // Act & Assert
        await Assert.That(() => UrlSplitted.Parse(null!)).Throws<ArgumentNullException>();
    }
    
    [Test]
    public async Task Parse_WithEmptyInput_ThrowsArgumentException() {
        // Act & Assert
        await Assert.That(() => UrlSplitted.Parse("")).Throws<ArgumentException>();
    }
    
    [Test]
    public async Task Parse_WithWhitespaceInput_ThrowsArgumentException() {
        // Act & Assert
        await Assert.That(() => UrlSplitted.Parse("   ")).Throws<ArgumentException>();
    }
    
    [Test]
    public async Task Parse_WithSingleSlash_ReturnsCorrectParts() {
        // Act
        var result = UrlSplitted.Parse("/");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(1);
        await Assert.That(result.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(result.Parts[0].Value).IsEqualTo("/");
    }
    
    [Test]
    public async Task Parse_WithSimplePath_ReturnsCorrectParts() {
        // Act
        var result = UrlSplitted.Parse("/users");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(2);
        
        await Assert.That(result.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(result.Parts[0].Value).IsEqualTo("/");
        
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[1].Value).IsEqualTo("users");
    }
    
    [Test]
    public async Task Parse_WithComplexPath_ReturnsCorrectParts() {
        // Act
        var result = UrlSplitted.Parse("/users/123/edit");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(6);
        
        await Assert.That(result.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[1].Value).IsEqualTo("users");
        
        await Assert.That(result.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        
        await Assert.That(result.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[3].Value).IsEqualTo("123");
        
        await Assert.That(result.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.Slash);

        await Assert.That(result.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[5].Value).IsEqualTo("edit");
    }
    
    [Test]
    public async Task Parse_WithQueryString_ReturnsCorrectParts() {
        // Act
        var result = UrlSplitted.Parse("?name=value");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(3);
        
        await Assert.That(result.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(result.Parts[0].Value).IsEqualTo("?");
        
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[1].Value).IsEqualTo("name");
        await Assert.That(result.Parts[1].Name).IsEqualTo("name");
        
        await Assert.That(result.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[2].Value).IsEqualTo("value");
        await Assert.That(result.Parts[2].Name).IsEqualTo("name");
    }
    
    [Test]
    public async Task Parse_WithMultipleQueryParameters_ReturnsCorrectParts() {
        // Act
        var result = UrlSplitted.Parse("?name=john&age=25&active=true");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(9);
        
        await Assert.That(result.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(result.Parts[0].Value).IsEqualTo("?");
        
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[1].Value).IsEqualTo("name");
        
        await Assert.That(result.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[2].Value).IsEqualTo("john");
        
        await Assert.That(result.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Ampersand);
        await Assert.That(result.Parts[3].Value).IsEqualTo("&");
        
        await Assert.That(result.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[4].Value).IsEqualTo("age");
        
        await Assert.That(result.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[5].Value).IsEqualTo("25");
        
        await Assert.That(result.Parts[6].Kind).IsEqualTo(UrlTemplatePartKind.Ampersand);
        await Assert.That(result.Parts[6].Value).IsEqualTo("&");
        
        await Assert.That(result.Parts[7].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[7].Value).IsEqualTo("active");
        
        await Assert.That(result.Parts[8].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[8].Value).IsEqualTo("true");
    }
    
    [Test]
    public async Task Parse_WithCompleteUrl_ReturnsCorrectParts() {
        // Act
        var result = UrlSplitted.Parse("/users/123/edit?tab=profile&mode=advanced");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(12);
        
        // Path parts
        await Assert.That(result.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[1].Value).IsEqualTo("users");
        await Assert.That(result.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(result.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[3].Value).IsEqualTo("123");
        await Assert.That(result.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(result.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[5].Value).IsEqualTo("edit");
        
        // Query parts
        await Assert.That(result.Parts[6].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(result.Parts[7].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[7].Value).IsEqualTo("tab");
        await Assert.That(result.Parts[8].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[8].Value).IsEqualTo("profile");
        await Assert.That(result.Parts[9].Kind).IsEqualTo(UrlTemplatePartKind.Ampersand);
        await Assert.That(result.Parts[10].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[10].Value).IsEqualTo("mode");
        await Assert.That(result.Parts[11].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[11].Value).IsEqualTo("advanced");
    }
    
    [Test]
    public async Task Parse_ParsesPathAndQuery() {
        // Act
        var result = UrlSplitted.Parse("/api/v1/users?limit=10");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(9);
        
        // Should parse only the path and query portions
        await Assert.That(result.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[1].Value).IsEqualTo("api");
        await Assert.That(result.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(result.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[3].Value).IsEqualTo("v1");
        await Assert.That(result.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(result.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[5].Value).IsEqualTo("users");
        await Assert.That(result.Parts[6].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(result.Parts[7].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[7].Value).IsEqualTo("limit");
        await Assert.That(result.Parts[8].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[8].Value).IsEqualTo("10");
    }

    // URL Encoding Tests
    [Test]
    public async Task Parse_WithUrlEncodedPathSegments_DecodesCorrectly() {
        // Act
        var result = UrlSplitted.Parse("/hello%20world/api%2Fv1");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(4);

        await Assert.That(result.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[1].Value).IsEqualTo("hello world"); // Decoded
        await Assert.That(result.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(result.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[3].Value).IsEqualTo("api/v1"); // Decoded
    }

    [Test]
    public async Task Parse_WithUrlEncodedQueryParameterNames_DecodesCorrectly() {
        // Act
        var result = UrlSplitted.Parse("?my%20param=value&other%2Bparam=test");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(6);

        await Assert.That(result.Parts[0].Kind).IsEqualTo(UrlTemplatePartKind.QuestionMark);
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[1].Value).IsEqualTo("my param"); // Decoded
        await Assert.That(result.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[2].Value).IsEqualTo("value");
        await Assert.That(result.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Ampersand);
        await Assert.That(result.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[4].Value).IsEqualTo("other+param"); // Decoded
        await Assert.That(result.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[5].Value).IsEqualTo("test"); // Decoded
    }

    [Test]
    public async Task Parse_WithUrlEncodedQueryParameterValues_DecodesCorrectly() {
        // Act
        var result = UrlSplitted.Parse("?name=john%20doe&search=hello%2Bworld&filter=name%3Dvalue");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(9);

        // Check first parameter
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[1].Value).IsEqualTo("name");
        await Assert.That(result.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[2].Value).IsEqualTo("john doe"); // Decoded
        await Assert.That(result.Parts[2].Name).IsEqualTo("name");

        // Check second parameter
        await Assert.That(result.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[4].Value).IsEqualTo("search");
        await Assert.That(result.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[5].Value).IsEqualTo("hello+world"); // Decoded
        await Assert.That(result.Parts[5].Name).IsEqualTo("search");

        // Check third parameter
        await Assert.That(result.Parts[7].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[7].Value).IsEqualTo("filter");
        await Assert.That(result.Parts[8].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[8].Value).IsEqualTo("name=value"); // Decoded
        await Assert.That(result.Parts[8].Name).IsEqualTo("filter");
    }

    [Test]
    public async Task Parse_WithSpecialCharactersInPath_DecodesCorrectly() {
        // Act
        var result = UrlSplitted.Parse("/users%2Bgroups/api%2Fv1/search%3Aadvanced");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(6);

        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[1].Value).IsEqualTo("users+groups"); // Decoded

        await Assert.That(result.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[3].Value).IsEqualTo("api/v1"); // Decoded

        await Assert.That(result.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(result.Parts[5].Value).IsEqualTo("search:advanced"); // Decoded
    }

    [Test]
    public async Task Parse_WithEmptyQueryParameterValue_HandlesCorrectly() {
        // Act
        var result = UrlSplitted.Parse("?name=&active=true&debug=");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(9);

        // Check first parameter with empty value
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[1].Value).IsEqualTo("name");
        await Assert.That(result.Parts[2].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[2].Value).IsEqualTo(""); // Empty value
        await Assert.That(result.Parts[2].Name).IsEqualTo("name");

        // Check parameter with value
        await Assert.That(result.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[4].Value).IsEqualTo("active");
        await Assert.That(result.Parts[5].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[5].Value).IsEqualTo("true");

        // Check last parameter with empty value
        await Assert.That(result.Parts[7].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[7].Value).IsEqualTo("debug");
        await Assert.That(result.Parts[8].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[8].Value).IsEqualTo(""); // Empty value
        await Assert.That(result.Parts[8].Name).IsEqualTo("debug");
    }

    [Test]
    public async Task Parse_WithQueryParameterWithoutValue_HandlesCorrectly() {
        // Act
        var result = UrlSplitted.Parse("?debug&verbose=true&active");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Parts).HasCount().EqualTo(7);

        // Check flag parameter without value
        await Assert.That(result.Parts[1].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[1].Value).IsEqualTo("debug");
        await Assert.That(result.Parts[1].Name).IsEqualTo("debug");

        // Check parameter with value
        await Assert.That(result.Parts[3].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[3].Value).IsEqualTo("verbose");
        await Assert.That(result.Parts[4].Kind).IsEqualTo(UrlTemplatePartKind.VariableValue);
        await Assert.That(result.Parts[4].Value).IsEqualTo("true");

        // Check another flag parameter without value
        await Assert.That(result.Parts[6].Kind).IsEqualTo(UrlTemplatePartKind.VariableName);
        await Assert.That(result.Parts[6].Value).IsEqualTo("active");
        await Assert.That(result.Parts[6].Name).IsEqualTo("active");
    }
}
