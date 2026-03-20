namespace HumanHands.Domain.Enums;

/// <summary>
/// Classifies the type of physical work being delegated to a human agent.
/// An LLM uses this to route the task to the most appropriate worker pool.
/// </summary>
public enum JobType
{
    /// <summary>A short trip to purchase, pick up, or drop off items (e.g., pharmacy run, grocery pickup).</summary>
    Errand = 1,

    /// <summary>Point-to-point transport of goods or documents from one location to another.</summary>
    Delivery = 2,

    /// <summary>Physical labour requiring human presence on-site (e.g., assembly, cleaning, moving).</summary>
    ManualLabor = 3,

    /// <summary>A bespoke task that does not fit the standard categories; the Description field must be detailed.</summary>
    Custom = 4
}
