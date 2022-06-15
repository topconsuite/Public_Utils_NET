using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.CommandResults
{
  public enum ECommandResultStatus
  {
    [Display(Name = "SUCCESS", Description = "The command was executed successfully")]
    SUCCESS,

    [Display(Name = "ERROR", Description = "The command was not executed successfully")]
    ERROR,

    [Display(Name = "ALERT", Description = "The command was executed successfully but there are some alerts")]
    ALERT
  }

  public interface ICommandResult
  {
    ECommandResultStatus Status { get; set; }
    string Message { get; set; }
    string ErrorCode { get; set; }
    IEnumerable<FluentValidation.Results.ValidationFailure> Notifications { get; set; }

    ICommandResult<TResult> ToCommandResult<TResult>()
      where TResult : BaseEntity;
    ICommandResult<IEnumerable<TResult>> ToEnumerableCommandResult<TResult>()
      where TResult : BaseEntity;
    IListCommandResult<TResult> ToListCommandResult<TResult>()
      where TResult : BaseEntity;
  }

  public interface ICommandResult<TResult> : ICommandResult
  {
    TResult Result { get; set; }
  }

  public interface IListCommandResult<TResult> : ICommandResult<IEnumerable<TResult>>
  {
    uint Page { get; }
    uint PerPage { get; }
    uint PageCount { get; }
    ulong TotalCount { get; }
  }
}
