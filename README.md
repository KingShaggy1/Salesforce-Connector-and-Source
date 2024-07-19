# Salesforce-Connector-and-Source for SSIS
SSIS custom connection manager and source

This repository hosts a custom solution for integrating Salesforce with SQL Server Integration Services (SSIS), featuring a specialized near bare bones connection manager and source component.

The provided code offers fundamental connectivity to Salesforce and serves as a source component for extracting Salesforce data. It includes a user interface that facilitates the selection of Salesforce objects for data retrieval or allows users to define custom queries directly within SSIS. Additionally, the solution supports the use of expressions to enable dynamic query customization.

To utilize the solution, simply copy the provided DLL files to the appropriate SSIS folder paths in your Visual Studio environment and install them in the Global Assembly Cache (GAC).

Please note that this solution is compatible only with Visual Studio 2016 and earlier versions of SSIS. For later versions, it is recommended to downgrade your SSIS project to the 2016 version to ensure compatibility with this integration.

![image](https://github.com/KingShaggy1/Salesforce-Connector-and-Source/assets/47197934/ab10514e-4846-41d9-a1b7-6b33f11790d4)

