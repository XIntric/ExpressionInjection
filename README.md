# Summary

Provide ability to declare abstract expressions, which can be injected and merged into expressions using method calls.

This enables the ability to make abstract methods for complex "subqueries" as methods, which then can be used in expressions for better readbility. The resulting expression injects any expression from injectable expression methods onto the final expression, which enables providers, for example EF, EF CORE, Linq2Sql or any other, to parse the final expression.

# Usage

The following requirements apply on injectable expression methods:

* Injectable expression methods must be tagged with the "Injectable" attribute.
* Injectable expression methods must be declared static.
* Injectable expression methods must declare its expression using a call to Injector.Inject().
* IQueryables using injectable expressions must enable expression injection using the extension method EnableInjection().

## Sample injectable expression

```C#
[Injectable]
public static bool IsEven(this int value)
    => Injector.Inject(() => (value % 2) == 0)
```

## Sample usage

```C#
    var ints = Enumerable.Range(0,10).ToList();
    var intsq = ints
            .AsQueryable()
            .EnableInjection(); //<-- This enables ability to use injectable expressions.

    var evenints = intsq.Where(i => i.IsEven()).ToList() //{0,2,4,6,8};
```

This sample injects the IsEven() method onto the expression directly, enabling it to be evaluated as a primitive expression. Any evaluator of the expression will see the expression as:

```C#
    var evenints = intsq.Where(i => (i % 2) == 0).ToList();
```


# Versioning
In version 0.x.x, minor version change possibly means breaking changes. After release of 1.0.0, breaking changes will only occur on major version changes.
