using System;
using System.Linq.Expressions;
using Telluria.Utils.Crud.Entities;

namespace Telluria.Utils.Crud.Commands.BaseCommands
{
    public class BaseFindCommand<TEntity> : ICommand where TEntity : BaseEntity
    {
        public Expression<Func<TEntity, bool>> Where { get; set; }
        public string[] Includes { get; set; }

        public BaseFindCommand(Expression<Func<TEntity, bool>> where, params string[] includes)
        {
            Where = where;
            Includes = includes;
        }
    }
}
