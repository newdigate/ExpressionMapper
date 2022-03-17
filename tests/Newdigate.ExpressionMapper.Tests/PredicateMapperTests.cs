using System;
using System.Linq.Expressions;
using MongoDB.Bson;
using Xunit;

namespace Newdigate.ExpressionMapper.Tests;

public class PredicateMapperTests
{
    private readonly IPredicateMapper<StudentRecord, StudentRecordRow> _expressionMapper =
        new PredicateMapper<StudentRecord, StudentRecordRow>(ConvertConstStringToObjectIdExpression);

    [Fact]
    public void WhenIDsAreNotTheSameTypeThenConstantExpressionShouldBeTranslated()
    {
        Expression<Func<StudentRecordRow, bool>> expected_dto_predicate = (StudentRecordRow e) => (e.Id == new ObjectId("10"));
        Expression<Func<StudentRecord, bool>>    expected_ent_predicate = (StudentRecord e)    => (e.Id == "10");

        Expression<Func<StudentRecordRow, bool>> mapped_dto_predicate = _expressionMapper.GetMappedExpression(rt => rt.Id == "10");

        Assert.Equal(expected_ent_predicate.ToString(), mapped_dto_predicate.ToString());
    }

    [Fact]
    public void WhenMultipleExpressionsLogicallyAndedThenMultpleLogicalExpressionsShouldBeTranslated()
    {
        Expression<Func<StudentRecordRow, bool>> expected_dto_predicate = (StudentRecordRow e) => (e.Id == new ObjectId("10") && e.Name == "Issue");
        Expression<Func<StudentRecord, bool>>    expected_ent_predicate = (StudentRecord e)    => (e.Id == "10" && e.Name == "Issue");

        Expression<Func<StudentRecordRow, bool>> mapped = _expressionMapper.GetMappedExpression(rt => rt.Id == "10" && rt.Name == "Issue");
        Assert.Equal(expected_dto_predicate.ToString(), mapped.ToString());
    }

    private class StudentRecord
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    private class StudentRecordRow
    {
        public ObjectId? Id { get; set; }
        public string? Name { get; set; }
    }

    private static Expression ConvertConstStringToObjectIdExpression(Type leftType, Type rightType, ConstantExpression constantExpression)
    {
        if (leftType == typeof(string?) && rightType == typeof(ObjectId?))
        {
            return (string? s) => new ObjectId(s);
        }
        return null;
    }

}