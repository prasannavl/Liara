The Liara Framework
==================================

**Project Status Note:** 
This project was just a high-performance experiment, and is now obsolete. Please check **ASP.NET vNext** at https://github.com/aspnet/home

Where elegance meets performance.

<img src="https://raw.github.com/prasannavl/liara/master/Logos/Header.png" alt="Liara Logo"/>

####High-performance, highly modular, fully asynchronous .NET based Web Framework - built with OWIN in mind.


Everything in Liara is just a MessageHandler -> Route Resolution, Formatters, Error handlers, Status handlers - Everything! - And every message handler is also an attribute. Add it globally, to LiaraModules, or to a single route, and even better - do all of it dynamically while running. Plus, its seriously fast. Faster than Microsoft's ASP.NET MVC, WebApi, ServiceStack or Nancy - Making it the fastest .NET framework out there.

If you don't like something swap it out with your own, or even trim everything down if you don't need it. Uses `async/await all the way - Top to bottom`. And the framework, inspired by Nancy auto-discovers pretty much everything. Reference Liara, and get right on to the application logic. Zero configurations - It just works.

From header and cookie parsing to Routing and IoC Logic, everything has been carefully designed with maximum performance, and developer simplicity in mind.

Documentation, and unit tests, and more demos, after finalization of the methods, and a simplified codebase. Current code is experimental for the proof-of-concept only.

Note: Even though, it already has better performance compared to the other frameworks, this current code is not optimized to its best. There are plenty of optimizations, for instance, objects pools to reduce GC pressure, that it could benefit from. 
