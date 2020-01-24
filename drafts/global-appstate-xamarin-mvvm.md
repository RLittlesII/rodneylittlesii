Title: Global Centralized AppState in MVVM
Drafted: 01/22/2020
Published: 01/22/2020
Tags:
    - State Management
    - MVVM
    - MVU
---

# Central State is bad
I've always thought that central state in an application is bad!  I have been using the MVVM pattern for years and the thought of global shared state always seemed odd.  


# MVVM - View Model maintains State

![MVVM](https://www.researchgate.net/publication/275258051/figure/fig3/AS:294465420972038@1447217435491/The-Model-View-ViewModel-MVVM-architectural-pattern-In-MVVM-the-View-layer-is.png)

In the MVVM pattern the View Model is considered the state.  It consumes data, mutates it's internal state and notifies the View.  It maintains it's state and dictates the state of the view.  So if I have encapsulated state, I don't need to maintain the cost of global state.  I can destroy the state when I navigate away from the page.

Then I thought, if I pass parameters, am I not in effect sharing state across boundaries?!  Is that violating the internalized state that I believed I was acheving with View Models?  Isn't the database just global state?  The questions became too much.

# MVU - No View Model, Needs Central State
![MVU](https://www.google.com/imgres?imgurl=https%3A%2F%2Fstaltz.com%2Fimg%2Fmvu-unidir-ui-arch.jpg&imgrefurl=https%3A%2F%2Fstaltz.com%2Funidirectional-user-interface-architectures.html&docid=7tQqoxvpNRfriM&tbnid=xy3N_3XLKt9UxM%3A&vet=10ahUKEwjgjLmS4JvnAhUGIKwKHRVnD0kQMwhgKAgwCA..i&w=1280&h=960&client=firefox-b-1-d&bih=1263&biw=1720&q=MVU%20pattern%20diagram&ved=0ahUKEwjgjLmS4JvnAhUGIKwKHRVnD0kQMwhgKAgwCA&iact=mrc&uact=8)

![MVU](https://staltz.com/img/flux-unidir-ui-arch.jpg)
### Comet
### Redux
### Vuex

# ViewModels React to State

# One ViewModel to rule them all?