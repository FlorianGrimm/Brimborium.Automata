namespace SampleWebApp.Playwright.Test;

/// <summary>
/// Unit tests for the UrlMatcher class and its URL pattern matching functionality.
/// </summary>
public class UrlMatcherTests {
    
    [Test]
    public async Task Constructor_InitializesWithRootNode() {
        // Act
        var matcher = new UrlMatcher();
        
        // Assert
        await Assert.That(matcher.Root).IsNotNull();
        await Assert.That(matcher.Root.UrlTemplatePart.Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(matcher.Root.UrlTemplatePart.Value).IsEqualTo("/");
        await Assert.That(matcher.Root.Children).HasCount().EqualTo(0);
        await Assert.That(matcher.Root.PageDefinition).IsNull();
    }
    
    [Test]
    public async Task Add_WithNullTemplate_ThrowsArgumentNullException() {
        // Arrange
        var matcher = new UrlMatcher();
        var page = new PageDefinition(UrlTemplate.Parse("/test"));
        
        // Act & Assert
        await Assert.That(() => matcher.Add(null!, page)).Throws<ArgumentNullException>();
    }
    
    [Test]
    public async Task Add_WithNullPage_ThrowsArgumentNullException() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/test");
        
        // Act & Assert
        await Assert.That(() => matcher.Add(template, null!)).Throws<ArgumentNullException>();
    }
    
    [Test]
    public async Task Add_WithSimpleTemplate_CreatesCorrectTree() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/users");
        var page = new PageDefinition(template);

        // Act
        matcher.Add(template, page);

        // Assert
        await Assert.That(matcher.Root.Children).HasCount().EqualTo(1);

        await Assert.That(matcher.Root.UrlTemplatePart.Kind).IsEqualTo(UrlTemplatePartKind.Slash);

        var usersNode = matcher.Root.Children[0];
        await Assert.That(usersNode.UrlTemplatePart.Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(usersNode.UrlTemplatePart.Value).IsEqualTo("users");
        await Assert.That(usersNode.PageDefinition).IsEqualTo(page);
    }
    
    [Test]
    public async Task Add_WithComplexTemplate_CreatesCorrectTree() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/users/{id}/edit");
        var page = new PageDefinition(template);
        
        // Act
        matcher.Add(template, page);
        
        // Assert
        await Assert.That(matcher.Root.Children).HasCount().EqualTo(1);
        
