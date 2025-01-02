Title: Null, The Absence of a Reference in C#
Drafted: 12/04/2024
Published: 12/04/2024
Tags:
    - C#
    - Language
    - Null
    - Nullability
---

## Null, the mistake we are learning

[Null Reference](https://en.wikipedia.org/wiki/Null_pointer) is what Tony Hoare called his billion-dollar mistake. I always say that learning from your mistakes makes you smart, and learning from others makes you wise. So this post will try to impart some wisdom from what I have learned by turning on the C# 8 Nullable Reference Type feature in large codebases.

#### Disclaimer

This is **not** a critique of the feature itself.  This is about `null`, how to take advantage of the feature, and things I think I know after enabling the feature a few times.

## The Absence of a Reference

![null toilet paper](https://blog.matesic.info/image.axd?picture=/Blog%20posts/2019/NULL/Papers1.png)

_above image borrowed_

The image demonstrates the "absence of a reference".  There is a distinct difference between "No Paper" and "Null".  Notice that in the `null` case there is no reference or understanding there is paper at all!  It could be an image of a hand towel holder.  There is no reference to the entity we care about.  `null` doesn't just mean you have none of a given type, it means there is no actual connection to the type.  So when we return `null` before turning on Nullable Reference Types, we are potentially violating the contract we have established with our consumer.

The language feature solves this by allowing developers to provide a type for the reference you expect.  Sort of a type-safe `null`.  The compiler can now understand `null` in the context of `type` because every `type` has _nullability_; the ability to be `null`.  Declaring `Nullable<T>` (`T?`), you are communicating to the compiler you expect it to handle the _null-state_ for the provided `type`.  If you attempt to assign `null` to `T`, the compiler complains because `T` does not have `?` which provides access to the _null-state_.

### Returning `null`

When you return `T?`, you are forcing any consumer of that return value to verify the _nullability_ and the _null-state_ of what is provided to them.  Whether it's a method or a property, every inspection will require an evaluation of the _null-state_.  Before this feature, you didn't have the information readily available at compile time, and lack of doing an evaluation would result in a [`NullReferenceException`](https://learn.microsoft.com/en-us/dotnet/api/system.nullreferenceexception).  

### `null` method arguments

When accepting `T?` as a method argument you are taking in a value that is potentially `null`.  You don't know the context of why it's null, so if your method requires the value to exist, you can check for null and throw.

Take the MAUI current application

```csharp

public class Application
{
    public Application? Current { get; }
}

...

public static true SomeExtension(this Application? application) // nullable argument
{
    ArgumentNullException.ThrowIfNull(application, nameof(application));

    // do stuff
}

...

Application.Current.SomeExtension();  // if this is null for a reason we don't expect, an informative exception is thrown.

```

## Turning on C# Nullability

Turning on C# Nullable Reference Types is like shining a spotlight on all the imperfections in a codebase.  The feature is fully analyzed by the compiler.  This means that once you turn the feature on, every opportunity for you to access an object with _null-state_ has to be either guarded against or tolerant of `null`.  You can turn the feature on at the assembly level, or the file level.  I would recommend starting small and understanding how hidden the absence of state is prevalent in the code base you turn it on for.

### Handle `null`

Unhandled `null` returns generally result in [`System.NullReferenceException`](https://learn.microsoft.com/en-us/dotnet/api/system.nullreferenceexception).  When defining an API surface with nullability turned on you cannot return `null` from a method or property without making the return value nullable.  Once you change the type to `T?`, you will get compilation errors (with the warnings as errors) when you are potentially creating a `NullReferenceException`.  This will allow you to handle these concerns accordingly and evaluate if this is recoverable for your application.

### Don't return `null` if you can return a default value

Given the below signature, we could optimize the implementation (and solidify the contract), by changing it.

```csharp
public IEnumerable<Thing>? Things()
{
  return apiClient.Get<Thing>();
}
```

to

```csharp
public IEnumerable<Thing> Things()
{
  IEnumerable<Thing>? result = apiClient.Get<Thing>();

  return result ?? Enumerable.Empty<Thing>();
}
```

In this, we have provided a default value for the return.  Our consumer now only needs to take the return value and start processing.  They don't have to check for the existence of a value, we've guaranteed that _something_ gets returned.

### `bool?` is not the new "3-way state"

Yes, the _null-state_ creates a built-in 3-state `enum`.  I know it's tempting to use this in places.  Resist.  While it is fine for some use cases, it is not scalable.  The second you need a fourth value, you have a lot of code to modify.

### methods can accept null arguments and still guard against them being null

Back to our `Application.Current?` example, you don't expect it to be null, but it _can_ be null.  If you don't want it to be null when you are using it, you should guard against it.  We don't want the objects _null-state_ to potentially bite us, we have to take responsibility for it.

### Nullable default method parameters are fine internal to a system, but shouldn't cross "subsystem" boundaries

This next tip is more for developers making public APIs that others consume.  Given the method with the following signature.  If in the future I decide to change `bool? shouldCare = null` to `bool? shouldCare`, by the rules of semver, this is a breaking API change.  It's not the end of the world, but I have made this mistake and burned a major version for no reason!

```csharp
public interface IDoStuff
{
  public Task MakeStuffHappen(string name, int quantity, bool? shouldCare = null);
}
```

### Use `System.Diagnostics.CodeAnalysis` attributes

[`System.Diagnostics.CodeAnalysis`](https://learn.microsoft.com/en-us/dotnet/api/system.diagnostics.codeanalysis) has attributes you can use to decorate your API surface, to indicate the presence of the _null-state_.  They are supported by the compiler.  If you know your method _might_ return `null`, consider using `MaybeNullAttribute`.  If you want to state an argument `is not null` you can use the `NotNullAttribute`.

### Encapsulate your nullability, don't force every consumer to check for null if you can guard against it for them

If you have a `private` method that returns `null`, consider checking that return if you don't want it to be `null`.  You can capture the _null-state_ and throw an exception with meaning.  You can return a default value.  You can ensure that the private `null` value doesn't bubble up to the public API surface.  There are a lot of options.  The option you should avoid if possible, letting the `null` return run rampant through your call stack.

### Don't act like it's not there
The "see no evil" approach causes defects.  Turning on the Nullable Reference Type feature will help you appreciate how the absence of a thing™ can severely affect your software.  There are times when the business expects a value to exist, but the software doesn't provide a value.  Remember some of us are writing software for users to consume.  It has to behave and function in a certain way based on a set of requirements.  So when it doesn't behave how we expect, we have to figure out why.  Ensuring your business logic properly accounts for and handles _null-state_ is important for long-term system health.

### Only Return Null When

- Doing so won't kill your application
- The application can recover from the _null-state_ inspection
- You expect every consumer of the method to gracefully handle a `null` return value

### The `default` value of a reference object is `null`

Next, we'll talk about `default`.  In general value types have a value.  The `default(int)` is `0`.  So if you create a reference type, guess what the `default` value is?  That's right `null`.  That means we can't return things like `default(T)!` and expect the application not to throw `NullReferenceException`.  This was one of those no-brainer things when I thought about it.  But I had to think about it.  This means that every reference type by default is `null`.  Not just once I turn on the feature, it has always been and I haven't always ensured there is a default value.

It's worth talking about [sentinel values](https://en.wikipedia.org/wiki/Sentinel_value) here.  I haven't historically been a fan of them in C#.  The problem is checking for _null-state_ doesn't guarantee the default value.  It's just the default of a constructed object is `null`.  So while I am not a fan of sentinel values, I do think there is a place for a type having a `Default`.  In many functional languages the concept of `None` or [`Optional<T>`](https://github.com/louthy/language-ext?tab=readme-ov-file#optional-and-alternative-value-monads) exists.  These provide you access to a default value that is not `null`.  So while I won't recommend returning -999 for an `int` return value.  I _might_ recommend a more type safe functional approach to ensure that `null` doesn't cause mischief.

### `OrDefault()` methods return `null`

- FirstOrDefault<T?>()
- LastOrDefault<T?>()
- SingleOrDefault<T?>()

These methods return the `default` value if one is not found.  The `default` value for a reference type with _null-state_ is `null`.  So if you don't guard the return value, you could have `NullReferenceException` problems all over your code.

## Tips for handling `null` in your code

### File or Assembly at a time

I would recommend a few approaches.

- File at a time
  - As you are changing an API surface and surrounding dependencies
  - If you are adding new files
- Assembly at a time
  - If you have multiple assemblies (like a lot of .NET applications I have seen)
  - Start with a single assembly

### Start with the edges of the application either the data layer or the UI layer

I wouldn't recommend jumping in at the beginning with your business logic.  It becomes overwhelming to retrofit at first.  I recommend starting with your data layer, or your UI layer.  These are usually the most straightforward and forgiving from what I have seen.  Basically, you want to pick an edge and work your way in, starting in the middle gets weird.

### Small commits so you can easily walk backward

Make small commits.  As you open up a cohesive area, commit.  I have backed out adding _nullability_ realizing I can encapsulate a `null` object.  Don't rush to completion, take the time to understand what are the concerns and constraints of your system and assign and handle nullability accordingly.

### Turn on C# nullability warnings as errors

Read the list of [nullable warnings](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/nullable-warnings) and choose which ones you want to make errors.  I don't have a recommendation, I am still learning it depends on the code base which errors matter.

### Don't use Null Reference Exceptions as your catch-all unhandled exception, it's not exceptional, you control it

Inpsect objects that have _nullability_.  Do not allow `NullReferenceException` to run rampant.  They are not exceptional, nor are they informative.  You are better off explicitly handling _nunllability_ and

### Don't use nullable enums, rather default the enum with a `None` or `NA` or even `Default`

If you are using an `enum` I would suggest using a default value of `None` or `NA` for your `enum`.  This will allow you to inspect anything as a default.

### Avoid `!` operator

At first glance, this operator seems like a good idea, but it defeats the purpose of the effort.  This operator tells the compiler that you got it.  You know that the compiler may _think_ it can be `null`, but you know it can't.  This operator defeats the purpose of turning on this language feature in most places.  The whole reason we are turning on the feature is so the compiler can help us identify potential `NullReferenceException`.  Instead, we use `!` all over the place and still end up with those exceptions.  Use it sparingly.  Initialization and LINQ lambdas are the main places I find myself using it.

### Pay attention to methods that return null but could benefit from returning a `default`

- Methods that return lists
  - `IEnumerable<T>?` => `[]`
- Methods that return nullable bool
  - `bool?` => `false`
- Methods that _might_ be better returning a default value
  - `enum`
_ 
### Use object initialization syntax

Some developers prefer constructors and some object initialization.  If you prefer object initialization for concerns like data transfer objects, consider using the [`init`](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/init) keyword.  This will signal to the compiler that the object should be initialized with a value, and that the value should not be null, because you cannot set it again after it has received its initial value.

### Write tests that verify behaviors

I am not going to tell you to test all the things™.  I am going to tell you that verifying the behavior of something that returns `null` is a good idea.  You want to know how your system responds to `null` inputs and outputs.  You don't want to leave it to chance.  A few well-constructed tests can give you some clarity and potentially reduce regressions.

## What's next?!

Hopefully this provides you some ideas to turning on C# Nullable Reference Types in a codebase.  I know this isn't a comprehensive list, nor is it a step by step guide.  I have tried to mimic one approach for every codebase I encounter.  I am learning that using the same guidelines and finding the right seams is a manageable approach.  So go forth and capture the _null-state_, don't allow it to bring down applications.  Enable the feature at your pace and understand what `nullability` means in your codebase!

### Links

- [Nullable References Documentation](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-references)
- [Nullable migration strategies](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-migration-strategies?source=recommendations)
- [Plan your migration](https://learn.microsoft.com/en-us/dotnet/csharp/nullable-migration-strategies?source=recommendations#plan-your-migration)
- [Null safety in C#](https://learn.microsoft.com/en-us/training/modules/csharp-null-safety/?source=recommendations)