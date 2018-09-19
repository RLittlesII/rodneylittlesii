Title: Azure Infrastructure as Code
Drafted: 09/17/2018
Published: 11/08/2018
Tags:
    - Azure
    - Dev Ops
    - Infrastructure As Code
---

# This is Microsoft?

I recently was fortunate enough to attend Azure Dev Days hosted by Microsoft in Houston, Texas.  I saw the Houston Azure Meetup coordinator, Adam Hems there.  I happened to be talking to him about Infrastructure as Code (IaC), and turns out I missed the meetup where [Sean Fitzsimmons](https://www.linkedin.com/in/sean-fitzsimmons-8a29405/) explained the basics of Azure Infrastructure as Code.  Sean happened to overhear my conversation with Adam and invited me for a two hour one on one session where he would break down the core concepts.  I bring this up to say, this feels like Microsoft making a good faith effort towards customer success with their tool chain.

# Why use infrastructure as code?

Are you trying to automate the operations department out of a day job?!  The answer is no.  The driver for automation is to empower the operations department to enforce a consistent approach to how infrastructure is created and managed.  Reasons for automation include, consistency of the infrastructure and the speed and agility of the deployment.  Imagine you create a service ticket for a test VM, once the ticket is approved, your VM is created.  No human had to, grab the ticket, service the ticket or notify you that your VM has been created.  This is acheivable using infrastructure as code.  If the creation of that VM is a consistent, repeatable script, this is a trivial task.  It isn't about taking operations out of the picture, it's quite the opposite.  Only involve operations for the creation of the IaC and then delegate management of execution to other processes.

# What tools are available?

Azure offers several ways to approach IaC:
- [PowerShell](https://docs.microsoft.com/en-us/powershell/azure/overview)
- [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/?view=azure-cli-latest)
- [ARM Templates](https://docs.microsoft.com/en-us/azure/azure-resource-manager/) (can be used with either aformentioned options)

Depending on the desired skill set of the operations team, some of these approaches may be more enticing than others.  Some departments want to use PowerShell, others will choose a more platform agnostic approach.  Regardless of your flavor this is where the rubber starts to meet the road.  I asked Sean the question we all face: **"What are the best practices around setting up your Azure resource groups to maximize your IaC offerings?"**.  As with all things, there are so many options and avenues to explore, it truly depends what your objectives are.

For me the Azure CLI feels cleaner and has a better cross platform experience. Even though PowerShell is now useable on Linux, the PowerShell API feels very archaic to me.  I can use the Azure CLI from PowerShell or Bash without any loss of knowledge of the API surface.  That said, the recommended approach (and I agree), is to use ARM Templates to define the infrastructure and use Azure CLI to execute the ARM Template against Azure.

# What's in a Template?
Azure Resource Manager (ARM) template, is a JSON document that adheres to a [schema](http://schema.management.azure.com/schemas/2015-01-01/deploymentTemplate.json#). ARM Templates define **state** of the infrastructure at the time it is provisioned.  This makes it easy to keep state static between executions, and only update the resources required since the last execution.

An ARM Template consists of:

- **Parameters:** Values that need to be changed per execution.  Default values are an option
- **Variables:** Values that are used in a template to simplify language expressions 
- **Resource:** Description of what will be deployed
- **Outputs:** Nested templates, post template execution
    - Nested templates are modularized reusable templates

## Deploy a Template
There are two distinct stages to Azure deployment.  The first step provisions your resources in Azure.  The second defines the state of the resource after it has been provisioned.  ARM Templates allow you to define the provisioned resources that need Azure to manage.  While a concept as a Desired State Configuration would manage the state of your infrastructure after it has been provisioned.

Templates are executed in `Incremental` mode by default.  That means that only changes since the last template execution will be applied.  So the state of your environment will only be updated after the last deployment.  You can override this and deploy the entire ARM template each execution.

It was not recommended to put Application Settings in the ARM Template.  Instead we should put those things in Azure Dev Ops and swap values in our build/release pipelines.

The main thread during this part of the conversation was **Resource Group Design becomes extremely important to get usefulness out of ARM Templates**

## Desired State Configuration
Candidly this was the least discussed topic due to time, and I know enough about desired state configuration to know the acronym.  The main take away's were

Desired State Config ...
- Needs to be hosted somewhere
- Should be called after resources have been provisioned
- Can be executed from the ARM Template

# Resources

The two hours I spent with Sean spawned more questions than could be answered.  I am sure after reading this is seems abrupt and incomplete, it is.  This is the notes I was able to capture in the time we had together, there were several more hours of content he could discuss.  Below is a list of resources that were provided to me to help get started.

- [Azure Quickstart Templates](https://azure.microsoft.com/en-us/resources/templates/)
- [Azure Quickstart Templates on Github](https://github.com/Azure/azure-quickstart-templates)