@web
Feature: Example Feature
    As a user
    I want to visit a website
    So I can perform some actions

Scenario: Login into Sauce App
	Given I navigate to "https://www.saucedemo.com/"
	Given User completes the login pg
	Then user should be routed to store