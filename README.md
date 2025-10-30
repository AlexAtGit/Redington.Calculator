# How to build & run the back-end solution

1. Use Visual Studio 2022 to open the following C# solution in folder ...\Redington\backend
   Redington.Calculator.sln

2. Build the solution

3. Ensure that the default project is Redington.Calculator.API

4. To run the unit tests, open Test Explorer and run all the tests

5. To run the API service locally, just run the default project above (start without debugging). 
   This will open a browser tab with the swagger page: 
   https://localhost:7001/swagger/index.html
   
   The API can be tested directly on the swagger page. Click on POST and then the "Try It Out" button.
   Enter values for the below
   {
	"probabilityA": 0,
	"probabilityB": 0,
	"functionName": "string"
   }
   Note that as you enter a value for probabilityA, the parameter name of the second value "probabilityB" is changed to "probability". I am not sure why yet. Please reset the name to "probabilityB" and enter a valid function name, e.g. "CombinedWith" and click Execute. Ypu should get a 200 response with the correct result. If you enter an invalid probability of the wrong function name, you should get a 400 (Bad Request) response together with an appropriate error message.
   
   Backend solution logs can be found under the subfolder named logs within Redington.Calculator.API
