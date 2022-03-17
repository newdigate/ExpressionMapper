namespace Newdigate.ExpressionMapper
{
    public interface IPredicateMapper<TEntity, TDataRow> : IExpressionMapperStruct<TEntity, TDataRow, bool>
    {
        //Expression<Func<TDataRow, bool>> GetMappedExpression(Expression<Func<TEntity, bool>> source);
    }

}

