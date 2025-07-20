namespace Brimborium.AutomataCode;

/// <summary>
/// Represents a hierarchical name structure using forward slash (/) as a separator.
/// This class provides immutable hierarchical naming with support for equality comparison.
/// </summary>
public class HierachicalName: IEquatable<HierachicalName> {
    private static HierachicalName? _Empty;

    /// <summary>
    /// Gets an empty hierarchical name instance (singleton).
    /// </summary>
    public static HierachicalName Empty => (_Empty ??= new());

    private readonly string _Value;

    /// <summary>
    /// Initializes a new instance of the <see cref="HierachicalName"/> class with an empty value.
    /// This constructor is private and used only for creating the Empty singleton.
    /// </summary>
    private HierachicalName() {
        this._Value = string.Empty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HierachicalName"/> class with the specified value.
    /// </summary>
    /// <param name="value">The hierarchical name value.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null.</exception>
    public HierachicalName(string value) {
        this._Value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HierachicalName"/> class by combining a name with an existing hierarchical name.
    /// The result will be in the format "name/right".
    /// </summary>
    /// <param name="name">The name to prepend.</param>
    /// <param name="right">The hierarchical name to append.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> or <paramref name="right"/> is null.</exception>
    public HierachicalName(string name, HierachicalName right) {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(right);
        this._Value = $"{name}/{right.ToString()}";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HierachicalName"/> class by combining an existing hierarchical name with a name.
    /// The result will be in the format "left/name".
    /// </summary>
    /// <param name="left">The hierarchical name to prepend.</param>
    /// <param name="name">The name to append.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="left"/> or <paramref name="name"/> is null.</exception>
    public HierachicalName(HierachicalName left, string name) {
        ArgumentNullException.ThrowIfNull(left);
        ArgumentNullException.ThrowIfNull(name);
        this._Value = $"{left.ToString()}/{name}";
    }

    /// <summary>
    /// Gets the string value of this hierarchical name.
    /// </summary>
    public string Value => this._Value;

    /// <summary>
    /// Returns the string representation of this hierarchical name.
    /// </summary>
    /// <returns>The hierarchical name as a string.</returns>
    public override string ToString() => this._Value;

    /// <summary>
    /// Determines whether the specified object is equal to the current hierarchical name.
    /// </summary>
    /// <param name="obj">The object to compare with the current hierarchical name.</param>
    /// <returns>true if the specified object is equal to the current hierarchical name; otherwise, false.</returns>
    public override bool Equals(object? obj) {
        if (obj is HierachicalName hierachicalName) {
            return this.Equals(hierachicalName);
        } else {
            return false;
        }
    }

    /// <summary>
    /// Determines whether the specified hierarchical name is equal to the current hierarchical name.
    /// Comparison is performed using ordinal string comparison.
    /// </summary>
    /// <param name="other">The hierarchical name to compare with the current hierarchical name.</param>
    /// <returns>true if the specified hierarchical name is equal to the current hierarchical name; otherwise, false.</returns>
    public bool Equals(HierachicalName? other)
        => (other is { } obj)
            && string.Equals(this._Value, obj._Value, StringComparison.Ordinal);

    /// <summary>
    /// Returns the hash code for this hierarchical name.
    /// </summary>
    /// <returns>A hash code for the current hierarchical name.</returns>
    public override int GetHashCode()
        => this._Value.GetHashCode();

}
