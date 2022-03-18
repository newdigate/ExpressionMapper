# ExpressionMapper
c# expression translation between database and service layers

* given a predicate expression for a service class named ```StudentRow```:
``` c#
Expression<Func<StudentRecord, bool>>    
    ent_predicate = 
        (StudentRecord e) => (e.Id == "10");
```
* map to a predicate expression for a data type named ```StudentRecordRow```
``` c#
Expression<Func<StudentRecordRow, bool>> 
  mapped_dto_predicate = 
    _expressionMapper
        .GetMappedExpression( ent_predicate );
```
* ```StudentRecord``` and ```StudentRecordRow``` are not related in a class heirachy, but do share certain property names, but not types
``` c#
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
```
* Properties in the expression are currently only matched on name. 
* Properties which match by name but not by type can be mapped by providing a conversion expression.
``` c#
    private static Expression ConvertConstStringToObjectIdExpression(Type leftType, Type rightType, ConstantExpression constantExpression)
    {
        if (leftType == typeof(string?) && rightType == typeof(ObjectId?))
        {
            return (string? s) => new ObjectId(s);
        }
        return null;
    }
    
    private readonly IPredicateMapper<StudentRecord, StudentRecordRow> _expressionMapper =
        new PredicateMapper<StudentRecord, StudentRecordRow>(ConvertConstStringToObjectIdExpression);
```
