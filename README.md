# Salesforce-Connector-and-Source
SSIS custom connection manager and source

The code in this repo provides almost bare bones connectivity to Salesforce and also as a source component to retrieve data from salesforce. It contains gui that allows you select the object you want to pull data from or for you to write a custom query to pull data from saleforces within SSIS.

You can just copy the dlls provide and place them in the appropriate SSIS folder paths of your Visual studio.

Note this only works for VS 2016 and earlier. When using later versions, it is recommended to downgrade your SSIS project to 2016 in order to use this.