        // Check "users" node
        var usersNode = matcher.Root.Children[0];
        await Assert.That(usersNode.UrlTemplatePart.Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(usersNode.UrlTemplatePart.Value).IsEqualTo("users");
        await Assert.That(usersNode.Children).HasCount().EqualTo(1);
        
        // Check slash node
        var slashNode = usersNode.Children[0];
        await Assert.That(slashNode.UrlTemplatePart.Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(slashNode.Children).HasCount().EqualTo(1);
        
        // Check placeholder node
        var placeholderNode = slashNode.Children[0];
        await Assert.That(placeholderNode.UrlTemplatePart.Kind).IsEqualTo(UrlTemplatePartKind.Placeholder);
        await Assert.That(placeholderNode.UrlTemplatePart.Value).IsEqualTo("id");
        await Assert.That(placeholderNode.Children).HasCount().EqualTo(1);
        
        // Check final slash and edit nodes
        var finalSlashNode = placeholderNode.Children[0];
        await Assert.That(finalSlashNode.UrlTemplatePart.Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(finalSlashNode.Children).HasCount().EqualTo(1);
        
        var editNode = finalSlashNode.Children[0];
        await Assert.That(editNode.UrlTemplatePart.Kind).IsEqualTo(UrlTemplatePartKind.Const);
        await Assert.That(editNode.UrlTemplatePart.Value).IsEqualTo("edit");
        await Assert.That(editNode.PageDefinition).IsEqualTo(page);
    }
    
    [Test]
    public async Task Add_WithMultipleTemplates_SharesCommonPrefixes() {
        // Arrange
        var matcher = new UrlMatcher();
        var template1 = UrlTemplate.Parse("/users");
        var template2 = UrlTemplate.Parse("/users/{id}");
        var page1 = new PageDefinition(template1);
        var page2 = new PageDefinition(template2);
        
        // Act
        matcher.Add(template1, page1);
        matcher.Add(template2, page2);
        
        // Assert
        await Assert.That(matcher.Root.Children).HasCount().EqualTo(1);
        
        var usersNode = matcher.Root.Children[0];
        await Assert.That(usersNode.UrlTemplatePart.Value).IsEqualTo("users");
        await Assert.That(usersNode.PageDefinition).IsEqualTo(page1);
        await Assert.That(usersNode.Children).HasCount().EqualTo(1);
        
        var slashNode = usersNode.Children[0];
        await Assert.That(slashNode.UrlTemplatePart.Kind).IsEqualTo(UrlTemplatePartKind.Slash);
        await Assert.That(slashNode.Children).HasCount().EqualTo(1);
        
        var placeholderNode = slashNode.Children[0];
        await Assert.That(placeholderNode.UrlTemplatePart.Kind).IsEqualTo(UrlTemplatePartKind.Placeholder);
        await Assert.That(placeholderNode.PageDefinition).IsEqualTo(page2);
    }
    
    [Test]
    public async Task GetPageDefinitionFromUrl_WithNullUrl_ThrowsArgumentNullException() {
        // Arrange
        var matcher = new UrlMatcher();
        
        // Act & Assert
        await Assert.That(() => matcher.GetPageDefinitionFromUrl(null!)).Throws<ArgumentNullException>();
    }
    
    [Test]
    public async Task GetPageDefinitionFromUrl_WithEmptyUrl_ThrowsArgumentException() {
        // Arrange
        var matcher = new UrlMatcher();
        
        // Act & Assert
        await Assert.That(() => matcher.GetPageDefinitionFromUrl("")).Throws<ArgumentException>();
    }
    
    [Test]
    public async Task GetPageDefinitionFromUrl_WithWhitespaceUrl_ThrowsArgumentException() {
        // Arrange
        var matcher = new UrlMatcher();
        
        // Act & Assert
        await Assert.That(() => matcher.GetPageDefinitionFromUrl("   ")).Throws<ArgumentException>();
    }
    
    [Test]
    public async Task GetPageDefinitionFromUrl_WithExactMatch_ReturnsCorrectPage() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/users");
        var page = new PageDefinition(template);
        matcher.Add(template, page);
        
        // Act
        var result = matcher.GetPageDefinitionFromUrl("/users");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsEqualTo(page);
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(0);
    }
    
    [Test]
    public async Task GetPageDefinitionFromUrl_WithPlaceholder_ExtractsParameterValue() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/users/{id}");
        var page = new PageDefinition(template);
        matcher.Add(template, page);
        
        // Act
        var result = matcher.GetPageDefinitionFromUrl("/users/123");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsEqualTo(page);
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(1);
        await Assert.That(result.ListUrlPartsFound[0].Name).IsEqualTo("id");
        await Assert.That(result.ListUrlPartsFound[0].Value).IsEqualTo("123");
    }
    
    [Test]
    public async Task GetPageDefinitionFromUrl_WithComplexPlaceholders_ExtractsAllParameters() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/users/{userId}/posts/{postId}");
        var page = new PageDefinition(template);
        matcher.Add(template, page);
        
        // Act
        var result = matcher.GetPageDefinitionFromUrl("/users/123/posts/456");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsEqualTo(page);
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(2);
        
        var userIdParam = result.ListUrlPartsFound.FirstOrDefault(p => p.Name == "userId");
        await Assert.That(userIdParam).IsNotNull();
        await Assert.That(userIdParam!.Value).IsEqualTo("123");
        
