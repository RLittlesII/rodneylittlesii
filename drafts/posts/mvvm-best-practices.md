Title: MVVM best practices
Drafted: 07/22/2017
Published: 01/26/2020
Tags:
    - Draft
---

# I've answered the same question twice ... BLOG POST!

# No Best Practices

# MVVM Maintainers are the frontline of ALL MVVM related questsions
## Whether they pertain to your framework or not

# A Binding Engine != MVVM

#### Why bind something through the ViewModel if it's technically static (not changing) ?!
    - Label Text
    - Button Text


## MVVM Implies

### Testability
#### Static Instances are not your friend

### SOLID
#### Maybe not all of SOLID but more SOLID than less



### Page and ViewModel are coupled.  Live with it.
- They Share a life cycle
- Page is Presentation
- ViewModel is the *STATE* of that view at any given point in time.

### ViewModel's tend to be the sewing room of applications
- People put logic there because there's "nowhere else to put it"
- Instead of creating clean abstractions, ViewModels tend to be where we stich together pieces of different interfaces
    - This violates single responsibility
    - ViewModels responsibility is to maintain state for the view, not piece together the return of a web request