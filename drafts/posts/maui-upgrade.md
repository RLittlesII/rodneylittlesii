Title: Xamarin to MAUI for a large enterprise mobile application
Drafted: 01/09/2023
Published: 01/09/2023
Tags:
    - Framework
    - MAUI
    - Xamarin
---

# Requirements

- [ ] Package Upgrades
    - [ ] Assess Packages that can upgrade
    - [ ] Assess Packages with no .net6 or higher version
    - [ ] Assess long term package strategy
    - [ ] Long term strategy for dotnet versions
        - [ ] [Docker Hosted Renovate](https://docs.renovatebot.com/getting-started/running/#docker-image)
            - [SelfHosting](https://docs.renovatebot.com/examples/self-hosting/)
            - [Docker Image](https://docs.renovatebot.com/examples/self-hosting/#docker)

- [ ] Package Deprecation
 - [ ] Identify packages we no longer consume

- [ ] Make Build Better
    - [ ] Add Coverage
        - [ ] [add code coverage](https://www.jetbrains.com/help/dotcover/Running_Coverage_Analysis_from_the_Command_LIne.html#basic)
    - [ ] Add [Code Cleanup](https://www.jetbrains.com/help/resharper/CleanupCode.html#configuring-cleanupcode-with-dotsettings)


- MAUI Upgrade Available
 - [ ] Check packages for `net6.0` or greater targets
 - If no package exists
    - [ ] determine if there is a comparable package
    - [ ] 