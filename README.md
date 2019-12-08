# eventsource-crm-example

proof-of-concept for Event Source data store using .NET Core &amp; Entity Framework

**FEEDBACK IS WELCOME & APPRECIATED** (just open an issue)

See Greg Young's [SimplestPossibleThing](https://github.com/gregoryyoung/m-r) CQRS repo which guided the start of this.


* **simple-cqrs/** : core abstractions and common functionality
* **crm-domain/** : domain-specific aggregate models, commands, and events
* **crm-webapp/** : ties it together and provides db access

[Reactive Extensions](https://github.com/dotnet/reactive) are used for the command bus.

[LanguageExt](https://github.com/louthy/language-ext) is used throughout.

Currently the "web app" has no UI; it is just an API. At this point only a few minimal commands are implemented: create, rename, and read a "contact" aggregate.

Look here to see the basic idea of what is involved in defining an aggregate:
```
crm-domain/Contact.cs
crm-domain/ContactCommands.cs
crm-domain/ContactCommandHandlers.cs
```

connection string is in appsettings.json:
```
"AggregateEventStore": "Data Source=127.0.0.1;Database=EventsourceCrmExample;Trusted_Connection=True"
```

Actual interaction with db (via EF):
```
crm-webapp/EventStore.cs
```

Install the .NET Core EF tooling with `dotnet tool install --global dotnet-ef`.  You can then create an empty database with `dotnet ef database update`.
