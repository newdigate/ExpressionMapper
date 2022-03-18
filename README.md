# ExpressionMapper
c# expression translation between database and service layers

```
        Expression<Func<StudentRecordRow, bool>> mapped_dto_predicate = _expressionMapper.GetMappedExpression( (StudentRecord rt) => rt.Id == "10");
```