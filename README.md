# ExpressionMapper
* a C# expression translator for database and service layer expressions
* this is a work-in-progress, please feel free to contribute.

## why ?
this allows services to be agnostic to data types. A bit like AutoMapper but specifically for predicate expressions.
A service class depending on data query/command/subscription providers shouldnt need to be coupled to any platform specific data-types.

``` c#
[ApiController]
[Route("[controller]")]
public class RecordTypeController : ControllerBase
{
    private readonly ILogger<RecordTypeController> _logger;
    private readonly IUniqueKeyQueryProvider<RecordType,string> _queryByUniqueKeyProvider;
    public RecordTypeController(
        ILogger<RecordTypeController> logger, 
        IUniqueKeyQueryProvider<RecordType, string> queryByUniqueKeyProvider)
    {
        _logger = logger;
        _queryByUniqueKeyProvider = queryByUniqueKeyProvider;
    }

    [HttpGet(Name = "GetRecordType")]
    public RecordType? GetRecordType()
    {
        RecordType? result = _queryByUniqueKeyProvider.Get("6224ff2fdc13b1955a4c2db8");
        return result;
    }
}

```


## TL/DR
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
