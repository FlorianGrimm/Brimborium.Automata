namespace SampleWebApp.Playwright.Utility;

public record UrlValued(
        UrlTemplate UrlTemplate,
        List<UrlPartValue> ListPartValue,
        UrlValuedFormats? UrlValuedFormats = default
        ) {
    public string GetUrl() {
        var sb = new StringBuilder();
        int mode = 0;
        for (int i = 0; i < this.UrlTemplate.Parts.Length; i++) {
            UrlTemplatePart? part = this.UrlTemplate.Parts[i];
            if (mode == 0) {
                if (part.Kind == UrlTemplatePartKind.Slash) {
                    sb.Append('/');
                    continue;
                }
                if (part.Kind == UrlTemplatePartKind.Const) {
                    sb.Append(WebUtility.UrlEncode(part.Value));
                    continue;
                }
                if (part.Kind == UrlTemplatePartKind.Placeholder) {
                    if (this.ListPartValue.FirstOrDefault(p => p.Name == part.Value) is { } partValue) {
                        partValue.AddValueToString(sb, this.UrlValuedFormats);
                    }
                    continue;
                }
                if (part.Kind == UrlTemplatePartKind.QuestionMark) {
                    sb.Append('?');
                    mode = 1;
                    continue;
                }
                throw new InvalidOperationException($"Unexpected part kind {part.Kind}");
            } else if (mode == 1) {
                if (part.Kind == UrlTemplatePartKind.VariableName) {
                    if (this.UrlTemplate.Parts.Length <= i + 1) {
                        throw new InvalidOperationException("Unexpected end of URL template");
                    }
                    var nextPart = this.UrlTemplate.Parts[i + 1];
                    if (nextPart.Kind != UrlTemplatePartKind.VariableValue) {
                        throw new InvalidOperationException("Expected VariableValue after VariableName");
                    }
                    if (this.ListPartValue.FirstOrDefault(p => p.Name == nextPart.Name) is { } partValue) {
                        sb.Append(WebUtility.UrlEncode(part.Value));
                        sb.Append('=');
                        partValue.AddValueToString(sb, this.UrlValuedFormats);
                        continue;
                    } else if (part.IsOptional) {
                        // skip optional parameter
                        continue;
                    } else {
                        // use value as the default value
                        sb.Append(WebUtility.UrlEncode(part.Name));
                        sb.Append('=');
                        sb.Append(WebUtility.UrlEncode(nextPart.Value));
                        continue;
                    }
                }
                if (part.Kind == UrlTemplatePartKind.VariableValue) {
                    throw new InvalidOperationException("Unexpected VariableValue");
                }
                if (part.Kind == UrlTemplatePartKind.Ampersand) {
                    sb.Append('&');
                    continue;
                }
                throw new InvalidOperationException($"Unexpected part kind {part.Kind}");
            }
        }
        return sb.ToString();
    }
}

public record UrlPartValue(string Name) {
    public virtual void AddValueToString(StringBuilder sb, UrlValuedFormats? urlValuedFormats) {
    }
}
public record UrlPartValueString(string Name, string Value) : UrlPartValue(Name) {
    public override void AddValueToString(StringBuilder sb, UrlValuedFormats? urlValuedFormats) {
        sb.Append(WebUtility.UrlEncode(this.Value));
    }
}

public record UrlPartValueInt(string Name, int Value) : UrlPartValue(Name) {
    public override void AddValueToString(StringBuilder sb, UrlValuedFormats? urlValuedFormats) {
        sb.Append(WebUtility.UrlEncode(this.Value.ToString()));
    }
}
public record UrlPartValueGuid(string Name, Guid Value) : UrlPartValue(Name) {
    public override void AddValueToString(StringBuilder sb, UrlValuedFormats? urlValuedFormats) {
        var guidFormat = (urlValuedFormats ?? UrlValuedFormats.Default).GuidFormat;
        sb.Append(WebUtility.UrlEncode(this.Value.ToString(guidFormat)));
    }
}

public record UrlPartValueDateTime(string Name, DateTime Value) : UrlPartValue(Name) {
    public override void AddValueToString(StringBuilder sb, UrlValuedFormats? urlValuedFormats) {
        var datetimeFormat = (urlValuedFormats ?? UrlValuedFormats.Default).DateTimeFormat;
        sb.Append(WebUtility.UrlEncode(this.Value.ToString(datetimeFormat)));
    }
}

public class UrlValuedFormats {
    public static UrlValuedFormats Default { get; } = new UrlValuedFormats();
    public string DateTimeFormat { get; set; } = "yyyy-MM-ddTHH:mm:ssZ";
    public string GuidFormat { get; set; } = "D";

    public UrlValuedFormats() {
    }
}