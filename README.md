# Salesforce-Connector-and-Source
SSIS custom connection manager and source

The code in this repo provides almost bare bones connectivity to Salesforce and also as a source component to retrieve data from salesforce. It contains UI that allows you select the object you want to pull data from or for you to write a custom query to pull data from saleforces within SSIS. You can also use expressions to customize your dynamic queries.

You can just copy the dlls provided and place them in the appropriate SSIS folder paths of your Visual studio, and also install them in the gac.

Note this only works for VS 2016 and earlier. When using later versions, it is recommended to downgrade your SSIS project to 2016 in order to use this.

![image](https://github.com/KingShaggy1/Salesforce-Connector-and-Source/assets/47197934/ab10514e-4846-41d9-a1b7-6b33f11790d4)

