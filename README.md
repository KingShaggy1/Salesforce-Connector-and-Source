# Salesforce-Connector-and-Source for SSIS
## SSIS custom connection manager and source

This repository hosts a custom solution for integrating Salesforce with SQL Server Integration Services (SSIS), featuring a specialized near bare bones connection manager and source component.

The provided code offers fundamental connectivity to Salesforce and serves as a source component for extracting Salesforce data. It includes a user interface that facilitates the selection of Salesforce objects for data retrieval or allows users to define custom queries directly within SSIS. Additionally, the solution supports the use of expressions to enable dynamic query customization.

To utilize the solution, simply copy the provided DLL files to the appropriate SSIS folder paths in your Visual Studio environment and install them in the Global Assembly Cache (GAC).

Please note that this solution is compatible only with Visual Studio 2016 and earlier versions of SSIS. For later versions, it is recommended to downgrade your SSIS project to the 2016 version to ensure compatibility with this integration.

## Basic Use

Add "KS Salesforce Connection" connection manager from the Connection Managers tab by right clicking and selecting add new connection. 

![image](https://github.com/user-attachments/assets/8a974e11-560b-46f0-9ee0-f65392cc6317)

Enter your salesforce credentials and test connection. Upon Connection success, click ok. Url should be in form such as https://MyDomainName.my.salesforce.com.

![image](https://github.com/user-attachments/assets/7e889a21-6d51-44eb-a994-d4d2e5667c78)

On your data flow tab drag the "KS Salesforce Source" to your data flow task. Select your salesforce connection under Select Connection dropdown. You can also use the new button to create a connection. In the Access Mode dropdown, select table or query.
Table option allows you to view the objects in salesforce while Query gives you the option to write your customized query. The preview button displays few records of your selected object or customized query.

![image](https://github.com/user-attachments/assets/8dbff33e-a6f2-49b3-92b2-056d7969a610)

![image](https://github.com/user-attachments/assets/61ea8958-f4f3-4a11-a676-9611758ca388)

### Using Expressions in Your Query

To integrate expressions into your query, follow these steps:

1. Add a Data Flow Task to your package.
2. In your Data Flow Task add the KS Salesforce Source.
3. Return to control flow and right-click on the Data Flow Task.
4. Select "Properties."
5. Under Expressions, open the ellipsis button.
6. In the Property Expressions Editor, choose your Salesforce connection and input your parameterized query in the Expression field. Click "OK" to confirm.
7. Switch to the Data Flow tab, locate the Salesforce source component, select your connection, and set the Access Mode to "Query."
8. Your parameterized query should be visible in the textbox.

![image](https://github.com/user-attachments/assets/3372b0c0-58eb-4326-a80e-fbcfaeea49f9)

![image](https://github.com/user-attachments/assets/dded7f2e-71aa-43c8-bca5-227f1b0a93d8)


# Happy coding from King Shaggy.

![image](https://github.com/KingShaggy1/Salesforce-Connector-and-Source/assets/47197934/ab10514e-4846-41d9-a1b7-6b33f11790d4)