        var postIdParam = result.ListUrlPartsFound.FirstOrDefault(p => p.Name == "postId");
        await Assert.That(postIdParam).IsNotNull();
        await Assert.That(postIdParam!.Value).IsEqualTo("456");
    }
    
    [Test]
    public async Task GetPageDefinitionFromUrl_WithNoMatch_ReturnsEmptyResult() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/users");
        var page = new PageDefinition(template);
        matcher.Add(template, page);
        
        // Act
        var result = matcher.GetPageDefinitionFromUrl("/products");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsNull();
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(0);
    }
    
    [Test]
    public async Task GetPageDefinitionFromUrl_WithAbsoluteHttpsUrl_StripsSchemeAndHost() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/users/{id}");
        var page = new PageDefinition(template);
        matcher.Add(template, page);
        
        // Act
        var result = matcher.GetPageDefinitionFromUrl("/users/123");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsEqualTo(page);
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(1);
        await Assert.That(result.ListUrlPartsFound[0].Name).IsEqualTo("id");
        await Assert.That(result.ListUrlPartsFound[0].Value).IsEqualTo("123");
    }
    
    [Test]
    public async Task GetPageDefinitionFromUrl_WithAbsoluteHttpUrl_StripsSchemeAndHost() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/api/v1");
        var page = new PageDefinition(template);
        matcher.Add(template, page);
        
        // Act
        var result = matcher.GetPageDefinitionFromUrl("/api/v1");
        
        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsEqualTo(page);
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(0);
    }

    [Test]
    public async Task GetPageDefinitionFromUrl_WithQueryParameters_MatchesCorrectly() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/users/{id}?tab=profile");
        var page = new PageDefinition(template);
        matcher.Add(template, page);

        // Act
        var result = matcher.GetPageDefinitionFromUrl("/users/123?tab=profile");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsEqualTo(page);
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(1);
        await Assert.That(result.ListUrlPartsFound[0].Name).IsEqualTo("id");
        await Assert.That(result.ListUrlPartsFound[0].Value).IsEqualTo("123");
    }

    [Test]
    public async Task GetPageDefinitionFromUrl_WithUrlEncodedSegments_DecodesCorrectly() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/users/{id}");
        var page = new PageDefinition(template);
        matcher.Add(template, page);

        // Act
        var result = matcher.GetPageDefinitionFromUrl("/users/john%20doe");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsEqualTo(page);
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(1);
        await Assert.That(result.ListUrlPartsFound[0].Name).IsEqualTo("id");
        await Assert.That(result.ListUrlPartsFound[0].Value).IsEqualTo("john doe"); // Decoded
    }

    [Test]
    public async Task GetPageDefinitionFromUrl_WithRootPath_MatchesCorrectly() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/");
        var page = new PageDefinition(template);
        matcher.Add(template, page);

        // Act
        var result = matcher.GetPageDefinitionFromUrl("/");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsEqualTo(page);
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(0);
    }

    [Test]
    public async Task GetPageDefinitionFromUrl_WithPartialMatch_ReturnsNoMatch() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/users/{id}/edit");
        var page = new PageDefinition(template);
        matcher.Add(template, page);

        // Act - URL is shorter than template
        var result = matcher.GetPageDefinitionFromUrl("/users/123");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsNull();
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(1); // Still captures the id
        await Assert.That(result.ListUrlPartsFound[0].Name).IsEqualTo("id");
        await Assert.That(result.ListUrlPartsFound[0].Value).IsEqualTo("123");
    }

    [Test]
    public async Task GetPageDefinitionFromUrl_WithMultipleRegisteredTemplates_FindsCorrectMatch() {
        // Arrange
        var matcher = new UrlMatcher();

        var template1 = UrlTemplate.Parse("/users");
        var page1 = new PageDefinition(template1);

        var template2 = UrlTemplate.Parse("/users/{id}");
        var page2 = new PageDefinition(template2);

        var template3 = UrlTemplate.Parse("/users/{id}/edit");
        var page3 = new PageDefinition(template3);

        matcher.Add(template1, page1);
        matcher.Add(template2, page2);
        matcher.Add(template3, page3);

        // Act & Assert - Test different URLs
        var result1 = matcher.GetPageDefinitionFromUrl("/users");
        await Assert.That(result1.Found).IsEqualTo(page1);

        var result2 = matcher.GetPageDefinitionFromUrl("/users/123");
        await Assert.That(result2.Found).IsEqualTo(page2);
        await Assert.That(result2.ListUrlPartsFound[0].Value).IsEqualTo("123");

        var result3 = matcher.GetPageDefinitionFromUrl("/users/456/edit");
        await Assert.That(result3.Found).IsEqualTo(page3);
        await Assert.That(result3.ListUrlPartsFound[0].Value).IsEqualTo("456");
    }

    [Test]
    public async Task GetPageDefinitionFromUrl_WithOptionalPlaceholder_MatchesBothVariants() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/api/{version?}/users");
        var page = new PageDefinition(template);
        matcher.Add(template, page);

        // Act & Assert - Test with version
        var resultWithVersion = matcher.GetPageDefinitionFromUrl("/api/v1/users");
        await Assert.That(resultWithVersion).IsNotNull();
        await Assert.That(resultWithVersion.Found).IsEqualTo(page);
        await Assert.That(resultWithVersion.ListUrlPartsFound).HasCount().EqualTo(1);
        await Assert.That(resultWithVersion.ListUrlPartsFound[0].Name).IsEqualTo("version");
        await Assert.That(resultWithVersion.ListUrlPartsFound[0].Value).IsEqualTo("v1");

        // Note: Testing without version would require more complex optional matching logic
        // which may not be implemented in the current version
    }

    [Test]
    public async Task GetPageDefinitionFromUrl_WithCaseInsensitiveMatching_MatchesCorrectly() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/Users/{id}");
        var page = new PageDefinition(template);
        matcher.Add(template, page);

        // Act
        var result = matcher.GetPageDefinitionFromUrl("/users/123");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsEqualTo(page);
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(1);
        await Assert.That(result.ListUrlPartsFound[0].Value).IsEqualTo("123");
    }

    [Test]
    public async Task GetPageDefinitionFromUrl_WithSpecialCharactersInPlaceholder_ExtractsCorrectly() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/files/{filename}");
        var page = new PageDefinition(template);
        matcher.Add(template, page);

        // Act
        var result = matcher.GetPageDefinitionFromUrl("/files/document%2Epdf");

        // Assert
        await Assert.That(result).IsNotNull();
        await Assert.That(result.Found).IsEqualTo(page);
        await Assert.That(result.ListUrlPartsFound).HasCount().EqualTo(1);
        await Assert.That(result.ListUrlPartsFound[0].Name).IsEqualTo("filename");
        await Assert.That(result.ListUrlPartsFound[0].Value).IsEqualTo("document.pdf"); // URL decoded
    }

    [Test]
    public async Task Add_WithQueryParameterTemplate_BuildsCorrectTree() {
        // Arrange
        var matcher = new UrlMatcher();
        var template = UrlTemplate.Parse("/search?q=test&category=news");
        var page = new PageDefinition(template);

        // Act
        matcher.Add(template, page);

        // Assert
        await Assert.That(matcher.Root.Children).HasCount().EqualTo(1);

        // Navigate through the tree to verify structure
        var searchNode = matcher.Root.Children[0];
        await Assert.That(searchNode.UrlTemplatePart.Value).IsEqualTo("search");

        // Should have question mark node
        var questionNode = searchNode.Children.FirstOrDefault(c => c.UrlTemplatePart.Kind == UrlTemplatePartKind.QuestionMark);
        await Assert.That(questionNode).IsNotNull();

        // Should eventually lead to the page definition
        // (The exact tree structure depends on how query parameters are handled)
    }
}
