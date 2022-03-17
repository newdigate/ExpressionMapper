using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Newdigate.ExpressionMapper
{
    public class PredicateMapper<TEntity, TDataRow> : IPredicateMapper<TEntity, TDataRow>
    {
        private readonly Func<Type, Type, ConstantExpression, Expression> _convertConstantExpression;

        public PredicateMapper(Func<Type, Type, ConstantExpression, Expression> convertConstantExpression)
        {
            _convertConstantExpression = convertConstantExpression;
        }
        public Expression<Func<TDataRow, bool>> GetMappedExpression(Expression<Func<TEntity, bool>> source)
        {
            ParameterExpression datarowParam = Expression.Parameter(typeof(TDataRow), "e");
            Expression mappedExpression = MapExpression(source.Body, datarowParam, null, out Type t);
            if (mappedExpression != null)
            {
                Expression<Func<TDataRow, bool>> result =
                    Expression.Lambda<Func<TDataRow, bool>>(
                        mappedExpression,
                        new ParameterExpression[] { datarowParam });
                return result;
            };
            return null;
        }

        private Expression MapExpression(Expression expression, ParameterExpression datarowParam, Type destinationType, out Type memberType)
        {
            memberType = null;
            if (expression is MemberExpression memberExpression)
            {
                return MapMemberExpression(memberExpression, datarowParam, out memberType);
            }
            else if (expression is ConstantExpression constantExpression)
            {
                return MapConstExpression(constantExpression, datarowParam, destinationType);
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                return MapBinaryExpression(binaryExpression, datarowParam);
            }
            return null;
        }

        private Expression MapBinaryExpression(BinaryExpression binaryExpression, ParameterExpression datarowParam)
        {
            Expression left = MapExpression(binaryExpression.Left, datarowParam, null, out Type leftType);
            Expression right = MapExpression(binaryExpression.Right, datarowParam, leftType, out Type rightType);
            return Expression.MakeBinary(binaryExpression.NodeType, left, right);
        }

        private Expression MapMemberExpression(MemberExpression memberExpression, ParameterExpression datarowParam, out Type memberType)
        {
            string memberName = memberExpression.Member.Name;
            MemberInfo member_tdatarow = typeof(TDataRow).GetMember(memberName).FirstOrDefault();
            MemberExpression dataMemberExpression = Expression.MakeMemberAccess(datarowParam, member_tdatarow);
            memberType = dataMemberExpression.Type;
            return dataMemberExpression;
        }

        private Expression MapConstExpression(ConstantExpression constantExpression, ParameterExpression datarowParam, Type destinationType)
        {
            Type typeSource = constantExpression.Type;
            if (typeSource == destinationType)
                return constantExpression;

            return _convertConstantExpression(typeSource, destinationType, constantExpression);
        }
    }

}

