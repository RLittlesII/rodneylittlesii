Title: Cake Extensibility using Add-Ins and Modules
Drafted: 09/10/2017
Published: 12/27/2018
Tags:
    - Cake
    - Dev Ops
    - Open Source
    - Continuous Delivery
---

# Build Extensibly

Cake is a build automation and orchestration tools that allows C# developers to write build scripts in C# and execute them cross platform.  The main reason why I bought into Cake as a DSL is that Cake is [extensible](https://en.wikipedia.org/wiki/Extensibility).  Out of the box Cake allows developers to plug into the core execution.  Cake has an Add-In system that allows you to extend the `ICakeContext`.  Cake has an extensive list of [Add Ins](https://cakebuild.net/addins/) already baked for your consumption, or you can write your own!  Cake also allows you to register custom implementations to execute in your run time.  This can be achieved by creating an instance of `ICakeModule`.  So, let's take a look at Add-Ins, Modules and a little about when to elect one over the other for a given use case.


## The Extensions

- **Add-Ins** extend Cake by providing wrappers around `ICakeContext` and will generally abstract some domain specific logic.

- **Modules** extend Cake by allowing a developer to provide custom implementations for certain interfaces in Cake.

- **Scripts** allow a developer to provide a set of scripts to load and execute during the Cake execution.

## Cake Add-Ins

In order to create an Addin you need to determine if you are going to create an extension method from `ICakeContext`.

```csharp
[CakeMethodAlias]
public static void ExtensionMethod(this ICakeContext context, ExtensionMethodSettings settings)
```

Or you can provide a class implementation as a cacheable property and wrap the `ICakeContext`.  This is becoming my preferred option when grouping functionality together.

```csharp
[CakePropertyAlias(Cache = true)]
public static PropertyProvider Property(this ICakeContext context)
```

To me one of the great things about Cake Add-Ins, I can unit test them to make sure the output to the Cake runtime is as expected.  That means I can abstract bigger pieces of build logic into a property or method alias and test the abstraction.  This allows me to unit test portions of my build, and give me better confidence in my build scripts.

The Property Alias is a cacheable property that can be reused throughout your Cake runtime.  The Method Alias is a single execution of a function.  I won't belabor the difference here, if you look through a few of the add-ins in the [Cake Contrib](https://github.com/Cake-Contrib) organization, you will see that several examples of each.

Add-Ins are great for:

- Implementing a ToolRunner for a CLI program
- Abstracting away build logic
- Implement C# code that you might want to execute during a Cake execution

## Cake Modules

Modules extend Cake by allowing a developer to provide a custom implementation for certain interfaces in Cake.  Cake uses Autofac for its dependency inversion container.  So, you can create an `ICakeModule` and register it against the container and have your custom implementations override Cake defaults!!!

You simply create your own module, register your interfaces to extend the run time and then Cake will do the rest.

#### Note: You have to add an assembly reference `[assembly: CakeModule(typeof(MyCakeModule))]`

```csharp
[assembly: CakeModule(typeof(MyCakeModule))]

namespace Cake.Extensibility
{
    public class MyCakeModule : ICakeModule
    {
        public void Register(ICakeContainerRegistrar registrar)
        {
            registrar.RegisterType<Decorator>().As<IDecorator>();

            registrar.RegisterType<Strategy>().As<IStrategy>();

            registrar.RegisterType<Provider>().As<IProvider>();
        }
    }
}
```

Cake Modules need to be registered at application startup.  Luckily for us, this is all handled for us by Cake. The Cake runtime will scan for any loaded modules and register them against the Dependency Inversion container.  Cake has a `ModuleLoader` that recursively searches for any `Cake.*.Module.dll`, scans the assembly and loads anything assignable from `ICakeModule`.

Modules are great for:

- Changing Cake functionality at run time
- Registering your own services
- Changing NuGet Source
- Overriding logging

# Unlimited Power!

This is a short primer on ways that you can extend the `ICakeContext` and provide your own Cake with nugets of your own making.  At this point we have taken a quick glance at the extensibility of the Cake build automation tool.  We've seen how we can write extension aliases that allow us to abstract complex logic.  We have further seen how we can load our own services written in C# to the Cake runtime for consumption in our scripts!
