# Memstate vs Entity Framework benchmark comparison

Memstate is an application persistence framework with ACID transactions for in-memory objects
See https://github.com/devrexlabs/memstate

This comparison project is based on a sample e-commerce datamodel and workload. The ecommerce application interface is IEcommerceService and there are 2 implementations, InMemoryEcommerceService and EntityFrameworkEcommerceService.

The benchmarks are run by measuring the time to execute an action a fixed number of iterations. You will find these in the test project.

* Create an empty sql server database and point the connectionstring variable in the UnitTest1 class to it.
* Run the tests! First time initialization of the sql database (seeding the data) will take several hours.
* The test output of each test will say how many transactions per second were reached
