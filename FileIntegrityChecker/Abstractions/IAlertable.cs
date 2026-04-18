using FileIntegrityChecker.Enums;

namespace FileIntegrityChecker.Abstractions;

/// <summary>
/// Defines the contract for components that raise alerts.
/// </summary>
// OOP: Interface
public interface IAlertable
{
    /// <summary>Fires an alert with the given message and severity.</summary>
    void RaiseAlert(string message, AlertLevel level);
}
