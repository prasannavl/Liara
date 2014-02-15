The Liara Framework
===================

Where elegance meets performance.

<img src="https://raw.github.com/prasannavl/liara/master/Logos/Header.png" alt="Liara Logo"/>

####High-performance, highly modular, fully asynchronous .NET based Web Framework - built with OWIN in mind.


Everything in Liara is just a MessageHandler -> Route Resolution, Formatters, Error handlers, Status handlers - Everything! - And every message handler is also an attribute. Add it globally, to LiaraModules, or to a single route.

If you don't like something swap it out with your own, or even trim everything down if you don't need it. Uses async/await all the way - Top to bottom. And the framework, inspired by Nancy auto-discovers pretty much everything. Reference Liara, and get right on to the application logic. Zero configurations - It just works.

From header and cookie parsing to Routing and IoC Logic, everything has been carefully designed with maximum performance, and developer simplicity in mind.

**TODO:**

* Trie-based Routing + Add support for route variables.
* Finish Request.Parameters -> Iterate through UrlRoute Parameters, Request Body, and QueryString.
* Enable MessageHandler attributes and Route Conditions for Routes.
* Add Razor View Engine Formatter. 
* Implement more ILiaraFormatSelector for direct selections, and add Extensions methods for quick selection on module return.
* Enable support for model binding, validation and route methods with parameters.
* Enable support for attribute Meta-data, using LiaraExplorer.
 
More demos, units tests, and documentation to come soon, as the above list nears completion.
