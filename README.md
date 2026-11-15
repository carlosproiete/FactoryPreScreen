## Impinj Coding Assesment:

```
TIME:            2-3 hours
LANGUAGES:       C# preferred
FRAMEWORKS:      NET or NET CORE preferred,
TEST FRAMEWORKS: MS BUILD/NUNIT/XUNIT preferred
TYPE:            Web Api
FILES:           In repository
TESTS:           preferred
SUBMIT:          ZipFile, Github Repository Link
```

## Overview:
This exercise is to asses a candidates ability to design and implement a solution to a problem in a given period of time.

We want to see a demonstration of your thought process, skills and development experiences.

This solution should be able to run locally.

_Be prepared to explain your design decisions and architecture!_


## Problem:
Create a web API that can parse a sales record file into an object. The API should return an object with the following fields:
* The least profitable product
* The product with the highest average discount
* The value of the highest average discount
* The product with the highest number of returned orders
* The state with the lowest total sales value
* The sales standard deviation

## Observations:
* Consider the least profitable product the one with the lowest total/absolute profit.
* If there is a tie, return the first product in the processed list.
* Include all orders, regardless of returns/devolutions, for all fields, except when calculating returns.
* The sales standard deviation is calculated using the Sales column (total sale value per record).

## Included:
* Sample input at `Input\SalesOrders.csv`
* A git ignore file for a C# project
* A git attributes file

<br></br>

_Hint: a web api skeleton can be created with `dotnet new webapi`_
