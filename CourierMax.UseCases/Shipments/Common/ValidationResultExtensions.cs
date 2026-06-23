using System.Collections.Immutable;
using FluentValidation.Results;
using ROP;

namespace CourierMax.UseCases;

public static class ValidationResultExtensions
{
    /// <summary>Convierte los errores de FluentValidation a ImmutableArray&lt;Error&gt; de ROP.</summary>
    public static ImmutableArray<Error> ToRopErrors(this ValidationResult result)
        => result.Errors.Select(e => Error.Create(e.ErrorMessage)).ToImmutableArray();
}
