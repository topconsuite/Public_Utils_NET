﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Telluria.Utils.Crud.CommandResults
{
  public enum CommandResultStatus
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
    CommandResultStatus Status { get; }
    string Message { get; }
    string ErrorCode { get; }
    List<FluentValidation.Results.ValidationFailure> Notifications { get; }
  }

  public interface ICommandResult<TResult> : ICommandResult
  {
    TResult Result { get; }
  }

  public interface IListCommandResult<TResult> : ICommandResult<IEnumerable<TResult>>
  {
    uint Page { get; }
    uint PerPage { get; }
    uint PageCount { get; }
    ulong TotalCount { get; }
  }
}
