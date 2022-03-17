using System;
using System.Linq.Expressions;

namespace Newdigate.ExpressionMapper
{
    public interface IExpressionMapper<TEntity, TDataRow>
    {
        Expression<Func<TDataRow, object>> GetMappedExpression(Expression<Func<TEntity, object>> source);
    }

}

