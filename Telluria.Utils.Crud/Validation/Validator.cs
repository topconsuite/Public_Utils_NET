using FluentValidation;
using FluentValidation.Results;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Validation
{
  public static class Validator
  {
    public static ValidationResult Validate<TValidator, TEntity>(TEntity instance, params string[] ruleSets)
      where TValidator : BaseEntityValidator<TEntity>, new()
      where TEntity : BaseEntity
    {
      var validator = new TValidator();
      var results = validator.Validate(instance, options => options.IncludeRuleSets(ruleSets));

      return results;
    }
  }
}
