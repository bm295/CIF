# language: en

Feature: Budget-based service offer recommendation
  As a customer
  I want to receive the best combination of a base plan and an add-on within my budget
  So that I can maximize the value of my purchase

  Background:
    Given the available base plans are:
      | id       | tên          | giá (VND) |
      | basic    | Cơ bản       | 100000    |
      | premium  | Nâng cao     | 200000    |
      | business | Doanh nghiệp | 350000    |
    And the available add-ons are:
      | id        | tên              | giá (VND) |
      | support   | Hỗ trợ ưu tiên   | 50000     |
      | report    | Báo cáo nâng cao | 75000     |
      | monitoring | Theo dõi KYC    | 125000    |

  Scenario: Recommend the highest-value combination within the budget
    Given the customer's budget is 250000 VND
    When the customer requests an offer recommendation
    Then the recommended base plan is "Premium"
    And the recommended add-on is "Priority support"
    And the total price is 250000 VND
    And the remaining budget is 0 VND

  Scenario: Report that no combination fits the budget
    Given the customer's budget is 10000 VND
    When the customer requests an offer recommendation
    Then the system reports that no offer combination fits the budget

  Scenario: Select the exact combination when several combinations are available
    Given the customer's budget is 275000 VND
    When the customer requests an offer recommendation
    Then the recommended base plan is "Premium"
    And the recommended add-on is "Advanced report"
    And the total price is 275000 VND
    And the remaining budget is 0 VND
