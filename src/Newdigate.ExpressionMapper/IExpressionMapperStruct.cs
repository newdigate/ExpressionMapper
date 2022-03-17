using System;
using System.Linq.Expressions;

namespace Newdigate.ExpressionMapper
{
    public interface IExpressionMapperStruct<TEntity, TDataRow, TStructReturnType> where TStructReturnType : struct
    {
        Expression<Func<TDataRow, TStructReturnType>> GetMappedExpression(Expression<Func<TEntity, bool>> source);
    }

}

