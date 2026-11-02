- Explain your main technical choices and why you preferred them over alternatives.
	- I chose EF Core with SQLite becasue its a simple option for a code-test-task like this and it keeps the service easy to run locally. I could consider Dapper, but the main task 
      was to aggregate KYC data and not to optimize database access, so I chose EF Core for its simplicity and ease of use.

- Describe any trade-offs or assumptions you made.
	- SQLite is suitable for light-weight local implementation and tests, I would not use it for a production environment. 
	- There was nothing in the requirements that specified the service must handle havy traffic, so i chose not to use a persistent cashe provider, however
	  the data it self is not likely to change often, so a cashe provider would be a good addition to the service in the future.
- 
- Explain why you chose the module you unit-tested.
	- I chose to unit test the KycAggregationService because it is the core service that aggregates KYC data from multiple sources.
	  Testing this service ensures that the aggregation logic is functioning correctly and that the data returned is accurate and complete. 
	  By focusing on this module, I can validate the integration of different components and ensure that the service behaves as expected under various scenarios.

- Describe what you would improve with more time.
	- I would add resilience patterns like retry and circuit breaker to handle transient failures when calling the Customer Data API
	- I would write more unit tests to cover edge cases and error scenarios.
	- I would also add integration tests to ensure that the service works correctly with the database and external APIs.
	- I would maybe add a caching strategy.
	- I would structure the project to have a more modular architecture, separating concerns into distinct layers
	  (like Clean Architecture where you have separate layers for API, infrastructure, application, and domain).
	  This would improve maintainability and testability, but feelt a bit overkill for this code-test-task.

- Note any topics you would like to discuss during the technical interview.