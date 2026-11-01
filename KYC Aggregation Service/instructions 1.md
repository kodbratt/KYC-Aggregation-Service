# KYC Aggregation Service

A know your customer (KYC) process is a procedure that financial institutions and other businesses follow to verify the identity of their customers. This process helps to prevent money laundering, terrorist financing, and fraud. It typically includes collecting, verifying, and documenting specific information about individuals or entities that engage in business activities with the organization.

This test is designed to understand how you approach real-world development tasks. You are welcome to use AI tools as part of your process. We are not looking for a perfect solution, but one that reflects your own judgment and technical preferences, with choices you understand and can explain.

Below, you will find the instructions for getting started with your task.
Please read them carefully to avoid unexpected issues. Best of luck!

## Getting started

1. Create a .NET solution containing one or more projects.
2. Initialize a Git repository in the solution directory.

## The Task

Your task is to build a backend service that implements the KYC Aggregation Service API described below. It needs to integrate with the [Customer Data API](https://customerdataapi-codetest-bughceejfwbwbkb9.swedencentral-01.azurewebsites.net/) to aggregate customer and KYC data.

When fetching KYC form data, use `/kyc-form/{ssn}`. The legacy route `/kyc-form/{ssn}/{asOfDate}` remains supported for existing clients and returns the same data, but requires a valid date in `yyyy-MM-dd` format.

Use `tax_country` from the KYC form when available; otherwise, return `null`.

<details>
<summary>KYC Aggregation Service API Specification</summary>

```json
{
    "openapi": "3.0.1",
    "info": {
      "title": "KYC Aggregation API Service",
      "description": "API for aggregating KYC (Know Your Customer) data",
      "version": "1.0.0"
    },
    "paths": {
      "/kyc-data/{ssn}": {
        "get": {
          "summary": "Get aggregated KYC data",
          "operationId": "GetAggregatedKycData",
          "tags": [
            "KYC Aggregation"
          ],
          "parameters": [
            {
              "name": "ssn",
              "in": "path",
              "required": true,
              "schema": {
                "type": "string"
              },
              "description": "Social Security Number of the customer"
            }
          ],
          "responses": {
            "200": {
              "description": "Successful response",
              "content": {
                "application/json": {
                  "schema": {
                    "$ref": "#/components/schemas/AggregatedKycData"
                  },
                  "example": {
                    "ssn": "19801115-1234",
                    "first_name": "Lars",
                    "last_name": "Larsson",
                    "address": "Smågatan 1, 123 22 Malmö",
                    "phone_number": "+46 70 123 45 67",
                    "email": "lars.larsson@example.com",
                    "tax_country": "SE",
                    "income": 550000
                  }
                }
              }
            },
            "404": {
              "description": "Customer data not found",
              "content": {
                "application/json": {
                  "example": {
                    "error": "Customer data not found for the provided SSN."
                  }
                }
              }
            },
            "500": {
              "description": "Internal server error",
              "content": {
                "application/json": {
                  "example": {
                    "error": "An unexpected error occurred while processing the request."
                  }
                }
              }
            }
          }
        }
      }
    },
    "components": {
      "schemas": {
        "AggregatedKycData": {
          "type": "object",
          "properties": {
            "ssn": {
              "type": "string",
              "example": "19830115-1234"
            },
            "first_name": {
              "type": "string",
              "example": "Sven"
            },
            "last_name": {
              "type": "string",
              "example": "Svensson"
            },
            "address": {
              "type": "string",
              "example": "Storgatan 1, 111 22 Stockholm"
            },
            "phone_number": {
              "type": "string",
              "example": "+46 70 123 45 67"
            },
            "email": {
              "type": "string",
              "example": "sven.svensson@example.com"
            },
            "tax_country": {
              "type": "string",
              "nullable": true,
              "example": "SE"
            },
            "income": {
              "type": "integer",
              "nullable": true,
              "example": 550000
            }
          },
          "required": [
            "first_name",
            "last_name",
            "address",
            "ssn",
            "tax_country"
          ]
        }
      }
    }
  }
```
</details>
<details>
<summary>Customer data API Specification</summary>

```json
{
  "openapi": "3.0.0",
  "info": {
    "title": "Customer Data API",
    "version": "1.0.0",
    "description": "API for retrieving customer personal details, contact information, and KYC form data."
  },
  "servers": [
    {
      "url": "https://customerdataapi-codetest-bughceejfwbwbkb9.swedencentral-01.azurewebsites.net",
      "description": "Production server"
    }
  ],
  "paths": {
    "/personal-details/{ssn}": {
      "get": {
        "summary": "Get customer personal details",
        "parameters": [
          {
            "in": "path",
            "name": "ssn",
            "required": true,
            "schema": {
              "type": "string"
            },
            "description": "Social Security Number of the customer"
          }
        ],
        "responses": {
          "200": {
            "description": "Successful response",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/PersonalDetails"
                }
              }
            }
          },
          "404": {
            "description": "Customer not found"
          },
          "400": {
            "description": "Invalid SSN format"
          },
          "429": {
            "description": "Rate limit exceeded. The Retry-After header specifies how many seconds to wait."
          }
        }
      }
    },
    "/contact-details/{ssn}": {
      "get": {
        "summary": "Get customer contact details",
        "parameters": [
          {
            "in": "path",
            "name": "ssn",
            "required": true,
            "schema": {
              "type": "string"
            },
            "description": "Social Security Number of the customer"
          }
        ],
        "responses": {
          "200": {
            "description": "Successful response",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/ContactDetails"
                }
              }
            }
          },
          "404": {
            "description": "Customer not found"
          },
          "400": {
            "description": "Invalid SSN format"
          },
          "429": {
            "description": "Rate limit exceeded. The Retry-After header specifies how many seconds to wait."
          }
        }
      }
    },
    "/kyc-form/{ssn}/{asOfDate}": {
      "get": {
        "summary": "Get customer KYC form data (legacy route)",
        "description": "Retained for existing clients. The date is validated but does not affect the returned data. New clients should use /kyc-form/{ssn}.",
        "parameters": [
          {
            "in": "path",
            "name": "ssn",
            "required": true,
            "schema": { "type": "string" }
          },
          {
            "in": "path",
            "name": "asOfDate",
            "required": true,
            "schema": { "type": "string", "format": "date" }
          }
        ],
        "responses": {
          "200": {
            "description": "Successful response",
            "content": {
              "application/json": {
                "schema": { "$ref": "#/components/schemas/KYCForm" }
              }
            }
          },
          "404": { "description": "Customer or KYC form not found" },
          "400": { "description": "Invalid SSN format or date" },
          "429": { "description": "Rate limit exceeded. The Retry-After header specifies how many seconds to wait." }
        }
      }
    },
    "/kyc-form/{ssn}": {
      "get": {
        "summary": "Get customer KYC form data",
        "parameters": [
          {
            "in": "path",
            "name": "ssn",
            "required": true,
            "schema": {
              "type": "string"
            },
            "description": "Social Security Number of the customer"
          }
        ],
        "responses": {
          "200": {
            "description": "Successful response",
            "content": {
              "application/json": {
                "schema": {
                  "$ref": "#/components/schemas/KYCForm"
                },
                "example": {
                  "items": [
                    {
                      "key": "tax_identification_number",
                      "value": "123-45-6789"
                    },
                    {
                      "key": "date_of_birth",
                      "value": "1980-01-15"
                    },
                    {
                      "key": "nationality",
                      "value": "Denmark"
                    },
                    {
                      "key": "occupation",
                      "value": "Software Engineer"
                    },
                    {
                      "key": "source_of_funds",
                      "value": "Employment Income"
                    },
                    {
                      "key": "politically_exposed_person",
                      "value": "No"
                    },
                    {
                      "key": "annual_income",
                      "value": "500000"
                    },
                    {
                      "key": "expected_transaction_volume",
                      "value": "100000"
                    },
                    {
                      "key": "purpose_of_account",
                      "value": "Savings and Investments"
                    },
                    {
                      "key": "risk_profile",
                      "value": "Medium"
                    },
                    {
                      "key": "tax_country",
                      "value": "DK"
                    },
                    {
                      "key": "kyc_registration_date",
                      "value": "1997-10-01"
                    }
                  ]
                }
              }
            }
          },
          "404": {
            "description": "Customer or KYC form not found"
          },
          "400": {
            "description": "Invalid SSN format"
          },
          "429": {
            "description": "Rate limit exceeded. The Retry-After header specifies how many seconds to wait."
          }
        }
      }
    }
  },
  "components": {
    "schemas": {
      "PersonalDetails": {
        "type": "object",
        "properties": {
          "firstName": {
            "type": "string"
          },
          "surName": {
            "type": "string"
          },
          "address": {
            "type": "string"
          }
        }
      },
      "ContactDetails": {
        "type": "object",
        "properties": {
          "addresses": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/Address"
            }
          },
          "emails": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/Email"
            }
          },
          "phoneNumbers": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/PhoneNumber"
            }
          }
        }
      },
      "Address": {
        "type": "object",
        "properties": {
          "street": {
            "type": "string"
          },
          "city": {
            "type": "string"
          },
          "state": {
            "type": "string"
          },
          "postalCode": {
            "type": "string"
          },
          "country": {
            "type": "string"
          }
        }
      },
      "Email": {
        "type": "object",
        "properties": {
          "preferred": {
            "type": "boolean"
          },
          "emailAddress": {
            "type": "string"
          }
        }
      },
      "PhoneNumber": {
        "type": "object",
        "properties": {
          "preferred": {
            "type": "boolean"
          },
          "number": {
            "type": "string"
          }
        }
      },
      "KYCForm": {
        "type": "object",
        "properties": {
          "items": {
            "type": "array",
            "items": {
              "$ref": "#/components/schemas/KYCItem"
            }
          }
        }
      },
      "KYCItem": {
        "type": "object",
        "properties": {
          "key": {
            "type": "string"
          },
          "value": {
            "type": "string"
          }
        }
      }
    }
  }
}
```
</details>

#### Test Data
There is test data available for the following SSNs:
```
19800115-1234
19900220-5678
19751230-9101
19850505-4321
19951212-3456
```

The Customer Data API allows 30 requests per minute per client IP across all
endpoints. When the limit is exceeded, it returns HTTP 429 with a `Retry-After`
header indicating how many seconds to wait.

## .NET Coding Test Instructions

Please complete the following tasks as part of your coding test. The goal is to evaluate your approach to software design, code quality, and problem-solving.

---

### 1. **Data persistence**
- On the first request for a customer, fetch the required data from the remote API and store it locally.
- On subsequent requests for the same customer, serve the stored data without calling the remote API again.
- The stored data must remain available after application restarts.
- Use an **ORM** (e.g., Entity Framework) and/or a **persistent caching provider** to implement this functionality.

---

### 2. **Error Handling**
- Implement robust error handling throughout the application.
- Ensure that:
  - **User-facing errors** are communicated clearly and appropriately.
  - **System-level errors** are logged to a suitable logging mechanism (e.g., file, console, or logging service).
- Choose an error-handling strategy that you believe is appropriate and explain your reasoning if necessary.

---

### 3. **Unit Testing**
- Write unit tests for **one module of your choice**.
- You do not need to test the entire application.
- The tests should:
  - Have a clear purpose.
  - Demonstrate your understanding of unit testing principles.
  - Verify that the application behaves as expected.

---

### 4. **Version Control**
- Use Git for version control.
- You may:
  - Create as many commits as you like.
  - Use branches as needed to organize your work.

---

### 5. **Code Comments**
- You are welcome to add comments to your code.
- Comments should clarify complex logic or design decisions.
- Avoid excessive or redundant commenting.

### 6. **Reflection and Discussion**
Include a `REFLECTION.md` file addressing the following:
- Explain your main technical choices and why you preferred them over alternatives.
- Describe any trade-offs or assumptions you made.
- Explain why you chose the module you unit-tested.
- Describe what you would improve with more time.
- Note any topics you would like to discuss during the technical interview.

Please write `REFLECTION.md` yourself, without the help of AI tools. We want to understand your own reasoning, and it does not need to be polished. Short notes or bullet points are fine.

## When you are done

1. Commit/merge your final solution to the master or main branch of the git repository.

2. Create a zip file with the project.

3. Send the result to us by the contact address.

If you have questions or problems, please dont hesitate to mail us!

## Contact address

laura.sonnert@dnbcarnegie.se