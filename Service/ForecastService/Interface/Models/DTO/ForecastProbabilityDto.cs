using JetBrains.Annotations;

namespace ForecastService.Interface.Models.DTO
{
    /// <summary>
    /// Probability
    /// </summary>
    [PublicAPI]
    public enum ForecastProbabilityDto
    {
        NotDefined,
        Guaranteed,
        High,
        Low,
    }
}
